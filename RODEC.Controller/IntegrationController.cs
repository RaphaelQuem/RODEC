using RODEC.DAO;
using RODEC.Infra;
using RODEC.Model;
using RODEC.Modelo;
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
        public void Export()
        {
            try
            {
                string loja;
                List<Task> tasks = new List<Task>();
                Task.Factory.StartNew(() => ExportItems());
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
        public void ExportItems()
        {
            try
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
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
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
                                Console.WriteLine(company.Code + ": " + ex.Message);
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
                            Console.WriteLine("LOJA:" + company.Code + " ITEM: " + item.BarCode);
                        }

                    }
                    Console.WriteLine("########## LOJA " + company.Code + " FINALIZADA ##########");

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(company.Code + ": " + ex.Message);
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

                Console.WriteLine(ex.Message);
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
                                        Console.WriteLine(company.Code + ": " + ex.Message);
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
                                    Console.WriteLine("LOJA:" + company.Code + " ITEM: " + item.BarCode);
                                }
                            }
                            Console.WriteLine("########## LOJA " + company.Code + " FINALIZADA ##########");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine( ex.Message);
            }
        }
    }
}
