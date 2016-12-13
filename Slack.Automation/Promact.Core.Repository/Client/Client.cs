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
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Promact.Core.Repository.EmailServiceTemplateRepository;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.DomainModel.DataRepository;

namespace Promact.Core.Repository.Client
{
    public class Client : IClient
    {
        #region Private Variables
        private HttpClient _chatUpdateMessage;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IOauthCallsRepository _oauthCallRepository;
        private readonly IEmailService _emailService;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IHttpClientService _httpClientService;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IEmailServiceTemplateRepository _emailTemplateRepository;
        private readonly IRepository<IncomingWebHook> _incomingWebHook;
        #endregion

        #region Constructor
        public Client(IOauthCallsRepository oauthCallRepository, IStringConstantRepository stringConstant,
            IEmailService emailService, IAttachmentRepository attachmentRepository, IHttpClientService httpClientService,
            IEnvironmentVariableRepository envVariableRepository, ISlackUserRepository slackUserRepository,
            IEmailServiceTemplateRepository emailTemplateRepository, IRepository<IncomingWebHook> incomingWebHook)
        {
            _stringConstant = stringConstant;
            _chatUpdateMessage = new HttpClient();
            _chatUpdateMessage.BaseAddress = new Uri(_stringConstant.SlackChatUpdateUrl);
            _oauthCallRepository = oauthCallRepository;
            _emailService = emailService;
            _attachmentRepository = attachmentRepository;
            _httpClientService = httpClientService;
            _envVariableRepository = envVariableRepository;
            _slackUserRepository = slackUserRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _incomingWebHook = incomingWebHook;
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
        /// <param name="replyText">Text to be send to slack</param>
        /// <param name="slackUserId">Slack user Id of user</param>
        public async Task SendMessageWithAttachmentIncomingWebhookAsync(LeaveRequest leaveRequest, string accessToken, string replyText, string slackUserId)
        {
            // getting attachment as a string to be send on slack
            var attachment = _attachmentRepository.SlackResponseAttachment(Convert.ToString(leaveRequest.Id), replyText);
            await GetAttachmentAndSendToTLAndManagementAsync(slackUserId, leaveRequest, accessToken, attachment);
        }

        /// <summary>
        /// Method used to send slack message and email to team leader and management without interactive button
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <param name="accessToken">User's OAuth access token</param>
        /// <param name="replyText">Reply text to send</param>
        /// <param name="slackUserId">USer's slack user Id</param>
        public async Task SendMessageWithoutButtonAttachmentIncomingWebhookAsync(LeaveRequest leaveRequest, string accessToken, string replyText, string slackUserId)
        {
            // getting attachment as a string to be send on slack
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(leaveRequest.Id), replyText);
            await GetAttachmentAndSendToTLAndManagementAsync(slackUserId, leaveRequest, accessToken, attachment);
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
            SlackUserDetails slackUser = await _slackUserRepository.GetByIdAsync(user.SlackUserId);
            var incomingWebHook = await _incomingWebHook.FirstOrDefaultAsync(x => x.UserId == slackUser.UserId);
            var slashIncomingWebhookText = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
            var slashIncomingWebhookJsonText = JsonConvert.SerializeObject(slashIncomingWebhookText);
            if (incomingWebHook != null)
                await _httpClientService.PostAsync(incomingWebHook.IncomingWebHookUrl, slashIncomingWebhookJsonText, _stringConstant.JsonContentString);
            EmailApplication email = new EmailApplication();
            // creating email templates corresponding to leave applied
            email.Body = _emailTemplateRepository.EmailServiceTemplateSickLeave(leaveRequest);
            email.From = managementEmail;
            email.Subject = _stringConstant.EmailSubject;
            email.To = user.Email;
            _emailService.Send(email);
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Private method to get reply text and send to team leader and management
        /// </summary>
        /// <param name="slackUserId">User's slack user Id</param>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <param name="accessToken">User's OAuth access token</param>
        /// <param name="attachment">Attachment to be send to team leader and management</param>
        private async Task GetAttachmentAndSendToTLAndManagementAsync(string slackUserId, LeaveRequest leaveRequest, string accessToken, List<SlashAttachment> attachment)
        {
            var teamLeaders = await _oauthCallRepository.GetTeamLeaderUserIdAsync(slackUserId, accessToken);
            var management = await _oauthCallRepository.GetManagementUserNameAsync(accessToken);
            var userDetail = await _oauthCallRepository.GetUserByUserIdAsync(slackUserId, accessToken);
            foreach (var user in management)
            {
                teamLeaders.Add(user);
            }
            foreach (var teamLeader in teamLeaders)
            {
                SlackUserDetails slackUser = await _slackUserRepository.GetByIdAsync(teamLeader.SlackUserId);
                var incomingWebHook = await _incomingWebHook.FirstOrDefaultAsync(x => x.UserId == slackUser.UserId);
                //Creating an object of SlashIncomingWebhook as this format of value required while responsing to slack
                var slashIncomingWebhookText = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
                var slashIncomingWebhookJsonText = JsonConvert.SerializeObject(slashIncomingWebhookText);
                if (incomingWebHook != null)
                    await _httpClientService.PostAsync(incomingWebHook.IncomingWebHookUrl, slashIncomingWebhookJsonText, _stringConstant.JsonContentString);
                EmailApplication email = new EmailApplication();
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
                email.From = userDetail.Email;
                email.Subject = _stringConstant.EmailSubject;
                email.To = teamLeader.Email;
                _emailService.Send(email);
            }
        }
        #endregion
    }
}
