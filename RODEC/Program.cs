using RODEC.Controller;
using RODEC.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RODEC
{
    class Program
    {
        static IntegrationController controller = new IntegrationController(null);
        IList<string> connectionStrings = new List<string>();
        static void Main(string[] args)
        {
            while (true)
            {
                controller.Export();
                
            }
        }
    }
}
