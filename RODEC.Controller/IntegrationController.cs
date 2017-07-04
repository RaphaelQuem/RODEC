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
        public void ExportItems()
        {
            try
            {
                using (Config cfg = Config.GetConfig())
                {
                    using (SqlConnection rodes = new SqlConnection(cfg.ConnectionStrings["RODES"]))
                    {
                        rodes.Open();


                        cpnDao = new CompanyDAO(rodes);
                        itmDao = new ItemDAO(rodes);

                        cpnDao.GetCompaniesIn(cfg.Lojas);

                        Company company = cpnDao.GetNext();
                        while (company != default(Company))
                        {
                            int iterador = 0;
                            decimal perc = cfg.AliquotasEstaduais[company.State];
                            bool nfce = cfg.LojasNFCE.Contains(company.Code);

                            try
                            {
                                using (SqlConnection sqlCon = new SqlConnection(cfg.ConnectionStrings[company.Code]))
                                {
                                    sqlCon.Open();

                                    fisDao = new FiscalItemDAO(sqlCon);
                                    itmDao.GetItemsToExport(company.Code);

                                    Item item = itmDao.GetNext();
                                    while (item != default(Item))
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
                                                SqlCommand sqlConfirm = rodes.CreateCommand();

                                                sqlConfirm.CommandText = "";
                                                sqlConfirm.CommandText += " INSERT INTO SIGECETQ ";
                                                sqlConfirm.CommandText += " ( ";
                                                sqlConfirm.CommandText += "     \"cbars\", ";
                                                sqlConfirm.CommandText += "     \"cidchaves\", ";
                                                sqlConfirm.CommandText += "     \"emps\" ";
                                                sqlConfirm.CommandText += " ) ";
                                                sqlConfirm.CommandText += " VALUES ";
                                                sqlConfirm.CommandText += " ( ";
                                                sqlConfirm.CommandText += "     '" + item.BarCode + "', ";
                                                sqlConfirm.CommandText += "     '" + company.Code + item.BarCode + "', ";
                                                sqlConfirm.CommandText += "     '" + company.Code + "' ";
                                                sqlConfirm.CommandText += " );";

                                                sqlConfirm.ExecuteNonQuery();
                                            }
                                            catch
                                            {

                                            }
                                        }

                                        item = itmDao.GetNext();
                                        Console.WriteLine("LOJA:" + company.Code + " #" + (iterador++).ToString());
                                    }

                                }
                                Console.WriteLine("########## LOJA " + company.Code + " FINALIZADA ##########");

                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine(ex.Message);
                            }
                            company = cpnDao.GetNext();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
    }
}
