using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupLibrary.Classes
{
    class LogWriter : IWriter
    {
        private StreamWriter writer = null;
        private string logLocation = Utilities.ROOT_LOCATION;

        public string LogLocation {
            get
            {
                return this.logLocation;
            }
            set
            {
                logLocation = (value.EndsWith(@"\")) ? value : value + @"\";
                if (!Directory.Exists(logLocation)) { Directory.CreateDirectory(Path.GetDirectoryName(logLocation)); }
            }
        }
        public string Type { get; set; }

        public void OpenWriter()
        {
            if (writer == null)
            {
                writer = File.AppendText(logLocation + "Log.txt");
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
                DateTime dateTime = DateTime.Now;
                writer.WriteLine(string.Format("{0}/{1}/{2} {3} - {4} - {5}", dateTime.Day, dateTime.Month, dateTime.Year, dateTime.ToString("HH:mm"), Type, data));
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
