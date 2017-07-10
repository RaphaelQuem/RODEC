using RODEC.DAO;
using RODEC.Infra;
using RODEC.Model;
using RODEC.Modelo;
using RODEC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RODEC.Controller
{
    public class IntegrationController
    {
        MonitorVM viewmodel;
        public IntegrationController(MonitorVM vm)
        {
            viewmodel = vm;
        }
        public void Export()
        {
            try
            {
                viewmodel.Status = "Rodando";
                viewmodel.CanRun = false;
                viewmodel.CanStop = true;
                Task.Factory.StartNew(() => ExportItems());
            }
            catch (Exception ex)
            {

                viewmodel.StringLogs += Environment.NewLine + (ex.Message);
            }
        }
        public void ExportItems()
        {
            try
            {
                while (viewmodel.Status.Equals("Rodando"))
                {
                    using (Config cfg = Config.GetConfig())
                    {
                        List<Task> tasks = new List<Task>();
                        using (SqlConnection rodes = new SqlConnection(cfg.ConnectionStrings["RODES"]))
                        {
                            rodes.Open();

                            CompanyDAO cpnDao = new CompanyDAO(rodes);

                            foreach (Company company in cpnDao.GetCompaniesIn(cfg.Lojas))
                            {
                                tasks.Add(Task.Factory.StartNew(() => { ExportCompanyItems(company.Clone(), cfg); }));
                            }

                            Task.WaitAll(tasks.ToArray());
                        }
                    }
                }
                viewmodel.Status = "Parado";
                viewmodel.CanRun = true;
                viewmodel.CanStop = false;

            }
            catch (Exception ex)
            {

                viewmodel.StringLogs += Environment.NewLine + (ex.Message);
            }
        }
        private void ExportCompanyItems(Company company, Config cfg)
        {
            try
            {
                ItemDAO itmDao;
                FiscalItemDAO fisDao;
                ExportedItemDAO expDao;
                using (SqlConnection rodes = new SqlConnection(cfg.ConnectionStrings["RODES"]))
                {
                    rodes.Open();
                    using (SqlConnection sqlCon = new SqlConnection(cfg.ConnectionStrings[company.Code]))
                    {
                        sqlCon.Open();

                        fisDao = new FiscalItemDAO(sqlCon);
                        itmDao = new ItemDAO(rodes);
                        expDao = new ExportedItemDAO(rodes);

                        decimal perc = cfg.AliquotasEstaduais[company.State];
                        bool nfce = cfg.LojasNFCE.Contains(company.Code);

                        foreach (Item item in itmDao.GetItemsToExport(company.Code))
                        {
                            bool atualizado = false;
                            item.Percentage = perc;
                            try
                            {
                                if (nfce)
                                {
                                    fisDao.SaveNFCE(item);
                                }
                                else
                                {
                                    fisDao.Save(item);
                                }
                                atualizado = true;

                            }
                            catch (Exception ex)
                            {
                                viewmodel.StringLogs += Environment.NewLine + (company.Code + ": " + ex.Message);
                                atualizado = false;

                            }
                            if (atualizado)
                            {
                                try
                                {
                                    expDao.Insert(item);
                                }
                                catch
                                {

                                }
                            }
                            viewmodel.StringLogs += Environment.NewLine + ("LOJA:" + company.Code + " ITEM: " + item.BarCode);
                        }

                    }
                    viewmodel.StringLogs += Environment.NewLine + ("########## LOJA " + company.Code + " FINALIZADA ##########");

                }
            }
            catch (Exception ex)
            {

                viewmodel.StringLogs += Environment.NewLine + ("LOG: "+company.Code + ": " + ex.Message);

            }
        }
        public void ExportSingle(string companycodes, string itemcode)
        {
            try
            {
                Task.Factory.StartNew(() => ExportSingleItem(companycodes, itemcode));
            }
            catch (Exception ex)
            {

                viewmodel.StringLogs += Environment.NewLine + (ex.Message);
            }
        }
        public void ExportSingleItem(string companycodes, string itemcode)
        {
            try
            {
                using (Config cfg = Config.GetConfig())
                {
                    string parameters = Console.ReadLine();

                    ItemDAO itmDao;
                    FiscalItemDAO fisDao;
                    ExportedItemDAO expDao;
                    CompanyDAO cpnDao;

                    using (SqlConnection rodes = new SqlConnection(cfg.ConnectionStrings["RODES"]))
                    {
                        rodes.Open();

                        cpnDao = new CompanyDAO(rodes);
                        foreach (Company company in cpnDao.GetCompaniesIn(companycodes.Split(',')))
                        {

                            using (SqlConnection sqlCon = new SqlConnection(cfg.ConnectionStrings[company.Code]))
                            {
                                sqlCon.Open();


                                fisDao = new FiscalItemDAO(sqlCon);
                                itmDao = new ItemDAO(rodes);
                                expDao = new ExportedItemDAO(rodes);




                                decimal perc = cfg.AliquotasEstaduais[company.State];
                                bool nfce = cfg.LojasNFCE.Contains(company.Code);

                                foreach (Item item in itmDao.GetSingleItemToExport(company.Code,itemcode))
                                {
                                    bool atualizado = false;
                                    item.Percentage = perc;
                                    try
                                    {
                                        if (nfce)
                                        {
                                            fisDao.SaveNFCE(item);
                                        }
                                        else
                                        {
                                            fisDao.Save(item);
                                        }
                                        atualizado = true;

                                    }
                                    catch (Exception ex)
                                    {
                                        viewmodel.StringLogs += Environment.NewLine + (company.Code + ": " + ex.Message);
                                        atualizado = false;

                                    }
                                    if (atualizado)
                                    {
                                        try
                                        {
                                            expDao.Insert(item);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    viewmodel.StringLogs += Environment.NewLine + ("LOJA:" + company.Code + " ITEM: " + item.BarCode);
                                }
                            }
                            viewmodel.StringLogs += Environment.NewLine + ("########## LOJA " + company.Code + " FINALIZADA ##########");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                viewmodel.StringLogs += Environment.NewLine + ( ex.Message);
            }
        }
    }
}
