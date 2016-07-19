using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Common { 
    public class AppConfig
    {

        public static string Conn_Task
        {
            get
            {
                return GetConnectionStrings("taskconnection");
            }
        }
        public static string[] SystemErrorMail
        {
            get
            {
                var ErrorMail = GetAppSetting("SystemErrorMail");
                if (ErrorMail != null)
                    return ErrorMail.Split(';');
                return null;
            }
        }
        private static readonly object _locktl = new object();


        public static int TaskInterval
        {
            get
            {
                if (taskInterval == null)
                {
                    lock (_locktl) {
                        if (taskInterval == null)
                        {
                            string taskIntervalStr = GetAppSetting("TaskInterval");
                            if (string.IsNullOrEmpty(taskIntervalStr))
                            {
                                return 1;
                            }

                            int tempTaskInterval = 1;
                            if (int.TryParse(taskIntervalStr, out tempTaskInterval))
                                taskInterval = tempTaskInterval;
                        }
                    }
                    
                }
                return taskInterval.Value;
            }
        }
        private static int? taskInterval;
        /// <summary>
        /// 服务器心跳时隔，分钟
        /// </summary>
        public const int ServerHeartInterval = 3;
        /// <summary>
        /// 服务器定义为超时时间
        /// </summary>
        public const int ServerNoServiceTime = 6;

        #region private
        private static string GetAppSetting(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get(key);
        }
        public static string GetConnectionStrings(string key)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
        #endregion
    }
}
