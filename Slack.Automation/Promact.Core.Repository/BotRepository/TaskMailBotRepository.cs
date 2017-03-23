using NLog;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.Util.StringConstants;
using SlackAPI.WebSocketMessages;
using System.Threading.Tasks;

namespace Promact.Core.Repository.BotRepository
{
    public class TaskMailBotRepository : ITaskMailBotRepository
    {
        #region Private Variables
        private ITaskMailRepository _taskMailRepository;
        private ISlackUserRepository _slackUserDetailsRepository;
        private IStringConstantRepository _stringConstant;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public TaskMailBotRepository(ITaskMailRepository taskMailRepository, ISlackUserRepository slackUserDetailsRepository,
            IStringConstantRepository stringConstant)
        {
            _taskMailRepository = taskMailRepository;
            _slackUserDetailsRepository = slackUserDetailsRepository;
            _stringConstant = stringConstant;
            _logger = LogManager.GetLogger("TaskBotModule");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to turn on task mail bot
        /// </summary>
        /// <param name="botToken">message from slack</param>
        public async Task<string> ConductTask(NewMessage message)
        {
            _logger.Info("Task mail bot connected");
            var user = await _slackUserDetailsRepository.GetByIdAsync(message.user);
            string replyText = string.Empty;
            var text = message.text.ToLower();
            if (user != null)
            {
                _logger.Info("User : " + user.Name);
                if (text == _stringConstant.TaskMailSubject.ToLower())
                {
                    _logger.Info("Task Mail process start - StartTaskMailAsync");
                    replyText = await _taskMailRepository.StartTaskMailAsync(user.UserId);
                    _logger.Info("Task Mail process done : " + replyText);
                }
                else
                {
                    _logger.Info("Task Mail process start - QuestionAndAnswerAsync");
                    replyText = await _taskMailRepository.QuestionAndAnswerAsync(text, user.UserId);
                    _logger.Info("Task Mail process done : " + replyText);
                }
            }
            else
            {
                replyText = _stringConstant.NoSlackDetails;
                _logger.Info("User is null : " + replyText);
            }
            _logger.Info("Reply message sended");
            return replyText;
        }
        #endregion
    }
}
