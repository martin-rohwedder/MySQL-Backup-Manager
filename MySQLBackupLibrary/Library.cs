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

        public void ChangeBackupLocation(string location)
        {
            configHandler.ModifyBackupLocation(location);
        }
    }
}
