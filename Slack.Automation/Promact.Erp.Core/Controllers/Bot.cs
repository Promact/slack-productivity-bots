using System;
using System.Net.Http;
using System.Threading.Tasks;
using SlackAPI;
using Autofac.Extras.NLog;
using Promact.Core.Repository.ScrumRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.StringConstants;
using SlackAPI.WebSocketMessages;

namespace Promact.Erp.Core.Controllers
{
    public class Bot
    {

        #region Private Variables

        private readonly ITaskMailRepository _taskMailRepository;
        private readonly ISlackUserRepository _slackUserDetails;
        private readonly ILogger _logger;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IScrumBotRepository _scrumBotRepository;
        private readonly IEnvironmentVariableRepository _environmentVariableRepository;

        #endregion

        #region Constructor

        public Bot(ITaskMailRepository taskMailRepository,
           ISlackUserRepository slackUserDetails, ILogger logger,
           IStringConstantRepository stringConstant, IScrumBotRepository scrumBotRepository,
           IEnvironmentVariableRepository environmentVariableRepository)
        {
            _taskMailRepository = taskMailRepository;
            _slackUserDetails = slackUserDetails;
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
            try
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
                        var user = _slackUserDetails.GetByIdAsync(message.user).Result;
                        string replyText = "";
                        var text = message.text.ToLower();
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
                catch (Exception)
                {
                    client.CloseSocket();
                }
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
        /// <param name="container"></param>
        public void Scrum()
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
                try
                {
                    string replyText = string.Empty;
                    replyText = _scrumBotRepository.ProcessMessagesAsync(message.user, message.channel, message.text).Result;

                    if (!String.IsNullOrEmpty(replyText))
                    {
                        client.SendMessage(showMethod, message.channel, replyText);
                    }
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.Flatten().InnerExceptions)
                    {
                        if (e is ForbiddenUserException)
                        {
                            client.SendMessage(showMethod, message.channel, _stringConstant.UnAuthorized);
                            _logger.Error("\n" + _stringConstant.LoggerScrumBot + _stringConstant.ForbiddenUser + ae.InnerException + "\n" + ae.StackTrace);
                        }
                        else if (e is TaskCanceledException)
                        {
                            client.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                            _logger.Error("\n" + _stringConstant.LoggerScrumBot + _stringConstant.OAuthNotResponding + ae.InnerException + "\n" + ae.StackTrace);
                            client.CloseSocket();
                        }
                        else if (e is HttpRequestException)
                        {
                            client.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                            _logger.Error("\n" + _stringConstant.LoggerScrumBot + _stringConstant.OAuthServerClosed + ae.InnerException + "\n" + ae.StackTrace);
                            client.CloseSocket();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            };
        }

        #endregion

    }
}