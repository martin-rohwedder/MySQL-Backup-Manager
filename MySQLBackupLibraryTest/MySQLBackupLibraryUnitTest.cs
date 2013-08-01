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
            lib.ChangeBackupLocation(@"C:\ProgramData\MySQLBackup\Backup\");
            lib = null;
        }
    }
}
