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
using Promact.Erp.Util.StringConstants;
using Promact.Core.Repository.EmailServiceTemplateRepository;

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
        #endregion

        #region Constructor
        public TaskMailRepository(IRepository<TaskMail> taskMail, IStringConstantRepository stringConstant,
            IOauthCallsRepository oauthCallsRepository, IRepository<TaskMailDetails> taskMailDetail,
            IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> user, IEmailService emailService,
            IBotQuestionRepository botQuestionRepository, ApplicationUserManager userManager,
            IEmailServiceTemplateRepository emailServiceTemplate)
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
            bool questionTextIsNull = string.IsNullOrEmpty(userAndTaskMailDetailsWithAccessToken.QuestionText);
            // if question text is null or not request to start task mail then only allowed
            if (questionTextIsNull || CheckQuestionTextIsRequestToStartMailOrNot(userAndTaskMailDetailsWithAccessToken.QuestionText))
            {
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
                    userAndTaskMailDetailsWithAccessToken.TaskMail = AddTaskMail(userAndTaskMailDetailsWithAccessToken.User.Id);
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
                        var nextQuestion = await NextQuestionForTaskMailAsync(previousQuestion.OrderNumber);
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
            return userAndTaskMailDetailsWithAccessToken.QuestionText;
        }

        /// <summary>
        ///Method geting Employee or list of Employees 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of task mail report</returns>
        public async Task<List<TaskMailReportAc>> GetUserInformationAsync(string userId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            var user = await _user.FirstAsync(x => x.Id == userId);
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
            //getting user information from Promact Oauth Server.
            List<UserRoleAc> userRoleAcList = await _oauthCallsRepository.GetUserRoleAsync(user.Id, accessToken);
            var userInformation = userRoleAcList.First(x => x.UserName == user.UserName);
            if (userInformation.Role == _stringConstant.RoleAdmin)
            {
                //if user is admin then remove from user list. because admin dose not have any taks mail.
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
        /// This Method use to fetch the task mail details.
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
                //getting the employee task mail reports   
                taskMailReportAcList = await GetTaskMailDetailsInformationAsync(userId, role, userName, loginId);
            }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                //getting the team members information.
                List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
                //getting maximum and minimum date from the team members task mails
                var maxMinTaskMailDate = await GetMaxMinDateAsync(userRoleAcList);
                //first time there are no selected date that's why pass maxdate as a selected date.
                //getting the team members task mail reports
                taskMailReportAcList = await TaskMailDetailsAsync(role, loginId,maxMinTaskMailDate.Item1.Date,maxMinTaskMailDate.Item1.Date,maxMinTaskMailDate.Item2.Date);
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
            {
                //getting the employee task mail reports for selected date  
                taskMailReportAcList = await TaskMailsDetailAsync(userId, userName, role,  loginId, createdOn, selectedDate);
            }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                //getting the team members information 
                List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
                //find maximum and minimum date from the team members task mails
                var maxMinTaskMailDate = await GetMaxMinDateAsync(userRoleAcList);
                //getting the team members task mail reports for selected date
                taskMailReportAcList = await TaskMailDetailsAsync(role, loginId, selectedDate.Date, maxMinTaskMailDate.Item1.Date, maxMinTaskMailDate.Item2.Date);
            }
            return taskMailReportAcList;
        }

        #endregion

        #region Private Methods
         
        /// <summary>
        /// Getting user information
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns>fetch users role</returns>
        private async Task<List<UserRoleAc>> GetUserRoleAsync(string loginId)
        {
            var user =await _user.FirstAsync(x => x.Id == loginId);
            // getting access token for that user
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
            //getting user information from Promact Oauth Server
            return await _oauthCallsRepository.GetListOfEmployeeAsync(user.Id, accessToken);
        }


        /// <summary>
        /// Getting max and min date from users task mails
        /// </summary>
        /// <param name="userRoleAcList"></param>
        /// <returns>max and min Date</returns> 
        private async Task<Tuple<DateTime, DateTime>> GetMaxMinDateAsync(List<UserRoleAc> userRoleAcList)
        {
            //getting list of userId.
            var userIdList = userRoleAcList.Select(x => x.UserId);
            //getting list of task mails using userIdList.
            var taskMails = (await _taskMail.FetchAsync(x => userIdList.Contains(x.EmployeeId))).ToList();
            //getting maximum and minimum date form the team members task mails
            DateTime maxDate = taskMails.Max(x => x.CreatedOn);
            DateTime minDate = taskMails.Min(x => x.CreatedOn);
            return new Tuple<DateTime, DateTime>(maxDate, minDate);
        }


        /// <summary>
        /// Task mail details for teamLeader
        /// </summary>
        /// <param name="role"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns>list of task mail reports</returns>
        private async Task<List<TaskMailReportAc>> TaskMailDetailsAsync(string role, string loginId, DateTime selectedDate,DateTime maxDate,DateTime minDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
            //getting the team members task mails using users information.
            foreach (var userRole in userRoleAcList)
            {
                TaskMailReportAc taskMailReportAc = await GetTaskReportAsync(userRole.UserId, role, userRole.Name, selectedDate, maxDate,minDate );
                taskMailReportAcList.Add(taskMailReportAc);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// Getting task mail reports
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="selectedDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns>task mail report</returns>
        private async Task<TaskMailReportAc> GetTaskReportAsync(string userId, string role, string userName, DateTime selectedDate, DateTime maxDate, DateTime minDate)
        {
            TaskMailReportAc taskMailReportAc;
            var taskMail = (await _taskMail.FirstOrDefaultAsync(y => y.EmployeeId == userId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(selectedDate)));
            if (taskMail != null)
            {
                //getting the team members task mails details.
                taskMailReportAc = await GetTaskMailReportAsync(userId, role, userName, taskMail.Id, taskMail.CreatedOn.Date, maxDate, minDate);
            }
            else
            {
                //if team member does not have any task mail than show default task mail to the end users. 
                taskMailReportAc = GetTaskMailReport(userId, role, userName, selectedDate.Date, maxDate.Date, minDate.Date);
            }
            return taskMailReportAc;
        }

        /// <summary>
        /// Task mail fetails for admin or employee 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <returns>list of task mail reports</returns>
        private async Task<List<TaskMailReportAc>> TaskMailsDetailAsync(string userId, string userName, string role, string createdOn, string loginId, DateTime selectedDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            //find maximum and minimum date from the employee task mails
            IEnumerable<TaskMail> taskMails = (await _taskMail.FetchAsync(x => x.EmployeeId == userId)).ToList();
            DateTime maxDate = taskMails.Max(x => x.CreatedOn);
            DateTime minDate = taskMails.Min(x => x.CreatedOn);
            //getting task mail information.
            TaskMailReportAc taskMailReportAc = await GetTaskReportAsync(userId, role, userName, selectedDate.Date, maxDate.Date, minDate.Date);
            taskMailReportAcList.Add(taskMailReportAc);
            return taskMailReportAcList;
        }

        /// <summary>
        /// Get default task mail 
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
            var taskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
            taskMailDetailReportList.Add(taskMailDetailReportAc);
            TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userId, role, userName, taskMailDetailReportList, createdOn: createdOn, maxDate: maxDate, minDate: minDate);
            return taskMailReportAc;
        }

        /// <summary>
        /// Getting taskmail details infromation
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
            // getting taskmail details infromation using taskId. 
            var taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskId));
            foreach (var taskMailDetail in taskMailDetailList)
            {
                TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc(taskMailDetail.Description, taskMailDetail.Comment, id: taskMailDetail.Id, hours: taskMailDetail.Hours, status: taskMailDetail.Status);
                taskMailDetailReportAcList.Add(taskmailReportAc);
            }
            TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userId, role, userName, taskMailDetailReportAcList, createdOn: createdOn, maxDate: maxDate, minDate: minDate);
            return taskMailReportAc;
        }

        /// <summary>
        /// Task mail details report information for the user role admin and employee
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="loginId"></param>
        /// <returns>List task mail Report</returns>
        private async Task<List<TaskMailReportAc>> GetTaskMailDetailsInformationAsync(string userId, string role, string userName, string loginId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            var taskMail = (await _taskMail.FetchAsync(y => y.EmployeeId == userId)).ToList();
            TaskMailReportAc taskMailReportAc;
            if (taskMail.Any())
            {
                //first time there are no selected date that's why pass maxdate as a selected date.
                taskMailReportAc = await GetTaskMailReportAsync(userId, role, userName, taskMail.OrderByDescending(y => y.CreatedOn).First().Id, taskMail.Max(x => x.CreatedOn).Date, taskMail.Max(x => x.CreatedOn).Date, taskMail.Min(x => x.CreatedOn).Date);
            }
            else
            {
                //if employee does not have any task mail than show default task mail to the end users. 
                taskMailReportAc = GetTaskMailReport(userId, role, userName, DateTime.Now.Date, DateTime.Now.Date, DateTime.Now.Date);
            }
            taskMailReportAcList.Add(taskMailReportAc);
            return taskMailReportAcList;
        }

        /// <summary>
        /// Private method to get user's details, user's accesstoken, user's task mail details and list or else appropriate message will be send
        /// </summary>
        /// <param name="slackUserId">User's SlackId</param>
        /// <returns></returns>
        private async Task<UserAndTaskMailDetailsWithAccessToken> GetUserAndTaskMailDetailsAsync(string slackUserId)
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
        private async Task<Question> NextQuestionForTaskMailAsync(QuestionOrder previousQuestionOrder)
        {
            var orderValue = (int)previousQuestionOrder;
            var typeValue = (int)BotQuestionType.TaskMail;
            orderValue++;
            // getting question by order number and question type as task mail
            var nextQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(orderValue, typeValue);
            return nextQuestion;
        }
        #endregion
    }
}