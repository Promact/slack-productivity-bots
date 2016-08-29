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
        List<SlashAttachment> SlackResponseAttachment(string leaveRequestId, string replyText);
        string ReplyText(string username, LeaveRequest leave);
        List<string> SlackText(string text);
        SlashCommand SlashCommandTransfrom(NameValueCollection value);
        Task<string> AccessToken(string username);
    }
}
