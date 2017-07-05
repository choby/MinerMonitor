using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerMonitor.Infrastructure.F2Pool
{
    public class WorkerItem
    {
        public string WorkerName
        {
            get;
            set;
        }

        public string SpeedPer15Minutes
        {
            get;
            set;
        }
        
        public string SpeedPerHour
        {
            get;set;
        }
        public string SpeedPer24Hours { get; set; }
        public DateTime LastSubmitTime { get; set; }
       
    }
}
