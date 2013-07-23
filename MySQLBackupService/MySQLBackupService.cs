using MySQLBackupService.Classes;
using Quartz;
using Quartz.Impl;
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
using System.Xml;

namespace MySQLBackupService
{
    public partial class MySQLBackupService : ServiceBase
    {
        public string error { get; set; }
        public string output { get; set; }
        public string databaseName { get; set; }
        public bool ServerDown { get; set; }

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
            ISchedulerFactory schedFactory = new StdSchedulerFactory();
            IScheduler scheduler = schedFactory.GetScheduler();
            scheduler.Start();

            //Scheduler Details
            IJobDetail jobDetail = new JobDetailImpl("BackupJob", typeof(MySqlDumpProcess));
            ITrigger trigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule(s => s.WithIntervalInMinutes(1).OnEveryDay()).Build();

            scheduler.ScheduleJob(jobDetail, trigger);

            //Log Information about the service has started
            this.Log("The MySQL Backup Service has been started", "INFO");
        }

        protected override void OnStop()
        {
            //Log Information about the service has stopped
            this.Log("The MySQL Backup Service has been stopped", "INFO");
        }

        /**
         * Log message to the Log.txt file. Type indicates the message type, eg. Error, information etc.
         */
        private void Log(string output, string type)
        {
            LogWriter logWriter = new LogWriter();
            try
            {
                logWriter.OpenWriter();
                logWriter.Type = type;
                logWriter.Write(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                logWriter.CloseWriter();
            }
        }
    }
}
