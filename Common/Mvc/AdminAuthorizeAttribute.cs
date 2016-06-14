using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace TaskManager.Common.Mvc
{ 
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (filterContext == null)
            {
                filterContext.Result = new RedirectResult("/Home/NoRight");
                return;
            }

           
            if (MyContext.Identity!="admin")
            {

                filterContext.Result = new RedirectResult("/Home/NoRight");
                return;
            }
           
        }

      
       
    }
}
