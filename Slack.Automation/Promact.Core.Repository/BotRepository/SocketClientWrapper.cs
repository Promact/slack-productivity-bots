using Promact.Erp.Util.StringConstants;
using SlackAPI;

namespace Promact.Core.Repository.BotRepository
{
    public class SocketClientWrapper : ISocketClientWrapper
    {
        #region Private Variable
        private readonly IStringConstantRepository _stringConstant;
        #endregion

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
        public SocketClientWrapper(IStringConstantRepository stringConstant)
        {
            _stringConstant = stringConstant;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Method to initialize scrum bot
        /// </summary>
        /// <param name="bottoken">scrum bot token</param>
        public void InitializeAndConnectScrumBot(string bottoken)
        {
            ScrumBot = new SlackSocketClient(bottoken);
            ScrumBot.Connect((connect) => { });
        }

        /// <summary>
        /// Method to initialize task mail bot
        /// </summary>
        /// <param name="bottoken"></param>
        public void InitializeAndConnectTaskBot(string bottoken)
        {
            TaskBot = new SlackSocketClient(bottoken);
            TaskBot.Connect((connect) => { });
        }

        /// <summary>
        /// Method to turn off bot by module name
        /// </summary>
        /// <param name="module">name of module</param>
        public void StopBotByModule(string module)
        {
            if (module == _stringConstant.TaskModule)
                TaskBot.CloseSocket();
            else if (module == _stringConstant.Scrum)
                ScrumBot.CloseSocket();
        }
        #endregion
    }
}
