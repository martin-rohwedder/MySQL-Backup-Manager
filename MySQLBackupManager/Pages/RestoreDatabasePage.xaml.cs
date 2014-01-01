using MySQLBackupLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MySQLBackupManager.Pages
{
    /// <summary>
    /// Interaction logic for RestoreDatabasePage.xaml
    /// </summary>
    public partial class RestoreDatabasePage : Page
    {
        private Library library = null;

        public RestoreDatabasePage()
        {
            InitializeComponent();
            library = new Library();
        }

        private void SelectBackupDumpFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.InitialDirectory = library.GetBackupLocation();

            //Set Extension filter and default extension
            dlg.DefaultExt = ".dump";
            dlg.Filter = "Backup Dump Files (*.dump)|*.dump|SQL Dump Files (*.sql)|*.sql";

            Nullable<bool> hasSelected = dlg.ShowDialog();

            if (hasSelected == true)
            {
                string fileName = dlg.FileName;

                Process process = null;

                try
                {
                    DatabaseInfo dbInfo = RetrieveDatabaseInformation(dlg.FileName);
                    if (dbInfo == null)
                    {
                        FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("We need some information, to restore your database. Please go to 'Add Database', and add the database, with the correct information, and then try to restore the database again. Notice, the database doesn't need to be created in MySQL.", "Failure", MessageBoxButton.OK);
                    }
                    else
                    {
                        if (FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("You are about to restore the database {0} with a backup file. This means that all content in the database will be overwritten, with the information from the backup file. This action can't be undone. Do you want to continue?", dbInfo.DatabaseName), "Proceed with database restore?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            library.RestoreDatabase(process, fileName, dbInfo);
                            FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("The database {0} has been restored from this backup dump file '{1}'", dbInfo.DatabaseName, fileName), "Success", MessageBoxButton.OK);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (process != null)
                    {
                        process.Close();
                    }
                }

            }
        }

        /**
         * Get the database information needed for the particular database in the dump file.
         */
        private DatabaseInfo RetrieveDatabaseInformation(string dumpFilePath)
        {
            string[] lines = System.IO.File.ReadAllLines(dumpFilePath);
            const string test = "Database: ";
            string databaseName = "";

            foreach (string line in lines)
            {
                if (line.Contains(test))
                {
                    int startPos = line.LastIndexOf(test) + test.Length;
                    databaseName = line.Substring(startPos);
                    break;
                }
            }

            return library.RetrieveDatabaseNode(databaseName); ;
        }
    }
}
