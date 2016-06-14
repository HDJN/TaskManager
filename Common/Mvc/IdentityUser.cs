using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TaskManager.Common.Mvc
{
    //public interface IUser
    //{
    //    string UserId { get; }
    //    string FullName { get; }

    //    string EmployID { get; }


    //    string OrgCode { get; set; }

    //    string OrgName { get; set; }
    //}

    public class IdentityUser
    {
        public string UserName { get; set; }
        public string UserId { get; set; }

        ///// <summary>
        ///// 所属组织分组Code
        ///// </summary>
        ///// <value>The org code.</value>
        //public string OrgCode { get; set; }
        ///// <summary>
        ///// 所属组织分组名称
        ///// </summary>
        ///// <value>The name of the org.</value>
        //public string OrgName { get; set; }
      

    }
}
