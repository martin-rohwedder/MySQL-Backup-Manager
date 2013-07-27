using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
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

namespace MySQLBackupManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //Load the user settings
            this.LoadUserSettings();
        }

        /**
         * Load User settings saved in the settings.settings file
         */
        private void LoadUserSettings()
        {
            AppearanceManager.Current.ThemeSource = Properties.Settings.Default.ApplicationTheme.Equals("LightThemeSource") ? AppearanceManager.LightThemeSource : AppearanceManager.DarkThemeSource;
            AppearanceManager.Current.AccentColor = Properties.Settings.Default.ApplicationColor;
            AppearanceManager.Current.FontSize = Properties.Settings.Default.ApplicationFontSize;
        }
    }
}
