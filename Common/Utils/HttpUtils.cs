using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace TaskManager.Common.Utils
{
    public static class HttpUtils
    {
        public static string Post(string url, Dictionary<string, string> data, Encoding encoding, int timeout= 60000)
        {
            string strdata = DictToStr(data, "&", encoding);
            return Net.HttpRequest(url, strdata, "POST", timeout, encoding);
        }

        public static string Get(string url, Dictionary<string, string> data, Encoding encoding, int timeout= 60000)
        {
            string strdata = DictToStr(data, "&", encoding);
            return Net.HttpRequest(url, strdata, "GET", timeout, encoding);
        }

        /// <summary>
        /// 字典转字符串
        /// </summary>
        /// <param name="dict">字典类型数据</param>
        /// <param name="str_join">连接字符串</param>
        /// <param name="coding">url编码</param>
        /// <returns>字典数据的key和value组成的字符串</returns>
        private static string DictToStr(Dictionary<string, string> dict, string str_join, Encoding coding)
        {
            if (dict == null)
                return string.Empty;
            //连接字符串
            str_join = str_join == null ? "&" : str_join;
            StringBuilder result = new StringBuilder();
            string value = string.Empty;
            int i = 0;
            foreach (KeyValuePair<string, string> kv in dict)
            {
                value = HttpUtility.UrlEncode(kv.Value, coding);
                result.AppendFormat("{0}{1}={2}", i > 0 ? str_join : "", kv.Key, value);
                i++;
            }
            return result.ToString();
        }
    }

    public static class HttpVerbs
    {

        public const string POST = "POST";
        public const string GET = "GET";
        public const string PUT = "PUT";
        public const string DELETE = "DELETE";
        public const string HEAD = "HEAD";
    }
    /**/
    /// <summary>
    /// Net : 提供静态方法，对常用的网络操作进行封装
    /// </summary>
    public sealed class Net
    {

        /// <summary>
        /// 返回URL内容,带POST数据提交
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="data">数据</param>
        /// <param name="method">GET/POST(默认)</param>
        /// <param name="timeout">超时时间（以毫秒为单位）</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static string HttpRequest(string url, string data, string method, int timeout, Encoding encoding)
        {
            string res = string.Empty;
            //Encoding encoding = Encoding.GetEncoding("utf-8");

            //请求
            WebRequest webRequest = null;
            Stream postStream = null;

            //响应
            WebResponse webResponse = null;
            StreamReader streamReader = null;

            try
            {
                if (method.ToUpper() == "GET")
                {
                    if (url.IndexOf("?") > 0)
                    {
                        url = url + '&' + data;
                    }
                    else
                    {
                        url = url + '?' + data;
                    }
                }
                //请求
                webRequest = WebRequest.Create(url);
                webRequest.Method = string.IsNullOrEmpty(method) ? "POST" : method;
                webRequest.Timeout = timeout;

                if (method.ToUpper() == "POST")
                {
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    byte[] postData = encoding.GetBytes(data);
                    webRequest.ContentLength = postData.Length;
                    postStream = webRequest.GetRequestStream();
                    postStream.Write(postData, 0, postData.Length);
                }
                //响应
                webResponse = webRequest.GetResponse();
                streamReader = new StreamReader(webResponse.GetResponseStream(), encoding);
                res = streamReader.ReadToEnd();
            }
            catch (WebException ex)
            {
                using (HttpWebResponse response = (HttpWebResponse)ex.Response)
                {
                    if (response != null)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            res = new StreamReader(responseStream).ReadToEnd();
                            res = ":(" + res;
                        }
                    }
                    else
                    {
                        res = ":(我猜是连接超时";
                    }
                }
            }
            catch (Exception ex)
            {
                res = ":("+ex.Message;
            }
            finally
            {
                if (postStream != null)
                {
                    postStream.Close();
                }
                if (streamReader != null)
                {
                    streamReader.Close();
                }
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }

            return res;
        }


      

    }
}
