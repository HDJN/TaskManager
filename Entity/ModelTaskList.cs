using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Entity
{
    public class ModelTaskList:Ts_Tasks
    {

        public int LastExecResultCode
        {
            get; set;
        }
        public DateTime NextExecTime
        {
            get
            {
                return LastExecTime.AddMinutes(Interval);
            }
        }
        public bool IsAbnormal {
            get { return NextExecTime < DateTime.Now; }
        }
        public string CreateUserName { get; set; }
        public DateTime LastExecTime { get; set; }

        public ModelTaskList(Ts_Tasks tasks)
        {
            if (tasks == null)
                return;
            this.Guid = tasks.Guid;
            this.ExecUrl = tasks.ExecUrl;
            this.Interval = tasks.Interval;
            this.Status = tasks.Status;
            this.Title = tasks.Title;
            this.TimeOut = tasks.TimeOut;
            this.Encoding = tasks.Encoding;          
            this.IsResponseNorm = tasks.IsResponseNorm;
            this.IsLogResult = tasks.IsLogResult;
            this.ExecType = tasks.ExecType;
            this.ExecMethod = tasks.ExecMethod;
            this.ExecParams = tasks.ExecParams;
            this.CreateUser = tasks.CreateUser;
            this.RunServerId = tasks.RunServerId;
        }
        public ModelTaskList(Ts_Tasks tasks, Ts_TaskExec taskExec):this(tasks)
        {
            if (taskExec != null)
            {
                this.LastExecTime = taskExec.LastExecTime ?? tasks.InsertTime.AddMinutes(tasks.Interval);
                this.LastExecResultCode = taskExec.LastExecResultCode??-1000;
            }
        }
        public ModelTaskList(Ts_Tasks tasks, Ts_TaskExec taskExec,string CreateUserName) : this(tasks)
        {
            if (taskExec != null)
            {
                this.LastExecTime = taskExec.LastExecTime ?? tasks.InsertTime.AddMinutes(tasks.Interval);
                this.LastExecResultCode = taskExec.LastExecResultCode ?? -1000;
            }
            this.CreateUserName = CreateUserName;
        }

        public ModelTaskList()
        {
        }
    }
}
