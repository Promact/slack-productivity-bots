using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumRepository
{
    public interface IScrumBotRepository
    {
        /// <summary>
        /// This will process the messages from slack and use appropriate methods to give a suitable response through Bot
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="channelId"></param>
        /// <param name="message"></param>
        /// <returns>reply message</returns>
        Task<string> ProcessMessagesAsync(string userId, string channelId, string message);
                

        /// <summary>
        /// Used to add temporary scrum details
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="slackUserId"></param>
        void AddTemporaryScrumDetails(int projectId,string slackUserId);


        /// <summary>
        /// Used to remove temporary scrum details of the given project
        /// </summary>
        /// <param name="projectId"></param>
        void RemoveTemporaryScrumDetailsAll(int projectId);
    }
}
