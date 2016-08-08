using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.Client
{
    public interface IClient
    {
        void UpdateMessage(SlashChatUpdateResponse leaveResponse, string replyText);
        void SendMessage(SlashCommand leave, string replyText);
        void SendMessageWithAttachmentIncomingWebhook(SlashCommand leave, string replyText, LeaveRequest leaveRequest);
        void WebRequestMethod(string Json, string url);
    }
}
