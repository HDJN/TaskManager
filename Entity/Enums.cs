using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TaskManager.Entity
{
   public enum ExecTypeEnum
    {
        HTTP=1,
        EXE =2
    }
    public enum StatusEnum
    {
        启用=1,
        禁用=-1,
    }
    public enum ExecMethodEnum {
        GET,
        POST,
        DELETE,
        HEAD,
        PUT
    }
}
