using Newtonsoft.Json;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.Email;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Core.Repository.EmailServiceTemplateRepository;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.DomainModel.DataRepository;
using System.Linq;
using Promact.Core.Repository.MailSettingDetailsByProjectAndModule;
using NLog;

namespace Promact.Core.Repository.Client
{
    public class Client : IClient
    {
        #region Private Variables
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IOauthCallsRepository _oauthCallRepository;
        private readonly IEmailService _emailService;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IHttpClientService _httpClientService;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IEmailServiceTemplateRepository _emailTemplateRepository;
        private readonly IRepository<IncomingWebHook> _incomingWebHook;
        private readonly ApplicationUserManager _userManager;
        private readonly IMailSettingDetailsByProjectAndModuleRepository _mailSettingDetails;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public Client(IOauthCallsRepository oauthCallRepository, IStringConstantRepository stringConstant,
            IEmailService emailService, IAttachmentRepository attachmentRepository, IHttpClientService httpClientService,
            IEnvironmentVariableRepository envVariableRepository, ISlackUserRepository slackUserRepository,
            IEmailServiceTemplateRepository emailTemplateRepository, IRepository<IncomingWebHook> incomingWebHook,
            ApplicationUserManager userManager, IMailSettingDetailsByProjectAndModuleRepository mailSettingDetails)
        {
            _stringConstant = stringConstant;
            _oauthCallRepository = oauthCallRepository;
            _emailService = emailService;
            _attachmentRepository = attachmentRepository;
            _httpClientService = httpClientService;
            _envVariableRepository = envVariableRepository;
            _slackUserRepository = slackUserRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _incomingWebHook = incomingWebHook;
            _userManager = userManager;
            _mailSettingDetails = mailSettingDetails;
            _logger = LogManager.GetLogger("ClientRepository");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The below method use for updating slack message without attachment.
        /// </summary>
        /// <param name="responseUrl">Incoming Web-hook url of user to whom message to be send</param>
        /// <param name="replyText">Reply text to be send</param>
        public async Task UpdateMessageAsync(string responseUrl, string replyText)
        {
            var slashResponseText = new SlashResponse() { Text = replyText };
            var slashResponseJsonText = JsonConvert.SerializeObject(slashResponseText);
            await _httpClientService.PostAsync(responseUrl, slashResponseJsonText, _stringConstant.JsonContentString);
        }

        /// <summary>
        /// The below method used for sending resposne back to slack for a slash command in ephemeral mood. Required field response_url.
        /// </summary>
        /// <param name="responseUrl">Incoming Web-hook url of user to whom message to be send</param>
        /// <param name="replyText">Text to be send to slack</param>
        public async Task SendMessageAsync(string responseUrl, string replyText)
        {
            var slashResponseText = new SlashResponse() { ResponseType = _stringConstant.ResponseTypeEphemeral, Text = replyText };
            var slashResponseJsonText = JsonConvert.SerializeObject(slashResponseText);
            await _httpClientService.PostAsync(responseUrl, slashResponseJsonText, _stringConstant.JsonContentString);
        }

        /// <summary>
        /// The below method is used for sending meassage to all the TL and management people using Incoming 
        /// Webhook.Required field channel name(whom to send) and here I had override the bot name and its identity.
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <param name="accessToken">OAuth access token of user</param>
        /// <param name="replyText">Txt to be send to slack</param>
        /// <param name="userId">userId of user</param>
        public async Task SendMessageWithAttachmentIncomingWebhookAsync(LeaveRequest leaveRequest, string accessToken, string replyText, string userId)
        {
            // getting attachment as a string to be send on slack
            var attachment = _attachmentRepository.SlackResponseAttachment(Convert.ToString(leaveRequest.Id), replyText);
            await GetAttachmentAndSendToTLAndManagementAsync(userId, leaveRequest, accessToken, attachment);
        }

        /// <summary>
        /// Method used to send slack message and email to team leader and management without interactive button
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <param name="accessToken">User's OAuth access token</param>
        /// <param name="replyText">Reply text to send</param>
        /// <param name="userId">UserId of user</param>
        public async Task SendMessageWithoutButtonAttachmentIncomingWebhookAsync(LeaveRequest leaveRequest, string accessToken, string replyText, string userId)
        {
            // getting attachment as a string to be send on slack
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(leaveRequest.Id), replyText);
            await GetAttachmentAndSendToTLAndManagementAsync(userId, leaveRequest, accessToken, attachment);
        }

        /// <summary>
        /// Method to send slack message to user whom leave has been applied by admin
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <param name="managementEmail">Management email address</param>
        /// <param name="replyText">Reply text to be send to user</param>
        /// <param name="user">User details</param>
        public async Task SendSickLeaveMessageToUserIncomingWebhookAsync(LeaveRequest leaveRequest, string managementEmail, string replyText, User user)
        {
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(leaveRequest.Id), replyText);
            SlackUserDetailAc slackUser = await _slackUserRepository.GetByIdAsync(user.SlackUserId);
            if (slackUser != null)
            {
                var incomingWebHook = await _incomingWebHook.FirstOrDefaultAsync(x => x.UserId == slackUser.UserId);
                var slashIncomingWebhookText = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
                var slashIncomingWebhookJsonText = JsonConvert.SerializeObject(slashIncomingWebhookText);
                if (incomingWebHook != null)
                    await _httpClientService.PostAsync(incomingWebHook.IncomingWebHookUrl, slashIncomingWebhookJsonText, _stringConstant.JsonContentString);
            }
            EmailApplication email = new EmailApplication();
            email.To = new List<string>();
            // creating email templates corresponding to leave applied
            email.Body = _emailTemplateRepository.EmailServiceTemplateSickLeave(leaveRequest);
            email.From = managementEmail;
            email.Subject = _stringConstant.EmailSubject;
            email.To.Add(user.Email);
            _emailService.Send(email);
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Private method to get reply text and send to team leader and management
        /// </summary>
        /// <param name="userId">User's user Id</param>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <param name="accessToken">User's OAuth access token</param>
        /// <param name="attachment">Attachment to be send to team leader and management</param>
        private async Task GetAttachmentAndSendToTLAndManagementAsync(string userId, LeaveRequest leaveRequest, string accessToken, List<SlashAttachment> attachment)
        {
            EmailApplication email = new EmailApplication();
            email.To = new List<string>();
            email.CC = new List<string>();
            var listOfprojectRelatedToUser = (await _oauthCallRepository.GetListOfProjectsEnrollmentOfUserByUserIdAsync(userId, accessToken)).Select(x => x.Id).ToList();
            _logger.Debug("List of project, user has enrollement : " + listOfprojectRelatedToUser.Count);
            foreach (var projectId in listOfprojectRelatedToUser)
            {
                var mailsetting = await _mailSettingDetails.GetMailSettingAsync(projectId, _stringConstant.LeaveModule, userId);
                email.To.AddRange(mailsetting.To);
                email.CC.AddRange(mailsetting.CC);
            }
            _logger.Debug("List of To : " + email.To.Count);
            _logger.Debug("List of CC : " + email.CC.Count);
            email.To = email.To.Distinct().ToList();
            email.CC = email.CC.Distinct().ToList();
            var teamLeaderIds = (await _oauthCallRepository.GetTeamLeaderUserIdAsync(userId, accessToken)).Select(x=>x.Id).ToList();
            _logger.Debug("List of team leaders : " + teamLeaderIds.Count);
            var managementIds = (await _oauthCallRepository.GetManagementUserNameAsync(accessToken)).Select(x=>x.Id).ToList();
            _logger.Debug("List of managements : " + managementIds.Count);
            var userEmail = (await _userManager.FindByIdAsync(userId)).Email;
            teamLeaderIds.AddRange(managementIds);
            foreach (var teamLeaderId in teamLeaderIds)
            {
                var user = await _userManager.FindByIdAsync(teamLeaderId);
                _logger.Debug("Team leader user name : " + user.UserName);
                var slackUser = await _slackUserRepository.GetByIdAsync(user.SlackUserId);
                if (slackUser != null)
                {
                    _logger.Debug("Slack details of team leader : " + slackUser.Name);
                    var incomingWebHook = await _incomingWebHook.FirstOrDefaultAsync(x => x.UserId == user.SlackUserId);
                    //Creating an object of SlashIncomingWebhook as this format of value required while responsing to slack
                    var slashIncomingWebhookText = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
                    var slashIncomingWebhookJsonText = JsonConvert.SerializeObject(slashIncomingWebhookText);
                    if (incomingWebHook != null)
                        await _httpClientService.PostAsync(incomingWebHook.IncomingWebHookUrl, slashIncomingWebhookJsonText, _stringConstant.JsonContentString);
                }
            }
            if (email.To.Any())
            {
                if (leaveRequest.EndDate != null)
                {
                    // creating email templates corresponding to leave applied for casual leave
                    email.Body = _emailTemplateRepository.EmailServiceTemplate(leaveRequest);
                }
                else
                {
                    // creating email templates corresponding to leave applied for casual leave
                    email.Body = _emailTemplateRepository.EmailServiceTemplateSickLeave(leaveRequest);
                }
                email.From = userEmail;
                email.Subject = _stringConstant.EmailSubject;
                _emailService.Send(email);
            }
        }
        #endregion
    }
}
