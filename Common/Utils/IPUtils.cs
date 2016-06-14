using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Management;
using System.Web;

namespace TaskManager.Common.Utils
{
    public class IPUtils
    {
        private static string _LocalIP = string.Empty;
        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            if (string.IsNullOrEmpty(_LocalIP))
            {
                try
                {
                    System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                    for (int i = 0; i < addressList.Length; i++)
                    {
                        _LocalIP = addressList[i].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("获取本机IP地址出错", ex);
                }
            }
            return _LocalIP;
        }

        private static string _LocalMAC = string.Empty;
      

        /// <summary>
        /// 获取web系统客户端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIP()
        {
            string usip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(usip))
                usip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return usip;
        }

    }
}
