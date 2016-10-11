using Autofac;
using Autofac.Extras.NLog;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.ScrumRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using SlackAPI;
using SlackAPI.WebSocketMessages;
using System;

namespace Promact.Erp.Web
{
    public class Bot
    {
        private static ITaskMailRepository _taskMailRepository;
        private static ISlackUserRepository _slackUserDetails;
        private static ILogger _logger;
        private static IScrumBotRepository _scrumBotRepository;
        private static IClient _email;
        private static IEnvironmentVariableRepository _environmentVariableRepository;
        /// <summary>
        /// Used to connect task mail bot and to capture task mail
        /// </summary>
        /// <param name="container"></param>
        public static void Main(IComponentContext container)
        {
            _logger = container.Resolve<ILogger>();
            try
            {
                _taskMailRepository = container.Resolve<ITaskMailRepository>();
                _slackUserDetails = container.Resolve<ISlackUserRepository>();

                _environmentVariableRepository = container.Resolve<IEnvironmentVariableRepository>();
                // assigning bot token on Slack Socket Client
                string botToken = _environmentVariableRepository.TaskmailAccessToken;
                SlackSocketClient client = new SlackSocketClient(botToken);
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
                        var user = _slackUserDetails.GetById(message.user);
                        string replyText = "";
                        var text = message.text;
                        if (text.ToLower() == StringConstant.TaskMailSubject.ToLower())
                        {
                            replyText = _taskMailRepository.StartTaskMail(user.Name).Result;
                        }
                        else
                        {
                            replyText = _taskMailRepository.QuestionAndAnswer(user.Name, text).Result;
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
                _logger.Error(StringConstant.LoggerErrorMessageTaskMailBot + " " + ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }



        /// <summary>
        /// Used for Scrum meeting bot connection and to conduct scrum meeting 
        /// </summary>
        /// <param name="container"></param>
        public static void ScrumMain(IComponentContext container)
        {
            _logger = container.Resolve<ILogger>();
            try
            {
                _environmentVariableRepository = container.Resolve<IEnvironmentVariableRepository>();
                string botToken = _environmentVariableRepository.ScrumBotToken;
                SlackSocketClient client = new SlackSocketClient(botToken);//scrumBot
                _scrumBotRepository = container.Resolve<IScrumBotRepository>();

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
                        string replyText = _scrumBotRepository.ProcessMessages(message.user, message.channel, message.text).Result;
                        if (!String.IsNullOrEmpty(replyText))
                            client.SendMessage(showMethod, message.channel, replyText);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("\n" + StringConstant.LoggerScrumBot + " " + ex.Message + "\n" + ex.StackTrace);
                        throw ex;
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.Error("\n" + StringConstant.LoggerScrumBot + " " + ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

    }
}