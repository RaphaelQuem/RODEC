using RODEC.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RODEC.DAO
{
    public class FiscalItemDAO
    {
        private SqlConnection connection;
        public FiscalItemDAO(SqlConnection con)
        {
            connection = con;
        }
        private bool Exists(decimal barCode)
        {

            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = " SELECT  *";
                comando.CommandText += " FROM   MCJ_PRODUTO";
                comando.CommandText += " WHERE  \"cod_produto\" = '" + barCode + "'";

                using (SqlDataReader rdr = comando.ExecuteReader())
                {
                    return rdr.Read();
                }
            }
        }
        public void Save(Item item)
        {
            if (Exists(item.BarCode))
            {
                Update(item);
            }
            else
            {
                Insert(item);
            }
        }
        private void Insert(Item item)
        {
            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = " INSERT INTO MCJ_PRODUTO( ";
                comando.CommandText += "       \"cod_produto\", ";
                comando.CommandText += "       \"descricao\", ";
                comando.CommandText += "       \"preco_unitario\", ";
                comando.CommandText += "       \"id_mcj_situacao_tributaria\", ";
                comando.CommandText += "       \"perc_carga_tributaria_federal\", ";
                comando.CommandText += "       \"fl_pis_cofins\" ";
                comando.CommandText += "        ) ";
                comando.CommandText += " VALUES('" + item.BarCode + "', ";
                comando.CommandText += "        '" + item.Description + "', ";
                comando.CommandText += "        NULL, ";
                comando.CommandText += "        1,";
                comando.CommandText += "        " + (item.TaxSituation.ToString().Trim().Equals(string.Empty) ? "1" : item.TaxSituation.ToString().Trim()) + ", ";
                comando.CommandText += "        1)";

                comando.ExecuteNonQuery();
            }
        }
        private void Update(Item item)
        {
            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = " UPDATE MCJ_PRODUTO ";
                comando.CommandText += " SET         \"descricao\"                         = '" + item.Description + "',";
                comando.CommandText += "             \"id_mcj_situacao_tributaria\"        = 1,";
                comando.CommandText += "             \"perc_carga_tributaria_federal\"     = " + (item.TaxSituation.ToString().Trim().Equals(string.Empty) ? "1" : item.TaxSituation.ToString().Trim()) + "";
                comando.CommandText += " WHERE       \"cod_produto\"                       = '" + item.BarCode + "'";

                comando.ExecuteNonQuery();
            }
        }
        public void SaveNFCE(Item item)
        {
            if (Exists(item.BarCode))
            {
                UpdateNFCE(item);
            }
            else
            {
                InsertNFCE(item);
            }
        }
        private void InsertNFCE(Item item)
        {
            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = " INSERT INTO MCJ_PRODUTO( ";
                comando.CommandText += "       \"cod_produto\", ";
                comando.CommandText += "       \"descricao\", ";
                comando.CommandText += "       \"preco_unitario\", ";
                comando.CommandText += "       \"id_mcj_situacao_tributaria\", ";
                comando.CommandText += "       \"perc_carga_tributaria_federal\", ";
                comando.CommandText += "       \"fl_pis_cofins\", ";
                comando.CommandText += "       \"perc_carga_tributaria_estadual\", ";
                comando.CommandText += "       \"perc_cargatributaria_municipal\", ";
                comando.CommandText += "       \"cod_ncm\", ";
                comando.CommandText += "       \"ex_tipi\", ";
                comando.CommandText += "       \"set_origem_produto\", ";
                comando.CommandText += "       \"cod_cest\", ";
                comando.CommandText += "       \"perc_carga_tributaria\" ";
                comando.CommandText += "        ) ";
                comando.CommandText += " VALUES('" + item.BarCode + "', ";
                comando.CommandText += "        '" + item.Description + "', ";
                comando.CommandText += "        NULL, ";
                comando.CommandText += "        1, ";
                comando.CommandText += "        9, ";
                comando.CommandText += "        1, ";
                comando.CommandText += "        " + item.Percentage.ToString() + ", ";
                comando.CommandText += "        5, ";
                comando.CommandText += "        '" + item.FiscalClassification + "', ";
                comando.CommandText += "        0, ";
                comando.CommandText += "        0, ";
                comando.CommandText += "        NULL, ";
                comando.CommandText += "        NULL)";

                comando.ExecuteNonQuery();
            }
        }
        private void UpdateNFCE(Item item)
        {
            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = " UPDATE MCJ_PRODUTO ";
                comando.CommandText += " SET         \"descricao\"                         = '" + item.Description + "',";
                comando.CommandText += "             \"preco_unitario\"                    = NULL,";
                comando.CommandText += "             \"id_mcj_situacao_tributaria\"        = 1,";
                comando.CommandText += "             \"perc_carga_tributaria_federal\"     = 9,";
                comando.CommandText += "             \"fl_pis_cofins\"                     = 1,";
                comando.CommandText += "             \"perc_carga_tributaria_estadual\"    = " + item.Percentage.ToString() + ",";
                comando.CommandText += "             \"perc_cargatributaria_municipal\"    = 5,";
                comando.CommandText += "             \"cod_ncm\"                           = '" + item.FiscalClassification + "',";
                comando.CommandText += "             \"ex_tipi\"                           = 0,";
                comando.CommandText += "             \"set_origem_produto\"                = 0,";
                comando.CommandText += "             \"cod_cest\"                          = NULL,";
                comando.CommandText += "             \"perc_carga_tributaria\"             = NULL";
                comando.CommandText += " WHERE       \"cod_produto\"                       = '" + item.BarCode + "'";

                comando.ExecuteNonQuery();
            }
        }
    }


}
