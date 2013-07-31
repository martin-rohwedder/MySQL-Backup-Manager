using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MySQLBackupLibrary
{
    public class Library
    {
        private const string CONFIGURATION_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"MySQLBackup\Configuration\";
        private const string DEFAULT_BACKUP_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"MySQLBackup\Backup\";

        public Library()
        {

        }

        private void CheckForConfigurationFile()
        {
            if (!File.Exists(CONFIGURATION_LOCATION + "Configuration.xml"))
            {

            }
        }

        private void CreateNewConfigurationFile()
        {
            XmlDocument document = new XmlDocument();

            //Create declaration
            XmlNode declarationNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(declarationNode);

            //Create configuration node
            XmlNode configNode = document.CreateElement("Configuration");
            document.AppendChild(configNode);

            //Create BackupLocation node
            XmlNode backupLocationNode = document.CreateElement("BackupLocation");
            backupLocationNode.AppendChild(document.CreateTextNode(DEFAULT_BACKUP_LOCATION));
            configNode.AppendChild(backupLocationNode);

            //Create DeleteBackupsOlderThan node
            XmlNode deleteBackupsOlderThanNode = document.CreateElement("DeleteBackupsOlderThan");
            deleteBackupsOlderThanNode.AppendChild(document.CreateTextNode("7"));
            configNode.AppendChild(deleteBackupsOlderThanNode);

            // TODO: document.Save();
        }
    }
}
