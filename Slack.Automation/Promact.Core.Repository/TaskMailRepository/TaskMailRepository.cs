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
            // method to get user's details, user's accesstoken, user's task mail details and list or else appropriate message will be send
            var userAndTaskMailDetailsWithAccessToken = await GetUserAndTaskMailDetails(userId);
            bool questionTextIsNull = string.IsNullOrEmpty(userAndTaskMailDetailsWithAccessToken.QuestionText);
            // if question text is null or not request to start task mail then only allowed
            if (questionTextIsNull || CheckQuestionTextIsRequestToStartMailOrNot(userAndTaskMailDetailsWithAccessToken.QuestionText))
            {
                QuestionOrder questionOrder = new QuestionOrder();
                Question previousQuestion;
                TaskMailDetails taskMailDetail = new TaskMailDetails();
                #region Task Mail Details
                TaskMailCondition taskMailCondition = new TaskMailCondition();
                if (userAndTaskMailDetailsWithAccessToken.TaskList == null)
                    taskMailCondition = TaskMailCondition.Null;
                switch (taskMailCondition)
                {
                    case TaskMailCondition.NotNull:
                        {
                            taskMailDetail = userAndTaskMailDetailsWithAccessToken.TaskList.Last();
                            previousQuestion = await _botQuestionRepository.FindByIdAsync(taskMailDetail.QuestionId);
                            questionOrder = previousQuestion.OrderNumber;
                            // If previous task mail is on the process and user started new task mail then will need to first complete pervious one
                            if (questionOrder <= QuestionOrder.TaskMailSend)
                                userAndTaskMailDetailsWithAccessToken.QuestionText = await QuestionAndAnswerAsync(null, userId);
                        }
                        break;
                    case TaskMailCondition.Null:
                        {
                            // If task mail is not started for that day
                            userAndTaskMailDetailsWithAccessToken.TaskMail = AddTaskMail(userAndTaskMailDetailsWithAccessToken.User.Id);
                        }
                        break;
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
        /// Method to conduct task mail after started
        /// </summary>
        /// <param name="answer">User's slack user Id</param>
        /// <returns>questionText in string format containing question statement</returns>
        public async Task<string> QuestionAndAnswerAsync(string answer, string userId)
        {
            // method to get user's details, user's accesstoken, user's task mail details and list or else appropriate message will be send
            var userAndTaskMailDetailsWithAccessToken = await GetUserAndTaskMailDetails(userId);
            try
            {
                // if question text is null then only allowed
                if (string.IsNullOrEmpty(userAndTaskMailDetailsWithAccessToken.QuestionText))
                {
                    if (userAndTaskMailDetailsWithAccessToken.TaskList != null)
                    {
                        var taskDetails = userAndTaskMailDetailsWithAccessToken.TaskList.Last();
                        // getting inform of previous question was asked to user
                        var previousQuestion = await _botQuestionRepository.FindByIdAsync(taskDetails.QuestionId);
                        // checking if previous question was last and answered by user and previous task report was completed then asked for new task mail
                        if (previousQuestion.OrderNumber <= QuestionOrder.TaskMailSend)
                        {
                            // getting next question to be asked to user
                            var nextQuestion = await NextQuestionForTaskMail(previousQuestion.OrderNumber);
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
                                                    nextQuestion = await NextQuestionForTaskMail(QuestionOrder.SendEmail);
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
                                                        nextQuestion = await NextQuestionForTaskMail(QuestionOrder.ConfirmSendEmail);
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
                                                        email.From = userAndTaskMailDetailsWithAccessToken.User.Email;
                                                        email.Subject = _stringConstant.TaskMailSubject;
                                                        // transforming task mail details to template page and getting as string
                                                        email.Body = _emailServiceTemplate.EmailServiceTemplateTaskMail(userAndTaskMailDetailsWithAccessToken.TaskList);
                                                        // if previous question was confirm send email of task and it was not null/wrong value then answer will send email and reply back with thank you and task mail stopped
                                                        taskDetails.QuestionId = nextQuestion.Id;
                                                        // getting team leader list
                                                        var teamLeaders = await _oauthCallsRepository.GetTeamLeaderUserIdAsync(userId, userAndTaskMailDetailsWithAccessToken.AccessToken);
                                                        // getting managemnt list
                                                        var managements = await _oauthCallsRepository.GetManagementUserNameAsync(userAndTaskMailDetailsWithAccessToken.AccessToken);
                                                        teamLeaders.AddRange(managements);
                                                        foreach (var teamLeader in teamLeaders)
                                                        {
                                                            email.To = teamLeader.Email;
                                                            // Email send 
                                                            _emailService.Send(email);
                                                        }
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
                            _taskMailDetail.Update(taskDetails);
                            _taskMail.Save();
                        }
                    }
                    else
                        userAndTaskMailDetailsWithAccessToken.QuestionText = _stringConstant.RequestToStartTaskMail;
                }
            }
            catch (SmtpException ex)
            {
                // Email error message will be send to user. But task mail will be added
                userAndTaskMailDetailsWithAccessToken.QuestionText = string.Format(_stringConstant.ReplyTextForSMTPExceptionErrorMessage,
                    _stringConstant.ErrorOfEmailServiceFailureTaskMail, ex.Message);
                _logger.Error(userAndTaskMailDetailsWithAccessToken.QuestionText, ex);
            }
            return userAndTaskMailDetailsWithAccessToken.QuestionText;
        }


        /// <summary>
        ///Method geting Employee or list of Employees 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailReportAc>> GetUserInformationAsync(string userId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            var user = await _user.FirstOrDefaultAsync(x => x.Id == userId);
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
            List<UserRoleAc> userRoleAcList = await _oauthCallsRepository.GetUserRoleAsync(user.Id, accessToken);
            if (userRoleAcList.FirstOrDefault(x => x.UserName == user.UserName).Role == _stringConstant.RoleAdmin)
            {
                userRoleAcList.Remove(userRoleAcList.FirstOrDefault(x => x.UserName == user.UserName));
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
        /// <returns></returns>
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
                DateTime maxDate = await GetMaxDateAsync(userRoleAcList);
                DateTime minDate = await GetMinDateAsync(userRoleAcList);
                foreach (var userRole in userRoleAcList)
                {
                    var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(maxDate));
                    if (taskMails != null && taskMails.Any())
                    {
                        var taskMail = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                        taskMailReportAcList = await GetTaskMailReportListAsync(userId, role, userName, taskMail.Id, taskMail.CreatedOn.Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                    }
                    else
                    {
                        taskMailReportAcList = GetTaskMailReportList(userId, role, userName, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                    }
                }
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
        /// <returns></returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string userId, string userName, string role, string createdOn, string loginId, string selectedDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            List<TaskMailDetailReportAc> taskMailDetailReportAcList = new List<TaskMailDetailReportAc>();
            if (role == _stringConstant.RoleAdmin || role == _stringConstant.RoleEmployee)
            { taskMailReportAcList = await TaskMailDetailsForSelectedDateAsync(userId, userName, role, createdOn, loginId, selectedDate); }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
                DateTime maxDate = await GetMaxDateAsync(userRoleAcList);
                DateTime minDate = await GetMinDateAsync(userRoleAcList);
                foreach (var userRole in userRoleAcList)
                {
                    DateTime selectedDateTime = Convert.ToDateTime(selectedDate);
                    var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(selectedDateTime));
                    if (taskMails != null && taskMails.Any())
                    {
                        var taskMail = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                        taskMailReportAcList = await GetTaskMailReportListAsync(userRole.UserId, role, userRole.Name, taskMail.Id, taskMail.CreatedOn.Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                    }
                    else
                    {
                        taskMailReportAcList = GetTaskMailReportList(userRole.UserId, role, userRole.Name, Convert.ToDateTime(selectedDate).Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                    }
                }
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// This Method use to fetch the task mail details for the next and previous date
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportNextPreviousDateAsync(string userId, string userName, string role, string createdOn, string loginId, string type)
        {
            DateTime createdDate;
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            List<TaskMailDetailReportAc> taskMailDetailReportAcList = new List<TaskMailDetailReportAc>();
            if (type == _stringConstant.NextPage)
            { createdDate = Convert.ToDateTime(createdOn).AddDays(+1); }
            else
            { createdDate = Convert.ToDateTime(createdOn).AddDays(-1); }

            if (role == _stringConstant.RoleAdmin || role == _stringConstant.RoleEmployee)
            {
                taskMailReportAcList = await TaskMailDetailsForNextPreviousDateAsync(userId, userName, role, Convert.ToDateTime(createdDate), loginId);
            }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
                DateTime maxDate = await GetMaxDateAsync(userRoleAcList);
                DateTime minDate = await GetMinDateAsync(userRoleAcList);
                foreach (var userRole in userRoleAcList)
                {

                    var employeeTaskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(createdDate));
                    if (employeeTaskMails != null && employeeTaskMails.Any())
                    {
                        var taskMails = employeeTaskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                        taskMailReportAcList = await GetTaskMailReportListAsync(userId, role, userName, taskMails.Id, taskMails.CreatedOn.Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                    }
                    else
                    {
                        taskMailReportAcList = GetTaskMailReportList(userRole.UserId, role, userRole.Name, Convert.ToDateTime(createdDate).Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                    }
                }
            }
            return taskMailReportAcList;
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// This Method use to fetch Maximum Date
        /// </summary>
        /// <param name="taskMails"></param>
        /// <returns></returns>
        private async Task<DateTime> GetMaxDateAsync(List<UserRoleAc> userRoleAcList)
        {
            DateTime? maxDate = null;
            foreach (var userRole in userRoleAcList)
            {
                List<TaskMail> taskMailList = (await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId)).ToList();
                if (taskMailList != null)
                {
                    var taskMail = taskMailList.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                    if (taskMail != null)
                    {
                        if (maxDate == null)
                        {
                            maxDate = taskMail.CreatedOn;
                        }
                        else
                        {
                            if (maxDate < taskMail.CreatedOn)
                            {
                                maxDate = taskMail.CreatedOn;
                            }
                        }
                    }
                }
            }
            return maxDate.Value;
        }

        /// <summary>
        /// This Method use to fetch Minimum Date
        /// </summary>
        /// <param name="taskMails"></param>
        /// <returns></returns>
        private async Task<DateTime> GetMinDateAsync(List<UserRoleAc> userRoleAcList)
        {
            DateTime? minDate = null;
            foreach (var userRole in userRoleAcList)
            {
                List<TaskMail> taskMailList = (await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId)).ToList();
                if (taskMailList != null)
                {
                    var taskMail = taskMailList.OrderBy(x => x.CreatedOn).FirstOrDefault();
                    if (taskMail != null)
                    {
                        if (minDate == null)
                        {
                            minDate = taskMail.CreatedOn;
                        }
                        else
                        {
                            if (minDate > taskMail.CreatedOn)
                            {
                                minDate = taskMail.CreatedOn;
                            }
                        }
                    }
                }
            }
            return minDate.Value;
        }

        /// <summary>
        /// Get Task Reportlist
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="createdOn"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns></returns>
        private List<TaskMailReportAc> GetTaskMailReportList(string userId, string role, string userName, DateTime createdOn, DateTime maxDate, DateTime minDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
            var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
            taskMailDetailReportList.Add(TaskMailDetailReportAc);
            TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userId, role, userName, taskMailDetailReportList, createdOn: createdOn, maxDate: maxDate, minDate: minDate);
            taskMailReportAcList.Add(taskMailReportAc);
            return taskMailReportAcList;
        }

        /// <summary>
        /// Get UserRoles
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns></returns>
        private async Task<List<UserRoleAc>> GetUserRoleAsync(string loginId)
        {
            var user = _user.FirstOrDefault(x => x.Id == loginId);
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
            return await _oauthCallsRepository.GetListOfEmployeeAsync(user.Id, accessToken);
        }

        /// <summary>
        /// Get Task Reportlist For TeamLeader
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="taskId"></param>
        /// <param name="createdOn"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns></returns>
        private async Task<List<TaskMailReportAc>> GetTaskMailReportListAsync(string userId, string role, string userName, int taskId, DateTime createdOn, DateTime maxDate, DateTime minDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            List<TaskMailDetailReportAc> taskMailDetailReportAcList = new List<TaskMailDetailReportAc>();
            List<TaskMailDetails> taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskId)).ToList();
            foreach (var taskMailDetail in taskMailDetailList)
            {
                TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc(taskMailDetail.Description, taskMailDetail.Comment, id: taskMailDetail.Id, hours: taskMailDetail.Hours, status: taskMailDetail.Status);
                taskMailDetailReportAcList.Add(taskmailReportAc);
            }
            TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userId, role, userName, taskMailDetailReportAcList, createdOn: createdOn, maxDate: maxDate, minDate: minDate);
            taskMailReportAcList.Add(taskMailReportAc);
            return taskMailReportAcList;
        }

        /// <summary>
        /// Task Mail Details Report Information For the User Role Admin and Employee
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="loginId"></param>
        /// <returns></returns>
        private async Task<List<TaskMailReportAc>> GetTaskMailDetailsInformationAsync(string userId, string role, string userName, string loginId)
        {

            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userId);
            if (taskMails.Any())
            {
                var taskMailId = taskMails.OrderByDescending(y => y.CreatedOn).FirstOrDefault().Id;
                List<TaskMailDetails> taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskMailId)).ToList();
                if (taskMailDetailList.Any())
                {
                    taskMailReportAcList = await GetTaskMailReportListAsync(userId, role, userName, taskMailId, taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date, taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date, taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date);
                }
                else
                {
                    taskMailReportAcList = GetTaskMailReportList(userId, role, userName, taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date, taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date, taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date);
                }
            }
            else
            {
                taskMailReportAcList = GetTaskMailReportList(userId, role, userName, DateTime.Now, DateTime.Now, DateTime.Now);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// TaskMailDetails Information For the selected date
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        private async Task<List<TaskMailReportAc>> TaskMailDetailsForSelectedDateAsync(string userId, string userName, string role, string createdOn, string loginId, string selectedDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            List<TaskMailDetailReportAc> taskMailDetailReportAcList = new List<TaskMailDetailReportAc>();
            DateTime pickDate = Convert.ToDateTime(selectedDate).Date;
            var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(pickDate));
            if (taskMails.Any())
            {
                DateTime maxDate = (await _taskMail.FetchAsync(x => x.EmployeeId == userId)).OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn;
                DateTime minDate = (await _taskMail.FetchAsync(x => x.EmployeeId == userId)).OrderBy(x => x.CreatedOn).FirstOrDefault().CreatedOn;
                if (taskMails != null)
                {
                    var taskMail = taskMails.FirstOrDefault();
                    taskMailReportAcList = await GetTaskMailReportListAsync(userId, role, userName, taskMail.Id, taskMail.CreatedOn.Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                }
                else
                {
                    taskMailReportAcList = GetTaskMailReportList(userId, role, userName, Convert.ToDateTime(selectedDate).Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                }
            }
            else
            {
                taskMailReportAcList = GetTaskMailReportList(userId, role, userName, Convert.ToDateTime(selectedDate).Date, Convert.ToDateTime(DateTime.Now).Date, Convert.ToDateTime(DateTime.Now).Date);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// This Method use to fetch the task mail details for the next and previous date
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <returns></returns>
        private async Task<List<TaskMailReportAc>> TaskMailDetailsForNextPreviousDateAsync(string userId, string userName, string role, DateTime createdOn, string loginId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            List<TaskMailDetailReportAc> taskMailDetailReportAcList = new List<TaskMailDetailReportAc>();
            var taskMailList = await _taskMail.FetchAsync(y => y.EmployeeId == userId);
            if (taskMailList != null)
            {
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(createdOn));
                var taskMail = taskMails.OrderByDescending(y => y.CreatedOn).FirstOrDefault();
                DateTime minDate = (await _taskMail.FetchAsync(y => y.EmployeeId == userId)).OrderBy(y => y.CreatedOn).FirstOrDefault().CreatedOn;
                DateTime maxDate = (await _taskMail.FetchAsync(y => y.EmployeeId == userId)).OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn;
                if (taskMail != null)
                {
                    taskMailReportAcList = await GetTaskMailReportListAsync(userId, role, userName, taskMail.Id, taskMail.CreatedOn.Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                }
                else
                {
                    taskMailReportAcList = GetTaskMailReportList(userId, role, userName, Convert.ToDateTime(createdOn).Date, Convert.ToDateTime(maxDate).Date, Convert.ToDateTime(minDate).Date);
                }
            }
            else
            {
                taskMailReportAcList = GetTaskMailReportList(userId, role, userName, Convert.ToDateTime(createdOn).Date, Convert.ToDateTime(createdOn).Date, Convert.ToDateTime(createdOn).Date);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// Private method to get user's details, user's accesstoken, user's task mail details and list or else appropriate message will be send
        /// </summary>
        /// <param name="slackUserId">User's SlackId</param>
        /// <returns></returns>
        private async Task<UserAndTaskMailDetailsWithAccessToken> GetUserAndTaskMailDetails(string slackUserId)
        {
            UserAndTaskMailDetailsWithAccessToken userAndTaskMailDetailsWithAccessToken = new UserAndTaskMailDetailsWithAccessToken();
            var user = await _user.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
            if (user != null)
            {
                // getting access token for that user
                userAndTaskMailDetailsWithAccessToken.AccessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                // getting user information from Promact Oauth Server
                userAndTaskMailDetailsWithAccessToken.User = await _oauthCallsRepository.GetUserByUserIdAsync(slackUserId, userAndTaskMailDetailsWithAccessToken.AccessToken);
                if (userAndTaskMailDetailsWithAccessToken.User.Id != null)
                {
                    // checking for previous task mail exist or not for today
                    userAndTaskMailDetailsWithAccessToken.TaskMail = await _taskMail.FirstOrDefaultAsync(x => x.EmployeeId == userAndTaskMailDetailsWithAccessToken.User.Id &&
                    x.CreatedOn.Day == DateTime.UtcNow.Day && x.CreatedOn.Month == DateTime.UtcNow.Month &&
                    x.CreatedOn.Year == DateTime.UtcNow.Year);
                    if (userAndTaskMailDetailsWithAccessToken.TaskMail != null)
                    {
                        // getting task mail details for started task mail
                        userAndTaskMailDetailsWithAccessToken.TaskList = await _taskMailDetail.FetchAsync(x => x.TaskId == userAndTaskMailDetailsWithAccessToken.TaskMail.Id);
                    }
                    else
                    {
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
        private TaskMail AddTaskMail(string employeeId)
        {
            TaskMail taskMail = new TaskMail();
            taskMail.CreatedOn = DateTime.UtcNow;
            taskMail.EmployeeId = employeeId;
            _taskMail.Insert(taskMail);
            _taskMail.Save();
            return taskMail;
        }

        /// <summary>
        /// Add task mail details 
        /// </summary>
        /// <param name="taskMailId">task mail Id</param>
        /// <returns>first question statement</returns>
        private async Task<string> AddTaskMailDetailAndGetQuestionStatementAsync(int taskMailId)
        {
            TaskMailDetails taskMailDetails = new TaskMailDetails();
            // getting first question of task mail type
            var firstQuestion = await _botQuestionRepository.FindFirstQuestionByTypeAsync(BotQuestionType.TaskMail);
            taskMailDetails.TaskId = taskMailId;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetail.Insert(taskMailDetails);
            _taskMailDetail.Save();
            return firstQuestion.QuestionStatement;
        }

        /// <summary>
        /// Method to get the next question of task mail by previous question order
        /// </summary>
        /// <param name="previousQuestionOrder">previous question order</param>
        /// <returns>question</returns>
        private async Task<Question> NextQuestionForTaskMail(QuestionOrder previousQuestionOrder)
        {
            var orderValue = (int)previousQuestionOrder;
            var typeValue = (int)BotQuestionType.TaskMail;
            orderValue++;
            // getting question by order number and question type as task mail
            var nextQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(orderValue,typeValue);
            return nextQuestion;
        }
        #endregion
    }
}