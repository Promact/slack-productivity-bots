using Autofac;
using NLog;
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
        private readonly IStringConstantRepository _stringConstant;
        private readonly IComponentContext _component;
        private readonly ILogger _scrumlogger;
        private readonly ISocketClientWrapper _socketClientWrapper;
       
        #endregion

        #region Constructor
        public ScrumRepository(IStringConstantRepository stringConstant,
            IComponentContext component, ISocketClientWrapper socketClientWrapper)
        {
            _stringConstant = stringConstant;
            _component = component;
            _scrumlogger = LogManager.GetLogger("ScrumBotModule");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to turn on scrum bot
        /// </summary>
        /// <param name="botToken">token of bot</param>
        public async Task<string> ConductScrum(NewMessage message)
        {
            _scrumlogger.Debug("Scrum bot got message :" + message);
            string replyText = string.Empty;
            try
            {
                Repository.ScrumRepository.IScrumBotRepository scrumBotRepository = _component.Resolve<Repository.ScrumRepository.IScrumBotRepository>();

                        _scrumlogger.Debug("Scrum bot got message : " + message.text + " From user : " + message.user + " Of channel : " + message.channel);
                        string replyText = scrumBotRepository.ProcessMessagesAsync(message.user, message.channel, message.text).Result;
                        _scrumlogger.Debug("Scrum bot got reply : " + replyText + " To user : " + message.user + " Of channel : " + message.channel);

                if (!String.IsNullOrEmpty(replyText))
                {
                    _scrumlogger.Debug("Scrum bot sending reply");
                    //_socketClientWrapper.ScrumBot.SendMessage(showMethod, message.channel, replyText);
                    _scrumlogger.Debug("Scrum bot sent reply");
                }
            }
            catch (TaskCanceledException ex)
            {
                replyText = _stringConstant.ErrorMsg;
                //_socketClientWrapper.ScrumBot.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                _scrumlogger.Trace(ex.StackTrace);
                _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " OAuth Server Not Responding " + ex.InnerException);
                //_socketClientWrapper.ScrumBot.CloseSocket();
                throw ex;
            }
            catch (HttpRequestException ex)
            {
                replyText = _stringConstant.ErrorMsg;
                //_socketClientWrapper.ScrumBot.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                _scrumlogger.Trace(ex.StackTrace);
                _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " OAuth Server Closed \nInner exception :\n" + ex.InnerException);
                //_socketClientWrapper.ScrumBot.CloseSocket();
                throw ex;
            }
            catch (Exception ex)
            {
                replyText = _stringConstant.ErrorMsg;
                //_socketClientWrapper.ScrumBot.SendMessage(showMethod, message.channel, _stringConstant.ErrorMsg);
                _scrumlogger.Trace(ex.StackTrace);
                _scrumlogger.Error("\n" + _stringConstant.LoggerScrumBot + " Generic exception \nMessage : \n" + ex.Message + "\nInner exception :\n" + ex.InnerException);
                //_socketClientWrapper.ScrumBot.CloseSocket();
                throw ex;
            }
            return replyText;
        }
    }
    #endregion
}
