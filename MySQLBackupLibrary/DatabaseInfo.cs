using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupLibrary
{
    public class DatabaseInfo
    {
        private int startTimeHour;
        private int startTimeMinute;

        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public TimeSpan StartTime { get { return new TimeSpan(startTimeHour, startTimeMinute, 0); } }

        public int StartTimeHour
        {
            set
            {
                startTimeHour = (value < 0 || value > 23) ? 0 : value;
            }
        }

        public int StartTimeMinute
        {
            set
            {
                startTimeMinute = (value < 0 || value > 59) ? 0 : value;
            }
        }

        public override string ToString()
        {
            return DatabaseName;
        }
    }
}
