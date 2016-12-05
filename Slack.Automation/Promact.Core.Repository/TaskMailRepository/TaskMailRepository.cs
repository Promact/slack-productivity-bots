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

        string questionText = "";
        private readonly IEmailServiceTemplateRepository _emailServiceTemplate;
        #endregion

        #region Constructor
        public TaskMailRepository(IRepository<TaskMail> taskMail, IStringConstantRepository stringConstant, IOauthCallsRepository oauthCallsRepository, IRepository<TaskMailDetails> taskMailDetail, IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> user, IEmailService emailService, IBotQuestionRepository botQuestionRepository, ApplicationUserManager userManager, IEmailServiceTemplateRepository emailServiceTemplate)
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
        /// Method to start task mail
        /// </summary>
        /// <param name="userId">User's slack user Id</param>
        /// <returns>questionText in string format containing question statement</returns>
        public async Task<string> StartTaskMailAsync(string userId)
        {
            // getting user name from user's slack name
            var user = await _user.FirstOrDefaultAsync(x => x.SlackUserId == userId);
            // getting access token for that user
            if (user != null)
            {
                // get access token of user for promact oauth server
                var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                // get user details from
                var oAuthUser = await _oauthCallsRepository.GetUserByUserIdAsync(userId, accessToken);
                TaskMailQuestion question = new TaskMailQuestion();
                Question previousQuestion = new Question();
                TaskMailDetails taskMailDetail = new TaskMailDetails();
                // getting user information from Promact Oauth Server
                TaskMail taskMail = null;
                List<TaskMail> taskMailList;
                // checking for previous task mail exist or not for today
                taskMailList = _taskMail.Fetch(x => x.EmployeeId == oAuthUser.Id && x.CreatedOn.Date == DateTime.UtcNow.Date).ToList();
                if (taskMailList.Count != 0)
                    taskMail = taskMailList.Last();
                if (taskMail != null)
                {
                    // if exist then check the whether the task mail was completed or not
                    taskMailDetail = await _taskMailDetail.FirstOrDefaultAsync(x => x.TaskId == taskMail.Id);
                    previousQuestion = await _botQuestionRepository.FindByIdAsync(taskMailDetail.QuestionId);
                    question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                }
                // if previous task mail completed then it will start a new one
                if (taskMail == null || question == TaskMailQuestion.TaskMailSend)
                {
                    // Before going to create new task it will check whether the user has send mail for today or not.
                    if (taskMailDetail != null && taskMailDetail.SendEmailConfirmation == SendEmailConfirmation.yes)
                        // If mail is send then user will not be able to add task mail for that day
                        questionText = _stringConstant.AlreadyMailSend;
                    else
                    {
                        // If mail is not send then user will be able to add task mail for that day
                        taskMail = new TaskMail();
                        taskMail.CreatedOn = DateTime.UtcNow;
                        taskMail.EmployeeId = oAuthUser.Id;
                        _taskMail.Insert(taskMail);
                        _taskMail.Save();
                        // getting first question of type 2
                        var firstQuestion = await _botQuestionRepository.FindByQuestionTypeAsync(2);
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
                    var questionText = await QuestionAndAnswerAsync(null, userId);
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
        public async Task<string> QuestionAndAnswerAsync(string answer,string userId)
        {
            // getting user name from user's slack name
            var user = await _user.FirstOrDefaultAsync(x => x.SlackUserId == userId);
            if (user != null)
            {
                // getting access token for that user
                var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                List<TaskMailDetails> taskList = new List<TaskMailDetails>();
                TaskMail taskMail = new TaskMail();
                // getting user information from Promact Oauth Server
                var oAuthUser = await _oauthCallsRepository.GetUserByUserIdAsync(userId, accessToken);
                try
                {
                    // checking for previous task mail exist or not for today
                    taskMail = _taskMail.Fetch(x => x.EmployeeId == oAuthUser.Id && x.CreatedOn.Date == DateTime.UtcNow.Date).Last();
                    // getting task mail details for pervious started task mail
                    var taskDetails = await _taskMailDetail.FirstOrDefaultAsync(x => x.TaskId == taskMail.Id);
                    // getting inform of previous question asked to user
                    var previousQuestion = await _botQuestionRepository.FindByIdAsync(taskDetails.QuestionId);
                    // checking if precious question was last and answer by user and previous task report was completed then asked for new task mail
                    if (previousQuestion.OrderNumber <= 7)
                    {
                        // getting next question to be asked to user
                        var nextQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync((previousQuestion.OrderNumber + 1), 2);
                        // Converting question Ordr to enum
                        var question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), 
                            previousQuestion.OrderNumber.ToString());
                        switch (question)
                        {
                            case TaskMailQuestion.YourTask:
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
                            case TaskMailQuestion.HoursSpent:
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
                                        if (hour > 0 && hour <= 8)
                                        {
                                            var todayTaskMail = _taskMail.Fetch(x => x.EmployeeId == taskMail.EmployeeId 
                                            && x.CreatedOn.Date == DateTime.UtcNow.Date);
                                            foreach (var item in todayTaskMail)
                                            {
                                                var recentTask = await _taskMailDetail.FirstOrDefaultAsync(x => x.TaskId == item.Id);
                                                totalHourSpented += recentTask.Hours;
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
                                                nextQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(6, 2);
                                                taskDetails.QuestionId = nextQuestion.Id;
                                                questionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                                    _stringConstant.HourLimitExceed, Environment.NewLine, 
                                                    nextQuestion.QuestionStatement);
                                                taskDetails.Comment = _stringConstant.StartWorking;
                                            }
                                        }
                                        else
                                            // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                                            questionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.TaskMailBotHourErrorMessage,
                                                Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
                                    else
                                        // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                                        questionText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                                            _stringConstant.TaskMailBotHourErrorMessage, Environment.NewLine,
                                            previousQuestion.QuestionStatement);
                                }
                                break;
                            case TaskMailQuestion.Status:
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
                            case TaskMailQuestion.Comment:
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
                            case TaskMailQuestion.SendEmail:
                                {
                                    SendEmailConfirmation sendEmailConfirmation;
                                    // checking whether string can be convert to TaskMailStatus type or not
                                    var sendEmailConfirmationConvertResult = Enum.TryParse(answer, out sendEmailConfirmation);
                                    if (sendEmailConfirmationConvertResult)
                                    {
                                        // convert answer to SendEmailConfirmation type
                                        var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation), 
                                            answer.ToLower().ToString());
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
                                                    nextQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(7, 2);
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
                                    break;
                                }
                            case TaskMailQuestion.ConfirmSendEmail:
                                {
                                    SendEmailConfirmation sendEmailConfirmation;
                                    // checking whether string can be convert to TaskMailStatus type or not
                                    var sendEmailConfirmationConvertResult = Enum.TryParse(answer, out sendEmailConfirmation);
                                    if (sendEmailConfirmationConvertResult)
                                    {
                                        // convert answer to SendEmailConfirmation type
                                        var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation),
                                            answer.ToLower().ToString());
                                        questionText = _stringConstant.ThankYou;
                                        switch (confirmation)
                                        {
                                            case SendEmailConfirmation.yes:
                                                {
                                                    // if previous question was confirm send email of task and it was not null/wrong value then answer will send email and reply back with thank you and task mail stopped
                                                    taskDetails.SendEmailConfirmation = SendEmailConfirmation.yes;
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    // get list of task done and register for today for that user
                                                    var taskMailList = _taskMail.Fetch(x => x.EmployeeId == oAuthUser.Id 
                                                    && x.CreatedOn.Date == DateTime.UtcNow.Date);
                                                    foreach (var item in taskMailList)
                                                    {
                                                        var taskDetail = await _taskMailDetail.FirstOrDefaultAsync(x => x.TaskId == item.Id);
                                                        taskList.Add(taskDetail);
                                                    }
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
                            default:
                                questionText = _stringConstant.RequestToStartTaskMail;
                                break;
                        }
                        _taskMailDetail.Update(taskDetails);
                        _taskMail.Save();
                    }
                }
                catch (SmtpException ex)
                {
                    // error message will be send to email. But leave will be applied
                    questionText = string.Format(_stringConstant.ReplyTextForSMTPExceptionErrorMessage, _stringConstant.ErrorOfEmailServiceFailureTaskMail, ex.Message.ToString());
                }
                catch (Exception)
                {
                    // if previous task mail doesnot exist then ask user to start task mail
                    questionText = _stringConstant.RequestToStartTaskMail;
                }
            }
            else
                // if user doesn't exist in oAuth server
                questionText = _stringConstant.YouAreNotInExistInOAuthServer;
            return questionText;
        }

        /// <summary>
        ///Method geting list of Employee 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailReportAc>> GetAllEmployeeAsync(string userId)
        {
            var user =await _user.FirstOrDefaultAsync(x => x.Id == userId);
            var accessToken = await _attachmentRepository.AccessToken(user.UserName);
            List<UserRoleAc> userRoleAcList = await _oauthCallsRepository.GetUserRoleAsync(user.Id, accessToken);
            if (userRoleAcList.FirstOrDefault(x => x.UserName == user.UserName).Role == _stringConstant.RoleAdmin)
            {
                foreach (var userRole in userRoleAcList)
                {
                   if (userId != userRole.UserId)
                    {
                        TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userRole.UserId, userRole.Role, userRole.Name, userEmail: userRole.UserName);
                        taskMailReportAcList.Add(taskMailReportAc);
                    }
                }
            }
            else if (userRoleAcList.FirstOrDefault(x => x.UserName == user.UserName).Role == _stringConstant.RoleTeamLeader || userRoleAcList.FirstOrDefault(x => x.UserName == user.UserName).Role == _stringConstant.RoleEmployee)
            {
                foreach (var userRole in userRoleAcList)
                {
                    TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userRole.UserId,userRole.Role,userRole.Name,userEmail:userRole.UserName);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
           }
           return taskMailReportAcList;
        }

        /// <summary>
        /// Task Mail Details Report Information For the User Role Admin and Employee
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserRole"></param>
        /// <param name="UserName"></param>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        private async Task<List<TaskMailReportAc>> GetTaskMailDetailsInformationAsync(string id, string role, string name, string loginId)
        {
            var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == id);
            if (taskMails.Count() != 0)
            {
                var taskMailId = taskMails.OrderBy(y => y.CreatedOn).FirstOrDefault().Id;
                List<TaskMailDetails> taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskMailId)).ToList();
                if (taskMailDetailList.Count() != 0)
                {
                   foreach (var taskMailDetail in taskMailDetailList)
                    {
                        TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc (taskMailDetail.Description,taskMailDetail.Comment,id: taskMailDetail.Id,hours: taskMailDetail.Hours,status: taskMailDetail.Status);
                        taskMailDetailReportAcList.Add(taskmailReportAc);
                    }
                    DateTime createdOn = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date;
                    DateTime minDate = taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date;
                    TaskMailReportAc taskMailReportAc = new TaskMailReportAc(id, role, name, taskMailDetailReportAcList, createdOn: createdOn, maxDate: createdOn, minDate: minDate);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
                else
                {
                    List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
                    DateTime createdOn = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date;
                    DateTime minDate = taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault().CreatedOn.Date;
                    var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable,status:TaskMailStatus.none);
                    taskMailDetailReportList.Add(TaskMailDetailReportAc);
                    TaskMailReportAc taskMailReportAc = new TaskMailReportAc(id, role, name, taskMailDetailReportList, createdOn: createdOn ,maxDate: createdOn,minDate: minDate);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
            }
            else
            {
                List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
                var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
                taskMailDetailReportList.Add(TaskMailDetailReportAc);
                TaskMailReportAc taskMailReportAc = new TaskMailReportAc(id, role, name, taskMailDetailReportList, createdOn: DateTime.Now,maxDate: DateTime.Now,minDate: DateTime.Now);
                taskMailReportAcList.Add(taskMailReportAc);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// This Method use to fetch the task mail detils.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserRole"></param>
        /// <param name="UserName"></param>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string id, string role, string name, string loginId)
        {
            if (role == _stringConstant.RoleAdmin || role == _stringConstant.RoleEmployee)
            {
                taskMailReportAcList = await GetTaskMailDetailsInformationAsync(id, role, name, loginId);
            }
            else if (role == _stringConstant.RoleTeamLeader)
            {

                var user = _user.FirstOrDefault(x => x.Id == loginId);
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                List<UserRoleAc> userRoles = await _oauthCallsRepository.GetListOfEmployeeAsync(user.Id, accessToken);
                foreach (var userRole in userRoles)
                {
                    List<TaskMail> taskMailList = (await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId)).ToList();
                    if (taskMailList.Count() != 0)
                    {
                        maxDate = GetMaxDate(taskMailList);
                        minDate = GetMinDate(taskMailList);
                    }
                }
                foreach (var userRole in userRoles)
                {
                    var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(maxDate));
                    if (taskMails != null && taskMails.Count() != 0)
                    {
                        var taskMail = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                        List<TaskMailDetails> taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskMail.Id)).ToList();
                        foreach (var taskMailDetail in taskMailDetailList)
                        {
                            TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc(taskMailDetail.Description, taskMailDetail.Comment, id: taskMailDetail.Id, hours: taskMailDetail.Hours, status: taskMailDetail.Status);
                            taskMailDetailReportAcList.Add(taskmailReportAc);
                        }
                        TaskMailReportAc taskMailReportAc = new TaskMailReportAc(id, role, name, taskMailDetailReportAcList,createdOn: taskMail.CreatedOn.Date , maxDate: Convert.ToDateTime(maxDate).Date, minDate: Convert.ToDateTime(minDate).Date);
                        taskMailReportAcList.Add(taskMailReportAc);
                    }
                    else
                    {
                        List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
                        var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status:TaskMailStatus.none);
                        taskMailDetailReportList.Add(TaskMailDetailReportAc);
                        TaskMailReportAc taskMailReportAc = new TaskMailReportAc(id, role, name, taskMailDetailReportList, createdOn: Convert.ToDateTime(maxDate).Date, maxDate: Convert.ToDateTime(maxDate).Date, minDate:Convert.ToDateTime(minDate).Date);
                        taskMailReportAcList.Add(taskMailReportAc);
                    }
                }
            }
            return taskMailReportAcList;
        }


        /// <summary>
        /// TaskMailDetails Information For the selected date
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserName"></param>
        /// <param name="UserRole"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="LoginId"></param>
        /// <param name="SelectedDate"></param>
        /// <returns></returns>
        private async Task<List<TaskMailReportAc>> TaskMailDetailsForSelectedDateAsync(string id, string name, string role, string createdOn, string loginId, string selectedDate)
        {
            DateTime slectedDateForAdmin = Convert.ToDateTime(selectedDate).Date;
            var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == id && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(slectedDateForAdmin));
            if (taskMails.Count() != 0)
            {
                maxDate = (await _taskMail.FetchAsync(x => x.EmployeeId == id)).OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn;
                minDate = (await _taskMail.FetchAsync(x => x.EmployeeId == id)).OrderBy(x => x.CreatedOn).FirstOrDefault().CreatedOn;
                if (taskMails != null)
                {
                    var taskMail = taskMails.FirstOrDefault();
                    List<TaskMailDetails> taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskMail.Id)).ToList();
                    foreach (var taskMailDetail in taskMailDetailList)
                    {
                        TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc(taskMailDetail.Description, taskMailDetail.Comment, id: taskMailDetail.Id, hours: taskMailDetail.Hours, status: taskMailDetail.Status);
                        taskMailDetailReportAcList.Add(taskmailReportAc);
                    }
                    TaskMailReportAc taskMailReportAc =new TaskMailReportAc(id, role, name, taskMailDetailReportAcList, createdOn: taskMail.CreatedOn.Date, maxDate: Convert.ToDateTime(maxDate).Date, minDate: Convert.ToDateTime(minDate).Date);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
                else
                {
                    List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
                    var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
                    taskMailDetailReportList.Add(TaskMailDetailReportAc);
                    var taskMailReportAc =new TaskMailReportAc(id, role, name, taskMailDetailReportList, createdOn:Convert.ToDateTime(selectedDate).Date, maxDate: Convert.ToDateTime(maxDate).Date, minDate:Convert.ToDateTime(minDate).Date);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
            }
            else
            {
                List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
                var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
                taskMailDetailReportList.Add(TaskMailDetailReportAc);
                TaskMailReportAc taskMailReportAc = new TaskMailReportAc(id, role, name, taskMailDetailReportList, createdOn: Convert.ToDateTime(selectedDate).Date, maxDate: Convert.ToDateTime(selectedDate).Date, minDate:Convert.ToDateTime(selectedDate).Date);
                taskMailReportAcList.Add(taskMailReportAc);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// this Method use to fetch the task mail details for the selected date.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserName"></param>
        /// <param name="UserRole"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="LoginId"></param>
        /// <param name="SelectedDate"></param>
        /// <returns></returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string id, string name, string role, string createdOn, string loginId, string selectedDate)
        {
            if (role == _stringConstant.RoleAdmin || role == _stringConstant.RoleEmployee)
            { taskMailReportAcList = await TaskMailDetailsForSelectedDateAsync(id, name, role, createdOn, loginId, selectedDate); }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                var user = _user.FirstOrDefault(x => x.Id == loginId);
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                List<UserRoleAc> userRolesAcList = await _oauthCallsRepository.GetListOfEmployeeAsync(user.Id, accessToken);
                foreach (var userRoleAc in userRolesAcList)
                {
                   var taskMailsRecodes = await _taskMail.FetchAsync(y => y.EmployeeId == userRoleAc.UserId);
                    if (taskMailsRecodes.Count() != 0)
                    {
                        maxDate = GetMaxDate(taskMailsRecodes);
                        minDate = GetMinDate(taskMailsRecodes);
                    }
                }
                foreach (var userRole in userRolesAcList)
                {
                    DateTime selectedDateTime = Convert.ToDateTime(selectedDate);
                    var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(selectedDateTime));
                    if (taskMails != null && taskMails.Count() != 0)
                    {
                        var taskMail = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                        List<TaskMailDetails> taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskMail.Id)).ToList();
                        foreach (var taskMailDetail in taskMailDetailList.ToList())
                        {
                            TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc(taskMailDetail.Description, taskMailDetail.Comment, id: taskMailDetail.Id, hours: taskMailDetail.Hours, status: taskMailDetail.Status);
                            taskMailDetailReportAcList.Add(taskmailReportAc);
                        }
                        TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userRole.UserId, role, userRole.Name, taskMailDetailReportAcList, createdOn: taskMail.CreatedOn, maxDate:  Convert.ToDateTime(maxDate).Date, minDate: Convert.ToDateTime(minDate).Date);
                        taskMailReportAcList.Add(taskMailReportAc);
                    }
                    else
                    {
                        List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
                        var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
                        taskMailDetailReportList.Add(TaskMailDetailReportAc);
                        TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userRole.UserId, role, userRole.Name, taskMailDetailReportList, createdOn:Convert.ToDateTime(selectedDate).Date,  maxDate: Convert.ToDateTime(maxDate).Date, minDate: Convert.ToDateTime(minDate).Date);
                        taskMailReportAcList.Add(taskMailReportAc);
                    }
                }
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// This Method use to fetch the task mail details for the next and previous date
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserName"></param>
        /// <param name="UserRole"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        private async Task<List<TaskMailReportAc>> TaskMailDetailsForNextPreviousDateAsync(string id, string name, string role, DateTime createdOn, string loginId)
        {
            var taskMailList = await _taskMail.FetchAsync(y => y.EmployeeId == id);
            if (taskMailList != null)
            {
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == id && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(createdOn));
                var taskMail = taskMails.OrderByDescending(y => y.CreatedOn).FirstOrDefault();
                minDate = (await _taskMail.FetchAsync(y => y.EmployeeId == id)).OrderBy(y => y.CreatedOn).FirstOrDefault().CreatedOn;
                maxDate = (await _taskMail.FetchAsync(y => y.EmployeeId == id)).OrderByDescending(x => x.CreatedOn).FirstOrDefault().CreatedOn;
                if (taskMail != null)
                {
                    List<TaskMailDetails> taskMailDetails = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskMail.Id)).ToList();
                    foreach (var taskMailDetail in taskMailDetails.ToList())
                    {
                        TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc(taskMailDetail.Description, taskMailDetail.Comment, id: taskMailDetail.Id, hours: taskMailDetail.Hours, status: taskMailDetail.Status);
                        taskMailDetailReportAcList.Add(taskmailReportAc);
                    }
                    TaskMailReportAc taskMailReportAc = new TaskMailReportAc(id, role, name, taskMailDetailReportAcList, createdOn: taskMail.CreatedOn.Date, maxDate:Convert.ToDateTime(maxDate).Date, minDate: Convert.ToDateTime(minDate).Date);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
                else
                {
                    List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
                    var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
                    taskMailDetailReportList.Add(TaskMailDetailReportAc);
                    TaskMailReportAc taskMailReportAc = new TaskMailReportAc(id, role, name, taskMailDetailReportList, createdOn:Convert.ToDateTime(createdOn).Date, maxDate:  Convert.ToDateTime(maxDate).Date, minDate: Convert.ToDateTime(minDate).Date);
                    taskMailReportAcList.Add(taskMailReportAc);
                }
            }
            else
            {
                List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
                var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
                taskMailDetailReportAcList.Add(TaskMailDetailReportAc);
                TaskMailReportAc taskMailReportAc = new TaskMailReportAc(id, role, name, taskMailDetailReportList, createdOn: Convert.ToDateTime(createdOn).Date, maxDate:Convert.ToDateTime(createdOn).Date,minDate: Convert.ToDateTime(createdOn).Date);
                taskMailReportAcList.Add(taskMailReportAc);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// This Method use to fetch the task mail details for the next and previous date
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserName"></param>
        /// <param name="UserRole"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="LoginId"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportNextPreviousDateAsync(string id, string name, string role, string createdOn, string loginId, string type)
        {
            DateTime? createdDate = null;
            if (type == _stringConstant.NextPage)
            { createdDate = Convert.ToDateTime(createdOn).AddDays(+1); }
            else
            { createdDate = Convert.ToDateTime(createdOn).AddDays(-1); }

            if (role == _stringConstant.RoleAdmin || role == _stringConstant.RoleEmployee)
            {
                taskMailReportAcList = await TaskMailDetailsForNextPreviousDateAsync(id, name, role, Convert.ToDateTime(createdDate), loginId);
            }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                var user = _user.FirstOrDefault(x => x.Id == loginId);
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                List<UserRoleAc> usersRole = await _oauthCallsRepository.GetListOfEmployeeAsync(user.Id, accessToken);
                foreach (var userRole in usersRole)
                {
                   var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId);
                    if (taskMails.Count() != 0)
                    {
                        maxDate = GetMaxDate(taskMails);
                        minDate = GetMinDate(taskMails);
                    }
                }
                foreach (var userRole in usersRole)
                {
                   
                    var employeeTaskMails = await _taskMail.FetchAsync(y => y.EmployeeId == userRole.UserId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(createdDate));
                    if (employeeTaskMails != null && employeeTaskMails.Count() != 0)
                    {
                        var taskMails = employeeTaskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                        List<TaskMailDetails> taskMailDetailList = (await _taskMailDetail.FetchAsync(x => x.TaskId == taskMails.Id)).ToList();
                        foreach (var taskMailDetail in taskMailDetailList.ToList())
                        {
                            TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc(taskMailDetail.Description, taskMailDetail.Comment, id: taskMailDetail.Id, hours: taskMailDetail.Hours, status: taskMailDetail.Status);
                            taskMailDetailReportAcList.Add(taskmailReportAc);
                        }
                        TaskMailReportAc taskMailReportAc = new TaskMailReportAc(taskMails.EmployeeId, role, userRole.Name, taskMailDetailReportAcList, createdOn: taskMails.CreatedOn.Date, maxDate: Convert.ToDateTime(maxDate).Date, minDate: Convert.ToDateTime(minDate).Date);
                        taskMailReportAcList.Add(taskMailReportAc);
                    }
                    else
                    {
                        List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
                        var TaskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
                        taskMailDetailReportList.Add(TaskMailDetailReportAc);
                        TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userRole.UserId, role, userRole.Name, taskMailDetailReportList, createdOn: Convert.ToDateTime(createdDate).Date, maxDate: Convert.ToDateTime(maxDate).Date, minDate: Convert.ToDateTime(minDate).Date);
                        taskMailReportAcList.Add(taskMailReportAc);
                    }
                }
            }
            return taskMailReportAcList;
        }

        
        /// <summary>
        /// This Method use to fetch Maximum Date
        /// </summary>
        /// <param name="taskMails"></param>
        /// <returns></returns>
        private DateTime GetMaxDate(IEnumerable<TaskMail> taskMails)
        {
            var taskMail = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
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
            return maxDate.Value;
        }

        /// <summary>
        /// This Method use to fetch Minimum Date
        /// </summary>
        /// <param name="taskMails"></param>
        /// <returns></returns>
        private DateTime GetMinDate(IEnumerable<TaskMail> taskMails)
        {
            DateTime? minDate = null;
            var taskMail = taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault();
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
            return minDate.Value;
        }

        
    }
}
