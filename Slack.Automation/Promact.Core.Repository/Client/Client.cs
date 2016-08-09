using Newtonsoft.Json;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.Email;
using Promact.Erp.Util.Email_Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
        public Client(IProjectUserCallRepository projectUser, IEmailService email, IAttachmentRepository attachmentRepository)
        {
            _chatUpdateMessage = new HttpClient();
            _chatUpdateMessage.BaseAddress = new Uri(AppSettingsUtil.ChatUpdateUrl);
            _projectUser = projectUser;
            _email = email;
            _attachmentRepository = attachmentRepository;
        }

        /// <summary>
        /// The below method use for updating slack message without attachment. 
        /// Required field token, channelId and message_ts which we had get at time of response from slack.
        /// </summary>
        /// <param name="leaveResponse">SlashChatUpdateResponse object send from slack</param>
        /// <param name="replyText">Text to be send to slack</param>
        public async void UpdateMessage(SlashChatUpdateResponse leaveResponse, string replyText)
        {
            // Call to an url using HttpClient.
            var responseUrl = string.Format("?token={0}&channel={1}&text={2}&ts={3}&as_user=true&pretty=1", HttpUtility.UrlEncode(leaveResponse.Token), HttpUtility.UrlEncode(leaveResponse.Channel.Id), HttpUtility.UrlEncode(replyText), HttpUtility.UrlEncode(leaveResponse.MessageTs));
            var response = await _chatUpdateMessage.GetAsync(responseUrl);
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
        public async void SendMessageWithAttachmentIncomingWebhook(SlashCommand leave, LeaveRequest leaveRequest)
        {
            // getting reply text to be send on slack corresponding to leave applied
            var replyText = _attachmentRepository.ReplyText(leave.Username, leaveRequest);
            // getting attachment as a string to be send on slack
            var attachment = _attachmentRepository.SlackResponseAttachment(Convert.ToString(leaveRequest.Id), replyText);
            var teamLeaders = await _projectUser.GetTeamLeaderUserName(leave.Username);
            var management = await _projectUser.GetManagementUserName();
            foreach (var user in management)
            {
                teamLeaders.Add(user);
            }
            foreach (var teamLeader in teamLeaders)
            {
                //Creating an object of SlashIncomingWebhook as this format of value required while responsing to slack
                var text = new SlashIncomingWebhook() { Channel = "@" + teamLeader, Username = "LeaveBot", Attachments = attachment };
                var textJson = JsonConvert.SerializeObject(text);
                WebRequestMethod(textJson, AppSettingsUtil.IncomingWebHookUrl);
            }
            var userDetail = await _projectUser.GetUserByUsername(leave.Username); 
            foreach (var teamLeader in teamLeaders)
            {
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
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest Class object</param>
        /// <returns>template emailBody as string</returns>
        private string EmailServiceTemplate(LeaveRequest leaveRequest)
        {
            LeaveApplication leaveTemplate = new LeaveApplication();
            // Assigning Value in template page
            leaveTemplate.Session = new Dictionary<string, object>
            {
                {StringConstant.FromDate,leaveRequest.FromDate.ToString(StringConstant.DateFormat) },
                {StringConstant.EndDate,leaveRequest.EndDate.ToString(StringConstant.DateFormat) },
                {StringConstant.Reason,leaveRequest.Reason },
                {StringConstant.Type,leaveRequest.Type },
                {StringConstant.Status,leaveRequest.Status.ToString() },
                {StringConstant.ReJoinDate,leaveRequest.RejoinDate.ToString(StringConstant.DateFormat) },
                {StringConstant.CreatedOn,leaveRequest.CreatedOn.ToString(StringConstant.DateFormat) },
            };
            leaveTemplate.Initialize();
            var emailBody = leaveTemplate.TransformText();
            return emailBody;
        }

        /// <summary>
        /// Method to send message on slack using WebRequest 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url">Json string and url</param>
        private void WebRequestMethod(string Json, string url)
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
    }
}
