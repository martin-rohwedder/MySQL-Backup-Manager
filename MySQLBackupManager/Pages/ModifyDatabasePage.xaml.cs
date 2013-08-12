using FirstFloor.ModernUI.Windows;
using MySQLBackupLibrary;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ModifyDatabasePage.xaml
    /// </summary>
    public partial class ModifyDatabasePage : Page, IContent
    {
        private readonly Library library = new Library();

        private string currentDatabaseName;
        public string CurrentDatabaseName
        {
            get
            {
                return this.currentDatabaseName;
            }
            set
            {
                this.currentDatabaseName = value;
            }
        }

        private DatabaseInfo currentDbInfo;
        public DatabaseInfo CurrentDbInfo
        {
            get
            {
                return this.currentDbInfo;
            }
            set
            {
                this.currentDbInfo = value;
            }
        }

        private string currentStartTime;
        public string CurrentStartTime
        {
            get
            {
                return this.currentStartTime;
            }
            set
            {
                this.currentStartTime = value;
            }
        }

        public ModifyDatabasePage()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            if (e.Fragment != "")
            {
                DatabaseInfo dbInfo = library.RetrieveDatabaseNode(e.Fragment);
                if (dbInfo == null)
                {
                    FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("The database requested was not found!", "Not Found", MessageBoxButton.OK);
                    NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
                }
                CurrentDbInfo = dbInfo;
                CurrentDatabaseName = dbInfo.DatabaseName;
                CurrentStartTime = dbInfo.StartTime.ToString();
            }
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void ModifyDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            string[] startTimeSplit = startTime.Text.Split(':');
            CurrentDbInfo.StartTimeHour = Convert.ToInt32(startTimeSplit[0]);
            CurrentDbInfo.StartTimeMinute = Convert.ToInt32(startTimeSplit[1]);

            if (CurrentDatabaseName.ToLower().Equals(CurrentDbInfo.DatabaseName.ToLower()))
            {
                library.UpdateDatabaseNode(CurrentDbInfo);
            }
            else
            {
                library.RemoveDatabaseNode(CurrentDatabaseName);
                library.InsertDatabaseNode(CurrentDbInfo);
            }


            CurrentDatabaseName = "";
            library.LogMessage("INFO", string.Format("The database {0} has been successfully modified", CurrentDbInfo.DatabaseName));
            NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
        }

        private void RemoveDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            var result = FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("Are you sure that you want to remove the database '{0}'?\nThis action can't be undone!", CurrentDatabaseName), "Remove Database", MessageBoxButton.YesNo);

            if (result.ToString().ToLower().Equals("yes"))
            {
                library.RemoveDatabaseNode(CurrentDatabaseName);
                library.LogMessage("INFO", string.Format("The database {0} has been successfully removed", CurrentDatabaseName));
                NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
            }
        }
    }
}
