using SlackAPI.WebSocketMessages;
using System.Threading.Tasks;

namespace Promact.Core.Repository.TaskMailRepository
{
    public interface ITaskMailRepository
    {
        /// <summary>
        /// Method to turn on task mail bot
        /// </summary>
        /// <param name="botToken">message for slack</param>
        Task<string> ProcessTask(NewMessage message);
    }
}
