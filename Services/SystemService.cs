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
using TaskManager.Common.Mvc;
using TaskManager.Common.Exceptions;

namespace TaskManager.Services
{
    public class SystemService :BaseService
    {

        private UserRepository userRepository = new UserRepository();

        private IORMRepository<Tu_Users> _ormUsers;
        public SystemService()
        {
            userRepository = new UserRepository();
            _ormUsers = userRepository.For<Tu_Users>();
        }

        public IdentityUser UserAuth(string userName, string passWord)
        {
            Tu_Users u = _ormUsers.Find(w => w.UserName == userName);
            if (u != null)
            {
                if (u.PassWord == passWord)
                {
                    //return GetUserInfo(userName);
                    IdentityUser id = new IdentityUser();
                    id.UserId = u.UserId.ToString();
                    id.UserName = u.UserName;

                    return id;
                }

            }
            throw new BOException("登录失败，账号或密码错误");
        }

      
        public List<Tu_Users> GetAllUser(int Status)
        {
            if (Status == -1)
                return _ormUsers.FindAll();
            return _ormUsers.FindAll(w => w.Status == Status);
        }

        public Tu_Users GetUserInfo(int UserId)
        {

            return _ormUsers.Find(w => w.UserId == UserId);
        }
        public bool DeleteUser(int UserId)
        {
            return _ormUsers.Delete(w => w.UserId == UserId) > 0;
        }
        public bool SaveUser(Tu_Users user)
        {
            if (user.UserId==0)
            {
                user.InsertTime = DateTime.Now;
                
                return  _ormUsers.Add(user) > 0;
             
            }
            return _ormUsers.Update(user) > 0;
        }
    }
}
