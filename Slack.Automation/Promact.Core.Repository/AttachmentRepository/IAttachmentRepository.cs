using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.AttachmentRepository
{
    public interface IAttachmentRepository
    {
        List<SlashAttachment> SlackResponseAttachment(string leaveRequestId, string replyText);
        string ReplyText(string username, LeaveRequest leave);
        List<string> SlackText(string text);
    }
}
