using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupService
{
    public partial class MySQLBackupService : ServiceBase
    {
        public MySQLBackupService()
        {
            InitializeComponent();
        }

        public void onDebug()
        {
            this.OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = @"C:\";
                process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");
                //Redirects the standard input so that commands can be sent to the shell
                process.StartInfo.RedirectStandardInput = true;

                process.OutputDataReceived += ProcessOutputDataHandler;
                process.ErrorDataReceived += ProcessErrorDataHandler;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.StandardInput.WriteLine("dir");
                process.StandardInput.WriteLine("exit");

                process.WaitForExit();
            }
        }

        protected override void OnStop()
        {
            
        }

        public static void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }

        public static void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }
    }
}
