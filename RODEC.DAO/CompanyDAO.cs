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
    public class CompanyDAO
    {
        private SqlConnection connection;
        private SqlDataReader reader;
        public CompanyDAO(SqlConnection con)
        {
            connection = con;
        }
        public List<Company> GetCompaniesIn(IList<string> lojas)
        {
            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = " ";
                comando.CommandText += " SELECT CEMPS Code, ESTAS State";
                comando.CommandText += " FROM   SIGCDEMP ";
                comando.CommandText += " WHERE  CEMPS IN ('" + string.Join("', '", lojas) + "') ";
                comando.CommandText += " ORDER BY CEMPS";

                reader = comando.ExecuteReader();

                return reader.ToModel<Company>();
            }
        }
        public Company GetNext()
        {
            return reader.GetNext<Company>();
        }
    }
}
