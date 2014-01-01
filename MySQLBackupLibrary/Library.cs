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

        public List<DatabaseInfo> RetrieveAllDatabaseNodes()
        {
            return databasesHandler.GetAllDatabaseNodes();
        }

        /**
         * Update an existing database node. Database Name is the only thing, which can't be updated.
         * If this is the case create a new database node instead.
         */
        public void UpdateDatabaseNode(DatabaseInfo dbInfo)
        {
            databasesHandler.UpdateDatabaseNode(dbInfo);
        }

        /**
         * Log a Message. Type indicates the importance of the message. Usually you'll use Information, Warning or error as the type.
         */
        public void LogMessage(string type, string message)
        {
            LogWriter logWriter = new LogWriter();
            logWriter.Type = type;

            logWriter.OpenWriter();
            logWriter.Write(message);
            logWriter.CloseWriter();
        }

        /**
         * Log a Message. Type indicates the importance of the message. Usually you'll use Information, Warning or error as the type.
         * Location should be the path where you want to save the log file.
         */
        public void LogMessage(string type, string message, string location)
        {
            LogWriter logWriter = new LogWriter();
            logWriter.Type = type;
            logWriter.LogLocation = location;

            logWriter.OpenWriter();
            logWriter.Write(message);
            logWriter.CloseWriter();
        }

        /**
         * Read the text from the log file from default location
         */
        public string GetLogText()
        {
            LogReader logReader = new LogReader();
            logReader.OpenReader();
            string text = logReader.Read();
            logReader.CloseReader();
            return text;
        }

        /**
         * Read the text from the log file from custom location
         */
        public string GetLogText(string location)
        {
            LogReader logReader = new LogReader();
            logReader.LogLocation = location;
            logReader.OpenReader();
            string text = logReader.Read();
            logReader.CloseReader();
            return text;
        }

        /**
         * Clear log file from default location
         */
        public void ClearLog()
        {
            System.IO.File.WriteAllText(Utilities.ROOT_LOCATION + "Log.txt", String.Empty);
        }

        /**
         * Clear log file from custom location
         */
        public void ClearLog(string location)
        {
            location = (location.EndsWith(@"\")) ? location : location + @"\";
            System.IO.File.WriteAllText(location + "Log.txt", String.Empty);
        }

        public string GetConfigRootLocation()
        {
            return Utilities.ROOT_LOCATION;
        }

        /**
         * Get the location of the bin where MySQL is installed. The Location is looked up from the registry. Returns null if nothing is found.
         */
        public string GetMySQLBinLocation()
        {
            return Utilities.RetrieveMySQLInstallationBinPath();
        }

        /**
         * Create a backup of a database. It can either use the automatic process, which iterates through all databases or the manual, which doesn't iterate
         */
        public void CreateBackup(System.Diagnostics.Process process, string databaseName, bool isManual)
        {
            MySQLDumpProcess dumpProcess = new MySQLDumpProcess(databaseName, this);
            if (isManual)
            {
                dumpProcess.ProcessMySqlDump(process, databaseName);
            }
            else
            {
                dumpProcess.ProcessMySqlDump(process);
            }
        }

        /**
         * Restore a specific database, from a backup dump file.
         */
        public void RestoreDatabase(System.Diagnostics.Process process, string dumpFilePath, DatabaseInfo dbInfo)
        {
            RestoreDatabaseProcess restoreProcess = new RestoreDatabaseProcess(this, dumpFilePath, dbInfo);
            restoreProcess.RestoreDatabase(process);
        }
    }
}
