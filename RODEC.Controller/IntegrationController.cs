using RODEC.DAO;
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

                        foreach (Company company in cpnDao.GetCompaniesIn(cfg.Lojas))
                        {

                            int iterador = 0;
                            double perc = cfg.AliquotasEstaduais[company.State];
                            bool nfce = cfg.LojasNFCE.Contains(company.Code);

                            try
                            {

                                using (SqlConnection sqlCon = new SqlConnection(cfg.ConnectionStrings[company.Code]))
                                {
                                    sqlCon.Open();
                                    SqlCommand comando = sqlCon.CreateCommand();

                                    comando = rodes.CreateCommand();
                                    comando.CommandText = " ";
                                    comando.CommandText += " SELECT	*  ";
                                    comando.CommandText += " FROM  ";
                                    comando.CommandText += " ( ";
                                    comando.CommandText += "              SELECT	DISTINCT	CASE B.ETIQS ";
                                    comando.CommandText += " 										WHEN 'S' THEN C.CBARS ";
                                    comando.CommandText += " 										ELSE A.cbars ";
                                    comando.CommandText += " 									END CODIGO_EC5, ";
                                    comando.CommandText += " 									ISNULL(A.SITTRICMS,1) SITTRICMS,  ";
                                    comando.CommandText += " 						          	A.CLFISCALS,   ";
                                    comando.CommandText += " 									A.DPROS, ";
                                    comando.CommandText += " 									ISNULL(B.ETIQS,'N') ETIQS, ";
                                    comando.CommandText += " 									C.EMPS ";
                                    comando.CommandText += "               FROM	    SIGCDPRO AS A WITH (NOLOCK)";
                                    comando.CommandText += " 						INNER  JOIN ";
                                    comando.CommandText += " 						SIGCDUNI AS B WITH (NOLOCK) ";
                                    comando.CommandText += " 							ON A.CUNIS = B.CUNIS ";
                                    comando.CommandText += " 						LEFT JOIN ";
                                    comando.CommandText += " 						SIGOPETQ AS C WITH (NOLOCK) ";
                                    comando.CommandText += " 							ON   A.CPROS = C.CPROS ";
                                    comando.CommandText += "               WHERE	A.MERCS IN('JOI','REL') ";
                                    comando.CommandText += " ) AS Y ";
                                    comando.CommandText += " WHERE		Y.CODIGO_EC5 IS NOT NULL ";
                                    comando.CommandText += " AND  		NOT EXISTS  ";
                                    comando.CommandText += " ( ";
                                    comando.CommandText += " 			SELECT * ";
                                    comando.CommandText += " 			FROM SIGECETQ AS X ";
                                    comando.CommandText += " 			WHERE X.CBARS  = Y.CODIGO_EC5 ";
                                    comando.CommandText += " 			AND   X.EMPS  = '" + company.Code + "' ";
                                    comando.CommandText += " ) ";
                                    comando.CommandText += " AND ";
                                    comando.CommandText += " ( ";
                                    comando.CommandText += "    ( Y.ETIQS ='S' AND  Y.EMPS= '" + company.Code + "') ";
                                    comando.CommandText += "        OR ";
                                    comando.CommandText += "    ( Y.ETIQS ='N') ";
                                    comando.CommandText += " ) ";


                                    using (IDataReader readerCadMat = comando.ExecuteReader())
                                    {

                                        while (readerCadMat.Read())
                                        {
                                            bool atualizado = false;

                                            try
                                            {
                                                SqlCommand sqlCom = sqlCon.CreateCommand();
                                                sqlCom.CommandText = " SELECT COUNT(*) QTD";
                                                sqlCom.CommandText += " FROM   MCJ_PRODUTO";
                                                sqlCom.CommandText += " WHERE  \"cod_produto\" = '" + readerCadMat["CODIGO_EC5"].ToString() + "'";

                                                using (IDataReader readerSql = sqlCom.ExecuteReader())
                                                {
                                                    if (readerSql.Read())
                                                    {
                                                        if (nfce)
                                                        {
                                                            if (Convert.ToInt32(readerSql["QTD"].ToString()) > 0)
                                                            {
                                                                sqlCom.CommandText = " UPDATE MCJ_PRODUTO ";
                                                                sqlCom.CommandText += " SET         \"descricao\"                         = '" + readerCadMat["DPROS"].ToString() + "',";
                                                                sqlCom.CommandText += "             \"preco_unitario\"                    = NULL,";
                                                                sqlCom.CommandText += "             \"id_mcj_situacao_tributaria\"        = 1,";
                                                                sqlCom.CommandText += "             \"perc_carga_tributaria_federal\"     = 9,";
                                                                sqlCom.CommandText += "             \"fl_pis_cofins\"                     = 1,";
                                                                sqlCom.CommandText += "             \"perc_carga_tributaria_estadual\"    = " + perc.ToString() + ",";
                                                                sqlCom.CommandText += "             \"perc_cargatributaria_municipal\"    = 5,";
                                                                sqlCom.CommandText += "             \"cod_ncm\"                           = '" + readerCadMat["CLFISCALS"].ToString() + "',";
                                                                sqlCom.CommandText += "             \"ex_tipi\"                           = 0,";
                                                                sqlCom.CommandText += "             \"set_origem_produto\"                = 0,";
                                                                sqlCom.CommandText += "             \"cod_cest\"                          = NULL,";
                                                                sqlCom.CommandText += "             \"perc_carga_tributaria\"             = NULL";
                                                                sqlCom.CommandText += " WHERE       \"cod_produto\"                       = '" + readerCadMat["CODIGO_EC5"].ToString() + "'";
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
                                                                sqlCom.CommandText += " VALUES('" + readerCadMat["CODIGO_EC5"].ToString() + "', ";
                                                                sqlCom.CommandText += "        '" + readerCadMat["DPROS"].ToString() + "', ";
                                                                sqlCom.CommandText += "        NULL, ";
                                                                sqlCom.CommandText += "        1, ";
                                                                sqlCom.CommandText += "        9, ";
                                                                sqlCom.CommandText += "        1, ";
                                                                sqlCom.CommandText += "        " + perc.ToString() + ", ";
                                                                sqlCom.CommandText += "        5, ";
                                                                sqlCom.CommandText += "        '" + readerCadMat["CLFISCALS"].ToString() + "', ";
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
                                                                sqlCom.CommandText += " SET         \"descricao\"                         = '" + readerCadMat["DPROS"].ToString() + "',";
                                                                sqlCom.CommandText += "             \"id_mcj_situacao_tributaria\"        = 1,";
                                                                sqlCom.CommandText += "             \"perc_carga_tributaria_federal\"     = " + (readerCadMat["SITTRICMS"].ToString().Trim().Equals(string.Empty) ? "1" : readerCadMat["SITTRICMS"].ToString().Trim()) + "";
                                                                sqlCom.CommandText += " WHERE       \"cod_produto\"                       = '" + readerCadMat["CODIGO_EC5"].ToString() + "'";
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
                                                                sqlCom.CommandText += " VALUES('" + readerCadMat["CODIGO_EC5"].ToString() + "', ";
                                                                sqlCom.CommandText += "        '" + readerCadMat["DPROS"].ToString() + "', ";
                                                                sqlCom.CommandText += "        NULL, ";
                                                                sqlCom.CommandText += "        1,";
                                                                sqlCom.CommandText += "        " + (readerCadMat["SITTRICMS"].ToString().Trim().Equals(string.Empty) ? "1" : readerCadMat["SITTRICMS"].ToString().Trim()) + ", ";
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
                                                    sqlConfirm.CommandText += "     '" + readerCadMat["CODIGO_EC5"].ToString() + "', ";
                                                    sqlConfirm.CommandText += "     '" + company.Code + readerCadMat["CODIGO_EC5"].ToString() + "', ";
                                                    sqlConfirm.CommandText += "     '" + company.Code + "' ";
                                                    sqlConfirm.CommandText += " );";

                                                    sqlConfirm.ExecuteNonQuery();
                                                }
                                                catch
                                                {

                                                }
                                            }

                                            Console.WriteLine("LOJA:" + company.Code + " #" + (iterador++).ToString());
                                        }
                                    }
                                }
                                Console.WriteLine("########## LOJA " + company.Code + " FINALIZADA ##########");

                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine(ex.Message);
                            }

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
