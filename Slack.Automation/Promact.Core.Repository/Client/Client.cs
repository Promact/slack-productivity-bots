using Newtonsoft.Json;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
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
        public Client(ISlackRepository slackRepository,IProjectUserCallRepository projectUser)
        {
            _chatUpdateMessage = new HttpClient();
            _chatUpdateMessage.BaseAddress = new Uri("https://slack.com/api/chat.update");
            _slackRepository = slackRepository;
            _projectUser = projectUser;
        }

        /// <summary>
        /// The below method use for updating slack message without attachment. Required field token, channelId and message_ts which we had get at time of response from slack.
        /// </summary>
        /// <param name="leaveResponse"></param>
        /// <param name="replyText"></param>
        public void UpdateMessage(SlashChatUpdateResponse leaveResponse, string replyText)
        {
            var response = _chatUpdateMessage.GetAsync("?token=" + HttpUtility.UrlEncode(leaveResponse.token) + "&channel=" + HttpUtility.UrlEncode(leaveResponse.channel.Id) + "&text=" + HttpUtility.UrlEncode(replyText) + "&ts=" + HttpUtility.UrlEncode(leaveResponse.message_ts) + "&as_user=true&pretty=1").Result;
        }

        /// <summary>
        /// The below method used for sending resposne back to slack for a slash command in ephemeral mood. Required field response_url.
        /// </summary>
        /// <param name="leave"></param>
        /// <param name="replyText"></param>
        public void SendMessage(SlashCommand leave, string replyText)
        {
            var example = new SlashResponse() { response_type = "ephemeral", text = replyText };
            var Json = JsonConvert.SerializeObject(example);
            var request = (HttpWebRequest)WebRequest.Create(leave.response_url);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json; charset=UTF-8";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(Json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// The below method is used for sending meassage to all the TL and management people using Incoming Webhook.Required field channel name(whom to send) and here i had override the bot name and its identity.
        /// </summary>
        /// <param name="leave"></param>
        /// <param name="replyText"></param>
        /// <param name="leaveRequestId"></param>
        public void SendMessageWithAttachmentIncomingWebhook(SlashCommand leave, string replyText, string leaveRequestId)
        {
            var attachment = _slackRepository.SlackResponseAttachment(leaveRequestId, replyText);
            var teamLeaders = _projectUser.GetTeamLeaderUserName(leave.user_name);
            var management = _projectUser.GetManagementUserName();
            foreach (var user in management)
            {
                teamLeaders.Add(user);
            }
            foreach (var teamLeader in teamLeaders)
            {
                var example = new SlashIncomingWebhook() { channel = "@"+teamLeader, username = "LeaveBot", attachments = attachment };
                var Json = JsonConvert.SerializeObject(example);
                var request = (HttpWebRequest)WebRequest.Create(leave.response_url);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/json; charset=UTF-8";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(Json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
            }
        }
    }
}
