using Autofac;
using NLog;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.Util.StringConstants;
using SlackAPI.WebSocketMessages;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Promact.Core.Repository.BotRepository
{
    public class ScrumRepository : IScrumRepository
    {
        #region Private Variables
        private readonly ISlackUserRepository _slackUserDetailsRepository;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IComponentContext _component;
        private readonly ILogger _scrumlogger;
        private readonly ISocketClientWrapper _socketClientWrapper;
        private string _scrumBotId = null;
        #endregion

        #region Constructor
        public ScrumRepository(ISlackUserRepository slackUserDetailsRepository, IStringConstantRepository stringConstant,
            IComponentContext component, ISocketClientWrapper socketClientWrapper)
        {
            _slackUserDetailsRepository = slackUserDetailsRepository;
            _stringConstant = stringConstant;
            _component = component;
            _scrumlogger = LogManager.GetLogger("ScrumBotModule");
            _socketClientWrapper = socketClientWrapper;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to turn off scrum bot
        /// </summary>
        public void TurnOffScrumBot()
        {
            _socketClientWrapper.ScrumBot.CloseSocket();
        }

        /// <summary>
        /// Method to turn on scrum bot
        /// </summary>
        /// <param name="botToken">token of bot</param>
        public void StartAndConnectScrumBot(string botToken)
        {
            if (!string.IsNullOrEmpty(botToken))
            {
                _socketClientWrapper.InitializeScrumBot(botToken);
                // Creating a Action<MessageReceived> for Slack Socket Client to get connected.
                MessageReceived messageReceive = new MessageReceived();
                messageReceive.ok = true;
                Action<MessageReceived> showMethod = (MessageReceived messageReceived) => new MessageReceived();
                _socketClientWrapper.ScrumBot.Connect((connect) => { });
                _socketClientWrapper.ScrumBot.OnMessageReceived += (message) =>
                {
                    _scrumlogger.Debug("Scrum bot got message :" + message);
                    try
                    {
                        Repository.ScrumRepository.IScrumBotRepository scrumBotRepository = _component.Resolve<Repository.ScrumRepository.IScrumBotRepository>();

                        _scrumlogger.Debug("Scrum bot got message : " + message.text + " From user : " + message.user + " Of channel : " + message.channel);
                        string replyText = scrumBotRepository.ProcessMessagesAsync(message.user, message.channel, message.text, _scrumBotId).Result;
                        _scrumlogger.Debug("Scrum bot got reply : " + replyText + " To user : " + message.user + " Of channel : " + message.channel);

                        if (!String.IsNullOrEmpty(replyText))
                        {
                            _scrumlogger.Debug("Scrum bot sending reply");
                            _socketClientWrapper.ScrumBot.SendMessage(showMethod, message.channel, replyText);
                            _scrumlogger.Debug("Scrum bot sent reply");
                        }
                    }
                    catch (TaskCanceledException ex)
                    {
                        _socketClientWrapper.ScrumBot.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                        _scrumlogger.Trace(ex.StackTrace);
                        _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " OAuth Server Not Responding " + ex.InnerException);
                        _socketClientWrapper.ScrumBot.CloseSocket();
                        throw ex;
                    }
                    catch (HttpRequestException ex)
                    {
                        _socketClientWrapper.ScrumBot.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                        _scrumlogger.Trace(ex.StackTrace);
                        _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " OAuth Server Closed \nInner exception :\n" + ex.InnerException);
                        _socketClientWrapper.ScrumBot.CloseSocket();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        _socketClientWrapper.ScrumBot.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                        _scrumlogger.Trace(ex.StackTrace);
                        _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " Generic exception \nMessage : \n" + ex.Message + "\nInner exception :\n" + ex.InnerException);
                        _socketClientWrapper.ScrumBot.CloseSocket();
                        throw ex;
                    }
                };
            }
        }
        #endregion
    }
}
