using MySQLBackupLibrary;
using MySQLBackupManager.Pages.Content;
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

namespace MySQLBackupManager.Views
{
    /// <summary>
    /// Interaction logic for DatabasesPage.xaml
    /// </summary>
    public partial class DatabasesPage : Page
    {
        public DatabasesPage()
        {
            InitializeComponent();

            this.DataContext = new DatabasesViewModel();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new DatabasesViewModel();
        }

        private void DatabasesListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(DatabasesListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                DatabaseInfo dbInfo = (DatabaseInfo) item.Content;
                NavigationCommands.GoToPage.Execute(new Uri("/Pages/ModifyDatabasePage.xaml#" + dbInfo.DatabaseName, UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
            }
        }
    }
}
