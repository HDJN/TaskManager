using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Entity
{
    public class TaskExecResult
    {
        public int Code { get { return code; } set { code = value; } }
        private int code = -1;
        public string Msg { get; set; }
        public object Data { get; set; }
    }
    public class TaskExecLog
    {
       public DateTime ExecEndTime { get; set; }
        public string ExecGuid { get; set; }
        public int ExecLogId { get; set; }
        public string Title { get; set; }
        public string ReceiveEmail { get; set; }
        public bool IsErrorAlert { get; set; }
        public DateTime ExecStatrtTime { get; set; }
}
}
