using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupService.Classes
{
    /**
     * Interface for Backup Writer
     * 
     * @author Martin Rohwedder
     * @since 18-07-2013
     * @version 1.0
     */
    interface IWriter
    {
        void OpenWriter();
        void Write(string data);
        void CloseWriter();
    }
}
