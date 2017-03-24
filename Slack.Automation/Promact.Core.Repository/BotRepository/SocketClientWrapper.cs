using Autofac;
using NLog;
using Promact.Core.Repository.ScrumRepository;
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
        private readonly ITaskMailBotRepository _taskMailBotRepository;
        private readonly ILogger _scrumlogger;
        private readonly IComponentContext _component;

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
        public SocketClientWrapper(IStringConstantRepository stringConstant, ITaskMailBotRepository taskMailBotRepository,
            IComponentContext component)
        {
            _stringConstant = stringConstant;
            _taskMailBotRepository = taskMailBotRepository;
            _component = component;
            _scrumlogger = LogManager.GetLogger("ScrumBotModule");
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
            Action<MessageReceived> showMethod = GetShowMethod();
            ScrumBot.Connect((connect) => { });
            ScrumBot.OnMessageReceived += async (message) =>
            {
                IScrumBotRepository scrumBotRepository = _component.Resolve<IScrumBotRepository>();

                _scrumlogger.Debug("Scrum bot got message : " + message.text + " From user : " + message.user + " Of channel : " + message.channel);
                string replyText = await scrumBotRepository.ProcessMessagesAsync(message.user, message.channel, message.text);
                _scrumlogger.Debug("Scrum bot got reply : " + replyText + " To user : " + message.user + " Of channel : " + message.channel);

                if (!String.IsNullOrEmpty(replyText))
                {
                    _scrumlogger.Debug("Scrum bot sending reply");
                    ScrumBot.SendMessage(showMethod, message.channel, replyText);
                    _scrumlogger.Debug("Scrum bot sent reply");
                }
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
