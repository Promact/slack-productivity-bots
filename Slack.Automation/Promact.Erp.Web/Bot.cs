using Autofac;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.Util;
using SlackAPI;
using SlackAPI.WebSocketMessages;
using System;

namespace Promact.Erp.Web
{
    public class Bot
    {
        private static ITaskMailRepository _taskMailRepository;
        private static ISlackUserRepository _slackUserDetails;
        public static void Main(IComponentContext container)
        {
            // assigning bot token on Slack Socket Client
            SlackSocketClient client = new SlackSocketClient(AppSettingsUtil.TaskmailAccessToken);
            try
            {
                _taskMailRepository = container.Resolve<ITaskMailRepository>();
                _slackUserDetails = container.Resolve<ISlackUserRepository>();
                // Creating a Action<MessageReceived> for Slack Socket Client to get connect. No use in task mail bot
                MessageReceived messageReceive = new MessageReceived();
                messageReceive.ok = true;
                Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
                // Telling Slack Socket Client to the bot whose access token was given early
                client.Connect((connected) =>{});
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
            catch (Exception ex)
            {
                client.CloseSocket();
                throw ex;
            }
        }
    }
}