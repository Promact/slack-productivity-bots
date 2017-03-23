using SlackAPI;

namespace Promact.Core.Repository.BotRepository
{
    public class SocketClientWrapper : ISocketClientWrapper
    {
        #region Public property
        /// <summary>
        /// Contain ScrumBot Client socket details
        /// </summary>
        public SlackSocketClient ScrumBot { get; private set; }

        /// <summary>
        /// Contain TaskMailBot Client socket details
        /// </summary>
        public SlackSocketClient TaskBot { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SocketClientWrapper()
        {
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Method to initialize scrum bot
        /// </summary>
        /// <param name="bottoken">scrum bot token</param>
        public void InitializeScrumBot(string bottoken)
        {
            ScrumBot = new SlackSocketClient(bottoken);
        }

        /// <summary>
        /// Method to initialize task mail bot
        /// </summary>
        /// <param name="bottoken"></param>
        public void InitializeTaskBot(string bottoken)
        {
            TaskBot = new SlackSocketClient(bottoken);
        }
        #endregion
    }
}
