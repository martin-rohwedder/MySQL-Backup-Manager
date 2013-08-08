using MySQLBackupManager.Pages.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupManager
{
    class Globals
    {
        private static DatabasesViewModel databasesViewModel;

        public static DatabasesViewModel DatabasesViewModel
        {
            get
            {
                if (databasesViewModel == null)
                {
                    databasesViewModel = new DatabasesViewModel();
                }
                return new DatabasesViewModel();
            }
        }
    }
}
