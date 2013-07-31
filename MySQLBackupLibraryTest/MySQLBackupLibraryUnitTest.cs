using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySQLBackupLibrary;
using System.IO;

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
        }

        [TestMethod]
        public void IsDatabasesFileCreatedAtCorrectLocationTest()
        {
            Library lib = new Library();

            Assert.IsTrue(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Databases.xml"));
        }
    }
}
