using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            #if DEBUG
            MySQLBackupService service = new MySQLBackupService();
            service.onDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            #else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MySQLBackupService() 
            };
            ServiceBase.Run(ServicesToRun);
            #endif
        }
    }
}
