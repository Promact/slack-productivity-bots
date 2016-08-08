using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.SlackRepository
{
    public interface ISlackRepository
    {
        Task<LeaveRequest> LeaveApply(List<string> slackRequest,string userName);
        Task<string> LeaveList(string userName);
        Task<string> CancelLeave(int leaveId, string userName);
        Task<string> LeaveStatus(string userId);
        string ChatPostAttachment(string text, string leaveRequestId);
        List<SlashAttachment> SlackResponseAttachment(string leaveRequestId, string replyText);
        LeaveRequest UpdateLeave(int leaveId, string status);
    }
}
