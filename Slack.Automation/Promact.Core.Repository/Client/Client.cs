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

namespace Promact.Core.Repository.Client
{
    public class Client : IClient
    {
        #region Private Variables
        private HttpClient _chatUpdateMessage;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IOauthCallsRepository _oauthCallRepository;
        private readonly IEmailService _email;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IHttpClientService _httpClientService;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IEmailServiceTemplateRepository _emailTemplateRepository;
        #endregion

        #region Constructor
        public Client(IOauthCallsRepository oauthCallRepository, IStringConstantRepository stringConstant,
            IEmailService email, IAttachmentRepository attachmentRepository, IHttpClientService httpClientService,
            IEnvironmentVariableRepository envVariableRepository, ISlackUserRepository slackUserRepository,
            IEmailServiceTemplateRepository emailTemplateRepository)
        {
            _stringConstant = stringConstant;
            _chatUpdateMessage = new HttpClient();
            _chatUpdateMessage.BaseAddress = new Uri(_stringConstant.SlackChatUpdateUrl);
            _oauthCallRepository = oauthCallRepository;
            _email = email;
            _attachmentRepository = attachmentRepository;
            _httpClientService = httpClientService;
            _envVariableRepository = envVariableRepository;
            _slackUserRepository = slackUserRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The below method use for updating slack message without attachment. 
        /// Required field token, channelId and message_ts which we had get at time of response from slack.
        /// </summary>
        /// <param name="leaveResponse">SlashChatUpdateResponse object send from slack</param>
        /// <param name="replyText">Text to be send to slack</param>
        public async Task UpdateMessageAsync(SlashChatUpdateResponse leaveResponse, string replyText)
        {
            // Call to an url using HttpClient.
            var responseUrl = string.Format("?token={0}&channel={1}&text={2}&ts={3}&as_user=true&pretty=1", HttpUtility.UrlEncode(leaveResponse.Token), HttpUtility.UrlEncode(leaveResponse.Channel.Id), HttpUtility.UrlEncode(replyText), HttpUtility.UrlEncode(leaveResponse.MessageTs));
            var response = await _httpClientService.GetAsync(_stringConstant.SlackChatUpdateUrl, responseUrl, leaveResponse.Token);
        }

        /// <summary>
        /// The below method used for sending resposne back to slack for a slash command in ephemeral mood. Required field response_url.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        public async Task SendMessageAsync(SlashCommand slackLeave, string replyText)
        {
            var text = new SlashResponse() { ResponseType = _stringConstant.ResponseTypeEphemeral, Text = replyText };
            var textJson = JsonConvert.SerializeObject(text);
            await _httpClientService.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString);
        }

        /// <summary>
        /// The below method is used for sending meassage to all the TL and management people using Incoming 
        /// Webhook.Required field channel name(whom to send) and here I had override the bot name and its identity.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <param name="slackUserId"></param>
        public async Task SendMessageWithAttachmentIncomingWebhookAsync(LeaveRequest leaveRequest, string accessToken, string replyText, string username, string slackUserId)
        {
            // getting attachment as a string to be send on slack
            var attachment = _attachmentRepository.SlackResponseAttachment(Convert.ToString(leaveRequest.Id), replyText);
            await GetAttachmentAndSendToTLAndManagementAsync(slackUserId, username, leaveRequest, accessToken, attachment);
        }

        /// <summary>
        /// Method used to send slack message and email to team leader and management without interactive button
        /// </summary>
        /// <param name="leave"></param>
        /// <param name="leaveRequest"></param>
        /// <param name="accessToken"></param>
        /// <param name="replyText"></param>
        /// <param name="slackUserId"></param>
        /// <returns></returns>
        public async Task SendMessageWithoutButtonAttachmentIncomingWebhookAsync(LeaveRequest leaveRequest, string accessToken, string replyText, string username, string slackUserId)
        {
            // getting attachment as a string to be send on slack
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(leaveRequest.Id), replyText);
            await GetAttachmentAndSendToTLAndManagementAsync(slackUserId, username, leaveRequest, accessToken, attachment);
        }

        /// <summary>
        /// Method to send slack message to user whom leave has been applied by admin
        /// </summary>
        /// <param name="leaveRequest"></param>
        /// <param name="managementEmail"></param>
        /// <param name="replyText"></param>
        /// <param name="user"></param>
        public async Task SendSickLeaveMessageToUserIncomingWebhookAsync(LeaveRequest leaveRequest, string managementEmail, string replyText, User user)
        {
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(leaveRequest.Id), replyText);
            SlackUserDetails slackUser = _slackUserRepository.GetById(user.SlackUserId);
            var text = new SlashIncomingWebhook() { Channel = "@" + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
            var textJson = JsonConvert.SerializeObject(text);
            await _httpClientService.PostAsync(_envVariableRepository.IncomingWebHookUrl, textJson, _stringConstant.JsonContentString);
            //WebRequestMethod(textJson, _envVariableRepository.IncomingWebHookUrl);
            EmailApplication email = new EmailApplication();
            // creating email templates corresponding to leave applied
            email.Body = _emailTemplateRepository.EmailServiceTemplateSickLeave(leaveRequest);
            email.From = managementEmail;
            email.Subject = _stringConstant.EmailSubject;
            email.To = user.Email;
            _email.Send(email);
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Private method to get reply text and send to team leader and management
        /// </summary>
        /// <param name="username"></param>
        /// <param name="leaveRequest"></param>
        /// <param name="accessToken"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private async Task GetAttachmentAndSendToTLAndManagementAsync(string slackUserId, string username, LeaveRequest leaveRequest, string accessToken, List<SlashAttachment> attachment)
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
                SlackUserDetails slackUser = _slackUserRepository.GetById(teamLeader.SlackUserId);
                //Creating an object of SlashIncomingWebhook as this format of value required while responsing to slack
                var text = new SlashIncomingWebhook() { Channel = "@" + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
                var textJson = JsonConvert.SerializeObject(text);
                await _httpClientService.PostAsync(_envVariableRepository.IncomingWebHookUrl, textJson, _stringConstant.JsonContentString);
                //WebRequestMethod(textJson, _envVariableRepository.IncomingWebHookUrl);
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
                _email.Send(email);
            }
        }
        #endregion
    }
}
