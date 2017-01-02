using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.Util.Email;
using System.Data.Entity;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Erp.DomainModel.DataRepository;
using System.Net.Mail;
using Promact.Erp.Util.StringConstants;
using Promact.Core.Repository.EmailServiceTemplateRepository;
using Autofac.Extras.NLog;

namespace Promact.Core.Repository.TaskMailRepository
{
    public class TaskMailRepository : ITaskMailRepository
    {
        #region Private Variables
        private readonly IRepository<TaskMail> _taskMail;
        private readonly IRepository<TaskMailDetails> _taskMailDetail;
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _user;
        private readonly IEmailService _emailService;
        private readonly ApplicationUserManager _userManager;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEmailServiceTemplateRepository _emailServiceTemplate;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public TaskMailRepository(IRepository<TaskMail> taskMail, IStringConstantRepository stringConstant, 
            IOauthCallsRepository oauthCallsRepository, IRepository<TaskMailDetails> taskMailDetail, 
            IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> user, IEmailService emailService, 
            IBotQuestionRepository botQuestionRepository, ApplicationUserManager userManager, 
            IEmailServiceTemplateRepository emailServiceTemplate, ILogger logger)
        {
            _taskMail = taskMail;
            _stringConstant = stringConstant;
            _oauthCallsRepository = oauthCallsRepository;
            _taskMailDetail = taskMailDetail;
            _attachmentRepository = attachmentRepository;
            _user = user;
            _emailService = emailService;
            _botQuestionRepository = botQuestionRepository;
            _userManager = userManager;
            _emailServiceTemplate = emailServiceTemplate;
            _logger = logger;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to start task mail
        /// </summary>
        /// <param name="userId">User's slack user Id</param>
        /// <returns>questionText in string format containing question statement</returns>
        public async Task<string> StartTaskMailAsync(string userId)
        {
            string questionText = _stringConstant.EmptyString;
            // getting user name from user's slack name
            var user = await _user.FirstOrDefaultAsync(x => x.SlackUserId == userId);
            // getting access token for that user
            if (user != null)
            {
                // get access token of user for promact oauth server
                var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                // get user details from
                var oAuthUser = await _oauthCallsRepository.GetUserByUserIdAsync(userId, accessToken);
                QuestionOrder question = new QuestionOrder();
                Question previousQuestion = new Question();
                TaskMailDetails taskMailDetail = new TaskMailDetails();
                // getting user information from Promact Oauth Server
                TaskMail taskMail = null;
                // checking for previous task mail exist or not for today
                taskMail = await _taskMail.FirstOrDefaultAsync(x => x.EmployeeId == oAuthUser.Id &&
                x.CreatedOn.Day == DateTime.UtcNow.Day && x.CreatedOn.Month == DateTime.UtcNow.Month &&
                x.CreatedOn.Year == DateTime.UtcNow.Year);
                if (taskMail != null)
                {
                    // if exist then check the whether the task mail was completed or not
                    IEnumerable<TaskMailDetails> taskMailDetailsList = await _taskMailDetail.FetchAsync(x => x.TaskId == taskMail.Id);
                    if (taskMailDetailsList.Any())
                        taskMailDetail = taskMailDetailsList.Last();
                    previousQuestion = await _botQuestionRepository.FindByIdAsync(taskMailDetail.QuestionId);
                    question = previousQuestion.OrderNumber;
                }
                // if previous task mail completed then it will start a new one
                if (taskMail == null || question == QuestionOrder.TaskMailSend)
                {
                    if (taskMail == null)
                    {
                        // If mail is not send then user will be able to add task mail for that day
                        taskMail = new TaskMail();
                        taskMail.CreatedOn = DateTime.UtcNow;
                        taskMail.EmployeeId = oAuthUser.Id;
                        _taskMail.Insert(taskMail);
                        _taskMail.Save();
                    }
                    // Before going to create new task it will check whether the user has send mail for today or not.
                    if (taskMailDetail != null && taskMailDetail.SendEmailConfirmation == SendEmailConfirmation.yes)
                        // If mail is send then user will not be able to add task mail for that day
                        questionText = _stringConstant.AlreadyMailSend;
                    else
                    {
                        // getting first question of type 2
                        var firstQuestion = await _botQuestionRepository.FindFirstQuestionByTypeAsync(BotQuestionType.TaskMail);
                        TaskMailDetails taskDetails = new TaskMailDetails();
                        taskDetails.QuestionId = firstQuestion.Id;
                        taskDetails.TaskId = taskMail.Id;
                        questionText = firstQuestion.QuestionStatement;
                        _taskMailDetail.Insert(taskDetails);
                        _taskMailDetail.Save();
                    }
                }
                else
                {
                    // if previous task mail is not completed then it will go for pervious task mail and ask user to complete it
                    questionText = await QuestionAndAnswerAsync(null, userId);
                }
            }
            else
                // if user doesn't exist then this message will be shown to user
                questionText = _stringConstant.YouAreNotInExistInOAuthServer;
            return questionText;
        }

        /// <summary>
        /// Method to conduct task mail after started
        /// </summary>
        /// <param name="answer">User's slack user Id</param>
        /// <returns>questionText in string format containing question statement</returns>
        public async Task<string> QuestionAndAnswerAsync(string answer, string userId)
        {
            string questionText = _stringConstant.EmptyString;
            try
            {
                // getting user name from user's slack name
                var user = await _user.FirstOrDefaultAsync(x => x.SlackUserId == userId);
                if (user != null)
                {
                    // getting access token for that user
                    var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                    IEnumerable<TaskMailDetails> taskList;
                    TaskMail taskMail = new TaskMail();
                    // getting user information from Promact Oauth Server
                    var oAuthUser = await _oauthCallsRepository.GetUserByUserIdAsync(userId, accessToken);
                    // checking for previous task mail exist or not for today
                    taskMail = await _taskMail.FirstOrDefaultAsync(x => x.EmployeeId == oAuthUser.Id &&
                    x.CreatedOn.Day == DateTime.UtcNow.Day && x.CreatedOn.Month == DateTime.UtcNow.Month &&
                    x.CreatedOn.Year == DateTime.UtcNow.Year);
                    if (taskMail != null)
                    {
                        // getting task mail details for pervious started task mail
                        var taskDetailsList = await _taskMailDetail.FetchAsync(x => x.TaskId == taskMail.Id);
                        if (taskDetailsList.Any())
                        {
                            var taskDetails = taskDetailsList.Last();
                            // getting inform of previous question asked to user
                            var previousQuestion = await _botQuestionRepository.FindByIdAsync(taskDetails.QuestionId);
                            // checking if precious question was last and answer by user and previous task report was completed then asked for new task mail
                            if ((int)previousQuestion.OrderNumber <= (int)QuestionOrder.TaskMailSend)
                            {
                                // getting next question to be asked to user
                                var orderValue = (int)previousQuestion.OrderNumber;
                                var typeValue = (int)BotQuestionType.TaskMail;
                                orderValue++;
                                var nextQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(orderValue, 
                                    typeValue);
                                // Converting question Ordr to enum
                                var question = (QuestionOrder)Enum.Parse(typeof(QuestionOrder),
                                    previousQuestion.OrderNumber.ToString());
                                switch (question)
                                {
                                    #region Your Task
                                    case QuestionOrder.YourTask:
                                        {
                                            // if previous question was description of task and it was not null/wrong vale then answer will ask next question
                                            if (answer != null)
                                            {
                                                taskDetails.Description = answer.ToLower();
                                                questionText = nextQuestion.QuestionStatement;
                                                taskDetails.QuestionId = nextQuestion.Id;
                                            }
                                            else
                                            {
                                                // if previous question was description of task and it was null then answer will ask for description again
                                                questionText = previousQuestion.QuestionStatement;
                                            }
                                        }
                                        break;
                                    #endregion

                                    #region Hour Spent
                                    case QuestionOrder.HoursSpent:
                                        {
                                            double answerType;
                                            // checking whether string can be convert to double type or not
                                            var answerConvertResult = double.TryParse(answer, out answerType);
                                            if (answerConvertResult)
                                            {
                                                // if previous question was hour of task and it was not null/wrong value then answer will ask next question
                                                var hour = Convert.ToDecimal(answer);
                                                decimal totalHourSpented = 0;
                                                // checking range of hours
                                                if (answerType > 0 && answerType <= 8)
                                                {
                                                    foreach (var task in taskDetailsList)
                                                    {
                                                        totalHourSpented += task.Hours;
                                                    }
                                                    totalHourSpented += hour;
                                                    if (totalHourSpented <= 8)
                                                    {
                                                        taskDetails.Hours = hour;
                                                        questionText = nextQuestion.QuestionStatement;
                                                        taskDetails.QuestionId = nextQuestion.Id;
                                                    }
                                                    else
                                                    {
                                                        orderValue = (int)QuestionOrder.ConfirmSendEmail;
                                                        nextQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(orderValue, typeValue);
                                                        taskDetails.QuestionId = nextQuestion.Id;
                                                        questionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                                            _stringConstant.HourLimitExceed, Environment.NewLine,
                                                            nextQuestion.QuestionStatement);
                                                        taskDetails.Comment = _stringConstant.StartWorking;
                                                    }
                                                }
                                                else
                                                    // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                                                    questionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                                        _stringConstant.TaskMailBotHourErrorMessage,
                                                        Environment.NewLine, previousQuestion.QuestionStatement);
                                            }
                                            else
                                                // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                                                questionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                                    _stringConstant.TaskMailBotHourErrorMessage, Environment.NewLine,
                                                    previousQuestion.QuestionStatement);
                                        }
                                        break;
                                    #endregion

                                    #region Status
                                    case QuestionOrder.Status:
                                        {
                                            TaskMailStatus taskMailStatusType;
                                            // checking whether string can be convert to TaskMailStatus type or not
                                            var taskMailStatusConvertResult = Enum.TryParse(answer, out taskMailStatusType);
                                            if (taskMailStatusConvertResult)
                                            {
                                                // if previous question was status of task and it was not null/wrong value then answer will ask next question
                                                var status = (TaskMailStatus)Enum.Parse(typeof(TaskMailStatus), answer.ToLower());
                                                taskDetails.Status = status;
                                                questionText = nextQuestion.QuestionStatement;
                                                taskDetails.QuestionId = nextQuestion.Id;
                                            }
                                            else
                                                // if previous question was status of task and it was null or wrong value then answer will ask for status again
                                                questionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
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
                                                taskDetails.Comment = answer.ToLower();
                                                questionText = nextQuestion.QuestionStatement;
                                                taskDetails.QuestionId = nextQuestion.Id;
                                            }
                                            else
                                            {
                                                // if previous question was comment of task and answer was null or wrong value then answer will ask for comment again
                                                questionText = previousQuestion.QuestionStatement;
                                            }
                                        }
                                        break;
                                    #endregion

                                    #region Send Email
                                    case QuestionOrder.SendEmail:
                                        {
                                            SendEmailConfirmation sendEmailConfirmation;
                                            // checking whether string can be convert to TaskMailStatus type or not
                                            var sendEmailConfirmationConvertResult = Enum.TryParse(answer, out sendEmailConfirmation);
                                            if (sendEmailConfirmationConvertResult)
                                            {
                                                // convert answer to SendEmailConfirmation type
                                                var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation),
                                                    answer.ToLower());
                                                switch (confirmation)
                                                {
                                                    case SendEmailConfirmation.yes:
                                                        {
                                                            // if previous question was send email of task and answer was yes then answer will ask next question
                                                            taskDetails.SendEmailConfirmation = SendEmailConfirmation.yes;
                                                            taskDetails.QuestionId = nextQuestion.Id;
                                                            questionText = nextQuestion.QuestionStatement;
                                                        }
                                                        break;
                                                    case SendEmailConfirmation.no:
                                                        {
                                                            // if previous question was send email of task and answer was no then answer will say thank you and task mail stopped
                                                            taskDetails.SendEmailConfirmation = SendEmailConfirmation.no;
                                                            orderValue = (int)QuestionOrder.TaskMailSend;
                                                            nextQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(orderValue, typeValue);
                                                            taskDetails.QuestionId = nextQuestion.Id;
                                                            questionText = _stringConstant.ThankYou;
                                                        }
                                                        break;
                                                }
                                            }
                                            else
                                                // if previous question was send email of task and answer was null/wrong value then answer will say thank you and task mail stopped
                                                questionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                                    _stringConstant.SendTaskMailConfirmationErrorMessage,
                                                    Environment.NewLine, previousQuestion.QuestionStatement);
                                        }
                                        break;
                                    #endregion

                                    #region Confirm Send Email
                                    case QuestionOrder.ConfirmSendEmail:
                                        {
                                            SendEmailConfirmation sendEmailConfirmation;
                                            // checking whether string can be convert to TaskMailStatus type or not
                                            var sendEmailConfirmationConvertResult = Enum.TryParse(answer, out sendEmailConfirmation);
                                            if (sendEmailConfirmationConvertResult)
                                            {
                                                // convert answer to SendEmailConfirmation type
                                                var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation),
                                                    answer.ToLower());
                                                questionText = _stringConstant.ThankYou;
                                                switch (confirmation)
                                                {
                                                    case SendEmailConfirmation.yes:
                                                        {
                                                            // if previous question was confirm send email of task and it was not null/wrong value then answer will send email and reply back with thank you and task mail stopped
                                                            taskDetails.SendEmailConfirmation = SendEmailConfirmation.yes;
                                                            taskDetails.QuestionId = nextQuestion.Id;
                                                            // get list of task done and register for today for that user
                                                            var taskMailDetails = await _taskMail.FirstOrDefaultAsync(x => x.EmployeeId == oAuthUser.Id &&
                                                            x.CreatedOn.Day == DateTime.UtcNow.Day && x.CreatedOn.Month == DateTime.UtcNow.Month &&
                                                            x.CreatedOn.Year == DateTime.UtcNow.Year);
                                                            //var taskMailDetails = taskMailList.Last();
                                                            taskList = await _taskMailDetail.FetchAsync(x => x.TaskId == taskMailDetails.Id);
                                                            var teamLeaders = await _oauthCallsRepository.GetTeamLeaderUserIdAsync(userId, accessToken);
                                                            var managements = await _oauthCallsRepository.GetManagementUserNameAsync(accessToken);
                                                            foreach (var management in managements)
                                                            {
                                                                teamLeaders.Add(management);
                                                            }
                                                            foreach (var teamLeader in teamLeaders)
                                                            {
                                                                // transforming task mail details to template page and getting as string
                                                                var emailBody = _emailServiceTemplate.EmailServiceTemplateTaskMail(taskList);
                                                                EmailApplication email = new EmailApplication();
                                                                email.Body = emailBody;
                                                                email.From = oAuthUser.Email;
                                                                email.To = teamLeader.Email;
                                                                email.Subject = _stringConstant.TaskMailSubject;
                                                                // Email send 
                                                                _emailService.Send(email);
                                                            }
                                                        }
                                                        break;
                                                    case SendEmailConfirmation.no:
                                                        {
                                                            // if previous question was confirm send email of task and it was not null/wrong value then answer will say thank you and task mail stopped
                                                            taskDetails.SendEmailConfirmation = SendEmailConfirmation.no;
                                                            taskDetails.QuestionId = nextQuestion.Id;
                                                        }
                                                        break;
                                                }
                                            }
                                            else
                                                // if previous question was send email of task and it was null or wrong value then it will ask for send task mail confirm again
                                                questionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                                    _stringConstant.SendTaskMailConfirmationErrorMessage,
                                                    Environment.NewLine, previousQuestion.QuestionStatement);
                                        }
                                        break;
                                    #endregion

                                    #region Default
                                    default:
                                        questionText = _stringConstant.RequestToStartTaskMail;
                                        break;
                                        #endregion
                                }
                                _taskMailDetail.Update(taskDetails);
                                _taskMail.Save();
                            }
                        }
                        else
                            questionText = _stringConstant.RequestToStartTaskMail;
                    }
                    else
                        // if previous task mail doesnot exist then ask user to start task mail
                        questionText = _stringConstant.RequestToStartTaskMail;
                }
                else
                    // if user doesn't exist in oAuth server
                    questionText = _stringConstant.YouAreNotInExistInOAuthServer;
            }
            catch (SmtpException ex)
            {
                // Email error message will be send to user. But task mail will be added
                questionText = string.Format(_stringConstant.ReplyTextForSMTPExceptionErrorMessage,
                    _stringConstant.ErrorOfEmailServiceFailureTaskMail, ex.Message);
                _logger.Error(questionText, ex);
            }
            return questionText;
        }


        /// <summary>
        ///Method geting Employee or list of Employees 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of task mail report</returns>
        public async Task<List<TaskMailReportAc>> GetUserInformationAsync(string userId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            var user = await _user.FirstOrDefaultAsync(x => x.Id == userId);
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
            List<UserRoleAc> userRoleAcList = await _oauthCallsRepository.GetUserRoleAsync(user.Id, accessToken);
            var userInformation = userRoleAcList.First(x => x.UserName == user.UserName);
            if (userInformation.Role == _stringConstant.RoleAdmin)
            {
                userRoleAcList.Remove(userInformation);
            }
            foreach (var userRole in userRoleAcList)
            {
                TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userRole.UserId, userRole.Role, userRole.Name, userEmail: userRole.UserName);
                taskMailReportAcList.Add(taskMailReportAc);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// This Method use to fetch the task mail detils.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="loginId"></param>
        /// <returns>list of task mail report with task mail details</returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string userId, string role, string userName, string loginId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            if (role == _stringConstant.RoleAdmin || role == _stringConstant.RoleEmployee)
            {
                taskMailReportAcList = await GetTaskMailDetailsInformationAsync(userId, role, userName, loginId);
            }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
                var MaxMinDate = await GetMaxMinDateAsync(userRoleAcList);
                taskMailReportAcList = await TaskMailDetails(role, loginId, MaxMinDate.Item1);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// this Method use to fetch the task mail details for the selected date.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <returns>list of task mail report with task mail details</returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string userId, string userName, string role, string createdOn, string loginId, DateTime selectedDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            if (role == _stringConstant.RoleAdmin || role == _stringConstant.RoleEmployee)
            { taskMailReportAcList = await TaskMailDetails(userId, userName, role,  loginId, createdOn, selectedDate); }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                taskMailReportAcList = await TaskMailDetails( role, loginId, selectedDate);
            }
            return taskMailReportAcList;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get UserRoles
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns>fetch users role</returns>
        private async Task<List<UserRoleAc>> GetUserRoleAsync(string loginId)
        {
            var user = _user.FirstOrDefault(x => x.Id == loginId);
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
            return await _oauthCallsRepository.GetListOfEmployeeAsync(user.Id, accessToken);
        }

        /// <summary>
        /// Task Mail Details For TeamLeader
        /// </summary>
        /// <param name="role"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <returns>list of task mail reports</returns>
        private async Task<List<TaskMailReportAc>> TaskMailDetails(string role, string loginId, DateTime selectedDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
            var MaxMinDate = await GetMaxMinDateAsync(userRoleAcList);
            foreach (var userRole in userRoleAcList)
            {
                var taskMail = (await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(selectedDate))).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                if (taskMail != null)
                {
                    TaskMailReportAc taskMailReportAc = await GetTaskMailReportAsync(userRole.UserId, role, userRole.Name, taskMail.Id, taskMail.CreatedOn.Date, MaxMinDate.Item1.Date, MaxMinDate.Item2.Date);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
                else
                {
                    TaskMailReportAc taskMailReportAc = GetTaskMailReport(userRole.UserId, role, userRole.Name, selectedDate.Date, MaxMinDate.Item1.Date, MaxMinDate.Item2.Date);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// Task Mail Details For Admin Or Employee 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <returns>list of task mail reports</returns>
        private async Task<List<TaskMailReportAc>> TaskMailDetails(string userId, string userName, string role, string createdOn, string loginId, DateTime selectedDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            var taskMail = (await _taskMail.FetchAsync(y => y.EmployeeId == userId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(selectedDate))).FirstOrDefault();
            DateTime maxDate = (await _taskMail.FetchAsync(x => x.EmployeeId == userId)).Max(x => x.CreatedOn);
            DateTime minDate = (await _taskMail.FetchAsync(x => x.EmployeeId == userId)).Min(x => x.CreatedOn);
            if (taskMail != null)
            {
                TaskMailReportAc taskMailReportAc = await GetTaskMailReportAsync(userId, role, userName, taskMail.Id, taskMail.CreatedOn.Date, maxDate.Date, minDate.Date);
                taskMailReportAcList.Add(taskMailReportAc);
            }
            else
            {
                TaskMailReportAc taskMailReportAc = GetTaskMailReport(userId, role, userName, selectedDate.Date, maxDate.Date, minDate.Date);
                taskMailReportAcList.Add(taskMailReportAc);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// Fetch max and min date of task mail
        /// </summary>
        /// <param name="userRoleAcList"></param>
        /// <returns>max and min Date</returns> 
        private async Task<Tuple<DateTime,DateTime>> GetMaxMinDateAsync(List<UserRoleAc> userRoleAcList)
        {
            var userIdList = userRoleAcList.Select(x => x.UserId);
            var taskMails = await _taskMail.FetchAsync(x => userIdList.Contains(x.EmployeeId));
            DateTime maxDate = taskMails.Max(x => x.CreatedOn);
            DateTime minDate = taskMails.Min(x => x.CreatedOn);
            return new Tuple<DateTime, DateTime>(maxDate,minDate);
        }

        /// <summary>
        /// Get Default task mail 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="createdOn"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns>Task mail report</returns>
        private TaskMailReportAc GetTaskMailReport(string userId, string role, string userName, DateTime createdOn, DateTime maxDate, DateTime minDate)
        {
            List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
            var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
            taskMailDetailReportList.Add(TaskMailDetailReportAc);
            TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userId, role, userName, taskMailDetailReportList, createdOn: createdOn, maxDate: maxDate, minDate: minDate);
            return taskMailReportAc;
        }

        /// <summary>
        /// Get Task Mail
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="taskId"></param>
        /// <param name="createdOn"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns>List of task mail report</returns>
        private async Task<TaskMailReportAc> GetTaskMailReportAsync(string userId, string role, string userName, int taskId, DateTime createdOn, DateTime maxDate, DateTime minDate)
        {
            List<TaskMailDetailReportAc> taskMailDetailReportAcList = new List<TaskMailDetailReportAc>();
            List<TaskMailDetails> taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskId)).ToList();
            foreach (var taskMailDetail in taskMailDetailList)
            {
                TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc(taskMailDetail.Description, taskMailDetail.Comment, id: taskMailDetail.Id, hours: taskMailDetail.Hours, status: taskMailDetail.Status);
                taskMailDetailReportAcList.Add(taskmailReportAc);
            }
            TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userId, role, userName, taskMailDetailReportAcList, createdOn: createdOn, maxDate: maxDate, minDate: minDate);
            return taskMailReportAc;
        }

        /// <summary>
        /// Task Mail Details Report Information For the User Role Admin and Employee
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="loginId"></param>
        /// <returns>List task mail Report</returns>
        private async Task<List<TaskMailReportAc>> GetTaskMailDetailsInformationAsync(string userId, string role, string userName, string loginId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            var taskMail = (await _taskMail.FetchAsync(y => y.EmployeeId == userId)).OrderByDescending(y => y.CreatedOn).FirstOrDefault();
            if (taskMail!=null)
            {
                List<TaskMailDetails> taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskMail.Id)).ToList();
                var maxDate = taskMail.CreatedOn.Date;
                var minDate= (await _taskMail.FetchAsync(y => y.EmployeeId == userId)).Min(x => x.CreatedOn);
                if (taskMailDetailList.Any())
                {
                    TaskMailReportAc taskMailReportAc = await GetTaskMailReportAsync(userId, role, userName, taskMail.Id, maxDate.Date, maxDate.Date, minDate.Date);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
                else
                {
                    TaskMailReportAc taskMailReportAc = GetTaskMailReport(userId, role, userName, maxDate.Date,maxDate.Date, minDate.Date);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
            }
            else
            {
                TaskMailReportAc taskMailReportAc = GetTaskMailReport(userId, role, userName, DateTime.Now.Date, DateTime.Now.Date, DateTime.Now.Date);
                taskMailReportAcList.Add(taskMailReportAc);
            }
            return taskMailReportAcList;
        }

        #endregion
    }
}