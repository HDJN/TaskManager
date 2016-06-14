using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Services;
using TaskManager.Entity;
using TaskManager.Entity.Filter;
using TaskManager.Common.Mvc;
using TaskManager.Common.Exceptions;

namespace TaskManager.Web.Controllers
{
    [MyAuthorize]
    public class TaskController : Controller
    {
        private TaskService _taskService = new TaskService();
        // GET: Task
        public ActionResult TaskList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult TaskList(int Status=-1)
        {
            var list=_taskService.GetTaskList(Status);
            PageList<ModelTaskList> pageList = new PageList<ModelTaskList>() { DataList = list };
            return Json(pageList);
        }
        public ActionResult TaskLogs(string TaskGuid)
        {
            ViewBag.TaskGuid = TaskGuid;
            return View();
        }
        [HttpPost]
        public JsonResult TaskLogs(TaskLogFilter filter,PageView pageView)
        {
            var list = _taskService.GetAllTaskLog(filter,pageView); 
            return Json(list);
        }
        public ActionResult TaskEdit(string TaskGuid)
        {
            InitTask();
            Ts_Tasks tasks = _taskService.GetTaskByGuid(TaskGuid);
            return View("TaskEdit", tasks);
        }
        private void InitTask() {
            List<SelectListItem> ExecTypeList = new List<SelectListItem>();
            foreach (int item in Enum.GetValues(typeof(ExecTypeEnum)))
            {
                ExecTypeList.Add(new SelectListItem() { Value = item.ToString(), Text = Enum.GetName(typeof(ExecTypeEnum), item) });
            }
            ViewBag.ExecTypeList = ExecTypeList;
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (int item in Enum.GetValues(typeof(StatusEnum)))
            {
                StatusList.Add(new SelectListItem() { Value = item.ToString(), Text = Enum.GetName(typeof(StatusEnum), item) });
            }
            ViewBag.StatusList = StatusList;
            List<SelectListItem> ExecMethodList = new List<SelectListItem>();
            foreach (var item in Enum.GetNames(typeof(ExecMethodEnum)))
            {
                ExecMethodList.Add(new SelectListItem() { Value = item, Text = item });
            }
            ExecMethodList.Add(new SelectListItem() { Value = "", Text = "请选择" });
            ViewBag.ExecMethodList = ExecMethodList;
            
        }
        public ActionResult TaskAdd()
        {
            InitTask();

            Ts_Tasks tasks = new Ts_Tasks();
           // tasks.ExecMethod = "GET";
            return View("TaskEdit", tasks);
        }
        [HttpPost]
        public ActionResult SaveTask(Ts_Tasks tasks) {
            JsonReturnMessages msg = new JsonReturnMessages();
            try
            {
                msg.IsSuccess = _taskService.SaveTask(tasks);
                _taskService.RestTask(tasks);
            }
            catch (BOException ex) {
                msg.Msg = ex.Message;
            }
            return Json(msg);
        }
        [HttpPost]
        public ActionResult RunTask(string TaskGuid)
        {
            JsonReturnMessages msg = new JsonReturnMessages();
            try
            {
                msg.IsSuccess = _taskService.RunTask(TaskGuid);
            }
            catch (Exception ex) {
                msg.Msg = ex.Message;
            }
            return Json(msg);
        }
    }
}
