using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MySQLBackupLibrary.Classes
{
    class DatabasesHandler
    {
        public void InsertNewDatabaseNode(DatabaseInfo databaseInfo)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Utilities.CONFIGURATION_LOCATION + "Databases.xml");

            //Create the database node
            XmlNode databaseNode = document.CreateNode(XmlNodeType.Element, "Database", null);
            XmlAttribute databaseNameAttr = document.CreateAttribute("Name");
            databaseNameAttr.InnerText = databaseInfo.DatabaseName;
            databaseNode.Attributes.Append(databaseNameAttr);

            //Create the Host node
            XmlNode hostNode = document.CreateElement("Host");
            hostNode.InnerText = databaseInfo.Host;
            databaseNode.AppendChild(hostNode);

            //Create the User node
            XmlNode userNode = document.CreateElement("User");
            userNode.InnerText = databaseInfo.User;
            databaseNode.AppendChild(userNode);

            //Create the Password Node
            XmlNode passwordNode = document.CreateElement("Password");
            passwordNode.InnerText = databaseInfo.Password;
            databaseNode.AppendChild(passwordNode);

            //Create the Backup Settings Node
            XmlNode backupSettingsNode = document.CreateNode(XmlNodeType.Element, "BackupSettings", null);

            //Create the Start Time Node
            XmlNode startTimeNode = document.CreateElement("StartTime");
            startTimeNode.InnerText = databaseInfo.StartTime.ToString();
            backupSettingsNode.AppendChild(startTimeNode);

            //Append backup Settings Node to database node
            databaseNode.AppendChild(backupSettingsNode);

            //Append the database node to the colections element Databases.
            document.DocumentElement.AppendChild(databaseNode);

            //Save the Databases file
            document.Save(Utilities.CONFIGURATION_LOCATION + "Databases.xml");
        }
    }
}
