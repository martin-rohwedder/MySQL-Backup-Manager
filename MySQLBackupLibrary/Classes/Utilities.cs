using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupLibrary.Classes
{
    class Utilities
    {
        public static readonly string ROOT_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\";
        public static readonly string CONFIGURATION_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\";
        public static readonly string DEFAULT_BACKUP_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Backup\";

        /**
         * Lookup the MySQL Installation Bin path from the registry
         */
        public static string RetrieveMySQLInstallationBinPath()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\MySQL AB");
            foreach (string subkey in registryKey.GetSubKeyNames())
            {
                if (subkey.ToLower().Contains("mysql server"))
                {
                    RegistryKey myKey = registryKey.OpenSubKey(subkey);
                    string location = (string)myKey.GetValue("Location");
                    return location + @"bin\";
                }
            }
            return null;
        }
    }
}
