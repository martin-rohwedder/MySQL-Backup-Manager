using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupService.Classes
{
    /**
     * BackupWriter will handle logic to write the backup file in the correct directory.
     * 
     * The BackupFile will be a SQL dump file, and the filename will have a
     * format which looks like this Backup-{databasename}-{Date}.sql
     * 
     * A backup file will be created inside a directory coresponding to the database name.
     * The main backup path will be specified by the user in a configurations file, so all
     * backups are stored in one place.
     * 
     * @author Martin Rohwedder
     * @since 18-07-2013
     * @version 1.0
     */
    class BackupWriter : IBackupWriter
    {
        private const string MAIN_PATH = @"C:\test\backup\MyDatabase\";
        private StreamWriter writer = null;

        /**
         * Open a StreamWriter instance
         */
        public void OpenWriter()
        {
            if (writer == null)
            {
                if (!Directory.Exists(MAIN_PATH))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(MAIN_PATH));
                }
                writer = new StreamWriter(MAIN_PATH + "backup.sql");
            }
            else
            {
                CloseWriter();
            }
        }

        /**
         * Write data to the Backup File
         */
        public void Write(string data)
        {
            if (writer != null)
            {
                writer.WriteLine(data);
            }
        }

        /**
         * Close the Backup Write
         */
        public void CloseWriter()
        {
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }
    }
}
