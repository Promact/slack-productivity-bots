using Promact.Erp.DomainModel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.SlackRepository
{
    public interface ISlackRepository
    {
        /// <summary>
        /// Method to convert List of string to leaveRequest object and call leave apply method to save the leave details
        /// </summary>
        /// <param name="slackRequest"></param>
        /// <param name="userName"></param>
        /// <returns>leaveRequest</returns>
        Task<LeaveRequest> LeaveApply(List<string> slackRequest, SlashCommand leave, string accessToken);

        /// <summary>
        /// Method to get leave Updated from slack button response
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="status"></param>
        void UpdateLeave(SlashChatUpdateResponse leaveResponse);

        /// <summary>
        /// Method to Get Leave List on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        Task SlackLeaveList(List<string> slackText, SlashCommand leave, string accessToken);

        /// <summary>
        /// Method to cancel leave by its Id from slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        Task SlackLeaveCancel(List<string> slackText, SlashCommand leave, string accessToken);

        /// <summary>
        /// Method to get last leave status and details on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        Task SlackLeaveStatus(List<string> slackText, SlashCommand leave, string accessToken);

        /// <summary>
        /// Method to check leave Balance from slack
        /// </summary>
        /// <param name="leave"></param>
        void SlackLeaveBalance(SlashCommand leave);

        /// <summary>
        /// Method for gettin help on slack regards Leave slash command
        /// </summary>
        /// <param name="leave"></param>
        void SlackLeaveHelp(SlashCommand leave);
    }
}
