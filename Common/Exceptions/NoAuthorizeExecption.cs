using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TaskManager.Common.Exceptions
{
    /// <summary>
    /// 没有登录的异常
    /// </summary>
    [Serializable]
    public class NoAuthorizeExecption : Exception
    {
        public NoAuthorizeExecption()
            : base("你没权限访问，请联系管理员！")
        {

        }
    }
}
