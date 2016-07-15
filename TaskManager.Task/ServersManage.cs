using System;
using System.Threading.Tasks;
using TaskManager.Common.Utils;
using TaskManager.Common;
using TaskManager.Entity;
using System.Collections.Generic;
using System.Threading;
namespace TaskManager.Tasks
{
    public class ServersManage
    {

        private ServersManage()
        {
            
        }
      
        private static ServersManage _ServerManage;
        private static readonly object _lockobj = new object();
        public static ServersManage GetInstance()
        {
            if (_ServerManage == null)
            {

                lock (_lockobj)
                {
                    if (_ServerManage == null)
                        _ServerManage = new ServersManage();
                }
            }
            return _ServerManage;
        }
        public Ts_Servers MyServer { get; set; }
        private Thread mainThread;
        private bool IsRun;
        private int ServerCount;
        public void StartUp(Func<int,bool> HeartAction, Func<int> QueryUseServerAction, Action<int> SetMain)
        {
        
            if (mainThread == null)
            {

                mainThread = new Thread(new ThreadStart(delegate () {
                    while (IsRun)
                    {
                        HeartAction(MyServer.Id);//心跳记录
                        int serverCount=QueryUseServerAction();
                        if (serverCount != ServerCount)
                        {
                            SetMain(MyServer.Id);
                            //服务器有变动
                        }
                        ServerCount = serverCount;

                        Thread.Sleep(1000 * 60 * AppConfig.ServerHeartInterval);//休息n分钟后再执行 
                    }
                }));

                mainThread.IsBackground = false;

            }
            IsRun = true;
            mainThread.Start();
        }
    }
}
