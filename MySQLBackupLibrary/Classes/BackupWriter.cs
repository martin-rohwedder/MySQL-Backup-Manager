using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupLibrary.Classes
{
    class BackupWriter : IWriter
    {
        private readonly ConfigurationHandler configHandler = new ConfigurationHandler();
        private StreamWriter writer;

        public string DatabaseName { get; set; }

        public void OpenWriter()
        {
            if (writer == null)
            {
                string backupLocation = this.configHandler.GetBackupLocation();
                if (!Directory.Exists(backupLocation + DatabaseName + @"\"))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(backupLocation + DatabaseName + @"\"));
                }

                DateTime dateTime = DateTime.Now;
                writer = new StreamWriter(backupLocation + DatabaseName + @"\" + string.Format("{0}_{1}-{2}-{3}_{4}.dump", DatabaseName, dateTime.Day, dateTime.Month, dateTime.Year, dateTime.ToString("HHmm")), false, Encoding.UTF8);
            }
            else
            {
                CloseWriter();
                OpenWriter();
            }
        }

        public void Write(string data)
        {
            if (writer != null)
            {
                writer.WriteLine(data);
            }
        }

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
