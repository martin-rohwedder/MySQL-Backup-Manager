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
        private ObservableCollection<DatabaseInfo> databases;
        public ObservableCollection<DatabaseInfo> Databases
        {
            get
            {
                return databases;
            }
        }

        public DatabasesViewModel()
        {
            if (databases == null)
            {
                databases = new ObservableCollection<DatabaseInfo>();
            }
            foreach (DatabaseInfo dbInfo in library.RetrieveAllDatabaseNodes())
            {
                databases.Add(dbInfo);
            }
        }
    }
}
