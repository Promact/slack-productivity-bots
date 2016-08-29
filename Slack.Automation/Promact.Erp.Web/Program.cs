using Autofac;
using Microsoft.AspNet.Identity;
using Promact.Core.Repository.Bot;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.ScrumRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.DomainModel.Models;

namespace Promact.Erp.Web
{
    public class Program
    {
        private static BotClient _client;
        private static IScrumBotRepository _scrumBotRepository;
        private static ITaskMailRepository _taskMailRepository;

        public static void Main(IComponentContext container)
        {
            _scrumBotRepository = container.Resolve<IScrumBotRepository>();

            _taskMailRepository = container.Resolve<ITaskMailRepository>();
            //create a new slack client
            _client = new BotClient("xoxb-61375498279-ZBxCBFUkvnlR4muKNiUh7tCG");//tsakmail 
            //_client = new BotClient("xoxb-71985530755-ZFMwfQPVez6RBX5zJUEBhFy0");//taskmail

            //connect to the slack service
            _client.ConnectClientThread();

            _client.Message += new BotClient.MessageEventHandler(ClientMessage);
        }


        private static void ClientMessage(MessageEventArgs e)
        {
            try
            {
                if (e.User == null)
                {
                    return;
                }
                if (e.GroupInfo != null)
                {
                    if (e.UserInfo.Name != "tsakmail")
                    {
                        var simpleText = e.Text.Split(null);
                        if (e.Text.ToLower().Equals("scrum time"))
                        {
                            var reply = _scrumBotRepository.StartScrum(e.GroupInfo.name);
                            //  _client.WriteMessage("ok");
                        }
                        else if (simpleText[0].ToLower().Equals("leave") && simpleText.Length == 2)
                        {
                            _scrumBotRepository.Leave(e.GroupInfo.name, e.Text);
                        }
                        else
                        {
                            _scrumBotRepository.AddScrumAnswer(e.UserInfo.Name, e.Text, e.GroupInfo.name);
                        }
                    }
                }
                else
                {
                    //if (e.UserInfo.Name != "tsakmail")
                    //{
                    //    var reply = _scrumBotRepository.StartScrum(e.channel);
                    //}
               
                    var text = e.Text.ToLower();
                    if (text == "task mail")
                    {
                         //_taskMailRepository.StartTaskMail(e.channel, "bcd34169-1434-40e9-abf5-c9e0e9d20cd8").Wait();
                    }
                    else
                    {

                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        ////This example method processes commands from users and optionally sends a message back to the user
        //private static void ProcessMessage(string userName, string channel, string text)
        //{
        //    try
        //    {
        //        // const Int32 MAX_NEWS_ITEMS = 10;
        //        PostMessageArguments args = new PostMessageArguments();
        //        args.Channel = channel;
        //        args.Username = userName;
        //        //args.as_user = true;
        //        text = text.Trim().ToLower();
        //        if (args.Username != "tsakmail")
        //        {
        //            switch (text)
        //            {
        //                case "scrum meeting":
        //                    //if (questionCount == 0)
        //                    //    args.text = "What did you do yesterday ?" + questionCount;
        //                    //else if (questionCount == 1)
        //                    //    args.text = "What will you do today ?" + questionCount;
        //                    //else if (questionCount == 2)
        //                    //    args.text = "Any Roadblock ?" + questionCount;
        //                    //client.Chat.PostMessage(args);
        //                    //questionCount++;
        //                    break;
        //                default:
        //                    //if (questionCount == 0)
        //                    //    args.text = "What did you do yesterday ?" + questionCount;
        //                    //else if (questionCount == 1)
        //                    //    args.text = "What will you do today ?" + questionCount;
        //                    //else if (questionCount == 2)
        //                    //    args.text = "Any Roadblock ?" + questionCount;
        //                    //if (!(string.IsNullOrEmpty(args.text)))
        //                    //{
        //                    //    client.Chat.PostMessage(args);
        //                    //    questionCount++;
        //                    //}
        //                    break;
        //            }
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

    }
}