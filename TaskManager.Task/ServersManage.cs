using System;
using System.Threading.Tasks;
using TaskManager.Common.Utils;
using TaskManager.Common;
using TaskManager.Entity;
using System.Collections.Generic;
using System.Threading;
namespace TaskManager.Tasks
{
    public class ServersManage : IDisposable
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
        public event Action<int,int> OnDeadServer;
        public event Action<int> OnServerCountChange;
        public void StartUp(Func<int, bool> HeartAction, Func<int> QueryUseServerAction)
        {

            if (mainThread == null)
            {

                mainThread = new Thread(new ThreadStart(delegate ()
                {
                    while (IsRun)
                    {
                        if (!TasksManage.GetInstance().CheckIsRun())//发现执行任务的线程没有执行，则停止当前线程，可以重新分配任务
                        {
                            IsRun = false;
                            if (OnDeadServer != null)
                                OnDeadServer(ServerCount-1, ServerCount);
                            return;
                        }
                        HeartAction(MyServer.Id);//心跳记录
                        int nowServerCount = QueryUseServerAction();
                        if (nowServerCount != ServerCount)
                        {
                            if (nowServerCount < ServerCount) {
                                if(OnDeadServer != null)
                                    OnDeadServer(nowServerCount, ServerCount);
                            }
                            if(OnServerCountChange!=null)
                                OnServerCountChange(MyServer.Id);
                            //服务器有变动
                        }
                        ServerCount = nowServerCount;

                        Thread.Sleep(1000 * 60 * AppConfig.ServerHeartInterval);//休息n分钟后再执行 
                    }
                }));

                mainThread.IsBackground = false;

            }
            IsRun = true;
            mainThread.Start();
        }

        public void Dispose()
        {
            IsRun = false;
            mainThread = null;
            _ServerManage = null;
        }
    }

}
