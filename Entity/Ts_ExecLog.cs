using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmiteRepository;
namespace TaskManager.Entity
{
    public partial class Ts_ExecLog
    { 
        [Ignore]
        public int ExecTime
        {
            get
            {
                  if(_ExecEndTime.HasValue)
                    return ( _ExecEndTime.Value- _ExecStatrtTime).Milliseconds;
                return -1;
            }
        }
       
    }
}
