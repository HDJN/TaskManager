using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TaskManager.Common.Mvc
{
    public class BaseController : Controller
    {
        public bool IsAdmin
        {
            get
            { return MyContext.Identity == "admin"; }

        }
    }
}
