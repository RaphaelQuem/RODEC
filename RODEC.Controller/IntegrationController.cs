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

                        Company company = cpnDao.GetCompaniesIn(cfg.Lojas).GetNext<Company>();
                        while (company != default(Company))
                        {

                            int iterador = 0;
                            double perc = cfg.AliquotasEstaduais[company.State];
                            bool nfce = cfg.LojasNFCE.Contains(company.Code);

                            try
                            {

                                using (SqlConnection sqlCon = new SqlConnection(cfg.ConnectionStrings[company.Code]))
                                {
                                    sqlCon.Open();


                                    Item item = itmDao.GetItemsToExport(company.Code).GetNext<Item>();
                                    while (item != default(Item))
                                    {
                                        bool atualizado = false;

                                        try
                                        {
                                            SqlCommand sqlCom = sqlCon.CreateCommand();
                                            sqlCom.CommandText = " SELECT COUNT(*) QTD";
                                            sqlCom.CommandText += " FROM   MCJ_PRODUTO";
                                            sqlCom.CommandText += " WHERE  \"cod_produto\" = '" + item.BarCode + "'";

                                            using (IDataReader readerSql = sqlCom.ExecuteReader())
                                            {
                                                if (readerSql.Read())
                                                {
                                                    if (nfce)
                                                    {
                                                        if (Convert.ToInt32(readerSql["QTD"].ToString()) > 0)
                                                        {
                                                            sqlCom.CommandText = " UPDATE MCJ_PRODUTO ";
                                                            sqlCom.CommandText += " SET         \"descricao\"                         = '" + item.Description + "',";
                                                            sqlCom.CommandText += "             \"preco_unitario\"                    = NULL,";
                                                            sqlCom.CommandText += "             \"id_mcj_situacao_tributaria\"        = 1,";
                                                            sqlCom.CommandText += "             \"perc_carga_tributaria_federal\"     = 9,";
                                                            sqlCom.CommandText += "             \"fl_pis_cofins\"                     = 1,";
                                                            sqlCom.CommandText += "             \"perc_carga_tributaria_estadual\"    = " + perc.ToString() + ",";
                                                            sqlCom.CommandText += "             \"perc_cargatributaria_municipal\"    = 5,";
                                                            sqlCom.CommandText += "             \"cod_ncm\"                           = '" + item.FiscalClassification + "',";
                                                            sqlCom.CommandText += "             \"ex_tipi\"                           = 0,";
                                                            sqlCom.CommandText += "             \"set_origem_produto\"                = 0,";
                                                            sqlCom.CommandText += "             \"cod_cest\"                          = NULL,";
                                                            sqlCom.CommandText += "             \"perc_carga_tributaria\"             = NULL";
                                                            sqlCom.CommandText += " WHERE       \"cod_produto\"                       = '" + item.BarCode + "'";
                                                        }
                                                        else
                                                        {
                                                            sqlCom.CommandText = " INSERT INTO MCJ_PRODUTO( ";
                                                            sqlCom.CommandText += "       \"cod_produto\", ";
                                                            sqlCom.CommandText += "       \"descricao\", ";
                                                            sqlCom.CommandText += "       \"preco_unitario\", ";
                                                            sqlCom.CommandText += "       \"id_mcj_situacao_tributaria\", ";
                                                            sqlCom.CommandText += "       \"perc_carga_tributaria_federal\", ";
                                                            sqlCom.CommandText += "       \"fl_pis_cofins\", ";
                                                            sqlCom.CommandText += "       \"perc_carga_tributaria_estadual\", ";
                                                            sqlCom.CommandText += "       \"perc_cargatributaria_municipal\", ";
                                                            sqlCom.CommandText += "       \"cod_ncm\", ";
                                                            sqlCom.CommandText += "       \"ex_tipi\", ";
                                                            sqlCom.CommandText += "       \"set_origem_produto\", ";
                                                            sqlCom.CommandText += "       \"cod_cest\", ";
                                                            sqlCom.CommandText += "       \"perc_carga_tributaria\" ";
                                                            sqlCom.CommandText += "        ) ";
                                                            sqlCom.CommandText += " VALUES('" + item.BarCode + "', ";
                                                            sqlCom.CommandText += "        '" + item.Description + "', ";
                                                            sqlCom.CommandText += "        NULL, ";
                                                            sqlCom.CommandText += "        1, ";
                                                            sqlCom.CommandText += "        9, ";
                                                            sqlCom.CommandText += "        1, ";
                                                            sqlCom.CommandText += "        " + perc.ToString() + ", ";
                                                            sqlCom.CommandText += "        5, ";
                                                            sqlCom.CommandText += "        '" + item.FiscalClassification + "', ";
                                                            sqlCom.CommandText += "        0, ";
                                                            sqlCom.CommandText += "        0, ";
                                                            sqlCom.CommandText += "        NULL, ";
                                                            sqlCom.CommandText += "        NULL)";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Convert.ToInt32(readerSql["QTD"].ToString()) > 0)
                                                        {

                                                            sqlCom.CommandText = " UPDATE MCJ_PRODUTO ";
                                                            sqlCom.CommandText += " SET         \"descricao\"                         = '" + item.Description + "',";
                                                            sqlCom.CommandText += "             \"id_mcj_situacao_tributaria\"        = 1,";
                                                            sqlCom.CommandText += "             \"perc_carga_tributaria_federal\"     = " + (item.TaxSituation.ToString().Trim().Equals(string.Empty) ? "1" : item.TaxSituation.ToString().Trim()) + "";
                                                            sqlCom.CommandText += " WHERE       \"cod_produto\"                       = '" + item.BarCode + "'";
                                                        }
                                                        else
                                                        {
                                                            sqlCom.CommandText = " INSERT INTO MCJ_PRODUTO( ";
                                                            sqlCom.CommandText += "       \"cod_produto\", ";
                                                            sqlCom.CommandText += "       \"descricao\", ";
                                                            sqlCom.CommandText += "       \"preco_unitario\", ";
                                                            sqlCom.CommandText += "       \"id_mcj_situacao_tributaria\", ";
                                                            sqlCom.CommandText += "       \"perc_carga_tributaria_federal\", ";
                                                            sqlCom.CommandText += "       \"fl_pis_cofins\" ";
                                                            sqlCom.CommandText += "        ) ";
                                                            sqlCom.CommandText += " VALUES('" + item.BarCode + "', ";
                                                            sqlCom.CommandText += "        '" + item.Description + "', ";
                                                            sqlCom.CommandText += "        NULL, ";
                                                            sqlCom.CommandText += "        1,";
                                                            sqlCom.CommandText += "        " + (item.TaxSituation.ToString().Trim().Equals(string.Empty) ? "1" : item.TaxSituation.ToString().Trim()) + ", ";
                                                            sqlCom.CommandText += "        1)";
                                                        }
                                                    }
                                                }

                                            }

                                            sqlCom.ExecuteNonQuery();
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
                                        item = itmDao.GetItemsToExport(company.Code).GetNext<Item>();
                                        Console.WriteLine("LOJA:" + company.Code + " #" + (iterador++).ToString());
                                    }

                                }
                                Console.WriteLine("########## LOJA " + company.Code + " FINALIZADA ##########");

                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine(ex.Message);
                            }
                            company = cpnDao.GetCompaniesIn(cfg.Lojas).GetNext<Company>();
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
