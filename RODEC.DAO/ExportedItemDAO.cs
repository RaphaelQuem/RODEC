using RODEC.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RODEC.DAO
{
    public class ExportedItemDAO
    {
        private SqlConnection connection;
        public ExportedItemDAO(SqlConnection con)
        {
            connection = con;
        }
        public void Insert(Item item)
        {
            using (SqlCommand comando = connection.CreateCommand())
            {
                comando.CommandText = "";
                comando.CommandText += " INSERT INTO SIGECETQ ";
                comando.CommandText += " ( ";
                comando.CommandText += "     \"cbars\", ";
                comando.CommandText += "     \"cidchaves\", ";
                comando.CommandText += "     \"emps\" ";
                comando.CommandText += " ) ";
                comando.CommandText += " VALUES ";
                comando.CommandText += " ( ";
                comando.CommandText += "     '" + item.BarCode + "', ";
                comando.CommandText += "     '" + item.CompanyCode + item.BarCode + "', ";
                comando.CommandText += "     '" + item.CompanyCode + "' ";
                comando.CommandText += " );";

                comando.ExecuteNonQuery();
            }
        }

    }
}
