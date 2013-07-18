using MySQLBackupService.Classes;
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
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", "root", "admin", "localhost", "movstreamdb");
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output = "";
                string error = process.StandardError.ReadToEnd();

                if (!error.Equals(""))
                {
                    //Can't find database error
                    if (error.Contains("Got error: 1049"))
                    {
                        output = error.Substring(error.IndexOf("Got error: 1049"));
                    }
                    //Can't find host error
                    else if (error.Contains("Got error: 2005"))
                    {
                        output = error.Substring(error.IndexOf("Got error: 2005"));
                    }
                    //Wrong user/password error
                    else if (error.Contains("Got error: 1045"))
                    {
                        output = error.Substring(error.IndexOf("Got error: 1045"));
                    }
                    //Can't connect to MySQL (probably is server down)
                    else if (error.Contains("Got error: 2003"))
                    {
                        output = error.Substring(error.IndexOf("Got error: 2003"));
                    }
                    else
                    {
                        output = error;
                    }
                }
                else
                {
                    output = process.StandardOutput.ReadToEnd();
                }

                BackupWriter writer = new BackupWriter();
                writer.OpenWriter();
                writer.Write(output);

                process.WaitForExit();
                writer.CloseWriter();
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
