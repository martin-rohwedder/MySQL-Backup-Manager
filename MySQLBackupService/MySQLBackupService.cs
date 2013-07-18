using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupService
{
    public partial class MySQLBackupService : ServiceBase
    {
        public MySQLBackupService()
        {
            InitializeComponent();
        }

        public void onDebug()
        {
            this.OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                DateTime time = DateTime.Now;
                string path = "C:\\test\\Backup" + time.Hour + time.Minute + time.Second + ".sql";
                StreamWriter writer = new StreamWriter(path);

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", "root", "admin", "localhost", "movstreamdb");
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output = process.StandardOutput.ReadToEnd();
                writer.WriteLine(output);
                process.WaitForExit();
                writer.Close();
                process.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnStop()
        {
            
        }

        public static void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }

        public static void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }
    }
}
