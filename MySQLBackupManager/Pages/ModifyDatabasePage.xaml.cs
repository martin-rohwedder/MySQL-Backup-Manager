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

        public ModifyDatabasePage()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            if (e.Fragment != "")
            {
                Library library = new Library();
                DatabaseInfo dbInfo = library.RetrieveDatabaseNode(e.Fragment);
                if (dbInfo == null)
                {
                    FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("The database requested was not found!", "Not Found", MessageBoxButton.OK);
                    NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
                }
                else
                {
                    CurrentDbInfo = dbInfo;
                }
            }
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            CurrentDbInfo = null;
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
    }
}
