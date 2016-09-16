using Promact.Erp.DomainModel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.SlackRepository
{
    public interface ISlackRepository
    {
        /// <summary>
        /// Method to get leave Updated from slack button response
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="status"></param>
        void UpdateLeave(SlashChatUpdateResponse leaveResponse);

        /// <summary>
        /// Method to excute for leave slash command
        /// </summary>
        /// <param name="leave"></param>
        /// <returns>Task</returns>
        Task LeaveRequest(SlashCommand leave);

        /// <summary>
        /// Method to send error message
        /// </summary>
        /// <param name="leave"></param>
        void Error(SlashCommand leave);
    }
}
