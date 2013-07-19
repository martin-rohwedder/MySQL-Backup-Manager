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
            BackupWriter writer = new BackupWriter();
            Process process = null;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", "root", "admin", "localhost", "movstreamdb");
                psi.UseShellExecute = false;

                process = Process.Start(psi);

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!this.HasErrorOccured(error))
                {
                    writer.OpenWriter();
                    writer.Write(output);
                }

                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                writer.CloseWriter();
                if (process != null)
                {
                    process.Close();
                }
            }
        }

        protected override void OnStop()
        {
            
        }

        /**
         * Find out if an error has occured during the backup dump. Returns true if error has occured
         */
        private bool HasErrorOccured(string errorOutput)
        {
            bool errorOccured = false;

            //Can't find database error
            if (errorOutput.Contains("Got error: 1049"))
            {
                this.LogError(errorOutput.Substring(errorOutput.IndexOf("Got error: 1049")));
                errorOccured = true;
            }
            //Can't find host error
            else if (errorOutput.Contains("Got error: 2005"))
            {
                this.LogError(errorOutput.Substring(errorOutput.IndexOf("Got error: 2005")));
                errorOccured = true;
            }
            //Wrong user/password error
            else if (errorOutput.Contains("Got error: 1045"))
            {
                this.LogError(errorOutput.Substring(errorOutput.IndexOf("Got error: 1045")));
                errorOccured = true;
            }
            //Can't connect to MySQL (probably is server down)
            else if (errorOutput.Contains("Got error: 2003"))
            {
                this.LogError(errorOutput.Substring(errorOutput.IndexOf("Got error: 2003")));
                errorOccured = true;
            }

            return errorOccured;
        }

        private void LogError(string error)
        {
            LogWriter logWriter = new LogWriter();
            try
            {
                logWriter.OpenWriter();
                logWriter.Write(error);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                logWriter.CloseWriter();
            }
        }
    }
}
