using Promact.Erp.DomainModel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.SlackRepository
{
    public interface ISlackRepository
    {
        Task<LeaveRequest> LeaveApply(List<string> slackRequest, SlashCommand leave, string accessToken);
        void UpdateLeave(SlashChatUpdateResponse leaveResponse);
        Task SlackLeaveList(List<string> slackText, SlashCommand leave, string accessToken);
        Task SlackLeaveCancel(List<string> slackText, SlashCommand leave, string accessToken);
        Task SlackLeaveStatus(List<string> slackText, SlashCommand leave, string accessToken);
        void SlackLeaveBalance(SlashCommand leave);
        void SlackLeaveHelp(SlashCommand leave);
    }
}
