using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumSetUpRepository
{
    public interface IScrumSetUpRepository
    {
        /// <summary>
        /// It is called when the message is "list links","link "project"" or "unlink "project"" - JJ
        /// </summary>
        /// <param name="slackUserId">UserId of slack user</param>
        /// <param name="slackChannel">slack channel from which message is send</param>
        /// <param name="message">message from slack</param>
        /// <returns>appropriate message</returns>
        Task<string> ProcessSetUpMessagesAsync(string slackUserId, SlackChannelDetails slackChannel, string message);


        /// <summary>
        /// Used to add channel manually by command "add channel channelname". - JJ
        /// </summary>
        /// <param name="slackChannelName">slack channel name from which message is send</param>
        /// <param name="slackChannelId">slack channel id from which message is send</param>
        /// <param name="slackUserId">slack user id of interacting user</param>
        /// <returns>status message</returns>
        Task<string> AddChannelManuallyAsync(string slackChannelName, string slackChannelId, string slackUserId);

    }
}
