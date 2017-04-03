using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.EnvironmentVariableRepository;
using System;
using System.Net.Mail;
using System.Text;
using NLog;

namespace Promact.Erp.Util.Email
{
    public class EmailService : IEmailService
    {
        #region Private Variables
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly ILogger _logger;
        #endregion

        #region Constructor

        public EmailService(IEnvironmentVariableRepository envVariableRepository)
        {
            _envVariableRepository = envVariableRepository;
            _logger = LogManager.GetLogger("EmailService");
        }

        #endregion

        #region Public Method
        /// <summary>
        /// Method used to send e-mails
        /// </summary>
        /// <param name="email">email.from, email.to, email.body, email.subject</param>
        public void Send(EmailApplication email)
        {
            try
            {
                _logger.Info("something");
                MailMessage message = new MailMessage();
                _logger.Debug("Email send from : " + email.From);
                message.From = new MailAddress(email.From);
                foreach (var to in email.To)
                {
                    _logger.Debug("Email send to : " + to);
                    message.To.Add(new MailAddress(to));
                }
                foreach (var cc in email.CC)
                {
                    _logger.Debug("Email send cc : " + cc);
                    message.CC.Add(new MailAddress(cc));
                }
                _logger.Debug("Email send subject : " + email.Subject);
                message.Subject = email.Subject;
                _logger.Debug("Email send body is null : " + string.IsNullOrEmpty(email.Body));
                message.Body = email.Body;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                _logger.Debug("Email send Host : " + _envVariableRepository.Host);
                client.Host = _envVariableRepository.Host;
                _logger.Debug("Email send Port : " + _envVariableRepository.Port);
                client.Port = _envVariableRepository.Port;
                _logger.Debug("Email send environment variable Username : " + _envVariableRepository.MailUserName);
                _logger.Debug("Email send environment variable password is null : " + string.IsNullOrEmpty(_envVariableRepository.Password));
                client.Credentials = new System.Net.NetworkCredential(_envVariableRepository.MailUserName, _envVariableRepository.Password);
                _logger.Debug("Email send enableSSL : " + _envVariableRepository.EnableSsl);
                client.EnableSsl = Convert.ToBoolean(_envVariableRepository.EnableSsl);
                _logger.Debug("Try to send email");
                client.Send(message);
                _logger.Debug("Email sended successfully");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
#pragma warning disable CS0618 // Type or member is obsolete
                    _logger.Error("Error Occured in email sending : " + ex.InnerException.Message, ex);
#pragma warning restore CS0618 // Type or member is obsolete
                else
#pragma warning disable CS0618 // Type or member is obsolete
                    _logger.Error("Error Occured in email sending : " + ex.Message, ex);
#pragma warning restore CS0618 // Type or member is obsolete
                throw ex;
            }
        }
        #endregion
    }
}

