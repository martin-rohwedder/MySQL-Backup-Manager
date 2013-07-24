using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MySQLBackupService.Classes
{
    class DeleteBackupFilesJob : IJob
    {
        /**
         * Execute method will execute be executed through the Quartz Scheduled Job.
         */
        public void Execute(IJobExecutionContext context)
        {
            this.DeleteOldBackupFiles();
        }

        /**
         * Delete old backup files. How old a backup file may become is specified by the user in the configurations file.
         */
        private void DeleteOldBackupFiles()
        {
            XmlDocument document = new XmlDocument();
            document.Load("Configuration/Configuration.xml");
            XmlNode numberOfDaysNode = document.SelectSingleNode("Configuration/DeleteBackupsOlderThan");
            XmlNode backupPathNode = document.SelectSingleNode("Configuration/BackupPath");

            string defaultBackupPath = AppDomain.CurrentDomain.BaseDirectory + @"backup\";

            if (!backupPathNode.InnerText.Trim().Equals(""))
            {
                if (!backupPathNode.InnerText.Trim().EndsWith(@"\"))
                {
                    defaultBackupPath = backupPathNode.InnerText + @"\";
                }
                else
                {
                    defaultBackupPath = backupPathNode.InnerText;
                }
            }

            //Delete Old Files. Add 1 to number of days, since it has to delete files older than the specified number of days
            this.ProcessDeleteFiles(defaultBackupPath, Convert.ToInt32(numberOfDaysNode.InnerText) + 1);

            this.Log(string.Format("Cleaning up backup files older than {0} days", numberOfDaysNode.InnerText), "INFO");
        }

        /**
         * Process which handle the deletion of files older than the specified number of days.
         */
        private void ProcessDeleteFiles(string targetDirectory, int numberOfDays)
        {
            string[] files = Directory.GetFiles(targetDirectory);
            foreach (string fileName in files)
            {
                DateTime fileCreatedDate = File.GetCreationTime(fileName);
                if (DateTime.Now - fileCreatedDate > TimeSpan.FromDays(numberOfDays))
                {
                    File.Delete(fileName);
                }
            }

            //Get all Subdirectories to the backup path
            string[] subDirs = Directory.GetDirectories(targetDirectory);
            foreach (string subDir in subDirs)
            {
                ProcessDeleteFiles(subDir, numberOfDays);
            }
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
