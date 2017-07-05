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
        private CompanyDAO cpnDao;
        private ItemDAO itmDao;
        private FiscalItemDAO fisDao;
        private ExportedItemDAO expDao;
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


                        cpnDao = new CompanyDAO(rodes);
                        


                        foreach (Company company in cpnDao.GetCompaniesIn(cfg.Lojas))
                        {
                             tasks.Add(Task.Factory.StartNew(() => { ExportCompanyItems(company.Clone(), cfg); }));
                        }

                    }
                    Task.WaitAll(tasks.ToArray());

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

                        foreach(Item item in itmDao.GetItemsToExport(company.Code))
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
                                Console.WriteLine(ex.Message);
                                atualizado = false;

                            }
                            if (atualizado)
                            {
                                try
                                {
                                    expDao.Insert(item);
                                }
                                catch(Exception  ex)
                                {

                                }
                            }
                            Console.WriteLine("LOJA:" + company.Code + " ITEM: " + item.BarCode);
                            // Console.WriteLine("LOJA:" + company.Code + " #" + (iterador++).ToString());
                        }

                    }
                    Console.WriteLine("########## LOJA " + company.Code + " FINALIZADA ##########");

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
    }
}
