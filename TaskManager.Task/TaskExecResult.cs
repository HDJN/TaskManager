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
}
