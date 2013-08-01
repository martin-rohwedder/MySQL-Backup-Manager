using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MySQLBackupLibrary.Classes
{
    class ConfigurationHandler
    {
        /**
         * Modify Backup Location
         */
        public void ModifyBackupLocation(string location)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");
            XmlNode backupLocationNode = document.SelectSingleNode("Configuration/BackupLocation");

            //If location doesn't ends with backslash, then add it before setting the backup location
            backupLocationNode.InnerText = (location.Trim().EndsWith(@"\")) ? location : location + @"\";

            document.Save(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");

            //Create the new directory if it doesn't exists
            CreateNewDirectory(backupLocationNode.InnerText);
        }

        /**
         * Modify the delete backups older than days. 0 respresent that there will never be deleted any backups.
         */
        public void ModifyDeleteBackupsOlderThan(uint days)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");
            XmlNode deleteBackupsOlderThanNode = document.SelectSingleNode("Configuration/DeleteBackupsOlderThan");

            deleteBackupsOlderThanNode.InnerText = Convert.ToString(days);
            document.Save(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");
        }

        /**
         * Create a new directory
         */
        private void CreateNewDirectory(string location)
        {
            if (!Directory.Exists(location)) { Directory.CreateDirectory(Path.GetDirectoryName(location)); }
        }
    }
}
