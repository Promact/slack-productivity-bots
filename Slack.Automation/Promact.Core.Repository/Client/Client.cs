using Newtonsoft.Json;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
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
    public class Client:IClient
    {
        private HttpClient _chatUpdateMessage;
        private readonly ISlackRepository _slackRepository;
        private readonly IProjectUserCallRepository _projectUser;
        private readonly IEmailService _email;
        public Client(ISlackRepository slackRepository,IProjectUserCallRepository projectUser,IEmailService email)
        {
            _chatUpdateMessage = new HttpClient();
            _chatUpdateMessage.BaseAddress = new Uri("https://slack.com/api/chat.update");
            _slackRepository = slackRepository;
            _projectUser = projectUser;
            _email = email;
        }

        /// <summary>
        /// The below method use for updating slack message without attachment. Required field token, channelId and message_ts which we had get at time of response from slack.
        /// </summary>
        /// <param name="leaveResponse">SlashChatUpdateResponse object send from slack</param>
        /// <param name="replyText">Text to be send to slack</param>
        public void UpdateMessage(SlashChatUpdateResponse leaveResponse, string replyText)
        {
            // Call to an url using HttpClient.
            var response = _chatUpdateMessage.GetAsync("?token=" + HttpUtility.UrlEncode(leaveResponse.token) + "&channel=" + HttpUtility.UrlEncode(leaveResponse.channel.Id) + "&text=" + HttpUtility.UrlEncode(replyText) + "&ts=" + HttpUtility.UrlEncode(leaveResponse.message_ts) + "&as_user=true&pretty=1").Result;
        }

        /// <summary>
        /// The below method used for sending resposne back to slack for a slash command in ephemeral mood. Required field response_url.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        public void SendMessage(SlashCommand leave, string replyText)
        {
            var text = new SlashResponse() { response_type = "ephemeral", text = replyText };
            var Json = JsonConvert.SerializeObject(text);
            WebRequestMethod(Json, leave.response_url);
        }

        /// <summary>
        /// The below method is used for sending meassage to all the TL and management people using Incoming 
        /// Webhook.Required field channel name(whom to send) and here I had override the bot name and its identity.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        /// <param name="leaveRequest">LeaveRequest object</param>
        public void SendMessageWithAttachmentIncomingWebhook(SlashCommand leave, string replyText, LeaveRequest leaveRequest)
        {
            var attachment = _slackRepository.SlackResponseAttachment(Convert.ToString(leaveRequest.Id), replyText);
            var teamLeaders = _projectUser.GetTeamLeaderUserName(leave.user_name);
            var management = _projectUser.GetManagementUserName();
            foreach (var user in management)
            {
                teamLeaders.Add(user);
            }
            foreach (var teamLeader in teamLeaders)
            {
                //Creating an object of SlashIncomingWebhook as this format of value required while responsing to slack
                var text = new SlashIncomingWebhook() { channel = "@"+teamLeader, username = "LeaveBot", attachments = attachment };
                var Json = JsonConvert.SerializeObject(text);
                WebRequestMethod(Json, leave.response_url);
            }
            EmailApplication email = new EmailApplication();
            email.Body = EmailServiceTemplate(leaveRequest);
            email.From = leave.user_name + "@promactinfo.com";
            email.Subject = "Leave Application";
            email.To = teamLeaders;
            _email.Send(email);
        }

        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest Class object</param>
        /// <returns></returns>
        public string EmailServiceTemplate(LeaveRequest leaveRequest)
        {
            LeaveApplication leaveTemplate = new LeaveApplication();
            // Assigning Value in template page
            leaveTemplate.Session = new Dictionary<string, object>
            {
                {"FromDate",leaveRequest.FromDate.ToString("dd,MM,yyyy") },
                {"EndDate",leaveRequest.EndDate.ToString("dd,MM,yyyy") },
                {"Reason",leaveRequest.Reason },
                {"Type",leaveRequest.Type },
                {"Status",leaveRequest.Status.ToString() },
                {"ReJoinDate",leaveRequest.RejoinDate.ToString("dd,MM,yyyy") },
                {"CreatedOn",leaveRequest.CreatedOn.ToString("dd,MM,yyyy") },
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
        public void WebRequestMethod(string Json, string url)
        {
            // Call to an url using HttpWebRequest
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json; charset=UTF-8";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                //adding rest portion of url
                streamWriter.Write(Json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
        }
    }
}
