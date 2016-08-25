using Autofac;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.DomainModel.ApplicationClass.Bot;
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
        public static void Main(IComponentContext container)
        {
            SlackSocketClient client = new SlackSocketClient("xoxb-72838792578-wclIZGTziSmKtqVjrymcWABA");

            try
            {
                _taskMailRepository = container.Resolve<ITaskMailRepository>();

                //ListBot bots;
                MessageReceived messageReceive = new MessageReceived();
                messageReceive.text = "hello";
                messageReceive.ok = true;
                Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
                client.Connect((connected) =>
                {
                    //bots = new ListBot(connected.bots);
                });

                client.OnMessageReceived += (message) =>
                {
                    string replyText = "";
                    var text = message.text;
                    if (text.ToLower() == "task mail")
                    {
                        replyText = _taskMailRepository.StartTaskMail("siddhartha", "bcd34169-1434-40e9-abf5-c9e0e9d20cd8").Result;
                    }
                    else
                    {
                        replyText = _taskMailRepository.QuestionAndAnswer("siddhartha", "bcd34169-1434-40e9-abf5-c9e0e9d20cd8", text).Result;
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