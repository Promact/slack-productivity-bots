using Autofac;
using NLog;
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


namespace Promact.Erp.Core.Controllers
{
    public class Bot
    {

        #region Private Variables

        private readonly ITaskMailRepository _taskMailRepository;
        private readonly ISlackUserRepository _slackUserDetailsRepository;
        private readonly ILogger _scrumlogger;
        private readonly ILogger _logger;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEnvironmentVariableRepository _environmentVariableRepository;
        private static string _scrumBotId;
        private readonly IComponentContext _component;

        #endregion


        #region Constructor

        public Bot(ITaskMailRepository taskMailRepository,
           ISlackUserRepository slackUserDetailsRepository,
           IStringConstantRepository stringConstant,
           IEnvironmentVariableRepository environmentVariableRepository, IComponentContext component)
        {
            _taskMailRepository = taskMailRepository;
            _slackUserDetailsRepository = slackUserDetailsRepository;
            _logger = LogManager.GetLogger("TaskBotModule");
            _scrumlogger = LogManager.GetLogger("ScrumBotModule");
            _stringConstant = stringConstant;
            _environmentVariableRepository = environmentVariableRepository;
            _component = component;
        }

        #endregion

        
        #region Public Methods


        /// <summary>
        /// Used to connect task mail bot and to capture task mail
        /// </summary>
        public void TaskMailBot()
        {
            _logger.Info("TaskMailAccessToken : " + _environmentVariableRepository.TaskmailAccessToken);
            SlackSocketClient client = new SlackSocketClient(_environmentVariableRepository.TaskmailAccessToken);
            // assigning bot token on Slack Socket Client
            // Creating a Action<MessageReceived> for Slack Socket Client to get connect. No use in task mail bot
            MessageReceived messageReceive = new MessageReceived();
            messageReceive.ok = true;
            Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
            // Telling Slack Socket Client to the bot whose access token was given early
            client.Connect((connected) => { });
            try
            {
                _logger.Info("Task mail bot connected");
                // Method will hit when someone send some text in task mail bot
                client.OnMessageReceived += (message) =>
                {
                    _logger.Info("Task mail bot receive message : " + message.text);
                    var user = _slackUserDetailsRepository.GetByIdAsync(message.user).Result;
                    _logger.Info("User : " + user.Name);
                    string replyText = string.Empty;
                    var text = message.text.ToLower();
                    if (user != null)
                    {
                        if (text == _stringConstant.TaskMailSubject.ToLower())
                        {
                            _logger.Info("Task Mail process start - StartTaskMailAsync");
                            replyText = _taskMailRepository.StartTaskMailAsync(user.UserId).Result;
                            _logger.Info("Task Mail process done : " + replyText);
                        }
                        else
                        {
                            _logger.Info("Task Mail process start - QuestionAndAnswerAsync");
                            replyText = _taskMailRepository.QuestionAndAnswerAsync(text, user.UserId).Result;
                            _logger.Info("Task Mail process done : " + replyText);
                        }
                    }
                    else
                    {
                        replyText = _stringConstant.NoSlackDetails;
                        _logger.Info("User is null : " + replyText);
                    }
                    // Method to send back response to task mail bot
                    client.SendMessage(showMethod, message.channel, replyText);
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


        /// <summary>
        /// Used for Scrum meeting bot connection and to conduct scrum meeting. - JJ 
        /// </summary>
        public void Scrum()
        {
            SlackSocketClient client = new SlackSocketClient(_environmentVariableRepository.ScrumBotToken);//scrumBot

            // Creating a Action<MessageReceived> for Slack Socket Client to get connected.
            MessageReceived messageReceive = new MessageReceived();
            messageReceive.ok = true;
            Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
            //Connecting the bot of the given token 
            client.Connect((connected) =>
            {
                _scrumBotId = connected.self.id;
            });

            // Method will be called when someone sends message
            client.OnMessageReceived += (message) =>
            {
                _scrumlogger.Debug("Scrum bot got message :" + message);
                try
                {
                    IScrumBotRepository scrumBotRepository = _component.Resolve<IScrumBotRepository>();

                    _scrumlogger.Debug("Scrum bot got message : " + message.text + " From user : " + message.user + " Of channel : " + message.channel);
                    string replyText = scrumBotRepository.ProcessMessagesAsync(message.user, message.channel, message.text, _scrumBotId).Result;
                    _scrumlogger.Debug("Scrum bot got reply : " + replyText + " To user : " + message.user + " Of channel : " + message.channel);

                    if (!String.IsNullOrEmpty(replyText))
                    {
                        _scrumlogger.Debug("Scrum bot sending reply");
                        client.SendMessage(showMethod, message.channel, replyText);
                        _scrumlogger.Debug("Scrum bot sent reply");
                    }
                }
                catch (TaskCanceledException ex)
                {
                    client.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                    _scrumlogger.Trace(ex.StackTrace);
                    _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " OAuth Server Not Responding " + ex.InnerException);
                    client.CloseSocket();
                    throw ex;
                }
                catch (HttpRequestException ex)
                {
                    client.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                    _scrumlogger.Trace(ex.StackTrace);
                    _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " OAuth Server Closed \nInner exception :\n" + ex.InnerException);
                    client.CloseSocket();
                    throw ex;
                }
                catch (Exception ex)
                {
                    client.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                    _scrumlogger.Trace(ex.StackTrace);
                    _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " Generic exception \nMessage : \n" + ex.Message + "\nInner exception :\n" + ex.InnerException);
                    client.CloseSocket();
                    throw ex;
                }
            };
        }


        #endregion
    }
}