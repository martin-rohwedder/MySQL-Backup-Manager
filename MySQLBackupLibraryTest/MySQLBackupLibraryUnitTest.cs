using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySQLBackupLibrary;
using System.IO;
using System.Xml;

namespace MySQLBackupLibraryTest
{
    [TestClass]
    public class MySQLBackupLibraryUnitTest
    {
        [TestMethod]
        public void IsConfigurationFileCreatedAtCorrectLocationTest()
        {
            Library lib = new Library();
            Assert.IsTrue(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Configuration.xml"));
            lib = null;
        }

        [TestMethod]
        public void IsDatabasesFileCreatedAtCorrectLocationTest()
        {
            Library lib = new Library();
            Assert.IsTrue(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Databases.xml"));
            lib = null;
        }

        [TestMethod]
        public void ModifyBackupLocationTest()
        {
            Library lib = new Library();
            lib.ChangeBackupLocation(@"C:\MyTestBackupLocation");

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Configuration.xml");
            XmlNode backupLocationNode = document.SelectSingleNode("Configuration/BackupLocation");

            Assert.AreEqual(@"C:\MyTestBackupLocation\", backupLocationNode.InnerText);

            //Delete the test directory
            Directory.Delete(@"C:\MyTestBackupLocation\");

            lib.ChangeBackupLocation(@"C:\ProgramData\MySQLBackup\Backup\");
            lib = null;
        }

        [TestMethod]
        public void ModifyDeleteBackupsOlderThanDaysTest()
        {
            Library lib = new Library();
            lib.ChangeDeleteBackupsOlderThanDays(14);

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Configuration.xml");
            XmlNode deleteBackupsOlderThanNode = document.SelectSingleNode("Configuration/DeleteBackupsOlderThan");

            Assert.AreEqual("14", deleteBackupsOlderThanNode.InnerText);
            lib.ChangeDeleteBackupsOlderThanDays(7);
            lib = null;
        }

        [TestMethod]
        public void RetrieveBackupLocationTest()
        {
            Library lib = new Library();
            string backupLocation = lib.GetBackupLocation();

            Assert.AreEqual(@"C:\ProgramData\MySQLBackup\Backup\", backupLocation);
            lib = null;
        }

        [TestMethod]
        public void RetrieveDeleteBackupOlderThanDaysTest()
        {
            Library lib = new Library();
            int days = lib.GetDeleteBackupsOlderThanDays();

            Assert.AreEqual(7, days);
            lib = null;
        }

        [TestMethod]
        public void WriteDataToBackupFileTest()
        {
            Library lib = new Library();
            string databaseName = "test_database";
            lib.WriteBackupFile(databaseName, "This is my Backup Test Output");

            //Only 1 file will be created always since this test method delete the file after the test
            string[] files = Directory.GetFiles(lib.GetBackupLocation() + databaseName + @"\");
            string fileName = Path.GetFileName(files[0]);

            StreamReader reader = new StreamReader(lib.GetBackupLocation() + databaseName + @"\" + fileName);
            string output = reader.ReadLine();
            reader.Close();

            Assert.AreEqual("This is my Backup Test Output", output);

            //Delete test file and directory
            File.Delete(lib.GetBackupLocation() + databaseName + @"\" + fileName);
            foreach (DirectoryInfo subDirectory in new DirectoryInfo(lib.GetBackupLocation()).GetDirectories()) subDirectory.Delete(true);

            lib = null;
        }

        [TestMethod]
        public void InsertNewDatabaseNodeToDatabasesXMLFileTest()
        {
            Library lib = new Library();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "test_database";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            lib.InsertDatabaseNode(dbInfo);

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Databases.xml");
            XmlNode databaseNode = document.SelectSingleNode("Databases/Database");
            string databaseNameAttr = databaseNode.Attributes["Name"].Value;

            Assert.AreEqual("test_database", databaseNameAttr);

            //remove the database node we just created
            databaseNode.ParentNode.RemoveChild(databaseNode);
            document.Save(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Databases.xml");

            lib = null;
        }

        [TestMethod]
        public void RemoveSpecificDatabaseNodeTest()
        {
            Library lib = new Library();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            lib.InsertDatabaseNode(dbInfo);
            lib.RemoveDatabaseNode(dbInfo.DatabaseName);

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Databases.xml");
            XmlNode databaseNode = document.SelectSingleNode("Databases/Database[@Name='" + dbInfo.DatabaseName + "']");

            Assert.IsNull(databaseNode);
            lib = null;
        }

        [TestMethod]
        public void RetrieveSpecificDatabaseNodeTest()
        {
            Library lib = new Library();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            lib.InsertDatabaseNode(dbInfo);

            DatabaseInfo dbInfo2 = lib.RetrieveDatabaseNode(dbInfo.DatabaseName);

            Assert.AreEqual("testdatabase", dbInfo2.DatabaseName);

            lib.RemoveDatabaseNode(dbInfo2.DatabaseName);
            lib = null;
        }

        [TestMethod]
        public void UpdateSpecificDatabaseNodeTest()
        {
            Library lib = new Library();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            lib.InsertDatabaseNode(dbInfo);

            //modify the dbInfo start time
            dbInfo.StartTimeHour = 22;
            dbInfo.StartTimeMinute = 59;

            lib.UpdateDatabaseNode(dbInfo);

            Assert.AreEqual("22:59:00", dbInfo.StartTime.ToString());

            lib.RemoveDatabaseNode(dbInfo.DatabaseName);
        }

        [TestMethod]
        public void LogMessageAtDefaultLocationTest()
        {
            Library lib = new Library();
            lib.LogMessage("INFO", "This is a test log message");

            StreamReader reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Log.txt");
            string output = reader.ReadLine();
            reader.Close();

            Assert.IsTrue(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Log.txt"));

            //Delete test Log file
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Log.txt");

            lib = null;
        }

        [TestMethod]
        public void GetLogTextAtDefaultLocation()
        {
            Library lib = new Library();
            Assert.IsNotNull(lib.GetLogText());
            lib = null;
        }

        [TestMethod]
        public void LogMessageAtCustomLocationTest()
        {
            Library lib = new Library();
            lib.LogMessage("ERROR", "This is a test log message", @"C:\LogMessageTest");

            StreamReader reader = new StreamReader(@"C:\LogMessageTest\Log.txt");
            string output = reader.ReadLine();
            reader.Close();

            Assert.IsTrue(File.Exists(@"C:\LogMessageTest\Log.txt"));

            //Delete test Log file and custom directory
            File.Delete(@"C:\LogMessageTest\Log.txt");
            Directory.Delete(@"C:\LogMessageTest\");

            lib = null;
        }

        [TestMethod]
        public void ClearLogAtDefaultLocationTest()
        {
            Library lib = new Library();
            lib.ClearLog();

            StreamReader reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Log.txt");
            string output = reader.ReadLine();
            reader.Close();

            Assert.IsNull(output);

            lib = null;
        }

        [TestMethod]
        public void ClearLogAtCustomLocationTest()
        {
            Library lib = new Library();
            lib.LogMessage("ERROR", "This is a test log message", @"C:\LogMessageTest");
            lib.ClearLog(@"C:\LogMessageTest");

            StreamReader reader = new StreamReader(@"C:\LogMessageTest\Log.txt");
            string output = reader.ReadLine();
            reader.Close();

            Assert.IsNull(output);

            //Delete test Log file and custom directory
            File.Delete(@"C:\LogMessageTest\Log.txt");
            Directory.Delete(@"C:\LogMessageTest\");
            lib = null;
        }

        [TestMethod]
        public void GetLogTextCustomDefaultLocation()
        {
            Library lib = new Library();
            lib.LogMessage("ERROR", "This is a test log message", @"C:\LogMessageTest");

            Assert.IsNotNull(lib.GetLogText(@"C:\LogMessageTest"));

            //Delete test Log file and custom directory
            File.Delete(@"C:\LogMessageTest\Log.txt");
            Directory.Delete(@"C:\LogMessageTest\");
            lib = null;
        }

        [TestMethod]
        public void RetrieveAllDatabaseNodesTest()
        {
            Library lib = new Library();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            lib.InsertDatabaseNode(dbInfo);
            dbInfo.DatabaseName = "NewDatabase";
            lib.InsertDatabaseNode(dbInfo);

            Assert.AreEqual(2, lib.RetrieveAllDatabaseNodes().Count);

            lib.RemoveDatabaseNode(dbInfo.DatabaseName);
            lib.RemoveDatabaseNode("TestDatabase");
            lib = null;
        }

        [TestMethod]
        public void GetConfigRootLocationTest()
        {
            Library lib = new Library();
            Assert.AreEqual(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\", lib.GetConfigRootLocation());
            lib = null;
        }

        [TestMethod]
        public void GetMySQLBinPathTest()
        {
            Library lib = new Library();
            Assert.IsTrue(lib.GetMySQLBinLocation().ToLower().Contains("mysql server"));
            lib = null;
        }
    }
}
