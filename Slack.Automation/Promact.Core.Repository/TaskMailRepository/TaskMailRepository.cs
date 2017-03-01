using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.Util.Email;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.Util.StringConstants;
using Promact.Core.Repository.EmailServiceTemplateRepository;
using Autofac.Extras.NLog;
using Promact.Core.Repository.MailSettingDetailsByProjectAndModule;
using System.Collections.Generic;

namespace Promact.Core.Repository.TaskMailRepository
{
    public class TaskMailRepository : ITaskMailRepository
    {
        #region Private Variables
        private readonly IRepository<TaskMail> _taskMailRepository;
        private readonly IRepository<TaskMailDetails> _taskMailDetailRepository;
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IEmailService _emailService;
        private readonly ApplicationUserManager _userManager;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEmailServiceTemplateRepository _emailServiceTemplate;
        private readonly ILogger _logger;
        private readonly IRepository<MailSetting> _mailSettingDataRepository;
        private readonly IMailSettingDetailsByProjectAndModuleRepository _mailSettingDetails;
        #endregion

        #region Constructor
        public TaskMailRepository(IRepository<TaskMail> taskMailRepository, IStringConstantRepository stringConstant,
            IOauthCallsRepository oauthCallsRepository, IRepository<TaskMailDetails> taskMailDetailRepository,
            IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> userRepository, IEmailService emailService,
            IBotQuestionRepository botQuestionRepository, ApplicationUserManager userManager,
            IEmailServiceTemplateRepository emailServiceTemplate, ILogger logger, IRepository<MailSetting> mailSettingDataRepository,
            IMailSettingDetailsByProjectAndModuleRepository mailSettingDetails)
        {
            _taskMailRepository = taskMailRepository;
            _stringConstant = stringConstant;
            _oauthCallsRepository = oauthCallsRepository;
            _taskMailDetailRepository = taskMailDetailRepository;
            _attachmentRepository = attachmentRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _botQuestionRepository = botQuestionRepository;
            _userManager = userManager;
            _emailServiceTemplate = emailServiceTemplate;
            _logger = logger;
            _mailSettingDataRepository = mailSettingDataRepository;
            _mailSettingDetails = mailSettingDetails;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to start task mail and send first question of task mail
        /// </summary>
        /// <param name="userId">User's slack user Id</param>
        /// <returns>questionText in string format containing question statement</returns>
        public async Task<string> StartTaskMailAsync(string userId)
        {
            // method to get user's details, user's accesstoken, user's task mail details and list or else appropriate message will be send
            var userAndTaskMailDetailsWithAccessToken = await GetUserAndTaskMailDetailsAsync(userId);
            _logger.Info("Task Mail Bot Module, Is task mail question text : " + userAndTaskMailDetailsWithAccessToken.QuestionText);
            bool questionTextIsNull = string.IsNullOrEmpty(userAndTaskMailDetailsWithAccessToken.QuestionText);
            // if question text is null or not request to start task mail then only allowed
            if (questionTextIsNull || CheckQuestionTextIsRequestToStartMailOrNot(userAndTaskMailDetailsWithAccessToken.QuestionText))
            {
                _logger.Info("Task Mail Bot Module, task mail process start - StartTaskMailAsync");
                TaskMailDetails taskMailDetail = new TaskMailDetails();
                #region Task Mail Details
                if (userAndTaskMailDetailsWithAccessToken.TaskList != null)
                {
                    taskMailDetail = userAndTaskMailDetailsWithAccessToken.TaskList.Last();
                    var previousQuestion = await _botQuestionRepository.FindByIdAsync(taskMailDetail.QuestionId);
                    var questionOrder = previousQuestion.OrderNumber;
                    // If previous task mail is on the process and user started new task mail then will need to first complete pervious one
                    if (questionOrder <= QuestionOrder.TaskMailSend)
                        userAndTaskMailDetailsWithAccessToken.QuestionText = await QuestionAndAnswerAsync(null, userId);
                }
                else
                {
                    // If task mail is not started for that day
                    userAndTaskMailDetailsWithAccessToken.TaskMail = await AddTaskMailAsync(userAndTaskMailDetailsWithAccessToken.User.Id);
                }
                #endregion

                #region If Task mail not send for that day
                // if question text is null or not request to start task mail then only allowed
                if (questionTextIsNull || CheckQuestionTextIsRequestToStartMailOrNot(userAndTaskMailDetailsWithAccessToken.QuestionText))
                {
                    SendEmailConfirmation confirmation = taskMailDetail.SendEmailConfirmation;
                    switch (confirmation)
                    {
                        case SendEmailConfirmation.yes:
                            {
                                // If mail is send then user will not be able to add task mail for that day
                                userAndTaskMailDetailsWithAccessToken.QuestionText = _stringConstant.AlreadyMailSend;
                            }
                            break;
                        case SendEmailConfirmation.no:
                            {
                                // If mail is not send then user will be able to add task mail for that day
                                userAndTaskMailDetailsWithAccessToken.QuestionText = await AddTaskMailDetailAndGetQuestionStatementAsync(userAndTaskMailDetailsWithAccessToken.TaskMail.Id);
                            }
                            break;
                    }
                }
                #endregion
            }
            return userAndTaskMailDetailsWithAccessToken.QuestionText;
        }

        /// <summary>
        /// Method to conduct task mail after been started, and send next question of task mail
        /// </summary>
        /// <param name="answer">Answer of previous question ask to user</param>
        /// <param name="userId">User's slack Id</param>
        /// <returns>questionText in string format containing next question statement</returns>
        public async Task<string> QuestionAndAnswerAsync(string answer, string userId)
        {
            // method to get user's details, user's accesstoken, user's task mail details and list or else appropriate message will be send
            var userAndTaskMailDetailsWithAccessToken = await GetUserAndTaskMailDetailsAsync(userId);
            _logger.Info("Task Mail Bot Module, Is task mail question text : " + userAndTaskMailDetailsWithAccessToken.QuestionText);
            // if question text is null then only allowed
            if (string.IsNullOrEmpty(userAndTaskMailDetailsWithAccessToken.QuestionText))
            {
                _logger.Info("Task Mail Bot Module, task mail process start - QuestionAndAnswerAsync");
                _logger.Info("Task Mail Bot Module, task mail list count : " + userAndTaskMailDetailsWithAccessToken.TaskList.Count());
                if (userAndTaskMailDetailsWithAccessToken.TaskList != null)
                {
                    var taskDetails = userAndTaskMailDetailsWithAccessToken.TaskList.Last();
                    // getting inform of previous question was asked to user
                    var previousQuestion = await _botQuestionRepository.FindByIdAsync(taskDetails.QuestionId);
                    // checking if previous question was last and answered by user and previous task report was completed then asked for new task mail
                    if (previousQuestion.OrderNumber <= QuestionOrder.TaskMailSend)
                    {
                        // getting next question to be asked to user
                        var nextQuestion = await NextQuestionForTaskMailAsync(previousQuestion.OrderNumber);
                        _logger.Info("Previous question ordernumber : " + previousQuestion.OrderNumber);
                        switch (previousQuestion.OrderNumber)
                        {
                            #region Your Task
                            case QuestionOrder.YourTask:
                                {
                                    // if previous question was description of task and it was not null/wrong vale then answer will ask next question
                                    if (answer != null)
                                    {
                                        taskDetails.Description = answer;
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    else
                                    {
                                        // if previous question was description of task and it was null then answer will ask for description again
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = previousQuestion.QuestionStatement;
                                    }
                                }
                                break;
                            #endregion

                            #region Hour Spent
                            case QuestionOrder.HoursSpent:
                                {
                                    // if previous question was hour of task and it was not null/wrong value then answer will ask next question
                                    decimal hour;
                                    // checking whether string can be convert to decimal or not
                                    var taskMailHourConvertResult = decimal.TryParse(answer, out hour);
                                    if (taskMailHourConvertResult)
                                    {
                                        decimal totalHourSpented = 0;
                                        // checking range of hours
                                        if (hour > 0 && hour <= Convert.ToInt32(_stringConstant.TaskMailMaximumTime))
                                        {
                                            // adding up all hours of task mail
                                            foreach (var task in userAndTaskMailDetailsWithAccessToken.TaskList)
                                            {
                                                totalHourSpented += task.Hours;
                                            }
                                            totalHourSpented += hour;
                                            // checking whether all up hour doesnot exceed task mail hour limit
                                            if (totalHourSpented <= Convert.ToInt32(_stringConstant.TaskMailMaximumTime))
                                            {
                                                taskDetails.Hours = hour;
                                                userAndTaskMailDetailsWithAccessToken.QuestionText = nextQuestion.QuestionStatement;
                                                taskDetails.QuestionId = nextQuestion.Id;
                                            }
                                            else
                                            {
                                                // getting last question of task mail
                                                nextQuestion = await NextQuestionForTaskMailAsync(QuestionOrder.SendEmail);
                                                taskDetails.QuestionId = nextQuestion.Id;
                                                userAndTaskMailDetailsWithAccessToken.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                                    _stringConstant.HourLimitExceed, Environment.NewLine, nextQuestion.QuestionStatement);
                                                taskDetails.Comment = _stringConstant.StartWorking;
                                            }
                                        }
                                        else
                                            // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                                            userAndTaskMailDetailsWithAccessToken.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                                _stringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
                                    else
                                        // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                            _stringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                }
                                break;
                            #endregion

                            #region Status
                            case QuestionOrder.Status:
                                {
                                    TaskMailStatus status;
                                    // checking whether string can be convert to TaskMailStatus type or not
                                    var taskMailStatusConvertResult = Enum.TryParse(answer, out status);
                                    if (taskMailStatusConvertResult)
                                    {
                                        // if previous question was status of task and it was not null/wrong value then answer will ask next question
                                        taskDetails.Status = status;
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    else
                                        // if previous question was status of task and it was null or wrong value then answer will ask for status again
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                            _stringConstant.TaskMailBotStatusErrorMessage,
                                            Environment.NewLine, previousQuestion.QuestionStatement);
                                }
                                break;
                            #endregion

                            #region Comment
                            case QuestionOrder.Comment:
                                {
                                    if (answer != null)
                                    {
                                        // if previous question was comment of task and answer was not null/wrong value then answer will ask next question
                                        taskDetails.Comment = answer;
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    else
                                    {
                                        // if previous question was comment of task and answer was null or wrong value then answer will ask for comment again
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = previousQuestion.QuestionStatement;
                                    }
                                }
                                break;
                            #endregion

                            #region Send Email
                            case QuestionOrder.SendEmail:
                                {
                                    SendEmailConfirmation confirmation;
                                    // checking whether string can be convert to TaskMailStatus type or not
                                    var sendEmailConfirmationConvertResult = Enum.TryParse(answer, out confirmation);
                                    if (sendEmailConfirmationConvertResult)
                                    {
                                        taskDetails.SendEmailConfirmation = confirmation;
                                        switch (confirmation)
                                        {
                                            case SendEmailConfirmation.yes:
                                                {
                                                    // if previous question was send email of task and answer was yes then answer will ask next question
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    userAndTaskMailDetailsWithAccessToken.QuestionText = nextQuestion.QuestionStatement;
                                                }
                                                break;
                                            case SendEmailConfirmation.no:
                                                {
                                                    // if previous question was send email of task and answer was no then answer will say thank you and task mail stopped
                                                    nextQuestion = await NextQuestionForTaskMailAsync(QuestionOrder.ConfirmSendEmail);
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    userAndTaskMailDetailsWithAccessToken.QuestionText = _stringConstant.ThankYou;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                        // if previous question was send email of task and answer was null/wrong value then answer will say thank you and task mail stopped
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                            _stringConstant.SendTaskMailConfirmationErrorMessage,
                                            Environment.NewLine, previousQuestion.QuestionStatement);
                                }
                                break;
                            #endregion

                            #region Confirm Send Email
                            case QuestionOrder.ConfirmSendEmail:
                                {
                                    SendEmailConfirmation confirmation;
                                    // checking whether string can be convert to TaskMailStatus type or not
                                    var sendEmailConfirmationConvertResult = Enum.TryParse(answer, out confirmation);
                                    if (sendEmailConfirmationConvertResult)
                                    {
                                        taskDetails.SendEmailConfirmation = confirmation;
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = _stringConstant.ThankYou;
                                        switch (confirmation)
                                        {
                                            case SendEmailConfirmation.yes:
                                                {
                                                    EmailApplication email = new EmailApplication();
                                                    email.To = new List<string>();
                                                    email.CC = new List<string>();
                                                    var listOfprojectRelatedToUser = (await _oauthCallsRepository.GetListOfProjectsEnrollmentOfUserByUserIdAsync(userAndTaskMailDetailsWithAccessToken.User.Id, userAndTaskMailDetailsWithAccessToken.AccessToken)).Select(x => x.Id).ToList();
                                                    foreach (var projectId in listOfprojectRelatedToUser)
                                                    {
                                                        var mailsetting = await _mailSettingDetails.GetMailSettingAsync(projectId, _stringConstant.TaskModule);
                                                        email.To.AddRange(mailsetting.To);
                                                        email.CC.AddRange(mailsetting.CC);
                                                    }
                                                    email.To = _mailSettingDetails.DeleteTheDuplicateString(email.To);
                                                    email.CC = _mailSettingDetails.DeleteTheDuplicateString(email.CC);
                                                    email.From = userAndTaskMailDetailsWithAccessToken.User.Email;
                                                    email.Subject = _stringConstant.TaskMailSubject;
                                                    // transforming task mail details to template page and getting as string
                                                    email.Body = _emailServiceTemplate.EmailServiceTemplateTaskMail(userAndTaskMailDetailsWithAccessToken.TaskList);
                                                    // if previous question was confirm send email of task and it was not null/wrong value then answer will send email and reply back with thank you and task mail stopped
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    // Email send 
                                                    if (email.To.Any())
                                                        _emailService.Send(email);
                                                }
                                                break;
                                            case SendEmailConfirmation.no:
                                                {
                                                    // if previous question was confirm send email of task and it was not null/wrong value then answer will say thank you and task mail stopped
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                        // if previous question was send email of task and it was null or wrong value then it will ask for send task mail confirm again
                                        userAndTaskMailDetailsWithAccessToken.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                            _stringConstant.SendTaskMailConfirmationErrorMessage,
                                            Environment.NewLine, previousQuestion.QuestionStatement);
                                }
                                break;
                            #endregion

                            #region Default
                            default:
                                userAndTaskMailDetailsWithAccessToken.QuestionText = _stringConstant.RequestToStartTaskMail;
                                break;
                                #endregion
                        }
                        _taskMailDetailRepository.Update(taskDetails);
                        await _taskMailDetailRepository.SaveChangesAsync();
                    }
                }
                else
                    userAndTaskMailDetailsWithAccessToken.QuestionText = _stringConstant.RequestToStartTaskMail;
            }
            return userAndTaskMailDetailsWithAccessToken.QuestionText;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Private method to get user's details, user's accesstoken, user's task mail details and list or else appropriate message will be send
        /// </summary>
        /// <param name="slackUserId">User's SlackId</param>
        /// <returns></returns>
        private async Task<UserAndTaskMailDetailsWithAccessToken> GetUserAndTaskMailDetailsAsync(string slackUserId)
        {
            UserAndTaskMailDetailsWithAccessToken userAndTaskMailDetailsWithAccessToken = new UserAndTaskMailDetailsWithAccessToken();
            var user = await _userRepository.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
            if (user != null)
            {
                _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, User : " + user.Email);
                // getting access token for that user
                _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, request for access token from oauth");
                userAndTaskMailDetailsWithAccessToken.AccessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, Accesstoken : " + userAndTaskMailDetailsWithAccessToken.AccessToken);
                // getting user information from Promact Oauth Server
                _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, requested for user details from oauth");
                userAndTaskMailDetailsWithAccessToken.User = await _oauthCallsRepository.GetUserByUserIdAsync(user.Id, userAndTaskMailDetailsWithAccessToken.AccessToken);
                if (userAndTaskMailDetailsWithAccessToken.User.Id != null)
                {
                    _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, receive user : " + userAndTaskMailDetailsWithAccessToken.User.Email);
                    // checking for previous task mail exist or not for today
                    userAndTaskMailDetailsWithAccessToken.TaskMail = await _taskMailRepository.FirstOrDefaultAsync(x => x.EmployeeId == userAndTaskMailDetailsWithAccessToken.User.Id &&
                    x.CreatedOn.Day == DateTime.UtcNow.Day && x.CreatedOn.Month == DateTime.UtcNow.Month &&
                    x.CreatedOn.Year == DateTime.UtcNow.Year);
                    if (userAndTaskMailDetailsWithAccessToken.TaskMail != null)
                    {
                        _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, Task mail details for today : " + userAndTaskMailDetailsWithAccessToken.TaskMail.CreatedOn);
                        // getting task mail details for started task mail
                        userAndTaskMailDetailsWithAccessToken.TaskList = await _taskMailDetailRepository.FetchAsync(x => x.TaskId == userAndTaskMailDetailsWithAccessToken.TaskMail.Id);
                        _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, Task mail list have count : " + userAndTaskMailDetailsWithAccessToken.TaskList.Count());
                    }
                    else
                    {
                        _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, Task mail not started for today");
                        // if task mail doesnot exist then ask user to start task mail
                        userAndTaskMailDetailsWithAccessToken.QuestionText = _stringConstant.RequestToStartTaskMail;
                    }
                }
                else
                    // if user doesn't exist in oAuth server
                    userAndTaskMailDetailsWithAccessToken.QuestionText = _stringConstant.YouAreNotInExistInOAuthServer;
            }
            else
                // if user doesn't exist in ERP server
                userAndTaskMailDetailsWithAccessToken.QuestionText = _stringConstant.YouAreNotInExistInOAuthServer;
            return userAndTaskMailDetailsWithAccessToken;
        }

        /// <summary>
        /// Method to check whether question text is similar to request to start task mail
        /// </summary>
        /// <param name="questionText">task mail question text</param>
        /// <returns>true or false</returns>
        private bool CheckQuestionTextIsRequestToStartMailOrNot(string questionText)
        {
            bool textIsTaskMailOrNot = string.Equals(questionText, _stringConstant.RequestToStartTaskMail);
            return textIsTaskMailOrNot;
        }

        /// <summary>
        /// Method to add task mail
        /// </summary>
        /// <param name="employeeId">User's Id</param>
        /// <returns>task mail details</returns>
        private async Task<TaskMail> AddTaskMailAsync(string employeeId)
        {
            _logger.Info("Task mail module add task mail start");
            TaskMail taskMail = new TaskMail();
            taskMail.CreatedOn = DateTime.UtcNow;
            taskMail.EmployeeId = employeeId;
            _taskMailRepository.Insert(taskMail);
            await _taskMailRepository.SaveChangesAsync();
            _logger.Info("Task mail module add task mail end");
            return taskMail;
        }

        /// <summary>
        /// Add task mail details 
        /// </summary>
        /// <param name="taskMailId">task mail Id</param>
        /// <returns>first question statement</returns>
        private async Task<string> AddTaskMailDetailAndGetQuestionStatementAsync(int taskMailId)
        {
            _logger.Info("Task mail module add task mail details start");
            TaskMailDetails taskMailDetails = new TaskMailDetails();
            // getting first question of task mail type
            var firstQuestion = await _botQuestionRepository.FindFirstQuestionByTypeAsync(BotQuestionType.TaskMail);
            taskMailDetails.TaskId = taskMailId;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailRepository.Insert(taskMailDetails);
            await _taskMailDetailRepository.SaveChangesAsync();
            _logger.Info("Task mail module add task mail details end");
            return firstQuestion.QuestionStatement;
        }

        /// <summary>
        /// Method to get the next question of task mail by previous question order
        /// </summary>
        /// <param name="previousQuestionOrder">previous question order</param>
        /// <returns>question</returns>
        private async Task<Question> NextQuestionForTaskMailAsync(QuestionOrder previousQuestionOrder)
        {
            _logger.Info("Task mail module NextQuestionForTaskMailAsync start");
            var orderValue = (int)previousQuestionOrder;
            var typeValue = (int)BotQuestionType.TaskMail;
            orderValue++;
            // getting question by order number and question type as task mail
            var nextQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(orderValue, typeValue);
            _logger.Info("Task mail module NextQuestionForTaskMailAsync end");
            return nextQuestion;
        }
        #endregion
    }
}