using MySQLBackupLibrary;
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
        private readonly Library library = new Library();

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
            int days = library.GetDeleteBackupsOlderThanDays();
            string backupLocation = library.GetBackupLocation();

            if (days > 0)
            {
                //Delete Old Files. Add 1 to number of days, since it has to delete files older than the specified number of days
                this.ProcessDeleteFiles(backupLocation, days + 1);

                library.LogMessage("INFO", string.Format("Cleaning up backup files older than {0} days", days));
            }
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
    }
}
