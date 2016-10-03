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
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(email.From);
                message.To.Add(new MailAddress(email.To));
                message.Subject = email.Subject;
                message.Body = email.Body;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.Host = EnvironmentVariableStore.GetEnvironmentVariableValues(StringConstant.Host);
                client.Port = Convert.ToInt32(EnvironmentVariableStore.GetEnvironmentVariableValues(StringConstant.Port) );
                client.Credentials = new System.Net.NetworkCredential(EnvironmentVariableStore.GetEnvironmentVariableValues(StringConstant.From), EnvironmentVariableStore.GetEnvironmentVariableValues(StringConstant.Password));
                client.EnableSsl = Convert.ToBoolean(EnvironmentVariableStore.GetEnvironmentVariableValues(StringConstant.EnableSsl));
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

