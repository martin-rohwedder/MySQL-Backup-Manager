using Microsoft.Win32;
using MySQLBackupLibrary;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MySQLBackupService.Classes
{
    class MySqlDumpProcess : IJob
    {
        private readonly Library library = new Library();

        //Properties
        public string databaseName { get; set; }

        /**
         * Execute method will execute be executed through the Quartz Scheduled Job.
         */
        public void Execute(IJobExecutionContext context)
        {
            Process process = null;

            try
            {
                library.CreateBackup(process, databaseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                }
                this.databaseName = "";
            }
        }
    }
}
