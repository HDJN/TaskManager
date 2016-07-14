using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Services;
using TaskManager.Common.Mvc;
using TaskManager.Entity;
using System.Web.Security;
using TaskManager.Common.Exceptions;

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        private SystemService _sysService = new SystemService();
        private TaskService _taskService = new TaskService();
        [MyAuthorize]
        public ActionResult Index()
        {
            return View();
        } 
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult NoRight() {
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult Login(string UserName, string PassWord)
        {
            JsonReturnMessages msg = new JsonReturnMessages();
            try
            {
                IdentityUser user = _sysService.UserAuth(UserName, PassWord);
            MyContext.CurrentUser = user;
                msg.IsSuccess = true;
            //FormsAuthentication.SetAuthCookie(user.UserId.ToLower(), false);
            FormsAuthentication.SetAuthCookie(user.UserName.ToLower(), false);
            }
            catch (BOException ex)
            {
                msg.Data = ex.ErrorCode;
                msg.Msg = ex.Message;
            }

            return Json(msg);

        }
        [MyAuthorize]
        public ActionResult RestTask()
        {
            _taskService.RestAll();
            return Content("重启成功");
        }
       
    }
}