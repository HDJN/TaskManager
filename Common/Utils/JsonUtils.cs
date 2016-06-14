using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TaskManager.Common.Utils
{
    public static class JsonUtils
    {
        public static T FromJson<T>(this string str)
        {
            if (str == null) str = string.Empty;
            return  JsonConvert.DeserializeObject<T>(str);
        }

        public static string ToJson(this Object value)
        {
            if (value == null) return string.Empty;
            return JsonConvert.SerializeObject(value);
        }
    }
}
