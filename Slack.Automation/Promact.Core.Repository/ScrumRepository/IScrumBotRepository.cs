using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumRepository
{
    public interface IScrumBotRepository
    {
        /// <summary>
        /// This will process the messages from slack and use appropriate methods to give a suitable response through Bot
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ChannelId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<string> ProcessMessages(string UserId, string ChannelId, string message);
    }
}
