using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Entity.Filter
{
    public class TaskLogFilter
    {
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string TaskGuid { get; set; }
    }
}
