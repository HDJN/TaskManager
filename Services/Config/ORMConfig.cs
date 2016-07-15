
using SmiteRepository;
using SmiteRepository.Extansions;
using TaskManager.Entity;
using System;
using TaskManager.Services;

namespace TaskManager.Services.Config
{
    public class ORMConfig
    {
        public static void RegisterORM()
        { 

            SmiteRepository.Extansions.RegisterORM.Register_CustomTableNameToInsert<Ts_ExecLog>(delegate (Ts_ExecLog Entity)
            {
                  return string.Format("ts_ExecLog_{0}",System.DateTime.Now.ToString("yyyyMM"));
            });


            SmiteRepository.Extansions.RegisterORM.Register_CustomTableNameToSelect<Ts_ExecLog>(w => 
            w.TaskGuid == "",
             delegate (object[] SqlParams,Ts_ExecLog Entity)
            {
                return new string[] { string.Format("ts_ExecLog_{0}", System.DateTime.Now.ToString("yyyyMM")) };
            });


            SmiteRepository.Extansions.RegisterORM.Register_CustomTableNameToSelect<Ts_ExecLog>(w => 
            w.TaskGuid == ""&& w.ExecStatrtTime > DateTime.Now && w.ExecEndTime <= DateTime.Now,
             delegate (object[] SqlParams, Ts_ExecLog Entity)
             {
                 DateTime start = (DateTime)SqlParams[1];
                 DateTime end = (DateTime)SqlParams[2];
              
                 return new string[] { string.Format("ts_ExecLog_{0}", System.DateTime.Now.ToString("yyyyMM")) };
             });

            
        }
    }
}
