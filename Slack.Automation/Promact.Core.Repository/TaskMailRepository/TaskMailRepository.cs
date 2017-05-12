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
using Promact.Core.Repository.EmailServiceTemplateRepository;
using Autofac.Extras.NLog;
using Promact.Core.Repository.MailSettingDetailsByProjectAndModule;
using System.Collections.Generic;
using Promact.Erp.Util.StringLiteral;
using AutoMapper;

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
        private readonly AppStringLiteral _stringConstant;
        private readonly IEmailServiceTemplateRepository _emailServiceTemplate;
        private readonly ILogger _logger;
        private readonly IRepository<MailSetting> _mailSettingDataRepository;
        private readonly IMailSettingDetailsByProjectAndModuleRepository _mailSettingDetails;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public TaskMailRepository(IRepository<TaskMail> taskMailRepository, ISingletonStringLiteral stringConstant,
            IOauthCallsRepository oauthCallsRepository, IRepository<TaskMailDetails> taskMailDetailRepository,
            IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> userRepository, IEmailService emailService,
            IBotQuestionRepository botQuestionRepository, ApplicationUserManager userManager,
            IEmailServiceTemplateRepository emailServiceTemplate, ILogger logger, IRepository<MailSetting> mailSettingDataRepository,
            IMailSettingDetailsByProjectAndModuleRepository mailSettingDetails, IMapper mapper)
        {
            _taskMailRepository = taskMailRepository;
            _stringConstant = stringConstant.StringConstant;
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
            _mapper = mapper;
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
            // method to get user's details, user's task mail details and list or else appropriate message will be send
            var userAndTaskMailDetails = await GetUserAndTaskMailDetailsAsync(userId);
            _logger.Info("Task Mail Bot Module, Is task mail question text : " + userAndTaskMailDetails.QuestionText);
            bool questionTextIsNull = string.IsNullOrEmpty(userAndTaskMailDetails.QuestionText);
            // if question text is null or not request to start task mail then only allowed
            if (questionTextIsNull || CheckQuestionTextIsRequestToStartMailOrNot(userAndTaskMailDetails.QuestionText))
            {
                _logger.Info("Task Mail Bot Module, task mail process start - StartTaskMailAsync");
                TaskMailDetails taskMailDetail = new TaskMailDetails();
                #region Task Mail Details
                if (userAndTaskMailDetails.TaskList != null)
                {
                    taskMailDetail = userAndTaskMailDetails.TaskList.Last();
                    var previousQuestion = await _botQuestionRepository.FindByIdAsync(taskMailDetail.QuestionId);
                    var questionOrder = previousQuestion.OrderNumber;
                    // If previous task mail is on the process and user started new task mail then will need to first complete pervious one
                    if (questionOrder < QuestionOrder.TaskMailSend)
                    {
                        userAndTaskMailDetails.QuestionText = await QuestionAndAnswerAsync(null, userId);
                        userAndTaskMailDetails.IsTaskMailContinue = true;
                    }
                }
                else
                {
                    // If task mail is not started for that day
                    userAndTaskMailDetails.TaskMail = await AddTaskMailAsync(userAndTaskMailDetails.User.Id);
                }
                #endregion

                #region If Task mail not send for that day
                // if question text is null or not request to start task mail then only allowed
                if (!userAndTaskMailDetails.IsTaskMailContinue && (questionTextIsNull || CheckQuestionTextIsRequestToStartMailOrNot(userAndTaskMailDetails.QuestionText)))
                {
                    SendEmailConfirmation confirmation = taskMailDetail.SendEmailConfirmation;
                    switch (confirmation)
                    {
                        case SendEmailConfirmation.yes:
                            {
                                // If mail is send then user will not be able to add task mail for that day
                                userAndTaskMailDetails.QuestionText = _stringConstant.AlreadyMailSend;
                            }
                            break;
                        case SendEmailConfirmation.no:
                            {
                                // If mail is not send then user will be able to add task mail for that day
                                userAndTaskMailDetails.QuestionText = await AddTaskMailDetailAndGetQuestionStatementAsync(userAndTaskMailDetails.TaskMail.Id);
                            }
                            break;
                    }
                }
                #endregion
            }
            return userAndTaskMailDetails.QuestionText;
        }

        /// <summary>
        /// Method to conduct task mail after been started, and send next question of task mail
        /// </summary>
        /// <param name="answer">Answer of previous question ask to user</param>
        /// <param name="userId">User's slack Id</param>
        /// <returns>questionText in string format containing next question statement</returns>
        public async Task<string> QuestionAndAnswerAsync(string answer, string userId)
        {
            // method to get user's details, user's task mail details and list or else appropriate message will be send
            var userAndTaskMailDetails = await GetUserAndTaskMailDetailsAsync(userId);
            _logger.Info("Task Mail Bot Module, Is task mail question text : " + userAndTaskMailDetails.QuestionText);
            // if question text is null then only allowed
            if (string.IsNullOrEmpty(userAndTaskMailDetails.QuestionText))
            {
                _logger.Info("Task Mail Bot Module, task mail process start - QuestionAndAnswerAsync");
                _logger.Info("Task Mail Bot Module, task mail list count : " + userAndTaskMailDetails.TaskList.Count());
                if (userAndTaskMailDetails.TaskList != null)
                {
                    var taskDetails = userAndTaskMailDetails.TaskList.Last();
                    // getting inform of previous question was asked to user
                    var previousQuestion = await _botQuestionRepository.FindByIdAsync(taskDetails.QuestionId);
                    // checking if previous question was last and answered by user and previous task report was completed then asked for new task mail
                    if (previousQuestion.OrderNumber <= QuestionOrder.TaskMailSend || previousQuestion.OrderNumber == QuestionOrder.RestartTask)
                    {
                        // getting next question to be asked to user
                        var nextQuestion = await NextQuestionForTaskMailAsync(previousQuestion.OrderNumber);
                        _logger.Info("Previous question ordernumber : " + previousQuestion.OrderNumber);
                        userAndTaskMailDetails.QuestionText = await ProcessingTaskMailByOrderNumberAsync(previousQuestion, nextQuestion,
                            answer, userAndTaskMailDetails, taskDetails);
                    }
                }
                else
                    userAndTaskMailDetails.QuestionText = _stringConstant.RequestToStartTaskMail;
            }
            return userAndTaskMailDetails.QuestionText;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Private method to get user's details, user's task mail details and list or else appropriate message will be send
        /// </summary>
        /// <param name="slackUserId">User's SlackId</param>
        /// <returns></returns>
        private async Task<UserAndTaskMailDetailsAC> GetUserAndTaskMailDetailsAsync(string slackUserId)
        {
            UserAndTaskMailDetailsAC userAndTaskMailDetails = new UserAndTaskMailDetailsAC();
            var user = await _userRepository.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
            if (user != null)
            {
                _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, User : " + user.Email);
                if (_taskMailRepository.Any(x => x.EmployeeId == user.Id && x.CreatedOn.Day == DateTime.UtcNow.Day &&
                 x.CreatedOn.Month == DateTime.UtcNow.Month && x.CreatedOn.Year == DateTime.UtcNow.Year))
                {
                    _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, get user details from promact slack table");
                    userAndTaskMailDetails.User = _mapper.Map<ApplicationUser, User>(user);
                }
                else
                {
                    // getting user information from Promact Oauth Server
                    _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, requested for user details from oauth");
                    userAndTaskMailDetails.User = await _oauthCallsRepository.GetUserByUserIdAsync(user.Id,
                        (await _attachmentRepository.UserAccessTokenAsync(user.UserName)));
                }
                if (userAndTaskMailDetails.User.Id != null)
                {
                    _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, receive user : " + userAndTaskMailDetails.User.Email);
                    // checking for previous task mail exist or not for today
                    userAndTaskMailDetails.TaskMail = await _taskMailRepository.FirstOrDefaultAsync(x => x.EmployeeId == userAndTaskMailDetails.User.Id &&
                    x.CreatedOn.Day == DateTime.UtcNow.Day && x.CreatedOn.Month == DateTime.UtcNow.Month &&
                    x.CreatedOn.Year == DateTime.UtcNow.Year);
                    if (userAndTaskMailDetails.TaskMail != null)
                    {
                        _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, Task mail details for today : " + userAndTaskMailDetails.TaskMail.CreatedOn);
                        // getting task mail details for started task mail
                        userAndTaskMailDetails.TaskList = await _taskMailDetailRepository.FetchAsync(x => x.TaskId == userAndTaskMailDetails.TaskMail.Id);
                        _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, Task mail list have count : " + userAndTaskMailDetails.TaskList.Count());
                    }
                    else
                    {
                        _logger.Info("Task Mail Bot Module GetUserAndTaskMailDetailsAsync, Task mail not started for today");
                        // if task mail doesnot exist then ask user to start task mail
                        userAndTaskMailDetails.QuestionText = _stringConstant.RequestToStartTaskMail;
                    }
                }
                else
                    // if user doesn't exist in oAuth server
                    userAndTaskMailDetails.QuestionText = _stringConstant.YouAreNotInExistInOAuthServer;
            }
            else
                // if user doesn't exist in ERP server
                userAndTaskMailDetails.QuestionText = _stringConstant.YouAreNotInExistInOAuthServer;
            return userAndTaskMailDetails;
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

        /// <summary>
        /// Method to process task mail
        /// </summary>
        /// <param name="previousQuestion">previous question asked on task mail</param>
        /// <param name="nextQuestion">next question will be ask on task mail</param>
        /// <param name="answer">reply of user for previous question</param>
        /// <param name="userAndTaskMailDetails">user and task mail details</param>
        /// <param name="taskDetails">task mail detail</param>
        /// <returns>reply text</returns>
        private async Task<string> ProcessingTaskMailByOrderNumberAsync(Question previousQuestion, Question nextQuestion, string answer,
            UserAndTaskMailDetailsAC userAndTaskMailDetails, TaskMailDetails taskDetails)
        {
            switch (previousQuestion.OrderNumber)
            {
                #region Your Task
                case QuestionOrder.YourTask:
                    userAndTaskMailDetails.QuestionText = await TaskMailOrderYourTaskAsync(previousQuestion.QuestionStatement, nextQuestion, answer, taskDetails);
                    break;
                #endregion

                #region Hour Spent
                case QuestionOrder.HoursSpent:
                    userAndTaskMailDetails.QuestionText = await TaskMailOrderHourSpentAsync(previousQuestion.QuestionStatement, nextQuestion,
                        answer, userAndTaskMailDetails, taskDetails);
                    break;
                #endregion

                #region Status
                case QuestionOrder.Status:
                    userAndTaskMailDetails.QuestionText = await TaskMailOrderStatusAsync(previousQuestion.QuestionStatement, nextQuestion,
                        answer, taskDetails);
                    break;
                #endregion

                #region Comment
                case QuestionOrder.Comment:
                    userAndTaskMailDetails.QuestionText = await TaskMailOrderCommentAsync(previousQuestion.QuestionStatement, nextQuestion,
                        answer, taskDetails);
                    break;
                #endregion

                #region Restart Task
                case QuestionOrder.RestartTask:
                    userAndTaskMailDetails.QuestionText = await TaskMailOrderRestartTaskAsync(previousQuestion, nextQuestion, answer,
                        userAndTaskMailDetails, taskDetails);
                    break;
                #endregion

                #region Send Email
                case QuestionOrder.SendEmail:
                    userAndTaskMailDetails.QuestionText = await TaskMailOrderSendMailAsync(previousQuestion, nextQuestion, answer,
                        userAndTaskMailDetails, taskDetails);
                    break;
                #endregion

                #region Confirm Send Email
                case QuestionOrder.ConfirmSendEmail:
                    userAndTaskMailDetails.QuestionText = await TaskMailOrderSendConfirmMailAsync(previousQuestion, nextQuestion, answer,
                        userAndTaskMailDetails, taskDetails);
                    break;
                #endregion

                #region Default
                default:
                    userAndTaskMailDetails.QuestionText = _stringConstant.RequestToStartTaskMail;
                    break;
                    #endregion
            }
            return userAndTaskMailDetails.QuestionText;
        }

        /// <summary>
        /// Method to process task mail sending procedure
        /// </summary>
        /// <param name="userAndTaskMailDetails">user and task mail details</param>
        private async Task SendMailForTaskMailAsync(UserAndTaskMailDetailsAC userAndTaskMailDetails)
        {
            EmailApplication email = new EmailApplication();
            email.To = new List<string>();
            email.CC = new List<string>();
            var listOfprojectRelatedToUser = (await _oauthCallsRepository.
                GetListOfProjectsEnrollmentOfUserByUserIdAsync(userAndTaskMailDetails.User.Id,
                (await _attachmentRepository.UserAccessTokenAsync(userAndTaskMailDetails.User.UserName))))
                .Select(x => x.Id).ToList();
            foreach (var projectId in listOfprojectRelatedToUser)
            {
                var mailsetting = await _mailSettingDetails.GetMailSettingAsync(projectId, _stringConstant.TaskModule, userAndTaskMailDetails.User.Id);
                email.To.AddRange(mailsetting.To);
                email.CC.AddRange(mailsetting.CC);
            }
            email.To = email.To.Distinct().ToList();
            email.CC = email.CC.Distinct().ToList();
            email.From = userAndTaskMailDetails.User.Email;
            email.Subject = _stringConstant.TaskMailSubject;
            // transforming task mail details to template page and getting as string
            email.Body = _emailServiceTemplate.EmailServiceTemplateTaskMail(userAndTaskMailDetails.TaskList);
            // Email send 
            if (email.To.Any())
                _emailService.Send(email);
        }

        /// <summary>
        /// Method to update task mail details
        /// </summary>
        /// <param name="taskMailDetails">task mail details</param>
        private async Task UpdateTaskMailAsync(TaskMailDetails taskMailDetails)
        {
            _taskMailDetailRepository.Update(taskMailDetails);
            await _taskMailDetailRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Method to process Your Task order of task mail
        /// </summary>
        /// <param name="previousQuestionStatement">previous question statement</param>
        /// <param name="nextQuestion">next question details</param>
        /// <param name="answer">reply from user for previous question</param>
        /// <param name="taskDetails">task mail details</param>
        /// <returns>reply text</returns>
        private async Task<string> TaskMailOrderYourTaskAsync(string previousQuestionStatement, Question nextQuestion, string answer,
            TaskMailDetails taskDetails)
        {
            string replyText = string.Empty;
            // if previous question was description of task and it was not null/wrong vale then answer will ask next question
            if (answer != null)
            {
                taskDetails.Description = answer;
                replyText = nextQuestion.QuestionStatement;
                taskDetails.QuestionId = nextQuestion.Id;
            }
            else
            {
                // if previous question was description of task and it was null then answer will ask for description again
                replyText = previousQuestionStatement;
            }
            await UpdateTaskMailAsync(taskDetails);
            return replyText;
        }

        /// <summary>
        /// Method to process Hour Spent order of task mail
        /// </summary>
        /// <param name="previousQuestionStatement">previous question statement</param>
        /// <param name="nextQuestion">next question will be ask on task mail</param>
        /// <param name="answer">reply of user for previous question</param>
        /// <param name="userAndTaskMailDetails">user and task mail details</param>
        /// <param name="taskDetails">task mail detail</param>
        /// <returns>reply text</returns>
        private async Task<string> TaskMailOrderHourSpentAsync(string previousQuestionStatement, Question nextQuestion, string answer,
            UserAndTaskMailDetailsAC userAndTaskMailDetails, TaskMailDetails taskDetails)
        {
            // if previous question was hour of task and it was not null/wrong value then answer will ask next question
            decimal hour;
            // checking whether string can be convert to decimal or not
            if (decimal.TryParse(answer, out hour))
            {
                decimal totalHourSpented = 0;
                decimal taskMailMaximumTime = Convert.ToDecimal(_stringConstant.TaskMailMaximumTime);
                // checking range of hours
                if (hour > 0 && hour <= taskMailMaximumTime)
                {
                    // adding up all hours of task mail
                    foreach (var task in userAndTaskMailDetails.TaskList)
                    {
                        totalHourSpented += task.Hours;
                    }
                    totalHourSpented += hour;
                    // checking whether all up hour doesnot exceed task mail hour limit
                    if (totalHourSpented <= taskMailMaximumTime)
                    {
                        taskDetails.Hours = hour;
                        userAndTaskMailDetails.QuestionText = nextQuestion.QuestionStatement;
                        taskDetails.QuestionId = nextQuestion.Id;
                    }
                    else
                    {
                        // getting last question of task mail
                        nextQuestion = await NextQuestionForTaskMailAsync(QuestionOrder.SendEmail);
                        taskDetails.QuestionId = nextQuestion.Id;
                        taskDetails.Hours = (taskMailMaximumTime - (totalHourSpented - hour));
                        taskDetails.Comment = _stringConstant.StartWorking;
                        userAndTaskMailDetails.QuestionText = string.Format(_stringConstant.HourLimitExceed, taskMailMaximumTime);
                        userAndTaskMailDetails.QuestionText += Environment.NewLine;
                        userAndTaskMailDetails.QuestionText += string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                _attachmentRepository.GetTaskMailInStringFormat(userAndTaskMailDetails.TaskList), Environment.NewLine, Environment.NewLine);
                        userAndTaskMailDetails.QuestionText += nextQuestion.QuestionStatement;
                    }
                }
                else
                    // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                    userAndTaskMailDetails.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                        _stringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, previousQuestionStatement);
            }
            else
                // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                userAndTaskMailDetails.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                    _stringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, previousQuestionStatement);
            await UpdateTaskMailAsync(taskDetails);
            return userAndTaskMailDetails.QuestionText;
        }

        /// <summary>
        /// Method to process Status order of task mail
        /// </summary>
        /// <param name="previousQuestionStatement">previous question statement</param>
        /// <param name="nextQuestion">next question will be ask on task mail</param>
        /// <param name="answer">reply of user for previous question</param>
        /// <param name="taskDetails">task mail detail</param>
        /// <returns>reply text</returns>
        private async Task<string> TaskMailOrderStatusAsync(string previousQuestionStatement, Question nextQuestion, string answer,
            TaskMailDetails taskDetails)
        {
            string replyText = string.Empty;
            TaskMailStatus status;
            // checking whether string can be convert to TaskMailStatus type or not
            var taskMailStatusConvertResult = Enum.TryParse(answer, out status);
            if (taskMailStatusConvertResult)
            {
                // if previous question was status of task and it was not null/wrong value then answer will ask next question
                taskDetails.Status = status;
                replyText = nextQuestion.QuestionStatement;
                taskDetails.QuestionId = nextQuestion.Id;
            }
            else
                // if previous question was status of task and it was null or wrong value then answer will ask for status again
                replyText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                    _stringConstant.TaskMailBotStatusErrorMessage,
                    Environment.NewLine, previousQuestionStatement);
            await UpdateTaskMailAsync(taskDetails);
            return replyText;
        }

        /// <summary>
        /// Method to process Comment order of task mail
        /// </summary>
        /// <param name="previousQuestionStatement">previous question statement</param>
        /// <param name="nextQuestion">next question will be ask on task mail</param>
        /// <param name="answer">reply of user for previous question</param>
        /// <param name="taskDetails">task mail detail</param>
        /// <returns>reply text</returns>
        private async Task<string> TaskMailOrderCommentAsync(string previousQuestionStatement, Question nextQuestion, string answer,
            TaskMailDetails taskDetails)
        {
            string replyText = string.Empty;
            if (answer != null)
            {
                // if previous question was comment of task and answer was not null/wrong value then answer will ask next question
                taskDetails.Comment = answer;
                nextQuestion = await NextQuestionForTaskMailAsync(QuestionOrder.RoadBlock);
                replyText = nextQuestion.QuestionStatement;
                taskDetails.QuestionId = nextQuestion.Id;
            }
            else
            {
                // if previous question was comment of task and answer was null or wrong value then answer will ask for comment again
                replyText = previousQuestionStatement;
            }
            await UpdateTaskMailAsync(taskDetails);
            return replyText;
        }

        /// <summary>
        /// Method to process Restart task order of task mail
        /// </summary>
        /// <param name="previousQuestion">previous question asked on task mail</param>
        /// <param name="nextQuestion">next question will be ask on task mail</param>
        /// <param name="answer">reply of user for previous question</param>
        /// <param name="userAndTaskMailDetails">user and task mail details</param>
        /// <param name="taskDetails">task mail detail</param>
        /// <returns>reply text</returns>
        private async Task<string> TaskMailOrderRestartTaskAsync(Question previousQuestion, Question nextQuestion, string answer,
            UserAndTaskMailDetailsAC userAndTaskMailDetails, TaskMailDetails taskDetails)
        {
            SendEmailConfirmation restartTaskConfirmation;
            if (Enum.TryParse(answer, out restartTaskConfirmation))
            {
                switch (restartTaskConfirmation)
                {
                    case SendEmailConfirmation.yes:
                        {
                            // if previous question was send email of task and answer was no then answer will say thank you and task mail stopped
                            nextQuestion = await NextQuestionForTaskMailAsync(QuestionOrder.ConfirmSendEmail);
                            taskDetails.QuestionId = nextQuestion.Id;
                            _taskMailDetailRepository.Update(taskDetails);
                            await _taskMailDetailRepository.SaveChangesAsync();
                            userAndTaskMailDetails.QuestionText = await StartTaskMailAsync(userAndTaskMailDetails.User.SlackUserId);
                        }
                        break;
                    case SendEmailConfirmation.no:
                        {
                            nextQuestion = await NextQuestionForTaskMailAsync(QuestionOrder.Comment);
                            taskDetails.QuestionId = nextQuestion.Id;
                            userAndTaskMailDetails.QuestionText = nextQuestion.QuestionStatement;
                            userAndTaskMailDetails.QuestionText += string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                Environment.NewLine, _stringConstant.TaskMailRestartSuggestionMessage, _stringConstant.RequestToStartTaskMail.ToLower());
                        }
                        break;
                }
            }
            else
                userAndTaskMailDetails.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                    _stringConstant.SendTaskMailConfirmationErrorMessage,
                    Environment.NewLine, previousQuestion.QuestionStatement);
            await UpdateTaskMailAsync(taskDetails);
            return userAndTaskMailDetails.QuestionText;
        }

        /// <summary>
        /// Method to process Send mail order of task mail
        /// </summary>
        /// <param name="previousQuestion">previous question asked on task mail</param>
        /// <param name="nextQuestion">next question will be ask on task mail</param>
        /// <param name="answer">reply of user for previous question</param>
        /// <param name="userAndTaskMailDetails">user and task mail details</param>
        /// <param name="taskDetails">task mail detail</param>
        /// <returns>reply text</returns>
        private async Task<string> TaskMailOrderSendMailAsync(Question previousQuestion, Question nextQuestion, string answer,
            UserAndTaskMailDetailsAC userAndTaskMailDetails, TaskMailDetails taskDetails)
        {
            SendEmailConfirmation confirmation;
            // checking whether string can be convert to TaskMailStatus type or not
            if (Enum.TryParse(answer, out confirmation))
            {
                taskDetails.SendEmailConfirmation = confirmation;
                switch (confirmation)
                {
                    case SendEmailConfirmation.yes:
                        {
                            // if previous question was send email of task and answer was yes then answer will ask next question
                            taskDetails.QuestionId = nextQuestion.Id;
                            userAndTaskMailDetails.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                _attachmentRepository.GetTaskMailInStringFormat(userAndTaskMailDetails.TaskList), Environment.NewLine, Environment.NewLine);
                            userAndTaskMailDetails.QuestionText += nextQuestion.QuestionStatement;
                        }
                        break;
                    case SendEmailConfirmation.no:
                        {
                            // if previous question was send email of task and answer was no then answer will say thank you and task mail stopped
                            nextQuestion = await NextQuestionForTaskMailAsync(QuestionOrder.ConfirmSendEmail);
                            taskDetails.QuestionId = nextQuestion.Id;
                            userAndTaskMailDetails.QuestionText = _stringConstant.ThankYou;
                        }
                        break;
                }
            }
            else
                // if previous question was send email of task and answer was null/wrong value then answer will say thank you and task mail stopped
                userAndTaskMailDetails.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                    _stringConstant.SendTaskMailConfirmationErrorMessage,
                    Environment.NewLine, previousQuestion.QuestionStatement);
            await UpdateTaskMailAsync(taskDetails);
            return userAndTaskMailDetails.QuestionText;
        }

        /// <summary>
        /// Method to process Send confirm mail order of task mail
        /// </summary>
        /// <param name="previousQuestion">previous question asked on task mail</param>
        /// <param name="nextQuestion">next question will be ask on task mail</param>
        /// <param name="answer">reply of user for previous question</param>
        /// <param name="userAndTaskMailDetails">user and task mail details</param>
        /// <param name="taskDetails">task mail detail</param>
        /// <returns>reply text</returns>
        private async Task<string> TaskMailOrderSendConfirmMailAsync(Question previousQuestion, Question nextQuestion, string answer,
            UserAndTaskMailDetailsAC userAndTaskMailDetails, TaskMailDetails taskDetails)
        {
            SendEmailConfirmation confirmation;
            // checking whether string can be convert to TaskMailStatus type or not
            var sendEmailConfirmationConvertResult = Enum.TryParse(answer, out confirmation);
            if (sendEmailConfirmationConvertResult)
            {
                taskDetails.SendEmailConfirmation = confirmation;
                userAndTaskMailDetails.QuestionText = _stringConstant.ThankYou;
                switch (confirmation)
                {
                    case SendEmailConfirmation.yes:
                        {
                            await SendMailForTaskMailAsync(userAndTaskMailDetails);
                            // if previous question was confirm send email of task and it was not null/wrong value then answer will send email and reply back with thank you and task mail stopped
                            taskDetails.QuestionId = nextQuestion.Id;

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
                userAndTaskMailDetails.QuestionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                    _stringConstant.SendTaskMailConfirmationErrorMessage,
                    Environment.NewLine, previousQuestion.QuestionStatement);
            await UpdateTaskMailAsync(taskDetails);
            return userAndTaskMailDetails.QuestionText;
        } 
        #endregion 
    }
}