using Autofac;
using NLog;
using Promact.Core.Repository.AppCredentialRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.StringConstants;
using SlackAPI;
using SlackAPI.WebSocketMessages;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Promact.Core.Repository.BotRepository
{
    public class BotRepository : IBotRepository
    {
        #region Private Variables
        private ITaskMailRepository _taskMailRepository;
        private readonly ISlackUserRepository _slackUserDetailsRepository;
        private readonly ILogger _scrumlogger;
        private readonly ILogger _logger;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEnvironmentVariableRepository _environmentVariableRepository;
        private readonly IAppCredentialRepository _appCredentialRepository;
        private static string _scrumBotId;
        private readonly IComponentContext _component;
        private static SlackSocketClient taskMailClient;
        private static SlackSocketClient scrumClient;
        #endregion

        #region Constructor
        public BotRepository(ISlackUserRepository slackUserDetailsRepository,
           IStringConstantRepository stringConstant, IAppCredentialRepository appCredentialRepository,
           IEnvironmentVariableRepository environmentVariableRepository, IComponentContext component)
        {
            //_taskMailRepository = taskMailRepository;
            _slackUserDetailsRepository = slackUserDetailsRepository;
            _logger = LogManager.GetLogger("TaskBotModule");
            _scrumlogger = LogManager.GetLogger("ScrumBotModule");
            _stringConstant = stringConstant;
            _appCredentialRepository = appCredentialRepository;
            _environmentVariableRepository = environmentVariableRepository;
            _component = component;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Used to connect task mail bot and to capture task mail
        /// </summary>
        public async Task TaskMailBot()
        {
            if (await SetBotTokenByModule(_stringConstant.TaskModule))
            {
                // assigning bot token on Slack Socket Client
                // Creating a Action<MessageReceived> for Slack Socket Client to get connect. No use in task mail bot
                MessageReceived messageReceive = new MessageReceived();
                messageReceive.ok = true;
                Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
                // Telling Slack Socket Client to the bot whose access token was given early
                taskMailClient.Connect((connected) => { });
                try
                {
                    _taskMailRepository = _component.Resolve<ITaskMailRepository>();
                    _logger.Info("Task mail bot connected");
                    // Method will hit when someone send some text in task mail bot
                    taskMailClient.OnMessageReceived += async (message) =>
                    {
                        _logger.Info("Task mail bot receive message : " + message.text);
                        var user = await _slackUserDetailsRepository.GetByIdAsync(message.user);
                        _logger.Info("User : " + user.Name);
                        string replyText = string.Empty;
                        var text = message.text.ToLower();
                        if (user != null)
                        {
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
                        taskMailClient.SendMessage(showMethod, message.channel, replyText);
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

        /// <summary>
        /// Used for Scrum meeting bot connection and to conduct scrum meeting. - JJ 
        /// </summary>
        public async Task Scrum()
        {
            if (await SetBotTokenByModule(_stringConstant.Scrum))
            {
                // Creating a Action<MessageReceived> for Slack Socket Client to get connected.
                MessageReceived messageReceive = new MessageReceived();
                messageReceive.ok = true;
                Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
                //Connecting the bot of the given token 
                scrumClient.Connect((connected) =>
                {
                    _scrumBotId = connected.self.id;
                });

                // Method will be called when someone sends message
                scrumClient.OnMessageReceived += async (message) =>
                {
                    _scrumlogger.Debug("Scrum bot got message :" + message);
                    try
                    {
                        IScrumBotRepository scrumBotRepository = _component.Resolve<IScrumBotRepository>();

                        _scrumlogger.Debug("Scrum bot got message : " + message.text + " From user : " + message.user + " Of channel : " + message.channel);
                        string replyText = await scrumBotRepository.ProcessMessagesAsync(message.user, message.channel, message.text, _scrumBotId);
                        _scrumlogger.Debug("Scrum bot got reply : " + replyText + " To user : " + message.user + " Of channel : " + message.channel);

                        if (!String.IsNullOrEmpty(replyText))
                        {
                            _scrumlogger.Debug("Scrum bot sending reply");
                            scrumClient.SendMessage(showMethod, message.channel, replyText);
                            _scrumlogger.Debug("Scrum bot sent reply");
                        }
                    }
                    catch (TaskCanceledException ex)
                    {
                        scrumClient.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                        _scrumlogger.Trace(ex.StackTrace);
                        _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " OAuth Server Not Responding " + ex.InnerException);
                        scrumClient.CloseSocket();
                        throw ex;
                    }
                    catch (HttpRequestException ex)
                    {
                        scrumClient.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                        _scrumlogger.Trace(ex.StackTrace);
                        _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " OAuth Server Closed \nInner exception :\n" + ex.InnerException);
                        scrumClient.CloseSocket();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        scrumClient.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                        _scrumlogger.Trace(ex.StackTrace);
                        _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " Generic exception \nMessage : \n" + ex.Message + "\nInner exception :\n" + ex.InnerException);
                        scrumClient.CloseSocket();
                        throw ex;
                    }
                };
            }
        }

        /// <summary>
        /// Method to set token for bot by module
        /// </summary>
        /// <param name="module">name of module</param>
        /// <returns>true for set, false for not set</returns>
        private async Task<bool> SetBotTokenByModule(string module)
        {
            var appCredential = await _appCredentialRepository.FetchAppCredentialByModule(module);
            if (!string.IsNullOrEmpty(appCredential?.BotToken))
            {
                if (module == _stringConstant.TaskModule)
                    taskMailClient = new SlackSocketClient(appCredential.BotToken);
                else if (module == _stringConstant.Scrum)
                    scrumClient = new SlackSocketClient(appCredential.BotToken);
                return true;
            }
            return false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to return on bot by module name
        /// </summary>
        /// <param name="module">module name</param>
        public async Task TurnOnBot(string module)
        {
            if (module == _stringConstant.TaskModule)
                await TaskMailBot();
            else if (module == _stringConstant.Scrum)
                await Scrum();
        }

        /// <summary>
        /// Method to return off bot by module name
        /// </summary>
        /// <param name="module">module name</param>
        public async Task TurnOffBot(string module)
        {
            if (module == _stringConstant.TaskModule)
                taskMailClient.CloseSocket();
            else if (module == _stringConstant.Scrum)
                scrumClient.CloseSocket();
            await _appCredentialRepository.ClearBotTokenByModule(module);
        }
        #endregion
    }
}
