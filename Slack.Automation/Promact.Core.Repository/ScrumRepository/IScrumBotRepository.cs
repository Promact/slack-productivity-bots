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
    }
}
