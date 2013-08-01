using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySQLBackupLibrary.Classes;

namespace MySQLBackupLibrary
{
    public class Library
    {
        private readonly ConfigurationHandler configHandler = new ConfigurationHandler();

        /**
         * Constructor
         */
        public Library()
        {
            //Create the Configuration Locations
            ConfigLocationCreator configLocationCreator = new ConfigLocationCreator();
        }

        /**
         * Modify the Backup Location, where backups are saved.
         */
        public void ChangeBackupLocation(string location)
        {
            configHandler.ModifyBackupLocation(location);
        }

        /**
         * Retrieve the Backup Location from the configuration file.
         */
        public string GetBackupLocation()
        {
            return configHandler.GetBackupLocation();
        }

        /**
         * Modify the Delete Backups Older Than Days. If 0 days is specified, then backups will never be deleted.
         */
        public void ChangeDeleteBackupsOlderThanDays(uint days)
        {
            configHandler.ModifyDeleteBackupsOlderThan(days);
        }

        /**
         * Retrieve the Delete Backups Older Than N Days from the configuration file.
         */
        public int GetDeleteBackupsOlderThanDays()
        {
            return (int) configHandler.GetDeleteBackupsOlderThanDays();
        }
    }
}
