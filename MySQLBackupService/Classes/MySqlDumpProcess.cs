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
        public string error { get; set; }
        public string output { get; set; }
        public string databaseName { get; set; }
        public bool ServerDown { get; set; }

        /**
         * Execute method will execute be executed through the Quartz Scheduled Job.
         */
        public void Execute(IJobExecutionContext context)
        {
            Process process = null;
            ServerDown = false;

            try
            {
                this.ProcessMySqlDump(process);
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
                this.output = "";
                this.error = "";
            }
        }

        /**
         * Process The MySQL Dump for each database
         */
        private void ProcessMySqlDump(Process process)
        {
            foreach (DatabaseInfo dbInfo in library.RetrieveAllDatabaseNodes())
            {
                System.Threading.Thread.Sleep(1000);   //Let Application Sleep for 1 second, preventing multiple backup executions of the same database.

                if (!ServerDown)
                {
                    string[] startTime = dbInfo.StartTime.ToString().Split(':');
                    if (Convert.ToInt32(startTime[0]) == DateTime.Now.Hour && Convert.ToInt32(startTime[1]) == DateTime.Now.Minute)
                    {
                        this.databaseName = dbInfo.DatabaseName;

                        ProcessStartInfo psi = new ProcessStartInfo();

                        string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
                        if (path == null)
                        {
                            Environment.SetEnvironmentVariable("PATH", this.RetrieveMySQLInstallationBinPath(), EnvironmentVariableTarget.User);
                        }
                        else if (!path.Contains(this.RetrieveMySQLInstallationBinPath()))
                        {
                            path += ";" + this.RetrieveMySQLInstallationBinPath();
                            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.User);
                        }

                        psi.FileName = "mysqldump";
                        psi.RedirectStandardInput = false;
                        psi.RedirectStandardOutput = true;
                        psi.RedirectStandardError = true;
                        psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", dbInfo.User, dbInfo.Password, dbInfo.Host, this.databaseName);
                        psi.UseShellExecute = false;
                        psi.CreateNoWindow = true;

                        process = Process.Start(psi);

                        this.output = process.StandardOutput.ReadToEnd();
                        this.error = process.StandardError.ReadToEnd();

                        if (!this.HasErrorOccured(this.error))
                        {
                            library.WriteBackupFile(this.databaseName, this.output);
                            library.LogMessage("INFO", "Backup created of the database " + this.databaseName);
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
         * Lookup the MySQL Installation Bin path from the registry
         */
        private string RetrieveMySQLInstallationBinPath()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\MySQL AB");
            foreach (string subkey in registryKey.GetSubKeyNames())
            {
                if (subkey.ToLower().Contains("mysql server"))
                {
                    RegistryKey myKey = registryKey.OpenSubKey(subkey);
                    string location = (string)myKey.GetValue("Location");
                    return location + @"bin\";
                }
            }
            return null;
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
                library.LogMessage("ERROR", errorOutput.Substring(errorOutput.IndexOf("Got error: 1049")));
                errorOccured = true;
            }
            //Can't find host error
            else if (errorOutput.Contains("Got error: 2005"))
            {
                library.LogMessage("ERROR", errorOutput.Substring(errorOutput.IndexOf("Got error: 2005")));
                errorOccured = true;
            }
            //Wrong user/password error
            else if (errorOutput.Contains("Got error: 1045"))
            {
                library.LogMessage("ERROR", errorOutput.Substring(errorOutput.IndexOf("Got error: 1045")));
                errorOccured = true;
            }
            //Can't connect to MySQL (probably is server down)
            else if (errorOutput.Contains("Got error: 2003"))
            {
                library.LogMessage("ERROR", errorOutput.Substring(errorOutput.IndexOf("Got error: 2003")).TrimEnd('\r', '\n'));
                this.ServerDown = true;
                errorOccured = true;
            }

            return errorOccured;
        }
    }
}
