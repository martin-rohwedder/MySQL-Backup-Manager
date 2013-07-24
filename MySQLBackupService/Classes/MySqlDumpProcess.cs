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
        //Properties
        public string error { get; set; }
        public string output { get; set; }
        public string databaseName { get; set; }
        public bool ServerDown { get; set; }

        /**
         * Execute method will execute be executed through the Quartz Scheduled Job.
         */
        public void Execute(IJobExecutionContext context)
        {
            BackupWriter writer = new BackupWriter();
            Process process = null;
            ServerDown = false;

            try
            {
                this.ProcessMySqlDump(process, writer);
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

        /**
         * Process The MySQL Dump for each database
         */
        private void ProcessMySqlDump(Process process, BackupWriter writer)
        {
            XmlDocument document = new XmlDocument();
            document.Load("Configuration/Databases.xml");

            XmlNodeList nodeList = document.SelectNodes("Databases/Database");
            foreach (XmlNode node in nodeList)
            {
                System.Threading.Thread.Sleep(1000);   //Let Application Sleep for 1 second, preventing multiple backup executions of the same database.

                if (!ServerDown)
                {
                    string[] timeArray = node["BackupSettings"].SelectSingleNode("StartTime").InnerText.Split(':');

                    if (Convert.ToInt32(timeArray[0]) == DateTime.Now.Hour && Convert.ToInt32(timeArray[1]) == DateTime.Now.Minute)
                    {
                        this.databaseName = node["DatabaseName"].InnerText;

                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "mysqldump";
                        psi.RedirectStandardInput = false;
                        psi.RedirectStandardOutput = true;
                        psi.RedirectStandardError = true;
                        psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", node["User"].InnerText, node["Password"].InnerText, node["Host"].InnerText, this.databaseName);
                        psi.UseShellExecute = false;
                        psi.CreateNoWindow = true;

                        process = Process.Start(psi);

                        this.output = process.StandardOutput.ReadToEnd();
                        this.error = process.StandardError.ReadToEnd();

                        if (!this.HasErrorOccured(this.error))
                        {
                            writer.DatabaseName = this.databaseName;
                            writer.OpenWriter();
                            writer.Write(this.output);
                            this.Log("Database backup created of the database " + this.databaseName, "INFO");
                        }
                    }
                }

                if (process != null)
                {
                    process.WaitForExit();
                }
            }
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
                this.Log(errorOutput.Substring(errorOutput.IndexOf("Got error: 1049")), "ERROR");
                errorOccured = true;
            }
            //Can't find host error
            else if (errorOutput.Contains("Got error: 2005"))
            {
                this.Log(errorOutput.Substring(errorOutput.IndexOf("Got error: 2005")), "ERROR");
                errorOccured = true;
            }
            //Wrong user/password error
            else if (errorOutput.Contains("Got error: 1045"))
            {
                this.Log(errorOutput.Substring(errorOutput.IndexOf("Got error: 1045")), "ERROR");
                errorOccured = true;
            }
            //Can't connect to MySQL (probably is server down)
            else if (errorOutput.Contains("Got error: 2003"))
            {
                this.Log(errorOutput.Substring(errorOutput.IndexOf("Got error: 2003")).TrimEnd('\r', '\n'), "ERROR");
                this.ServerDown = true;
                errorOccured = true;
            }

            return errorOccured;
        }

        /**
         * Log message to the Log.txt file. Type indicates the message type, eg. Error, information etc.
         */
        private void Log(string output, string type)
        {
            LogWriter logWriter = new LogWriter();
            try
            {
                logWriter.OpenWriter();
                logWriter.Type = type;
                logWriter.Write(output);
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
