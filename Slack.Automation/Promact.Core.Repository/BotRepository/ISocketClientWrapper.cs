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
        void InitializeAndConnectScrumBot(string bottoken);

        /// <summary>
        /// Method to initialize task mail bot
        /// </summary>
        /// <param name="bottoken"></param>
        void InitializeAndConnectTaskBot(string bottoken);

        /// <summary>
        /// Method to turn off bot by module name
        /// </summary>
        /// <param name="module">name of module</param>
        void StopBotByModule(string module);
    }
}
