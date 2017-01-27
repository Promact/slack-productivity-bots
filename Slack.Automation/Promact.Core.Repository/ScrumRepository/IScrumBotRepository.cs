using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumRepository
{
    public interface IScrumBotRepository
    {

        /// <summary>
        /// This will process the messages from slack and use appropriate methods to give a suitable response through Bot - JJ
        /// </summary>
        /// <param name="userId">UserId of slack user</param>
        /// <param name="channelId">Slack Channel Id</param>
        /// <param name="message">message from slack</param>
        /// <param name="scrumBotId">Id of the bot connected for conducting scrum</param>
        /// <returns>reply message</returns>
        Task<string> ProcessMessagesAsync(string userId, string channelId, string message, string scrumBotId);


        /// <summary>
        /// Store the scrum details temporarily in a database - JJ
        /// </summary>
        /// <param name="scrumId">Id of scrum of the channel for the day</param>
        /// <param name="slackUserId">UserId of slack user</param>
        /// <param name="answerCount">Number of answers of the user</param>
        /// <param name="questionId">The Id of the last question asked to the user</param>
        Task AddTemporaryScrumDetailsAsync(int scrumId, string slackUserId, int answerCount, int questionId);

    }
}
