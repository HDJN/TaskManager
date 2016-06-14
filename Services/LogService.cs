using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace TaskManager.Services
{
    public class LogService
    {
        /*
            logger.Trace("Sample trace message");
            logger.Debug("Sample debug message");
            logger.Info("Sample informational message");
            logger.Warn("Sample warning message");
            logger.Error("Sample error message");
            logger.Fatal("Sample fatal error message");

            // alternatively you can call the Log() method 
            // and pass log level as the parameter.
            logger.Log(LogLevel.Info, "Sample fatal error message");
        */
        private static Logger _logger = LogManager.GetLogger("Myb");
        public void Trace(string msg)
        {
            _logger.Trace(msg);
        }
        public void Debug(string msg)
        {
            _logger.Debug(msg);
        }
        public void Warn(string msg)
        {
            _logger.Warn(msg);
        }
        public void Info(string msg)
        {
            _logger.Info(msg);
        }
        public void Error(string msg)
        {
            _logger.Error(msg);
        }
        public void Error(string msg,Exception ex)
        {
            _logger.Error(string.Format("{0}\r\n{1}",msg,ex.StackTrace));
        }
        public void Fatal(string msg)
        {
            _logger.Fatal(msg);
        }
    }
}
