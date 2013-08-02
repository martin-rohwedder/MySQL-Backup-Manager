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
        private readonly DatabasesHandler databasesHandler = new DatabasesHandler();

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

        /**
         * Write output to a backup file for the specified database
         */
        public void WriteBackupFile(string databaseName, string output)
        {
            BackupWriter backupWriter = new BackupWriter();

            backupWriter.DatabaseName = databaseName;
            backupWriter.OpenWriter();
            backupWriter.Write(output);
            backupWriter.CloseWriter();

            backupWriter = null;
        }

        /**
         * Insert a new Database node to the Databases.xml file
         */
        public void InsertDatabaseNode(DatabaseInfo databaseInfo)
        {
            databasesHandler.InsertNewDatabaseNode(databaseInfo);
        }

        /**
         * Remove a specific database node from the Databases.xml file according to the database name provided.
         */
        public void RemoveDatabaseNode(string databaseName)
        {
            databasesHandler.RemoveExistingDatabaseNode(databaseName);
        }

        /**
         * Get a specific database node as a DatabaseInfo object
         */
        public DatabaseInfo RetrieveDatabaseNode(string databaseName)
        {
            return databasesHandler.GetSpecificDatabaseNode(databaseName);
        }

        /**
         * Update an existing database node. Database Name is the only thing, which can't be updated.
         * If this is the case create a new database node instead.
         */
        public void UpdateDatabaseNode(DatabaseInfo dbInfo)
        {
            databasesHandler.UpdateDatabaseNode(dbInfo);
        }
    }
}
