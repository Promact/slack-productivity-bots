using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.Util.StringConstants;
using SlackAPI;
using SlackAPI.WebSocketMessages;
using System;

namespace Promact.Core.Repository.BotRepository
{
    public class SocketClientWrapper : ISocketClientWrapper
    {
        #region Private Variable
        private readonly IStringConstantRepository _stringConstant;
        private readonly ITaskMailRepository _taskMailRepository;
        private readonly IScrumRepository _scrumRepository;
        #endregion

        #region Public property
        /// <summary>
        /// Contain ScrumBot Client socket details
        /// </summary>
        private SlackSocketClient ScrumBot { get; set; }

        /// <summary>
        /// Contain TaskMailBot Client socket details
        /// </summary>
        private SlackSocketClient TaskBot { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SocketClientWrapper(IStringConstantRepository stringConstant, ITaskMailRepository taskMailRepository,
            IScrumRepository scrumRepository)
        {
            _stringConstant = stringConstant;
            _taskMailRepository = taskMailRepository;
            _scrumRepository = scrumRepository;
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
            var showMethod = GetShowMethod();
            ScrumBot.Connect((connect) => { });
            ScrumBot.OnMessageReceived += async (message) =>
            {
                var replyText = await _scrumRepository.ConductScrum(message);
                ScrumBot.SendMessage(showMethod, message.channel, replyText);
            };
        }

        /// <summary>
        /// Method to initialize task mail bot
        /// </summary>
        /// <param name="bottoken"></param>
        public void InitializeAndConnectTaskBot(string bottoken)
        {
            TaskBot = new SlackSocketClient(bottoken);
            var showMethod = GetShowMethod();
            TaskBot.Connect((connect) => { });
            TaskBot.OnMessageReceived += async (message) =>
            {
                var replyText = await _taskMailRepository.ProcessTask(message);
                TaskBot.SendMessage(showMethod, message.channel, replyText);
            };
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

        #region Private Method
        /// <summary>
        /// Method to create action of receive message
        /// </summary>
        /// <returns>action of message receive</returns>
        private Action<MessageReceived> GetShowMethod()
        {
            // Creating a Action<MessageReceived> for Slack Socket Client to get connected.
            MessageReceived messageReceive = new MessageReceived();
            messageReceive.ok = true;
            Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
            return showMethod;
        }
        #endregion
    }
}
