using FirstFloor.ModernUI.Presentation;
using MySQLBackupLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySQLBackupManager.Pages.Commands;

namespace MySQLBackupManager.Pages.Content
{
    class SettingsConfigurationViewModel : NotifyPropertyChanged
    {
        private readonly Library library = new Library();

        //Object Variables
        private string backupLocation;
        private int deleteBackupAfterDays;

        public SettingsConfigurationViewModel()
        {
            ChangeLocationButton = new ButtonClickCommand(this.ChangeBackupLocationClickEvent);

            SyncFromConfiguration();
        }

        /**
         * Synchronise with the values from the Configuration file.
         */
        private void SyncFromConfiguration()
        {
            this.backupLocation = library.GetBackupLocation();
            this.deleteBackupAfterDays = library.GetDeleteBackupsOlderThanDays();
        }

        //Properties
        public string BackupLocation
        {
            get { return this.backupLocation; }
            set
            {
                this.backupLocation = value;
                OnPropertyChanged("BackupLocation");

                //Save the modification in the Configurations File.
                library.ChangeBackupLocation(this.backupLocation);
            }
        }

        public int DeleteBackupAfterDays
        {
            get { return this.deleteBackupAfterDays; }
            set
            {
                this.deleteBackupAfterDays = value;
                OnPropertyChanged("DeleteBackupAfterDays");

                //Save the modification in the Configurations File.
                library.ChangeDeleteBackupsOlderThanDays(Convert.ToUInt32(this.deleteBackupAfterDays));
            }
        }

        public ButtonClickCommand ChangeLocationButton { get; set; }

        private void ChangeBackupLocationClickEvent()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.BackupLocation = dialog.SelectedPath;
            }
        }
    }
}
