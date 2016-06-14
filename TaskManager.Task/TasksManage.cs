using System;
using System.Threading.Tasks;
using TaskManager.Common.Utils;
using TaskManager.Common;
using TaskManager.Entity;
using System.Collections.Generic;
using System.Threading;
namespace TaskManager.Tasks
{
    public class TasksManage
    {

        private TasksManage()
        {

            _listTask = new List<ExecTaskInfo>();

        }
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
        private List<ExecTaskInfo> _listTask;
        private static TasksManage _taskManager;
        private static readonly object _lockobj = new object();
        private static readonly object _locklist = new object();
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
        public void StartUp(Action<ExecTaskInfo, TaskExecResult> ExecEndAction)
        {
            Task.Run(() =>
            {
                while (true)
                {

                    List<ExecTaskInfo> tempList;
                    lock (_locklist)
                    {
                        tempList = _listTask.FindAll(x => x.IsExec && x.NextExecTime <= DateTime.Now);
                    }
                    if (tempList.Count == 0)
                    {
                        Thread.Sleep(1000 * 3);
                        continue;
                    }
                    foreach (var item in tempList)
                    {
                        RunTask(item, ExecEndAction);
                    }
                    Thread.Sleep(1000 * 60 * AppConfig.TaskInterval);//休息n分钟后再执行
                }
            });
        }


        /// <summary>
        /// 执行一个任务
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        public Task<TaskExecResult> RunTask(ExecTaskInfo taskInfo, Action<ExecTaskInfo, TaskExecResult> ExecEndAction)
        {
            Task<TaskExecResult> taskRun = Task.Run<TaskExecResult>(() =>
            {
                taskInfo.LastExecTime = DateTime.Now;
                TaskExecResult execResult = new TaskExecResult();
                string rsp = null;
                if (taskInfo.ExecType == (int)ExecTypeEnum.HTTP)
                {
                    rsp = Net.HttpRequest(taskInfo.ExecUrl, taskInfo.Params, taskInfo.ExecMethod, taskInfo.Timeout, taskInfo.Encoding);
                    taskInfo.ExecEndTime = DateTime.Now;

                    if (rsp.StartsWith(":("))
                    {
                        execResult.Code = -999;
                        execResult.Msg = string.Format("任务：{0}, 调用异常 url:{1} ", taskInfo.Title, taskInfo.ExecUrl);
                        execResult.Data = rsp;
                        ExecEndAction(taskInfo, execResult);
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
                            ExecEndAction(taskInfo, execResult);
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
                    if (ProcessUtil.StartProcess(taskInfo.ExecUrl, taskInfo.Params, taskInfo.Timeout, ref rsp))
                    {
                        taskInfo.ExecEndTime = DateTime.Now;
                        execResult.Code = 0;
                        if (taskInfo.IsLogResult)
                            execResult.Data = rsp;
                        execResult.Msg = string.Format("任务：{0},调用 url:{1} , 处理成功 ", taskInfo.Title, taskInfo.ExecUrl);
                    }
                    else
                    {
                        taskInfo.ExecEndTime = DateTime.Now;
                        execResult.Code = -800;
                        execResult.Msg = string.Format("任务：{0},调用 url:{1} , 转换返回结果失败 结果:{1} ", taskInfo.Title, taskInfo.ExecUrl, rsp);
                    }

                }
                else {
                    execResult.Code = -1;
                    execResult.Msg = "执行类型不存在";
                }
                ExecEndAction(taskInfo, execResult);
                return execResult;
            });
            return taskRun;
        }
    }
}
