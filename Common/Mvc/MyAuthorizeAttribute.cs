using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace TaskManager.Common.Mvc
{ 
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (filterContext == null)
            {
                filterContext.Result = new RedirectResult("/Home/Login");
                return;
            }

           
            if (!MyContext.IsIsAuthenticated)
            {

                filterContext.Result = new RedirectResult("/Home/Login");
                return;
            }
           
        }

      
       
    }
}
