using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;

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
            Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(StringConstant.WebConfig);
            MailSettingsSectionGroup mailSettings = configurationFile.GetSectionGroup(StringConstant.MailSetting) as MailSettingsSectionGroup;
            MailMessage message = new MailMessage();
            message.From = new MailAddress(email.From);
            message.To.Add(new MailAddress(email.To));
            message.Subject = email.Subject;
            message.Body = email.Body;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Host = mailSettings.Smtp.Network.Host;
            client.Port = mailSettings.Smtp.Network.Port;
            client.Credentials = new System.Net.NetworkCredential(mailSettings.Smtp.From, mailSettings.Smtp.Network.Password);
            client.EnableSsl = mailSettings.Smtp.Network.EnableSsl;
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

