using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.Job
{
    public class JobDetail
    {
        public string JobName
        {
            get;
            set;
        }
        public string Cron
        {
            get;
            set;
        }
        public string ConsolePath
        {
            get;
            set;
        }
    }
}
