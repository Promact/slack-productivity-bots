using Autofac;
using NLog;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.Util.StringConstants;
using SlackAPI.WebSocketMessages;
using System;

namespace Promact.Core.Repository.BotRepository
{
    public class TaskMailBotRepository : ITaskMailBotRepository
    {
        #region Private Variables
        private ITaskMailRepository _taskMailRepository;
        private ISlackUserRepository _slackUserDetailsRepository;
        private IStringConstantRepository _stringConstant;
        private readonly IComponentContext _component;
        private readonly ILogger _logger;
        private readonly ISocketClientWrapper _socketClientWrapper;
        #endregion

        #region Constructor
        public TaskMailBotRepository(ITaskMailRepository taskMailRepository, ISlackUserRepository slackUserDetailsRepository,
            IStringConstantRepository stringConstant, IComponentContext component, ISocketClientWrapper socketClientWrapper)
        {
            _component = component;
            _logger = LogManager.GetLogger("TaskBotModule");
            _socketClientWrapper = socketClientWrapper;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to turn on task mail bot
        /// </summary>
        /// <param name="botToken">token of bot</param>
        public void StartAndConnectTaskMailBot(string botToken)
        {
            if (!string.IsNullOrEmpty(botToken))
            {
                _socketClientWrapper.InitializeAndConnectTaskBot(botToken);
                // Creating a Action<MessageReceived> for Slack Socket Client to get connected.
                MessageReceived messageReceive = new MessageReceived();
                messageReceive.ok = true;
                Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
                _socketClientWrapper.TaskBot.Connect((connect) => { });
                try
                {
                    _taskMailRepository = _component.Resolve<ITaskMailRepository>();
                    _slackUserDetailsRepository = _component.Resolve<ISlackUserRepository>();
                    _stringConstant = _component.Resolve<IStringConstantRepository>();
                    _logger.Info("Task mail bot connected");
                    // Method will hit when someone send some text in task mail bot
                    _socketClientWrapper.TaskBot.OnMessageReceived += async (message) =>
                    {
                        _logger.Info("Task mail bot receive message : " + message.text);
                        var user = await _slackUserDetailsRepository.GetByIdAsync(message.user);
                        string replyText = string.Empty;
                        var text = message.text.ToLower();
                        if (user != null)
                        {
                            _logger.Info("User : " + user.Name);
                            if (text == _stringConstant.TaskMailSubject.ToLower())
                            {
                                _logger.Info("Task Mail process start - StartTaskMailAsync");
                                replyText = await _taskMailRepository.StartTaskMailAsync(user.UserId);
                                _logger.Info("Task Mail process done : " + replyText);
                            }
                            else
                            {
                                _logger.Info("Task Mail process start - QuestionAndAnswerAsync");
                                replyText = await _taskMailRepository.QuestionAndAnswerAsync(text, user.UserId);
                                _logger.Info("Task Mail process done : " + replyText);
                            }
                        }
                        else
                        {
                            replyText = _stringConstant.NoSlackDetails;
                            _logger.Info("User is null : " + replyText);
                        }
                    // Method to send back response to task mail bot
                    _socketClientWrapper.TaskBot.SendMessage(showMethod, message.channel, replyText);
                        _logger.Info("Reply message sended");
                    };
                }
                catch (Exception ex)
                {
                    _logger.Error(_stringConstant.LoggerErrorMessageTaskMailBot + _stringConstant.Space + ex.Message +
                        Environment.NewLine + ex.StackTrace);
                    throw ex;
                }
            }
        }
        #endregion
    }
}
