using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MySQLBackupLibrary.Classes
{
    class ConfigLocationCreator
    {
        private readonly string CONFIGURATION_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\";
        private readonly string DEFAULT_BACKUP_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Backup\";

        public string ConfigurationLocation { get { return this.CONFIGURATION_LOCATION; } }
        public string DefaultBackupLocation { get{ return this.DEFAULT_BACKUP_LOCATION; } }

        /**
         * Constructor
         */
        public ConfigLocationCreator()
        {
            //Build Common Application Data Locations used by the library
            BuildCommonApplicationDataLocation();

            //Check if Configuration.xml and Databases.xml files has been created. If not create them
            BuildConfigurationFiles();
        }

        /**
         * Build the directory structure for the configuration and the backup in the common application data location if it doesn't exists
         */
        private void BuildCommonApplicationDataLocation()
        {
            if (!Directory.Exists(CONFIGURATION_LOCATION)) { Directory.CreateDirectory(Path.GetDirectoryName(CONFIGURATION_LOCATION)); }
            if (!Directory.Exists(DEFAULT_BACKUP_LOCATION)) { Directory.CreateDirectory(Path.GetDirectoryName(DEFAULT_BACKUP_LOCATION)); }
        }

        /**
         * Check if the Configuration.xml and Databases.xml exists in the Configurations location. If they doesn't exists, then create the files.
         */
        private void BuildConfigurationFiles()
        {
            if (!File.Exists(CONFIGURATION_LOCATION + "Configuration.xml")) { CreateNewConfigurationFile(); }
            if (!File.Exists(CONFIGURATION_LOCATION + "Databases.xml")) { CreateNewDatabasesFile(); }
        }

        /**
         * Create the Configuration.xml file with default values
         */
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

            document.Save(CONFIGURATION_LOCATION + "Configuration.xml");
        }

        /**
         * Create the Databases.xml file with default values.
         */
        private void CreateNewDatabasesFile()
        {
            XmlDocument document = new XmlDocument();

            //Create declaration
            XmlNode declarationNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(declarationNode);

            //Create Databases node
            XmlNode databasesNode = document.CreateElement("Databases");
            document.AppendChild(databasesNode);

            document.Save(CONFIGURATION_LOCATION + "Databases.xml");
        }
    }
}
