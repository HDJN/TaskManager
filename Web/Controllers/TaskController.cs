﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Services;
using TaskManager.Entity;
using TaskManager.Entity.Filter;
using TaskManager.Common.Mvc;
using TaskManager.Common.Exceptions;
using TaskManager.Tasks;
using TaskManager.Common.Corn;

namespace TaskManager.Web.Controllers
{
    [MyAuthorize]
    public class TaskController : BaseController
    {
        private TaskService _taskService = new TaskService();
        // GET: Task
        public ActionResult TaskList()
        {
            ViewBag.IsAdmin = IsAdmin;        
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
            foreach (var item in list.DataList)
            {
                item.ExecResult = Server.HtmlEncode(item.ExecResult);
            }
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
            tasks.TimeOut = 30;
            tasks.RunServerId = _taskService.GetNewRunServerId();
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
        [AdminAuthorize]
        [HttpPost]
        public ActionResult RemoveTask(string TaskGuid)
        {
            JsonReturnMessages msg = new JsonReturnMessages();
            try
            {
              _taskService.RemoveTask(TaskGuid);
                msg.IsSuccess = true;
            }
            catch (BOException ex)
            {
                msg.Msg = ex.Message;
            }
            return Json(msg);
        }
        [HttpPost]
        public ActionResult QueryNextTime(string Interval)
        {
            JsonReturnMessages msg = new JsonReturnMessages();
            try
            {
                msg.IsSuccess = true;
                CronExpression expression = new CronExpression(Interval);
                List<string> nextTimeList = new List<string>();
                DateTimeOffset now = DateTimeOffset.Now;
                for (int i = 0; i < 10; i++)
                {
                    var lastOffset = expression.GetNextValidTimeAfter(now);
                    if (lastOffset != null)
                    {
                        now = lastOffset.Value;
                        nextTimeList.Add(lastOffset.Value.LocalDateTime.ToString());
                    }
                }
                msg.Data = nextTimeList;
            }
            catch (Exception ex)
            {
                msg.Msg = "cron表达式错误";
            }
            return Json(msg);
        }
    }
}
