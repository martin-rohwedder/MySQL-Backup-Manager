using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
    class BackupWriter : IWriter
    {
        private string default_path = AppDomain.CurrentDomain.BaseDirectory + @"backup\";
        private StreamWriter writer = null;

        //Properties
        public string DatabaseName { get; set; }

        /**
         * Open a StreamWriter instance
         */
        public void OpenWriter()
        {
            if (writer == null)
            {
                if (!Directory.Exists(this.GetBackupPath() + DatabaseName + @"\"))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(this.GetBackupPath() + DatabaseName + @"\"));
                }
                DateTime dateTime = DateTime.Now;
                writer = new StreamWriter(this.GetBackupPath() + DatabaseName + @"\" + string.Format("{0}_{1}-{2}-{3}_{4}{5}.sql", DatabaseName, dateTime.Day, dateTime.Month, dateTime.Year, dateTime.Hour, dateTime.Minute));
            }
            else
            {
                CloseWriter();
                OpenWriter();
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

        /**
         * Retrieve the backup path specififed in the configuration. If empty use default path.
         */
        private string GetBackupPath()
        {
            XmlDocument document = new XmlDocument();
            document.Load("Configuration/Configuration.xml");
            XmlNode backupPathNode = document.SelectSingleNode("Configuration/BackupPath");

            //Return default path if node value is empty
            if (backupPathNode.InnerText.Trim().Equals(""))
            {
                return default_path;
            }
            else
            {
                //Insert backslash at the end of path if it's not present
                if (!backupPathNode.InnerText.Trim().EndsWith(@"\"))
                {
                    return backupPathNode.InnerText + @"\";
                }
                else
                {
                    return backupPathNode.InnerText;
                }
            }
        }
    }
}
