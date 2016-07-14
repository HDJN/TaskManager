using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Services;
using TaskManager.Entity;
using TaskManager.Entity.Filter;
using TaskManager.Common.Mvc;

namespace TaskManager.Web.Controllers
{
    [MyAuthorize]
    public class ServerController : BaseController
    {
        private ServerService _serverService = new ServerService();
        // GET: Task
        public ActionResult ServerList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ServerList(int Status=-1)
        {
            var list = _serverService.GetAllServer();
            PageList<Ts_Servers> pageList = new PageList<Ts_Servers>() { DataList = list };
            return Json(pageList);
        }
    }
}
