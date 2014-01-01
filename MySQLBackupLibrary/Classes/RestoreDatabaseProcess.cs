using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupLibrary.Classes
{
    class RestoreDatabaseProcess
    {
        private Library library = null;
        private string dumpFilePath = "";
        private DatabaseInfo dbInfo = null;

        public RestoreDatabaseProcess(Library library, string dumpFilePath, DatabaseInfo dbInfo)
        {
            this.library = library;
            this.dumpFilePath = dumpFilePath;
            this.dbInfo = dbInfo;
        }

        /**
         * Restore a database, from a specific backup dump file.
         */
        public void RestoreDatabase(Process process)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "mysql";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.StandardOutputEncoding = Encoding.UTF8;
            psi.Arguments = string.Format(@"-u {0} -p{1} -h {2}", dbInfo.User, dbInfo.Password, dbInfo.Host);
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            process = Process.Start(psi);

            using (var stdin = new System.IO.StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8))
            using (var reader = System.IO.File.OpenText(@dumpFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    stdin.WriteLine(line);
                }
                stdin.Close();
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!this.HasErrorOccured(error))
            {
                library.LogMessage("INFO", "The database " + dbInfo.DatabaseName + ", has been restored, from this backup dump file '" + dumpFilePath + "'");
            }

            if (process != null)
            {
                process.WaitForExit();
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
                errorOccured = true;
            }

            return errorOccured;
        }
    }
}
