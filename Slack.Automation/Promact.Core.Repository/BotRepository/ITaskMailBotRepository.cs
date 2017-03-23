using SlackAPI.WebSocketMessages;
using System.Threading.Tasks;

namespace Promact.Core.Repository.BotRepository
{
    public interface ITaskMailBotRepository
    {
        /// <summary>
        /// Method to turn on task mail bot
        /// </summary>
        /// <param name="botToken">message for slack</param>
        Task<string> ConductTask(NewMessage message);
    }
}
