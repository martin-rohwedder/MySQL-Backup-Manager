using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using MySQLBackupLibrary;

namespace MySQLBackupService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += new InstallEventHandler(ServiceInstaller_AfterInstall);
        }

        void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            this.CheckUserPathVariable();

            ServiceController sc = new ServiceController("MySQL Backup Service");
            sc.Start();
        }

        private void CheckUserPathVariable()
        {
            Library library = new Library();
            string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            if (path == null)
            {
                Environment.SetEnvironmentVariable("PATH", library.GetMySQLBinLocation(), EnvironmentVariableTarget.Machine);
                //this.RestartService();
            }
            else if (!path.Contains(library.GetMySQLBinLocation()))
            {
                path += ";" + library.GetMySQLBinLocation();
                Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Machine);
                //this.RestartService();
            }
        }
    }
}
