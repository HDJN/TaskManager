using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Entity
{
    public class ModelTaskList:Ts_Tasks
    {

        public DateTime LasExecTime
        {
            get; set;
        }
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
        }
        public ModelTaskList(Ts_Tasks tasks, Ts_TaskExec taskExec):this(tasks)
        {
            if (taskExec != null)
            {
                this.LastExecTime = taskExec.LastExecTime ?? tasks.InsertTime.AddMinutes(tasks.Interval);
                this.LastExecResultCode = taskExec.LastExecResultCode??-1000;
            }
        }
      
        public ModelTaskList()
        {
        }
    }
}
