using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using System.Threading.Tasks;

namespace Promact.Core.Repository.Client
{
    public interface IClient
    {
        /// <summary>
        /// The below method use for updating slack message without attachment. 
        /// Required field token, channelId and message_ts which we had get at time of response from slack.
        /// </summary>
        /// <param name="leaveResponse">SlashChatUpdateResponse object send from slack</param>
        /// <param name="replyText">Text to be send to slack</param>
        Task UpdateMessage(SlashChatUpdateResponse leaveResponse, string replyText);

        /// <summary>
        /// The below method used for sending resposne back to slack for a slash command in ephemeral mood. Required field response_url.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        void SendMessage(SlashCommand leave, string replyText);

        /// <summary>
        /// The below method is used for sending meassage to all the TL and management people using Incoming 
        /// Webhook.Required field channel name(whom to send) and here I had override the bot name and its identity.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        /// <param name="leaveRequest">LeaveRequest object</param>
        Task SendMessageWithAttachmentIncomingWebhook(SlashCommand leave, LeaveRequest leaveRequest,string accessToken);

        /// <summary>
        /// Method to send message on slack using WebRequest 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url">Json string and url</param>
        void WebRequestMethod(string Json, string url);
    }
}
