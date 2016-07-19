using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmiteRepository;
using TaskManager.Common;
namespace TaskManager.Entity
{
    public partial class Ts_Servers
    { 
        [Ignore]
        public bool IsAbnormal
        {
            get { return LastHeartTime.AddMinutes(AppConfig.ServerNoServiceTime+1) < DateTime.Now; }
        }

    }
}
