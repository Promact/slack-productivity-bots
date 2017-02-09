using Promact.Erp.DomainModel.ApplicationClass;
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
        Task UpdateMessageAsync(string responseUrl, string replyText);

        /// <summary>
        /// The below method used for sending resposne back to slack for a slash command in ephemeral mood. Required field response_url.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        Task SendMessageAsync(string responseUrl, string replyText);

        /// <summary>
        /// The below method is used for sending meassage to all the TL and management people using Incoming 
        /// Webhook.Required field channel name(whom to send) and here I had override the bot name and its identity.
        /// </summary>
        /// <param name="leave">Slash Command object</param>
        /// <param name="replyText">Text to be send to slack</param>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <param name="userId"></param>
        Task SendMessageWithAttachmentIncomingWebhookAsync(LeaveRequest leaveRequest,string accessToken, string replyText, string userId);

        /// <summary>
        /// Method used to send slack message and email to team leader and management without interactive button
        /// </summary>
        /// <param name="leaveRequest">leave details</param>
        /// <param name="accessToken">User's access token</param>
        /// <param name="replyText">To be replied text</param>
        /// <param name="userId">userId of user</param>
        /// <returns></returns>
        Task SendMessageWithoutButtonAttachmentIncomingWebhookAsync(LeaveRequest leaveRequest, string accessToken, string replyText, string userId);

        /// <summary>
        /// Method to send slack message to user whom leave has been applied by admin
        /// </summary>
        /// <param name="leaveRequest">leave details</param>
        /// <param name="managementEmail">management people email address</param>
        /// <param name="replyText">To be replied text</param>
        /// <param name="user">user details</param>
        Task SendSickLeaveMessageToUserIncomingWebhookAsync(LeaveRequest leaveRequest, string managementEmail, string replyText, User user);
    }
}
