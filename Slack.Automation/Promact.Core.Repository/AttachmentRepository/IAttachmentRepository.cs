using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System.Collections.Generic;

namespace Promact.Core.Repository.AttachmentRepository
{
    public interface IAttachmentRepository
    {
        List<SlashAttachment> SlackResponseAttachment(string leaveRequestId, string replyText);
        string ReplyText(string username, LeaveRequest leave);
        List<string> SlackText(string text);
    }
}
