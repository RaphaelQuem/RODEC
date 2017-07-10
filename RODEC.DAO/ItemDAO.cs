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
        public List<Item> GetSingleItemToExport(string companyCode,string itemcode)
        {
            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = " ";
                comando.CommandText += " SELECT CODIGO_EC5 BarCode, " + Environment.NewLine;
                comando.CommandText += "      	SITTRICMS TaxSituation, " + Environment.NewLine;
                comando.CommandText += "      	CLFISCALS FiscalClassification, " + Environment.NewLine;
                comando.CommandText += "      	DPROS Description, " + Environment.NewLine;
                comando.CommandText += "      	ETIQS SingleLabel, " + Environment.NewLine;
                comando.CommandText += "      	'" + companyCode + "' CompanyCode " + Environment.NewLine;
                comando.CommandText += " FROM  " + Environment.NewLine;
                comando.CommandText += " ( " + Environment.NewLine;
                comando.CommandText += "              SELECT	DISTINCT	CASE B.ETIQS " + Environment.NewLine;
                comando.CommandText += " 										WHEN 'S' THEN C.CBARS " + Environment.NewLine;
                comando.CommandText += " 										ELSE A.cbars " + Environment.NewLine;
                comando.CommandText += " 									END CODIGO_EC5, " + Environment.NewLine;
                comando.CommandText += " 									ISNULL(A.SITTRICMS,'1') SITTRICMS,  " + Environment.NewLine;
                comando.CommandText += " 						          	A.CLFISCALS,   " + Environment.NewLine;
                comando.CommandText += " 									A.DPROS, " + Environment.NewLine;
                comando.CommandText += " 									ISNULL(B.ETIQS,'N') ETIQS, " + Environment.NewLine;
                comando.CommandText += " 									C.EMPS " + Environment.NewLine;
                comando.CommandText += "               FROM	    SIGCDPRO AS A WITH (NOLOCK)" + Environment.NewLine;
                comando.CommandText += " 						INNER  JOIN " + Environment.NewLine;
                comando.CommandText += " 						SIGCDUNI AS B WITH (NOLOCK) " + Environment.NewLine;
                comando.CommandText += " 							ON A.CUNIS = B.CUNIS " + Environment.NewLine;
                comando.CommandText += " 						LEFT JOIN " + Environment.NewLine;
                comando.CommandText += " 						SIGOPETQ AS C WITH (NOLOCK) " + Environment.NewLine;
                comando.CommandText += " 							ON   A.CPROS = C.CPROS " + Environment.NewLine;
                comando.CommandText += "               WHERE	A.MERCS IN('JOI','REL') " + Environment.NewLine;
                comando.CommandText += " ) AS Y " + Environment.NewLine;
                comando.CommandText += " WHERE		ISNULL(Y.CODIGO_EC5,0) != 0 " + Environment.NewLine;
                comando.CommandText += " AND		Y.CODIGO_EC5 = '"+ itemcode + "'"+ Environment.NewLine;



                reader = comando.ExecuteReader();

                return reader.ToModel<Item>();
            }

        }
        public List<Item> GetItemsToExport(string companyCode)
        {
            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = " ";
                comando.CommandText += " SELECT	DISTINCT TOP 100 CODIGO_EC5 BarCode, " + Environment.NewLine;
                comando.CommandText += "      	SITTRICMS TaxSituation, " + Environment.NewLine;
                comando.CommandText += "      	CLFISCALS FiscalClassification, " + Environment.NewLine;
                comando.CommandText += "      	DPROS Description, " + Environment.NewLine;
                comando.CommandText += "      	ETIQS SingleLabel, " + Environment.NewLine;
                comando.CommandText += "      	'" + companyCode + "' CompanyCode " + Environment.NewLine;
                comando.CommandText += " FROM  " + Environment.NewLine;
                comando.CommandText += " ( " + Environment.NewLine;
                comando.CommandText += "              SELECT	DISTINCT	CASE B.ETIQS " + Environment.NewLine;
                comando.CommandText += " 										WHEN 'S' THEN C.CBARS " + Environment.NewLine;
                comando.CommandText += " 										ELSE A.cbars " + Environment.NewLine;
                comando.CommandText += " 									END CODIGO_EC5, " + Environment.NewLine;
                comando.CommandText += " 									ISNULL(A.SITTRICMS,'1') SITTRICMS,  " + Environment.NewLine;
                comando.CommandText += " 						          	A.CLFISCALS,   " + Environment.NewLine;
                comando.CommandText += " 									A.DPROS, " + Environment.NewLine;
                comando.CommandText += " 									ISNULL(B.ETIQS,'N') ETIQS, " + Environment.NewLine;
                comando.CommandText += " 									C.EMPS " + Environment.NewLine;
                comando.CommandText += "               FROM	    SIGCDPRO AS A WITH (NOLOCK)" + Environment.NewLine;
                comando.CommandText += " 						INNER  JOIN " + Environment.NewLine;
                comando.CommandText += " 						SIGCDUNI AS B WITH (NOLOCK) " + Environment.NewLine;
                comando.CommandText += " 							ON A.CUNIS = B.CUNIS " + Environment.NewLine;
                comando.CommandText += " 						LEFT JOIN " + Environment.NewLine;
                comando.CommandText += " 						SIGOPETQ AS C WITH (NOLOCK) " + Environment.NewLine;
                comando.CommandText += " 							ON   A.CPROS = C.CPROS " + Environment.NewLine;
                comando.CommandText += "               WHERE	A.MERCS IN('JOI','REL') " + Environment.NewLine;
                comando.CommandText += " ) AS Y " + Environment.NewLine;
                comando.CommandText += " WHERE		ISNULL(Y.CODIGO_EC5,0) != 0 " + Environment.NewLine;
                comando.CommandText += " AND  		NOT EXISTS  " + Environment.NewLine;
                comando.CommandText += " ( " + Environment.NewLine;
                comando.CommandText += " 			SELECT * " + Environment.NewLine;
                comando.CommandText += " 			FROM SIGECETQ AS X " + Environment.NewLine;
                comando.CommandText += " 			WHERE X.CBARS  = CAST(Y.CODIGO_EC5 AS CHAR) " + Environment.NewLine;
                comando.CommandText += " 			AND   X.EMPS  = '" + companyCode + "' " + Environment.NewLine;
                comando.CommandText += " ) " + Environment.NewLine;
                comando.CommandText += " AND " + Environment.NewLine;
                comando.CommandText += " ( " + Environment.NewLine;
                comando.CommandText += "    ( Y.ETIQS ='S' AND  Y.EMPS= '" + companyCode + "') " + Environment.NewLine;
                comando.CommandText += "        OR " + Environment.NewLine;
                comando.CommandText += "    ( Y.ETIQS ='N') " + Environment.NewLine;
                comando.CommandText += " )  " + Environment.NewLine;

                reader = comando.ExecuteReader();

                return reader.ToModel<Item>();
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
