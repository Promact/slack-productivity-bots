using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;

namespace Promact.Core.Repository.Client
{
    public interface IClient
    {
        void UpdateMessage(SlashChatUpdateResponse leaveResponse, string replyText);
        void SendMessage(SlashCommand leave, string replyText);
        void SendMessageWithAttachmentIncomingWebhook(SlashCommand leave, LeaveRequest leaveRequest);
    }
}
