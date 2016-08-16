using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Common.Corn;
namespace TaskManager.Entity
{
    public class ExecTaskInfo
    {
        public string ExecUrl { get; set; }
        public string Title { get; set; }
        public int ExecType { get; set; }
        public string Interval { get; set; }
        public string Params { get; set; }
        public Encoding Encoding { get; set; }
        public int Timeout { get; set; }
        public bool IsResponseNorm { get; set; }

        public string Guid { get; set; }
        public bool IsExec { get; set; }
        public bool IsLogResult { get; set; }
        public string ExecMethod { get; set; }
        public DateTime ExecEndTime { get; set; }
        public bool IsErrorAlert { get; set; }
        public string ReceiveEmail { get; set; }
        public DateTime? NextExecTime
        {
            get
            {
                CronExpression expression = new CronExpression(this.Interval);
                var lastOffset = expression.GetNextValidTimeAfter(new DateTimeOffset(LastExecTime));
                if (lastOffset != null)
                    return lastOffset.Value.LocalDateTime;
                return null;
            }
        }
        public DateTime LastExecTime { get; set; }

        public ExecTaskInfo(Ts_Tasks tasks)
        {
            if (tasks == null)
                return;
            this.Guid = tasks.Guid;
            this.ExecUrl = tasks.ExecUrl;
            this.Interval = tasks.Interval;

            this.IsExec = tasks.Status == 1;
            this.Title = tasks.Title;
            this.Timeout = tasks.TimeOut * 1000;

            if (string.IsNullOrEmpty(tasks.Encoding))
            {
                this.Encoding = Encoding.UTF8;
            }
            else
            {
                try
                {
                    this.Encoding = Encoding.GetEncoding(tasks.Encoding);
                }
                catch
                {
                    this.Encoding = Encoding.UTF8;
                }
            }

            this.IsResponseNorm = tasks.IsResponseNorm;
            this.IsLogResult = tasks.IsLogResult;
            this.ExecType = tasks.ExecType;
            this.ExecMethod = tasks.ExecMethod;
            this.Params = tasks.ExecParams;
            this.IsErrorAlert = tasks.IsErrorAlert;
            this.ReceiveEmail = tasks.ReceiveEmail;
        }
        public ExecTaskInfo(Ts_Tasks tasks, Ts_TaskExec taskExec):this(tasks)
        {
            if (taskExec != null)
            {
                this.LastExecTime = taskExec.LastExecTime ?? DateTime.MinValue;
            }
        }
      
        public ExecTaskInfo()
        {
        }
    }
}
