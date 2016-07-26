using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmiteRepository;
using TaskManager.Entity;
using TaskManager.Repositories;
using TaskManager.Tasks;
using TaskManager.Common;
using TaskManager.Common.Utils;
using System.Linq.Expressions;
using TaskManager.Entity.Filter;
using TaskManager.Common.Mvc;
using TaskManager.Common.Exceptions;
using System.Net;

namespace TaskManager.Services
{
    public class ServerService : BaseService
    {
        private ServerRepository _serverRepository = new  ServerRepository();
        private TaskRepository _taskRepository = new TaskRepository();

        private IORMRepository<Ts_Servers> _ormServers; 

        private MailService _mailService;
        private ServersManage _serverManager;
        public ServerService() {
            _ormServers = _serverRepository.For<Ts_Servers>(); 
            _mailService = new MailService();
            _serverManager=ServersManage.GetInstance();
        }
        public bool Heart(int ServerId) {
            Ts_Servers Server = new Ts_Servers()
            {
                LastHeartTime = DateTime.Now,
            };
            try
            {

               return _ormServers.Update(Server, w => w.Id ==ServerId ) > 0;
                
            }
            catch (Exception ex) {
               
                log.Fatal(string.Format("Server:{0},服务器心跳异常", _serverManager.MyServer.ServerName),ex);
                return false;
            }
        }


        public int Register() {
            
            if (_serverManager.MyServer != null) {
                throw new Exception("已注册过，不能重试注册");
            }
            Ts_Servers Server = new Ts_Servers()
            {
                LastHeartTime = DateTime.Now,
            };
            int result = 0;
            string ServerName = Dns.GetHostName();
            try
            {
                
                IPAddress[] ServerIPs = Dns.GetHostAddresses(ServerName);

                var tempserver = GetServerId(ServerName);
                if (tempserver != null)
                {
                    if (!tempserver.IsEnable)
                    {
                        return -1;
                    }
                    result = _ormServers.Update(Server, w => w.Id == tempserver.Id);
                    Server.Id = tempserver.Id;
                }
                else
                {
                    Server.IsEnable = true;
                    Server.ServerName = ServerName;
                    Server.IsMain = false;
                    Server.ServerIP = ServerIPs.Select<IPAddress, string>(x => x.ToString()).ToJson();
                    result = (int)_ormServers.Add(Server);
                    Server.Id = result;
                }
                if (result <= 0)
                {
                    log.Fatal(string.Format("Server:{0},服务器注册失败，系统将无法正常运行", ServerName));
                    throw new Exception(string.Format("Server:{0},服务器注册失败，系统将无法正常运行", ServerName));
                }
                _serverManager.MyServer = new Ts_Servers {
                 Id= Server.Id,
                    ServerName =ServerName
                };

                _serverManager.OnServerCountChange += _serverManager_OnServerCountChange;
                _serverManager.OnDeadServer += _serverManager_OnDeadServer;
                _serverManager.StartUp(Heart, QueryUseServerAction);
                return Server.Id;
            }
            catch (Exception ex)
            {
                log.Fatal(string.Format("Server:{0},服务器注册异常，系统将无法正常运行", ServerName), ex);
                throw new Exception(string.Format("Server:{0},服务器注册异常，系统将无法正常运行", ServerName), ex);
            }
        }
        /// <summary>
        /// 测试服务器稳定用
        /// </summary>
        /// <param name="ServerName"></param>
        /// <returns></returns>
        public int RegisterTest(string ServerName) {
            
            if (_serverManager.MyServer != null) {
                throw new Exception("已注册过，不能重试注册");
            }
            Ts_Servers Server = new Ts_Servers()
            {
                LastHeartTime = DateTime.Now,
            };
            int result = 0;
          //  string ServerName = Dns.GetHostName();
            try
            {
                
               // IPAddress[] ServerIPs = Dns.GetHostAddresses(ServerName);

                var tempserver = GetServerId(ServerName);
                if (tempserver != null)
                {
                    if (!tempserver.IsEnable)
                    {
                        return -1;
                    }
                    result = _ormServers.Update(Server, w => w.Id == tempserver.Id);
                    Server.Id = tempserver.Id;
                }
                else
                {
                    Server.IsEnable = true;
                    Server.ServerName = ServerName;
                 //   Server.ServerIP = ServerIPs.Select<IPAddress, string>(x => x.ToString()).ToJson();
                    result = (int)_ormServers.Add(Server);
                    Server.Id = result;
                }
                if (result <= 0)
                {
                    log.Fatal(string.Format("Server:{0},服务器注册失败，系统将无法正常运行", ServerName));
                    throw new Exception(string.Format("Server:{0},服务器注册失败，系统将无法正常运行", ServerName));
                }
                _serverManager.MyServer = new Ts_Servers {
                 Id= Server.Id,
                    ServerName =ServerName
                };

                _serverManager.OnServerCountChange += _serverManager_OnServerCountChange;
                _serverManager.OnDeadServer += _serverManager_OnDeadServer;
                _serverManager.StartUp(Heart, QueryUseServerAction);
                return Server.Id;
            }
            catch (Exception ex)
            {
                log.Fatal(string.Format("Server:{0},服务器注册异常，系统将无法正常运行", ServerName), ex);
                throw new Exception(string.Format("Server:{0},服务器注册异常，系统将无法正常运行", ServerName), ex);
            }
        }
        private void _serverManager_OnDeadServer(int nowServerCount, int oldServerCount)
        {
            log.Fatal(string.Format("有服务器不能正常运行了，机器数有原来的{0},变为{1}", oldServerCount, nowServerCount));
        }

        private void _serverManager_OnServerCountChange(int myServerId)
        {
            SetMain(myServerId);
        }

        private int QueryUseServerAction() {
                DateTime outTime = DateTime.Now.AddMinutes(0 - AppConfig.ServerNoServiceTime);
                return _ormServers.Count(w => w.IsEnable == true && w.LastHeartTime >= outTime);          
        }
        public Ts_Servers GetServerId(string ServerName) {
           return _ormServers.Find( w => w.ServerName == ServerName);
        }

        public List<Ts_Servers> GetAllServer() {
            return _ormServers.FindAll();
        }
        public List<Ts_Servers> GetUseServer()
        {
            DateTime outTime = DateTime.Now.AddMinutes(0-AppConfig.ServerNoServiceTime);
            return _ormServers.FindAll(w=>w.IsEnable==true&&w.LastHeartTime>= outTime);
        }
        public bool DisableServer(int Id) {

            return _ormServers.Update(new Ts_Servers() { IsEnable = false }, w => w.Id == Id) > 0;
        }
        private void SetMain(int myServerId) {
           

            bool IsGetMainLock = false;
            try
            {
                IsGetMainLock = _serverRepository.GetMainLock(myServerId);
                AllocationTask();
            }
            catch (Exception ex) {
                log.Error("ServerService类，SetMain方法异常", ex);
            }
            finally
            {
                if (IsGetMainLock)
                {
                    _serverRepository.UnMainLock(myServerId);
                }
            }
            
            
        }
        public bool AllocationTask()
        {
            var ServerList = GetUseServer();
            var serverCount = ServerList.Count;
            if (serverCount == 0)
                return false;

            for (int i = 0; i < serverCount; i++)
            {
                _taskRepository.AllocationTask(serverCount, ServerList[i].Id,i);
            }
            return true;
        }
    }
}
