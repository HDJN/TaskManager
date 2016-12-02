using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmiteRepository;
using TaskManager.Entity;
using TaskManager.Repositories;
using TaskManager.Tasks;
using TaskManager.Common.Utils;
using System.Linq.Expressions;
using TaskManager.Entity.Filter;
using TaskManager.Common.Mvc;
using TaskManager.Common.Exceptions;
using TaskManager.Common.Corn;
namespace TaskManager.Services
{
    public class TaskService:BaseService
    {
        private TaskRepository taskRepository = new TaskRepository();
       
        private IORMRepository<Ts_Tasks> _ormTasks;
        private IORMRepository<Ts_ExecLog> _ormExecLog;
        private IORMRepository<Ts_TaskExec> _ormTaskExec;

        private SystemService _systemService;
        private MailService _mailService;
        private ServerService _serverService;
        public TaskService() {
            _ormTasks = taskRepository.For<Ts_Tasks>();
            _ormExecLog = taskRepository.For<Ts_ExecLog>();
            _ormTaskExec = taskRepository.For<Ts_TaskExec>();
            _mailService = new MailService();
            _systemService = new SystemService();
            _serverService = new ServerService();
        }
        public Ts_Tasks GetTaskByGuid(string TaskGuid) {
            return _ormTasks.Find(w => w.Guid == TaskGuid);
        }
        public bool SaveTask(Ts_Tasks Tasks) {
            try
            {
                new CronExpression(Tasks.Interval);
            } catch (Exception ex) {
                throw new BOException("设置的cron表达式格式不正确");
            }
            if (Tasks.TimeOut<=0)
            {
                throw new BOException("超时时间必需大于0");
            }
            if (!string.IsNullOrEmpty(Tasks.Encoding))
            {
                try
                {
                     Encoding.GetEncoding(Tasks.Encoding);
                }
                catch
                {
                    throw new BOException("编码输入错误");
                }
            }
            Tasks.RunServerId = GetNewRunServerId();
            if (Tasks.ExecType == (int)ExecTypeEnum.EXE)
                Tasks.IsResponseNorm = false;
            if (string.IsNullOrEmpty(Tasks.Guid)){
                Tasks.CreateUser = MyContext.CurrentUser.UserId;
                Tasks.Guid = Guid.NewGuid().ToString();
                Tasks.InsertTime = DateTime.Now;
                _ormTasks.Add(Tasks);
                    _ormTaskExec.Add(new Ts_TaskExec() { TaskGuid = Tasks.Guid });
                return true;
            }
            int result = _ormTasks.Update(Tasks);
            log.Info(string.Format("保存任务记录：{0},执行结果:{1}，操作用户:{2}", Tasks.ToJson(),result,MyContext.CurrentUser.UserName));
            return result > 0;
        }
        public bool RemoveTask(string TaskGuid) {
            bool result = true;
            try
            {
                result &= _ormTaskExec.Delete(w => w.TaskGuid == TaskGuid) > 0;
                result &= _ormTasks.Delete(w => w.Guid == TaskGuid) > 0;
            }
            catch (Exception ex) {
                throw new BOException("删除不成功，消息:"+ex.Message);
            }

            return result;
        }
        public List<Ts_Tasks> GetAllTask(int Status)
        {
            if (Status==-1)   
             return _ormTasks.FindAll();
            return _ormTasks.FindAll(w => w.Status == Status);
        }
        public List<Ts_Tasks> GetAllTask(int Status,int CreateUser)
        {
            if (Status == -1)
                return _ormTasks.FindAll(w=> w.CreateUser == CreateUser);
            return _ormTasks.FindAll(w => w.Status == Status&&w.CreateUser== CreateUser);
        }
        public List<ModelTaskList> GetTaskList(int Status)
        {
            List<Ts_Tasks> list = null;
            if (MyContext.Identity == "admin")
            {
                list = GetAllTask(Status);
            }
            else {
                list = GetAllTask(Status,MyContext.CurrentUser.UserId);
            }
            return list.Select( task => new ModelTaskList(task,_ormTaskExec.Find(w=>w.TaskGuid==task.Guid), _systemService.GetUserName(task.CreateUser))).ToList();
        }
        public bool RunTask(string TaskGuid) {
            var task = _ormTasks.Find(w => w.Guid == TaskGuid);
            if (task == null)
                throw new BOException("找不到任务ID");
            var taskinfo = new ExecTaskInfo(task, GetTaskExecByGuid(task.Guid));
            var taskManager = TasksManage.GetInstance();
            taskManager.OnTaskExecAfter += TaskManager_OnTaskExecAfter;
            taskManager.OnTaskExecBefore += TaskManager_OnTaskExecBefore;
            taskManager.RunTask(taskinfo);
            return true;
        }
        public Ts_TaskExec GetTaskExecByGuid(string TaskGuid) {
            return _ormTaskExec.Find(w => w.TaskGuid == TaskGuid);
        }
        public PageList<Ts_ExecLog> GetAllTaskLog(TaskLogFilter filter,PageView page )
        {
            Expression<Predicate< Ts_ExecLog>> where = w => w.TaskGuid == filter.TaskGuid;
            if (filter.BeginTime.HasValue)
                where=where.And(w => w.ExecStatrtTime > filter.BeginTime);
            if (filter.EndTime.HasValue)
                where = where.And(w => w.ExecEndTime <= filter.EndTime);

            return new PageList<Ts_ExecLog>(_ormExecLog.GetPage(page, where));
        }
        public List<Ts_Tasks> GetNormTask()
        {
            var myserver = ServersManage.GetInstance().MyServer;
            if (myserver == null)
            {
                log.Fatal(string.Format("服务器没有注册，系统将无法正常运行"));
                throw new Exception(string.Format("服务器没有注册，系统将无法正常运行"));
            }
            return _ormTasks.FindAll(w => w.Status == 1&&w.RunServerId==myserver.Id);
        }
        public int  GetNewRunServerId( )
        {
            var ServerList = _serverService.GetUseServer();
            var serverCount = ServerList.Count;
            if (serverCount == 0)
                return 0;
            var serverIndex= new Random().Next(0, serverCount);
            return ServerList[serverIndex].Id;
        }

      
        public void StartUp() {
           var listTask= GetNormTask();
            TasksManage taskManager = TasksManage.GetInstance();
            foreach (var task in listTask)
            {
                taskManager.SetTask(new ExecTaskInfo(task,GetTaskExecByGuid(task.Guid)));
            }

            taskManager.OnTaskExecAfter += TaskManager_OnTaskExecAfter;
            taskManager.OnTaskExecBefore += TaskManager_OnTaskExecBefore;
            taskManager.StartUp(GetNormExecTaskInfo);
            
        }

        private void TaskManager_OnTaskExecAfter(TaskExecLog taskExecLog, TaskExecResult result)
        {

            try
            {
                Ts_ExecLog tsLog = new Ts_ExecLog();
                tsLog.ExecEndTime = taskExecLog.ExecEndTime;
                result.Data = result.Data ?? string.Empty;
                tsLog.ExecResult = result.ToJson();
                if (tsLog.ExecResult.Length > 2000)
                    tsLog.ExecResult = tsLog.ExecResult.Substring(0, 2000);
                tsLog.ExecResultCode = result.Code;
                tsLog.ExecStatrtTime = taskExecLog.ExecStatrtTime;

                if (_ormExecLog.Update(tsLog, w => w.Id == taskExecLog.ExecLogId)!=1)
                {
                    log.Error(string.Format("执行后更新记录异常,logId={0},结果:{1}",taskExecLog.ExecLogId, tsLog.ExecResult));
                }

                
            }
            catch (Exception ex)
            {
                log.ErrorAndEmail(string.Format("保存执行日志结果异常TaskManager_OnTaskExecAfter,参数：{0}", taskExecLog.ToJson()), ex);
            }
            try
            {
                Ts_TaskExec taskExec = new Ts_TaskExec();
                taskExec.LastExecResultCode = result.Code;
                _ormTaskExec.Update(taskExec, w => w.TaskGuid == taskExecLog.ExecGuid);
            }
            catch (Exception ex)
            {
                log.ErrorAndEmail(string.Format("修改执行Code异常TaskManager_OnTaskExecAfter,参数：{0}", taskExecLog.ToJson()), ex);
            }

            if (result.Code != 0 && taskExecLog.IsErrorAlert)
            {
                _mailService.SendEmail(string.Format("任务【{0}】执行异常", taskExecLog.Title),
                    string.Format("您的任务:{0}\r\n执行异常:\r\n{1}", taskExecLog.Title, result.Data),
                    taskExecLog.ReceiveEmail);
            }
        }

        private int TaskManager_OnTaskExecBefore(ExecTaskInfo task)
        {
            int logId = 0;
            try
            {
                Ts_ExecLog tsLog = new Ts_ExecLog();
                tsLog.ExecParams = task.Params;
                tsLog.ExecStatrtTime = task.LastExecTime;
                tsLog.ExecUrl = task.ExecUrl;
                tsLog.TaskGuid = task.Guid;

                 logId =(int) _ormExecLog.Add(tsLog);

            }
            catch (Exception ex)
            {
                log.ErrorAndEmail(string.Format("保存执行日志结果异常TaskManager_OnTaskExecBefore,参数：{0}", task.ToJson()), ex);

            }
            try
            {

                Ts_TaskExec taskExec = new Ts_TaskExec();
                taskExec.LastExecId = logId;
                taskExec.LastExecTime = task.LastExecTime;
                taskExec.TaskGuid = task.Guid;

                _ormTaskExec.Update(taskExec);
            }
            catch (Exception ex) {
                log.ErrorAndEmail(string.Format("修改最后一次执行时间异常TaskManager_OnTaskExecBefore,参数：{0}", task.ToJson()), ex);

            }
            return logId;

        }

      

        public void RestAll() {
            //TasksManage.GetInstance().RemoveAllTask(); 
            //var listTask = GetNormTask();
            //foreach (var task in listTask)
            //{
            //    RestTask(task);
            //}
            
            TasksManage.GetInstance().SetAllTask(GetNormExecTaskInfo());
        }
        
        private List<ExecTaskInfo> GetNormExecTaskInfo() {
            var listTask = GetNormTask();
            return listTask.Select(task =>
                new ExecTaskInfo(task, GetTaskExecByGuid(task.Guid))
                ).ToList();
        }
        public void RestTask(Ts_Tasks task) {
            TasksManage.GetInstance().SetTask(new ExecTaskInfo(task, GetTaskExecByGuid(task.Guid)));
        }
    }
}
