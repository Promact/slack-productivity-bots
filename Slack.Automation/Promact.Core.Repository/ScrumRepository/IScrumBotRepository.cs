﻿using System.Threading.Tasks;

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
        /// Store the scrum details temporarily in a database
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="slackUserId"></param>
        /// <param name="answerCount"></param>
        /// <param name="questionId"></param>
        Task AddTemporaryScrumDetailsAsync(int projectId, string slackUserId, int answerCount, int questionId);

    }
}
