using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
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
                client.Host = Environment.GetEnvironmentVariable(StringConstant.Host, EnvironmentVariableTarget.Process);
                client.Port = Convert.ToInt32(Environment.GetEnvironmentVariable(StringConstant.Port, EnvironmentVariableTarget.Process) );
                client.Credentials = new System.Net.NetworkCredential(Environment.GetEnvironmentVariable(StringConstant.From, EnvironmentVariableTarget.Process), Environment.GetEnvironmentVariable(StringConstant.Password, EnvironmentVariableTarget.Process));
                client.EnableSsl = Convert.ToBoolean(Environment.GetEnvironmentVariable(StringConstant.EnableSsl, EnvironmentVariableTarget.Process));
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

