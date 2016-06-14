using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.Common;
using SmiteRepository.Sqlserver;
using SmiteRepository;
namespace TaskManager.Repositories
{
    public class TaskRepository : BaseRepository
    {
        public TaskRepository() : base(AppConfig.Conn_Task)
        {

        }

    }
}
