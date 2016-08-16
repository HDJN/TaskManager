using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TaskManager.Common.Mvc
{
    public class MyContext
    {

        /// <summary>
        /// 当前登录账户的标识
        /// </summary>
        /// <value>The identity.</value>
        public static string Identity
        {
            get
            {

                return CurrentUser.UserName;
            }
        }
        public static bool IsIsAuthenticated
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;    
            }
        }
        /// <summary>
        /// 当前登录账户的完整信息
        /// </summary>
        /// <value>The current user.</value>
        public static IdentityUser CurrentUser
        {
            get
            {

                if (HttpContext.Current.Session["UseInfo"] != null)
                {
                    return (IdentityUser)(HttpContext.Current.Session["UseInfo"]);
                }
                else
                {
                    string userinfo= HttpContext.Current.User.Identity.Name;
                    if (string.IsNullOrEmpty(userinfo)) {
                        FormsAuthentication.SignOut();
                        return null;
                    }
                    
                        IdentityUser user=Newtonsoft.Json.JsonConvert.DeserializeObject<IdentityUser>(userinfo);
                    HttpContext.Current.Session["UseInfo"] = user;
                    return user;
                }
            }
            set {
                HttpContext.Current.Session["UseInfo"] = value;
            }
        }

      
    }
}
