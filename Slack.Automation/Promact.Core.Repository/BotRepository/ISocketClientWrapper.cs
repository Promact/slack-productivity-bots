using SlackAPI;

namespace Promact.Core.Repository.BotRepository
{
    public interface ISocketClientWrapper
    {
        /// <summary>
        /// Contain ScrumBot Client socket details
        /// </summary>
        SlackSocketClient ScrumBot { get; }

        /// <summary>
        /// Contain TaskMailBot Client socket details
        /// </summary>
        SlackSocketClient TaskBot { get; }

        /// <summary>
        /// Method to initialize scrum bot
        /// </summary>
        /// <param name="bottoken">scrum bot token</param>
        void InitializeScrumBot(string bottoken);

        /// <summary>
        /// Method to initialize task mail bot
        /// </summary>
        /// <param name="bottoken"></param>
        void InitializeTaskBot(string bottoken);
    }
}
