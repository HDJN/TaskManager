using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using TaskManager.Common;
using System.Net.Mail;
using System.Net;

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
        public void ErrorAndEmail(string msg,Exception ex)
        {
            _logger.Error(string.Format("{0}\r\n{1}",msg,ex.StackTrace));
            SendEmail("系统异常，尽快检查", msg, AppConfig.SystemErrorMail);
        }
        public void ErrorAndEmail(string msg)
        {
            _logger.Error(msg);
            SendEmail("系统异常，尽快检查", msg, AppConfig.SystemErrorMail);
        }
        public void Error(string msg, Exception ex)
        {
            _logger.Error(string.Format("{0}\r\n{1}", msg, ex.StackTrace));
        }
        public void Fatal(string msg, Exception ex)
        {
            msg = string.Format("{0}\r\n{1}", msg, ex.StackTrace);
            _logger.Error(msg);
            SendEmail("系统异常，不能继续运行", msg, AppConfig.SystemErrorMail);
        }
        public void Fatal(string msg)
        {
            _logger.Fatal(msg);
            SendEmail("系统异常，不能继续运行", msg, AppConfig.SystemErrorMail);
        }
        private bool SendEmail(string subject, string body, string[] sendUsersAddress)
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "192.168.1.210";
                smtp.Credentials = new NetworkCredential("deploy", "cndns22!");
                smtp.Port = 25;
                MailMessage mm = new MailMessage();
                mm.From = new MailAddress("deploy@corp.ppdai.com", "noreply");
                for (int i = 0; i < sendUsersAddress.Length; i++)
                {
                    mm.To.Add(sendUsersAddress[i]);//将邮件发送给Gmail
                    //mm.To.Add("zhangpengchao@ppdai.com");//将邮件发送给Gmail
                }
                mm.Body = body;
                mm.Subject = subject;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtp.Send(mm);
            }
            catch (Exception ex)
            {
                Error("发送邮件失败!", ex);
                return false;
            }
            return true;
        }
    }
}
