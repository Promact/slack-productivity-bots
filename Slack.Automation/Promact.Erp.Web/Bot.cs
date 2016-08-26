using Autofac;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.DomainModel.ApplicationClass.Bot;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using SlackAPI;
using SlackAPI.WebSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Promact.Erp.Web
{
    public class Bot
    {
        private static ITaskMailRepository _taskMailRepository;
        private static ISlackUserRepository _slackUserDetails;
        public static void Main(IComponentContext container)
        {
            SlackSocketClient client = new SlackSocketClient("xoxb-72838792578-wclIZGTziSmKtqVjrymcWABA");

            try
            {
                _taskMailRepository = container.Resolve<ITaskMailRepository>();
                _slackUserDetails = container.Resolve<ISlackUserRepository>();
                MessageReceived messageReceive = new MessageReceived();
                messageReceive.ok = true;
                Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
                client.Connect((connected) =>{});

                client.OnMessageReceived += (message) =>
                {
                    var user = _slackUserDetails.GetById(message.user);
                    string replyText = "";
                    var text = message.text;
                    if (text.ToLower() == "task mail")
                    {
                        replyText = _taskMailRepository.StartTaskMail(user.Name).Result;
                    }
                    else
                    {
                        replyText = _taskMailRepository.QuestionAndAnswer(user.Name, text).Result;
                    }
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