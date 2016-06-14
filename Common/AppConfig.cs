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
     


        public static int TaskInterval
        {
            get
            {
                string taskIntervalStr= GetAppSetting("TaskInterval");
                if (string.IsNullOrEmpty(taskIntervalStr)) {
                    return 1;
                }
                int taskInterval = 1;
                if (int.TryParse(taskIntervalStr, out taskInterval))
                    return taskInterval;
                return 1;
            }
        }

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
