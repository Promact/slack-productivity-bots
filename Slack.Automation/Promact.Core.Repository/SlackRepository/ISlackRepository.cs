using Promact.Erp.DomainModel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.SlackRepository
{
    public interface ISlackRepository
    {
        Task<LeaveRequest> LeaveApply(List<string> slackRequest, SlashCommand leave);
        void UpdateLeave(SlashChatUpdateResponse leaveResponse);
        void SlackLeaveList(List<string> slackText, SlashCommand leave);
        void SlackLeaveCancel(List<string> slackText, SlashCommand leave);
        void SlackLeaveStatus(List<string> slackText, SlashCommand leave);
        void SlackLeaveBalance(SlashCommand leave);
        void SlackLeaveHelp(SlashCommand leave);
    }
}
