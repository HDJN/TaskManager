using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.Common;
using SmiteRepository.Sqlserver;
using SmiteRepository;
namespace TaskManager.Repositories
{
    public class UserRepository: BaseRepository
    {
        public UserRepository() : base(AppConfig.Conn_Task)
        {

        }

    }
}
