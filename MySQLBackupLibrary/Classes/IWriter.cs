using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupLibrary.Classes
{
    interface IWriter
    {
        void OpenWriter();
        void Write(string data);
        void CloseWriter();
    }
}
