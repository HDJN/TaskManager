using System;
using System.Threading.Tasks;
using TaskManager.Common.Utils;
using TaskManager.Common;
using TaskManager.Entity;
using System.Collections.Generic;
using System.Threading;
namespace TaskManager.Tasks
{
    public class TasksManage:IDisposable
    {
        private TasksManage()
        {
            _listTask = new List<ExecTaskInfo>();
        }
        //private Func<List<ExecTaskInfo>> _queryTaskListAction;
        //private bool SetQueryTaskListAction(Func<List<ExecTaskInfo>> QueryTaskListAction)
        //{
        //    if (_queryTaskListAction == null)
        //    { _queryTaskListAction = QueryTaskListAction; return true; }
        //    return false;
        //}

        #region 缓存任务列表相关
        private readonly object _locklist = new object();
        private List<ExecTaskInfo> _listTask;
        public void SetTask(ExecTaskInfo execTask)
        {
            lock (_locklist)
            {

                var tempTask = _listTask.FindIndex(x => x.Guid == execTask.Guid);
                if (tempTask > -1)
                {
                    if (execTask.IsExec)
                        _listTask[tempTask] = execTask;
                    else
                        _listTask.RemoveAt(tempTask);

                }
                else
                {
                    if (execTask.IsExec)
                        _listTask.Add(execTask);
                }

            }
        }
        public void SetAllTask(List<ExecTaskInfo> execTask)
        {

            lock (_locklist)
            {
                _listTask.Clear();
                _listTask.AddRange(execTask.FindAll(x => x.IsExec));
            }
        }
        public bool RemoveTask(string TaskGuid)
        {
            lock (_locklist)
            {
                var tempTask = _listTask.FindIndex(x => x.Guid == TaskGuid);
                if (tempTask > -1)
                {
                    _listTask.RemoveAt(tempTask);
                    return true;
                }
                else { return false; }

            }
        }
        public bool RemoveTask(ExecTaskInfo execTask)
        {
            lock (_locklist)
            {
                var tempTask = _listTask.FindIndex(x => x.Guid == execTask.Guid);
                if (tempTask > -1)
                {
                    _listTask.RemoveAt(tempTask);
                    return true;
                }
                else { return false; }

            }
        }
        public void RemoveAllTask()
        {
            lock (_locklist)
            {
                _listTask.Clear();
            }
        }
        #endregion

        #region 执行相关事件定义
        public event Func<ExecTaskInfo,int> OnTaskExecBefore;
        public event Action<TaskExecLog, TaskExecResult> OnTaskExecAfter;
        #endregion

        #region 执行任务线程相关
        private bool IsRun = false;
        private Thread mainThread = null;


        public void StartUp(Func<List<ExecTaskInfo>> QueryTaskListAction)
        {
            // SetQueryTaskListAction(QueryTaskListAction);
            if (mainThread == null)
            {

                mainThread = new Thread(new ThreadStart(delegate () {
                    while (IsRun)
                    {

                        List<ExecTaskInfo> tempList = QueryTaskListAction();
                        //lock (_locklist)
                        //{
                        //    tempList = _listTask.FindAll(x => x.IsExec && x.NextExecTime <= DateTime.Now);
                        //}
                        //if (tempList.Count == 0)
                        //{
                        //    Thread.Sleep(1000 * 3);
                        //    continue;
                        //}
                        foreach (var item in tempList)
                        {
                            if (item.NextExecTime!=null&&item.NextExecTime <= DateTime.Now)
                                RunTask(item);
                        }
                        SetLastRunTime();
                        Thread.Sleep(1000 * 60 * AppConfig.TaskInterval);//休息n分钟后再执行

                    }
                }));

                mainThread.IsBackground = false;

            }
            IsRun = true;
            mainThread.Start();
        }


        /// <summary>
        /// 执行一个任务
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        public Task<TaskExecResult> RunTask(ExecTaskInfo taskInfo)
        {
            Task<TaskExecResult> taskRun = Task.Run<TaskExecResult>(() =>
            {
                
                taskInfo.LastExecTime = DateTime.Now;
                int execLogId = 0;
                if (OnTaskExecBefore != null) {
                   execLogId= OnTaskExecBefore(taskInfo);
                }

                TaskExecLog taskExecLog = new TaskExecLog() {
                    ExecGuid = taskInfo.Guid,
                    ExecLogId = execLogId,
                    IsErrorAlert = taskInfo.IsErrorAlert,
                    Title = taskInfo.Title,
                    ReceiveEmail = taskInfo.ReceiveEmail,
                    ExecStatrtTime = taskInfo.LastExecTime
                };

                TaskExecResult execResult = new TaskExecResult();
                string rsp = null;
                if (taskInfo.ExecType == (int)ExecTypeEnum.HTTP)
                {
                    rsp = Net.HttpRequest(taskInfo.ExecUrl, taskInfo.Params, taskInfo.ExecMethod, taskInfo.Timeout, taskInfo.Encoding, taskInfo.IsResponseNorm || taskInfo.IsLogResult);
                    taskExecLog.ExecEndTime = DateTime.Now;

                    if (rsp.StartsWith(":("))
                    {
                        execResult.Code = -999;
                        execResult.Msg = string.Format("任务：{0}, 调用异常 url:{1} ", taskInfo.Title, taskInfo.ExecUrl);
                        execResult.Data = rsp;
                        if (OnTaskExecAfter != null) {
                            OnTaskExecAfter(taskExecLog, execResult);
                        }
                        return execResult;
                    }
                    if (taskInfo.IsResponseNorm)
                    {
                        try
                        {
                            execResult = rsp.FromJson<TaskExecResult>();
                        }
                        catch (Exception ex)
                        {
                            execResult.Code = -900;
                            execResult.Msg = string.Format("任务：{0},调用 url:{1} , 转换返回结果失败 结果:{1} ", taskInfo.Title, taskInfo.ExecUrl, rsp);
                            execResult.Data = ex;
                            if (OnTaskExecAfter != null)
                            {
                                OnTaskExecAfter(taskExecLog, execResult);
                            }
                            return execResult;
                        }
                    }
                    else
                    {
                        if (taskInfo.IsLogResult)
                            execResult.Data = rsp;
                        execResult.Code = 0;
                        execResult.Msg = string.Format("任务：{0},调用 url:{1} , 处理成功 ", taskInfo.Title, taskInfo.ExecUrl);
                    }

                }
                else if (taskInfo.ExecType == (int)ExecTypeEnum.EXE)
                {
                    if (ProcessUtil.StartProcess(taskInfo.ExecUrl, taskInfo.Params, taskInfo.Timeout, ref rsp, taskInfo.IsLogResult))
                    {
                        taskExecLog.ExecEndTime = DateTime.Now;
                        execResult.Code = 0;
                        if (taskInfo.IsLogResult)
                            execResult.Data = rsp;
                        execResult.Msg = string.Format("任务：{0},调用 url:{1} , 处理成功 ", taskInfo.Title, taskInfo.ExecUrl);
                    }
                    else
                    {
                        taskExecLog.ExecEndTime = DateTime.Now;
                        execResult.Code = -800;
                        execResult.Msg = string.Format("任务：{0},调用 url:{1} , 转换返回结果失败 结果:{1} ", taskInfo.Title, taskInfo.ExecUrl, rsp);
                    }

                }
                else
                {
                    execResult.Code = -1;
                    execResult.Msg = "执行类型不存在";
                }
                if (OnTaskExecAfter != null)
                {
                    OnTaskExecAfter(taskExecLog, execResult);
                }
                return execResult;
            });
            return taskRun;
        }
        #endregion


        #region 单例
        private static TasksManage _taskManager;
        private static readonly object _lockobj = new object();
        public static TasksManage GetInstance()
        {
            if (_taskManager == null)
            {

                lock (_lockobj)
                {
                    if (_taskManager == null)
                        _taskManager = new TasksManage();
                }
            }
            return _taskManager;
        }

        #endregion 

        

        #region 服务器集群，检测线程是否在运行

        private DateTime _lastRunTime = DateTime.Now;
        private readonly object _locktime = new object();
        private void SetLastRunTime() {
            lock (_locktime) {
                _lastRunTime = DateTime.Now;
            }
        }
        public DateTime GetLastRunTime()
        {
            lock (_locktime)
            {
                return _lastRunTime;
            }
        }
        /// <summary>
        /// 检测线程是否还在运行
        /// </summary>
        /// <returns></returns>
        public bool CheckIsRun() {

            DateTime tempLastRunTime;
            lock (_locktime)
            {
                tempLastRunTime = _lastRunTime;
            }
            return DateTime.Now<= tempLastRunTime.AddMinutes(AppConfig.TaskInterval+1);

        }

        #endregion
        
       

        public void Dispose()
        {
            IsRun = false;
            OnTaskExecAfter = null;
            OnTaskExecBefore = null;
            _listTask = null;
            mainThread = null;
            _taskManager = null;
        }
        

    }
}
