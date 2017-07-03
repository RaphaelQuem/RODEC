using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace RODEC.Modelo
{
    public class Config : IDisposable
    {
        public static Config GetConfig()
        {
           using (StreamReader config = new StreamReader("C:\\RODESEC5.INI"))
            {
                string x = config.ReadToEnd();
                return JsonConvert.DeserializeObject<Config>(x);
            }
        }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public IList<string> Lojas { get; set; }
        public IList<string> LojasNFCE { get; set; }
        public Dictionary<string, double> AliquotasEstaduais { get; set; }

        public void Dispose()
        {
            ConnectionStrings = null;
            Lojas = null;
            LojasNFCE = null;
            AliquotasEstaduais = null;
        }
    }
}
