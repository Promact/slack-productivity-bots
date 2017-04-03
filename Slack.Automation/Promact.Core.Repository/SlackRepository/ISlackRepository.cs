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
        /// <param name="leaveResponse">leave update response from slack</param>
        Task UpdateLeaveAsync(SlashChatUpdateResponse leaveResponse);

        /// <summary>
        /// Method to operate leave slack command
        /// </summary>
        /// <param name="leave">slash command object</param>
        Task LeaveRequestAsync(SlashCommand leave);

        /// <summary>
        /// Method to send error message to user od slack
        /// </summary>
        /// <param name="errorMessage">Message to send</param>
        /// <param name="responseUrl">Incoming webhook url</param>
        Task ErrorAsync(string responseUrl, string errorMessage);
    }
}
