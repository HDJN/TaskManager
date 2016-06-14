using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager.Common.Exceptions
{
    public class BOException : Exception
    {
        private int _ErrorCode = -1;

        public int ErrorCode
        {
            get { return _ErrorCode; }
            set { _ErrorCode = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        public BOException(string message)
            : base(message)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public BOException(string message, Exception ex)
            : base(message, ex)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        public BOException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public BOException(int errorCode, string message, Exception ex)
            : base(message, ex)
        {
            ErrorCode = errorCode;
        }
    }
}
