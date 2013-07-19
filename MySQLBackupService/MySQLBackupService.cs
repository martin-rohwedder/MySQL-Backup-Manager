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
using System.Xml;

namespace MySQLBackupService
{
    public partial class MySQLBackupService : ServiceBase
    {
        public string error { get; set; }
        public string output { get; set; }
        public string databaseName { get; set; }

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
                this.ProcessMySqlDump(process);

                if (!this.HasErrorOccured(this.error))
                {
                    writer.DatabaseName = this.databaseName;
                    writer.OpenWriter();
                    writer.Write(this.output);
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
                this.databaseName = "";
                this.output = "";
                this.error = "";
            }
        }

        protected override void OnStop()
        {
            
        }

        private void ProcessMySqlDump(Process process)
        {
            XmlDocument document = new XmlDocument();
            document.Load("Configuration/Databases.xml");
            XmlNode host = document.SelectSingleNode("/Databases/Database/Host/text()");
            XmlNode user = document.SelectSingleNode("/Databases/Database/User/text()");
            XmlNode password = document.SelectSingleNode("/Databases/Database/Password/text()");
            XmlNode databaseName = document.SelectSingleNode("/Databases/Database/DatabaseName/text()");
            this.databaseName = databaseName.Value;

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "mysqldump";
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", user.Value, password.Value, host.Value, this.databaseName);
            psi.UseShellExecute = false;

            process = Process.Start(psi);

            this.output = process.StandardOutput.ReadToEnd();
            this.error = process.StandardError.ReadToEnd();
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
