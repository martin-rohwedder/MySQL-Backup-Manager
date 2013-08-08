using MySQLBackupLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupManager.Pages.Content
{
    class DatabasesViewModel
    {
        private readonly Library library = new Library();
        private ObservableCollection<DatabaseInfo> databases = new ObservableCollection<DatabaseInfo>();

        public ObservableCollection<DatabaseInfo> Databases
        {
            get
            {
                foreach (DatabaseInfo dbInfo in library.RetrieveAllDatabaseNodes())
                {
                    databases.Add(dbInfo);
                }
                return databases;
            }
        }

        /**
         * Add a Database Info object to the collection
         */
        public void addDatabase(DatabaseInfo dbInfo)
        {
            this.databases.Add(dbInfo);
            library.InsertDatabaseNode(dbInfo);
            library.LogMessage("INFO", string.Format("The database {0} is now ready for backup", dbInfo.DatabaseName));
        }
    }
}
