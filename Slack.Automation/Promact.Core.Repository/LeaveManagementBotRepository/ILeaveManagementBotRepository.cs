using System.Threading.Tasks;

namespace Promact.Core.Repository.LeaveManagementBotRepository
{
    public interface ILeaveManagementBotRepository
    {
        /// <summary>
        /// Method to process leave request - SS
        /// </summary>
        /// <param name="slackUserId">user's slack Id</param>
        /// <param name="answer">text send from user</param>
        /// <returns>reply to be send</returns>
        Task<string> ProcessLeaveAsync(string slackUserId, string answer);
    }
}
