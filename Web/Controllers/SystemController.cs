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
    public class SystemController: BaseController
    {
        private SystemService _syskService = new SystemService();
        // GET: Task
        [AdminAuthorize]
        public ActionResult UserList()
        {
            return View();
        }
        [AdminAuthorize]
        [HttpPost]
        public JsonResult UserList(int Status=-1)
        { 
            var list= _syskService.GetAllUser(Status);
            PageList<Tu_Users> pageList = new PageList<Tu_Users>() { DataList = list };
            return Json(pageList);
        }
        [AdminAuthorize]
        public ActionResult UserEdit(int UserId)
        { 
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (int item in Enum.GetValues(typeof(StatusEnum)))
            {
                StatusList.Add(new SelectListItem() { Value = item.ToString(), Text = Enum.GetName(typeof(StatusEnum), item) });
            }
            ViewBag.StatusList = StatusList;
            Tu_Users user = _syskService.GetUserInfo(UserId);
            return View("UserEdit", user);
        }
        [AdminAuthorize]
        public ActionResult DeleteUser(int UserId)
        {
            JsonReturnMessages msg = new JsonReturnMessages();
            msg.IsSuccess = _syskService.DeleteUser(UserId);
            return Json(msg);
        }
        [AdminAuthorize]
        public ActionResult UserAdd()
        { 

            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (int item in Enum.GetValues(typeof(StatusEnum)))
            {
                StatusList.Add(new SelectListItem() { Value = item.ToString(), Text = Enum.GetName(typeof(StatusEnum), item) });
            }
            ViewBag.StatusList = StatusList;
            Tu_Users user = new Tu_Users(); 
            return View("UserEdit", user);
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult SaveUser(Tu_Users user) { 
            JsonReturnMessages msg = new JsonReturnMessages();
           msg.IsSuccess= _syskService.SaveUser(user);
            return Json(msg);
        }
    }
}
