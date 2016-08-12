using Promact.Erp.DomainModel.Models;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.Client
{
    public interface IClient
    {
        void UpdateMessage(SlashChatUpdateResponse leaveResponse, string replyText);
        void SendMessage(SlashCommand leave, string replyText);
        void SendMessageWithAttachmentIncomingWebhook(SlashCommand leave, LeaveRequest leaveRequest);
    }
}
