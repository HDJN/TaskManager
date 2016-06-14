using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TaskManager.Services;
using TaskManager.Common.Utils;

namespace WebTest.Controllers
{
    public class TestApiController : Controller
    {
        private LogService _log = new LogService();
        [HttpGet]
        public ActionResult GetTest1(string Name) {
            var result = new { Code=0,Msg="ok",Data=DateTime.Now};
            _log.Trace("test1访问时间" + DateTime.Now);
            return Content(result.ToJson());
        }
        [HttpGet]
        public ActionResult GetTest2(string Name)
        {

            _log.Trace("test2访问时间" + DateTime.Now);
            return Content("ok");
        }
    }
}
