using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Services;
using TaskManager.Services.Config;

namespace TaskManager.WinService
{
    partial class TaskWinService : ServiceBase
    {
        public TaskWinService()
        {
            InitializeComponent();
        }
         protected override void OnStart(string[] args)
        {
            ORMConfig.RegisterORM();
            _serverService.Register();
            _taskService.StartUp();
        }
        TaskService _taskService = new  TaskService();
        ServerService _serverService = new ServerService();

        protected override void OnStop()
        {
            TaskManager.Tasks.TasksManage.GetInstance().Dispose();
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }
    }
}
