using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupLibrary.Classes
{
    class LogReader
    {
        private StreamReader reader = null;
        private string logLocation = Utilities.ROOT_LOCATION;

        public string LogLocation
        {
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

        public void OpenReader()
        {
            if (reader == null)
            {
                reader = new StreamReader(LogLocation + "Log.txt");
            }
            else
            {
                CloseReader();
                OpenReader();
            }
        }

        public string Read()
        {
            string text = null;

            if (reader != null)
            {
                text = reader.ReadToEnd();
            }

            return text;
        }

        public void CloseReader()
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
        }
    }
}
