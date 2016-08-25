using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System.Threading.Tasks;

namespace Promact.Core.Repository.Client
{
    public interface IClient
    {
        Task UpdateMessage(SlashChatUpdateResponse leaveResponse, string replyText);
        void SendMessage(SlashCommand leave, string replyText);
        Task SendMessageWithAttachmentIncomingWebhook(SlashCommand leave, LeaveRequest leaveRequest,string accessToken);
         void WebRequestMethod(string Json, string url);
    }
}
