using Newtonsoft.Json;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.Email;
using Promact.Erp.Util.Email_Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Promact.Core.Repository.Client
{
    public class Client : IClient
    {
        private HttpClient _chatUpdateMessage;
        private readonly IProjectUserCallRepository _projectUser;
        private readonly IEmailService _email;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IHttpClientRepository _httpClientRepository;
        public Client(IProjectUserCallRepository projectUser, IEmailService email, IAttachmentRepository attachmentRepository,IHttpClientRepository httpClientRepository)
        {
            _chatUpdateMessage = new HttpClient();
            _chatUpdateMessage.BaseAddress = new Uri(StringConstant.SlackChatUpdateUrl);
            _projectUser = projectUser;
            _email = email;
            _attachmentRepository = attachmentRepository;
            _httpClientRepository = httpClientRepository;
        }

        /// <summary>
        /// The below method use for updating slack message without attachment. 
        /// Required field token, channelId and message_ts which we had get at time of response from slack.
        /// </summary>
        /// <param name="leaveResponse">SlashChatUpdateResponse object send from slack</param>
        /// <param name="replyText">Text to be send to slack</param>
        public async Task UpdateMessage(SlashChatUpdateResponse leaveResponse, string replyText)
        {
            // Call to an url using HttpClient.
            var responseUrl = string.Format("?token={0}&channel={1}&text={2}&ts={3}&as_user=true&pretty=1", HttpUtility.UrlEncode(leaveResponse.Token), HttpUtility.UrlEncode(leaveResponse.Channel.Id), HttpUtility.UrlEncode(replyText), HttpUtility.UrlEncode(leaveResponse.MessageTs));
            var response = await _httpClientRepository.GetAsync(StringConstant.SlackChatUpdateUrl, responseUrl,leaveResponse.Token);
        }

        /// <summary>
        /// The below method used for sending resposne back to slack for a slash command in ephemeral mood. Required field response_url.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        public void SendMessage(SlashCommand leave, string replyText)
        {
            var text = new SlashResponse() { ResponseType = StringConstant.ResponseTypeEphemeral, Text = replyText };
            var textJson = JsonConvert.SerializeObject(text);
            WebRequestMethod(textJson, leave.ResponseUrl);
        }

        /// <summary>
        /// The below method is used for sending meassage to all the TL and management people using Incoming 
        /// Webhook.Required field channel name(whom to send) and here I had override the bot name and its identity.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        /// <param name="leaveRequest">LeaveRequest object</param>
        public async Task SendMessageWithAttachmentIncomingWebhook(LeaveRequest leaveRequest,string accessToken,string replyText, string username)
        {
            // getting attachment as a string to be send on slack
            var attachment = _attachmentRepository.SlackResponseAttachment(Convert.ToString(leaveRequest.Id), replyText);
            await GetAttachmentAndSendToTLAndManagement(username, leaveRequest, accessToken, attachment);
        }

        /// <summary>
        /// Method used to send slack message and email to team leader and management without interactive button
        /// </summary>
        /// <param name="leave"></param>
        /// <param name="leaveRequest"></param>
        /// <param name="accessToken"></param>
        /// <param name="replyText"></param>
        /// <returns></returns>
        public async Task SendMessageWithoutButtonAttachmentIncomingWebhook(LeaveRequest leaveRequest, string accessToken,string replyText,string username)
        {
            // getting attachment as a string to be send on slack
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(leaveRequest.Id), replyText);
            await GetAttachmentAndSendToTLAndManagement(username, leaveRequest, accessToken, attachment);
        }

        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest template object</param>
        /// <returns>template emailBody as string</returns>
        private string EmailServiceTemplate(LeaveRequest leaveRequest)
        {
            try
            {
                LeaveApplication leaveTemplate = new LeaveApplication();
                // Assigning Value in template page
                leaveTemplate.Session = new Dictionary<string, object>
            {
                {StringConstant.FromDate,leaveRequest.FromDate.ToString(StringConstant.DateFormat) },
                {StringConstant.EndDate,leaveRequest.EndDate.Value.ToString(StringConstant.DateFormat) },
                {StringConstant.Reason,leaveRequest.Reason },
                {StringConstant.Type,leaveRequest.Type.ToString() },
                {StringConstant.Status,leaveRequest.Status.ToString() },
                {StringConstant.ReJoinDate,leaveRequest.RejoinDate.Value.ToString(StringConstant.DateFormat) },
                {StringConstant.CreatedOn,leaveRequest.CreatedOn.ToString(StringConstant.DateFormat) },
            };
                leaveTemplate.Initialize();
                var emailBody = leaveTemplate.TransformText();
                return emailBody;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to send message on slack using WebRequest 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url">Json string and url</param>
        public void WebRequestMethod(string Json, string url)
        {
            try
            {
                // Call to an url using HttpWebRequest
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = StringConstant.WebRequestContentType;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    //adding rest portion of url
                    streamWriter.Write(Json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Private method to get reply text and send to team leader and management
        /// </summary>
        /// <param name="username"></param>
        /// <param name="leaveRequest"></param>
        /// <param name="accessToken"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private async Task GetAttachmentAndSendToTLAndManagement(string username, LeaveRequest leaveRequest, string accessToken, List<SlashAttachment> attachment)
        {
            var teamLeaders = await _projectUser.GetTeamLeaderUserName(username, accessToken);
            var management = await _projectUser.GetManagementUserName(accessToken);
            var userDetail = await _projectUser.GetUserByUsername(username, accessToken);
            foreach (var user in management)
            {
                teamLeaders.Add(user);
            }
            foreach (var teamLeader in teamLeaders)
            {
                //Creating an object of SlashIncomingWebhook as this format of value required while responsing to slack
                var text = new SlashIncomingWebhook() { Channel = "@" + teamLeader.SlackUserName, Username = StringConstant.LeaveBot, Attachments = attachment };
                var textJson = JsonConvert.SerializeObject(text);
                WebRequestMethod(textJson, Environment.GetEnvironmentVariable(StringConstant.IncomingWebHookUrl, EnvironmentVariableTarget.Process));
                EmailApplication email = new EmailApplication();
                // creating email templates corresponding to leave applied
                email.Body = EmailServiceTemplate(leaveRequest);
                email.From = userDetail.Email;
                email.Subject = StringConstant.EmailSubject;
                email.To = teamLeader.Email;
                _email.Send(email);
            }
        }

        /// <summary>
        /// Method to send slack message to user whom leave has been applied by admin
        /// </summary>
        /// <param name="leaveRequest"></param>
        /// <param name="managementEmail"></param>
        /// <param name="replyText"></param>
        /// <param name="user"></param>
        public void SendSickLeaveMessageToUserIncomingWebhook(LeaveRequest leaveRequest, string managementEmail, string replyText, User user)
        {
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(leaveRequest.Id), replyText);
            var text = new SlashIncomingWebhook() { Channel = "@" + user.SlackUserName, Username = StringConstant.LeaveBot, Attachments = attachment };
            var textJson = JsonConvert.SerializeObject(text);
            WebRequestMethod(textJson, Environment.GetEnvironmentVariable(StringConstant.IncomingWebHookUrl, EnvironmentVariableTarget.Process));
            EmailApplication email = new EmailApplication();
            // creating email templates corresponding to leave applied
            email.Body = EmailServiceTemplate(leaveRequest);
            email.From = managementEmail;
            email.Subject = StringConstant.EmailSubject;
            email.To = user.Email;
            _email.Send(email);
        }
    }
}
