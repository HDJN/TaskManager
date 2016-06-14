using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using TaskManager.Common;

namespace TaskManager.Services
{
    public class MailService:BaseService
    {
        public bool SendEmail(string subject, string body, string sendUsersAddress)
        {
            return SendEmail(subject, body, sendUsersAddress.Split(';'));
        }
        public bool SendEmail(string subject, string body, string[] sendUsersAddress)
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
                log.Error("发送邮件失败!", ex);
                return false;
            }
            return true;
        }
    }
}
