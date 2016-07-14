using System;
using System.Threading.Tasks;
using TaskManager.Common.Utils;
using TaskManager.Common;
using TaskManager.Entity;
using System.Collections.Generic;
using System.Threading;
namespace TaskManager.Tasks
{
    public class ServerManage
    {

        private ServerManage()
        {
            
        }
      
        private static ServerManage _ServerManage;
        private static readonly object _lockobj = new object();
        public static ServerManage GetInstance()
        {
            if (_ServerManage == null)
            {

                lock (_lockobj)
                {
                    if (_ServerManage == null)
                        _ServerManage = new ServerManage();
                }
            }
            return _ServerManage;
        }
        public Ts_Servers MyServer { get; set; }
    }
}
