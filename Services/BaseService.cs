using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Services
{
    public class BaseService
    {
        public BaseService()
        {
            log = new LogService();
        }
        protected LogService log;
    }
}
