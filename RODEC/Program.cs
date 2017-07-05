using RODEC.Controller;
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
        static IntegrationController controller = new IntegrationController();
        private static Thread exportingThread;
        IList<string> connectionStrings = new List<string>();
        static void Main(string[] args)
        {
            exportingThread = new Thread(controller.ExportItems);
            exportingThread.Start();

            while(exportingThread.IsAlive)
                Console.Write("x");
        }
    }
}
