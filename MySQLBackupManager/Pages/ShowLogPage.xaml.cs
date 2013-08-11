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
    /// Interaction logic for ShowLogPage.xaml
    /// </summary>
    public partial class ShowLogPage : Page
    {
        private readonly Library library = new Library();

        public ShowLogPage()
        {
            InitializeComponent();

            LogTextBox.Text = library.GetLogText().Replace("\n", "");
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            library.ClearLog();
            LogTextBox.Text = library.GetLogText().Replace("\n", "");
        }
    }
}
