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
        private StreamWriter writer;

        public string DatabaseName { get; set; }

        public void OpenWriter()
        {
            throw new NotImplementedException();
        }

        public void Write(string data)
        {
            throw new NotImplementedException();
        }

        public void CloseWriter()
        {
            throw new NotImplementedException();
        }
    }
}
