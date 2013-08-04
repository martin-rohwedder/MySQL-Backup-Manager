using MySQLBackupLibrary;
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
        private readonly Library library = new Library();

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
            if (library.RetrieveAllDatabaseNodes().Count > 0)
            {
                foreach (DatabaseInfo dbNode in library.RetrieveAllDatabaseNodes())
                {
                    library.RemoveDatabaseNode(dbNode.DatabaseName);
                }
            }

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.DatabaseName = "movstreamdb";
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            int minute = DateTime.Now.Minute;
            int hour = DateTime.Now.Hour;
            if (minute + 1 > 59)
            {
                dbInfo.StartTimeMinute = 0;
                hour++;
                if (hour > 23)
                {
                    dbInfo.StartTimeHour = 0;
                }
                else
                {
                    dbInfo.StartTimeHour = hour;
                }
            }
            else
            {
                dbInfo.StartTimeHour = hour;
                dbInfo.StartTimeMinute = minute + 1;
            }
            library.InsertDatabaseNode(dbInfo);

            this.OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            ISchedulerFactory schedFactory = new StdSchedulerFactory();
            IScheduler scheduler = schedFactory.GetScheduler();
            scheduler.Start();

            //MySQL Backup Job Scheduler Details
            IJobDetail backupJobDetail = new JobDetailImpl("BackupJob", typeof(MySqlDumpProcess));
            ITrigger backupTrigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule(s => s.WithIntervalInMinutes(1).OnEveryDay()).Build();

            //Delete Old Backups Job Scheduler Details
            IJobDetail deleteBackupJobDetail = new JobDetailImpl("DeleteBackupJob", typeof(DeleteBackupFilesJob));
            ITrigger deleteBackupTrigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule(s => s.WithIntervalInHours(24).OnEveryDay().StartingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(0,0,5))).Build();

            scheduler.ScheduleJob(backupJobDetail, backupTrigger);   //Schedule MySQL Backup Job
            scheduler.ScheduleJob(deleteBackupJobDetail, deleteBackupTrigger);   //Schedule Delete Backup Job

            //Log Information about the service has started
            new Library().LogMessage("INFO", "The MySQL Backup Service has been started");
        }

        protected override void OnStop()
        {
            //Log Information about the service has stopped
            library.LogMessage("INFO", "The MySQL Backup Service has been stopped");
        }
    }
}
