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
                return ( _ExecEndTime- _ExecStatrtTime ).Milliseconds;
            }
        }
       
    }
}
