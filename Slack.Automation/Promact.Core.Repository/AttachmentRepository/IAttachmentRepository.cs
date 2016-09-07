using Microsoft.AspNet.Identity;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Promact.Core.Repository.AttachmentRepository
{
    public interface IAttachmentRepository
    {
        /// <summary>
        /// Method to create attchment of slack with a text as reply, can be used generically
        /// </summary>
        /// <param name="leaveRequestId"></param>
        /// <param name="replyText"></param>
        /// <returns>string attachment</returns>
        List<SlashAttachment> SlackResponseAttachment(string leaveRequestId, string replyText);

        /// <summary>
        /// Method will create text corresponding to leave details and user, which will to be send on slack as reply
        /// </summary>
        /// <param name="username"></param>
        /// <param name="leave"></param>
        /// <returns>string replyText</returns>
        string ReplyText(string username, LeaveRequest leave);

        /// <summary>
        /// Way to break string by spaces only if spaces are not between quotes
        /// </summary>
        /// <param name="text"></param>
        /// <returns>List of string slackText</returns>
        List<string> SlackText(string text);

        /// <summary>
        /// Method to transform NameValueCollection to SlashCommand class
        /// </summary>
        /// <param name="value"></param>
        /// <returns>leave of class SlashCommand</returns>
        SlashCommand SlashCommandTransfrom(NameValueCollection value);

        /// <summary>
        /// Method to get accessToken for Promact OAuth corresponding to username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>access token from AspNetUserLogin table</returns>
        Task<string> AccessToken(string username);
    }
}
