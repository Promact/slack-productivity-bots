using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Net.Mail;
using System.Text;

namespace Promact.Erp.Util.Email
{
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Method used to send e-mails
        /// </summary>
        /// <param name="email">email.from, email.to, email.body, email.subject</param>
        public void Send(EmailApplication email)
        {
            MailMessage message = new MailMessage(email.From, email.To);
            message.Subject = email.Subject;
            message.Body = email.Body;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

