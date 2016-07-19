using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.Common;
using SmiteRepository.Sqlserver;
using SmiteRepository;
namespace TaskManager.Repositories
{
    public class ServerRepository : BaseRepository
    {
        public ServerRepository() : base(AppConfig.Conn_Task)
        {
           
           
        }
        public bool GetMainLock(int selfId) {

            const string sql =@"if(not exists(select 1 from ts_Servers with(updlock) where isMain=1))
                            begin
                                update ts_Servers set isMain = 1 where Id = @SelfId
                                select 1
                            end";
            var MainId = base.GetScalar<int?>(sql, new {
                SelfId = selfId
           });
            return MainId == 1;
        }

        public bool UnMainLock(int selfId)
        {

          const  string sql = @"update ts_Servers set isMain=0 where isMain=1";
            return base.ExecuteCommand(sql) > 0;
        }


    }
}
