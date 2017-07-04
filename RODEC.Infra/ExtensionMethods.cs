using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RODEC.Infra
{
    public static class ExtensionMethods
    {
        public static T GetNext<T>(this SqlDataReader rdr)
        {

            if (rdr.Read())
            {
                T obj = (T)Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo info in obj.GetType().GetProperties())
                {
                    try
                    {
                        if (rdr[info.Name].GetType().Name.Equals("String"))
                            info.SetValue(obj, rdr[info.Name].ToString().Trim());
                        else
                            info.SetValue(obj, rdr[info.Name]);
                    }
                    catch(Exception ex)
                    {
                    }
                }
                return obj;
            }
            else
                return default(T);
        }
        public static List<T> ToModel<T>(this IDataReader rdr)
        {

            List<T> objlist = new List<T>();

            while (rdr.Read())
            {
                T obj = (T)Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo info in obj.GetType().GetProperties())
                {
                    try
                    {
                        if (rdr[info.Name].GetType().Name.Equals("String"))
                            info.SetValue(obj, rdr[info.Name].ToString().Trim());
                        else
                            info.SetValue(obj, rdr[info.Name]);
                    }
                    catch
                    { }
                }
                objlist.Add(obj);
            }

            return objlist.Count.Equals(0) ? null : objlist;
        }
    }
}
