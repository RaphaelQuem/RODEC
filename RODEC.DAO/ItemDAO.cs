using RODEC.Infra;
using RODEC.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RODEC.DAO
{
    public class ItemDAO
    {
        private SqlConnection connection;
        private SqlDataReader reader;
        public ItemDAO (SqlConnection con)
        {
            connection = con;
        }
        public void GetItemsToExport(string companyCode)
        {
            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = " ";
                comando.CommandText += " SELECT	CODIGO_EC5 BarCode, ";
                comando.CommandText += "      	SITTRICMS TaxSituation, ";
                comando.CommandText += "      	CLFISCALS FiscalClassification, ";
                comando.CommandText += "      	DPROS Description, ";
                comando.CommandText += "      	ETIQS SingleLabel, ";
                comando.CommandText += "      	EMPS CompanyCode ";
                comando.CommandText += " FROM  ";
                comando.CommandText += " ( ";
                comando.CommandText += "              SELECT	DISTINCT	CASE B.ETIQS ";
                comando.CommandText += " 										WHEN 'S' THEN C.CBARS ";
                comando.CommandText += " 										ELSE A.cbars ";
                comando.CommandText += " 									END CODIGO_EC5, ";
                comando.CommandText += " 									ISNULL(A.SITTRICMS,'1') SITTRICMS,  ";
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
                comando.CommandText += " WHERE		ISNULL(Y.CODIGO_EC5,0) != 0 ";
                comando.CommandText += " AND  		NOT EXISTS  ";
                comando.CommandText += " ( ";
                comando.CommandText += " 			SELECT * ";
                comando.CommandText += " 			FROM SIGECETQ AS X ";
                comando.CommandText += " 			WHERE X.CBARS  = CAST(Y.CODIGO_EC5 AS CHAR) ";
                comando.CommandText += " 			AND   X.EMPS  = '" + companyCode + "' ";
                comando.CommandText += " ) ";
                comando.CommandText += " AND ";
                comando.CommandText += " ( ";
                comando.CommandText += "    ( Y.ETIQS ='S' AND  Y.EMPS= '" + companyCode + "') ";
                comando.CommandText += "        OR ";
                comando.CommandText += "    ( Y.ETIQS ='N') ";
                comando.CommandText += " ) ";

                reader = comando.ExecuteReader();
            }

        }
        public Item GetNext()
        {
            return reader.GetNext<Item>();
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


    }
}
