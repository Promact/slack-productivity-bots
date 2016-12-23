using Autofac.Extras.NLog;
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
        private readonly ILogger _logger;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IScrumBotRepository _scrumBotRepository;
        private readonly IEnvironmentVariableRepository _environmentVariableRepository;
        #endregion

        #region Constructor
        public Bot(ITaskMailRepository taskMailRepository,
           ISlackUserRepository slackUserDetailsRepository, ILogger logger,
           IStringConstantRepository stringConstant, IScrumBotRepository scrumBotRepository,
           IEnvironmentVariableRepository environmentVariableRepository)
        {
            _taskMailRepository = taskMailRepository;
            _slackUserDetailsRepository = slackUserDetailsRepository;
            _logger = logger;
            _stringConstant = stringConstant;
            _scrumBotRepository = scrumBotRepository;
            _environmentVariableRepository = environmentVariableRepository;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Used to connect task mail bot and to capture task mail
        /// </summary>
        public void TaskMailBot()
        {
            // assigning bot token on Slack Socket Client
            SlackSocketClient client = new SlackSocketClient(_environmentVariableRepository.TaskmailAccessToken);
            // Creating a Action<MessageReceived> for Slack Socket Client to get connect. No use in task mail bot
            MessageReceived messageReceive = new MessageReceived();
            messageReceive.ok = true;
            Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
            // Telling Slack Socket Client to the bot whose access token was given early
            client.Connect((connected) => { });
            try
            {
                // Method will hit when someone send some text in task mail bot
                client.OnMessageReceived += (message) =>
                {
                    var user = _slackUserDetailsRepository.GetByIdAsync(message.user).Result;
                    string replyText = _stringConstant.EmptyString;
                    var text = message.text;
                    if (user != null)
                    {
                        if (text.ToLower() == _stringConstant.TaskMailSubject.ToLower())
                        {
                            replyText = _taskMailRepository.StartTaskMailAsync(user.UserId).Result;
                        }
                        else
                        {
                            replyText = _taskMailRepository.QuestionAndAnswerAsync(text, user.UserId).Result;
                        }
                    }
                    else
                    {
                        replyText = _stringConstant.NoSlackDetails;
                    }
                        // Method to send back response to task mail bot
                        client.SendMessage(showMethod, message.channel, replyText);
                };
            }
            catch (Exception ex)
            {
                client.CloseSocket();
                _logger.Error(_stringConstant.LoggerErrorMessageTaskMailBot + _stringConstant.Space + ex.Message +
                Environment.NewLine + ex.StackTrace);
            }
        }


        /// <summary>
        /// Used for Scrum meeting bot connection and to conduct scrum meeting 
        /// </summary>
        public void ScrumMain()
        {
            string botToken = _environmentVariableRepository.ScrumBotToken;
            SlackSocketClient client = new SlackSocketClient(botToken);//scrumBot      
                                                                       // Creating a Action<MessageReceived> for Slack Socket Client to get connected.
            MessageReceived messageReceive = new MessageReceived();
            messageReceive.ok = true;
            Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
            //Connecting the bot of the given token 
            client.Connect((connected) => { });

            // Method will be called when someone sends message
            client.OnMessageReceived += (message) =>
            {
                _logger.Info("Scrum bot got message :" + message);
                try
                {
                    _logger.Info("Scrum bot got message, inside try");
                    string replyText = string.Empty;
                    replyText = _scrumBotRepository.ProcessMessages(message.user, message.channel, message.text).Result;

                    if (!String.IsNullOrEmpty(replyText))
                    {
                        _logger.Info("Scrum bot got reply");
                        client.SendMessage(showMethod, message.channel, replyText);
                    }
                }
                catch (AggregateException ex)
                {
                    foreach (var exception in ex.InnerExceptions)
                    {
                        _logger.Error("\n" + _stringConstant.LoggerScrumBot + " " + exception.InnerException + "\n" + exception.StackTrace);
                    }
                    client.CloseSocket();
                    throw ex;
                }
            };
        }
        #endregion
    }
}
