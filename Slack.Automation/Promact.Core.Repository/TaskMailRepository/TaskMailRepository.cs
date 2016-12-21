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
                // checking for previous task mail exist or not for today
                taskMail = await _taskMail.FirstOrDefaultAsync(x => x.EmployeeId == oAuthUser.Id &&
                x.CreatedOn.Day == DateTime.UtcNow.Day && x.CreatedOn.Month == DateTime.UtcNow.Month &&
                x.CreatedOn.Year == DateTime.UtcNow.Year);
                if (taskMail != null)
                {
                    // if exist then check the whether the task mail was completed or not
                    IEnumerable<TaskMailDetails> taskMailDetailsList = await _taskMailDetail.FetchAsync(x => x.TaskId == taskMail.Id);
                    taskMailDetail = taskMailDetailsList.Last();
                    previousQuestion = await _botQuestionRepository.FindByIdAsync(taskMailDetail.QuestionId);
                    question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                }
                // if previous task mail completed then it will start a new one
                if (taskMail == null || question == TaskMailQuestion.TaskMailSend)
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
        public async Task<string> QuestionAndAnswerAsync(string answer, string userId)
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
                try
                {
                    // checking for previous task mail exist or not for today
                    taskMail = await _taskMail.FirstOrDefaultAsync(x => x.EmployeeId == oAuthUser.Id &&
                    x.CreatedOn.Day == DateTime.UtcNow.Day && x.CreatedOn.Month == DateTime.UtcNow.Month &&
                    x.CreatedOn.Year == DateTime.UtcNow.Year);
                    // getting task mail details for pervious started task mail
                    var taskDetailsList = await _taskMailDetail.FetchAsync(x => x.TaskId == taskMail.Id);
                    var taskDetails = taskDetailsList.Last();
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
                                        if (answerType > 0 && answerType <= 8)
                                        {
                                            foreach(var task in taskDetailsList)
                                            {
                                                totalHourSpented += task.Hours;
                                            }
                                            //var todayTaskMail = _taskMail.Fetch(x => x.EmployeeId == taskMail.EmployeeId
                                            //&& x.CreatedOn.Date == DateTime.UtcNow.Date);
                                            //foreach (var item in todayTaskMail)
                                            //{
                                            //    var recentTask = await _taskMailDetail.FirstOrDefaultAsync(x => x.TaskId == item.Id);
                                            //    totalHourSpented += recentTask.Hours;
                                            //}
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
        public async Task<List<TaskMailUserAc>> GetAllEmployeeAsync(string userId)
        {
            var user = await _user.FirstOrDefaultAsync(x => x.Id == userId);
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
            var jsonResult = await _oauthCallsRepository.GetUserRoleAsync(user.Id, accessToken);
            var role = jsonResult.FirstOrDefault(x => x.UserName == user.UserName);
            List<TaskMailUserAc> taskMailUsertAc = new List<TaskMailUserAc>();
            if (role.Role == _stringConstant.RoleAdmin)
            {
                foreach (var j in jsonResult)
                {
                    List<TaskMail> taskMails = new List<TaskMail>();
                    var employeeId = await _userManager.FindByNameAsync(j.UserName);//_user.FirstOrDefault(x => x.UserName == j.UserName);

                    if (employeeId != null && employeeId.Id != userId)
                    {
                        TaskMailUserAc taskmailUserAc = new TaskMailUserAc
                        {
                            UserName = j.Name,
                            UserId = employeeId.Id,
                            UserRole = j.Role,
                            UserEmail = j.UserName
                        };
                        taskMailUsertAc.Add(taskmailUserAc);
                    }
                }
            }
            else if (role.Role == _stringConstant.RoleTeamLeader)
            {
                foreach (var j in jsonResult)
                {
                    List<TaskMail> taskMails = new List<TaskMail>();
                    var employeeId = await _userManager.FindByNameAsync(j.UserName);
                    if (employeeId != null)
                    {
                        TaskMailUserAc taskmailUserAc = new TaskMailUserAc
                        {
                            UserName = j.Name,
                            UserId = employeeId.Id,
                            UserRole = j.Role,
                            UserEmail = j.UserName

                        };
                        taskMailUsertAc.Add(taskmailUserAc);
                    }
                }
            }
            else if (role.Role == _stringConstant.RoleEmployee)
            {
                foreach (var j in jsonResult)
                {
                    List<TaskMail> taskMails = new List<TaskMail>();
                    var employeeId = await _userManager.FindByNameAsync(j.UserName);//_user.FirstOrDefault(x => x.UserName == j.UserName);
                    if (employeeId != null)
                    {
                        TaskMailUserAc taskmailUserAc = new TaskMailUserAc
                        {
                            UserName = j.Name,
                            UserId = employeeId.Id,
                            UserRole = j.Role,
                            UserEmail = j.UserName
                        };
                        taskMailUsertAc.Add(taskmailUserAc);
                    }
                }
            }

            return taskMailUsertAc;
        }

        /// <summary>
        /// Task Mail Details Report Information For the User Role Admin and Employee
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserRole"></param>
        /// <param name="UserName"></param>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailUserAc>> GetTaskMailDetailsInformationAsync(string UserId, string UserRole, string UserName, string LoginId)
        {
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);
            if (taskMails.Count() != 0)
            {
                var task = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                var taskMailMinDate = taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault();
                IEnumerable<TaskMailDetails> taskMailDetails = await _taskMailDetail.FetchAsync(x => x.TaskId == task.Id);
                if (taskMailDetails.Count() != 0)
                {
                    List<TaskMailDetails> taskmailDetails = new List<TaskMailDetails>();
                    taskmailDetails = taskMailDetails.ToList();
                    taskmailDetails.ForEach(taskMail =>
                    {
                        TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                        {
                            Id = taskMail.Id,
                            Description = taskMail.Description,
                            Comment = taskMail.Comment,
                            Status = taskMail.Status,
                            Hours = taskMail.Hours
                        };
                        taskMailReportAc.Add(taskmailReportAc);
                    });

                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        UserRole = UserRole,
                        CreatedOn = task.CreatedOn.Date,
                        TaskMails = taskMailReportAc,
                        IsMax = task.CreatedOn.Date,
                        IsMin = taskMailMinDate.CreatedOn.Date
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
                else
                {
                    List<TaskMailReportAc> taskMailReportObject = new List<TaskMailReportAc>();
                    TaskMailReportAc taskmailReportAcForTeamLeader = new TaskMailReportAc
                    {
                        Id = 0,
                        Description = _stringConstant.NotAvailable,
                        Comment = _stringConstant.NotAvailable,
                        Hours = 0
                    };
                    taskMailReportObject.Add(taskmailReportAcForTeamLeader);
                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        UserRole = UserRole,
                        CreatedOn = DateTime.Now.Date,
                        TaskMails = taskMailReportObject,
                        IsMax = task.CreatedOn.Date,
                        IsMin = taskMailMinDate.CreatedOn.Date
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
            }
            else
            {
                List<TaskMailReportAc> taskMailReportObject = new List<TaskMailReportAc>();
                TaskMailReportAc taskmailReportAcForTeamLeader = new TaskMailReportAc
                {
                    Id = 0,
                    Description = _stringConstant.NotAvailable,
                    Comment = _stringConstant.NotAvailable,
                    Hours = 0
                };
                taskMailReportObject.Add(taskmailReportAcForTeamLeader);
                TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                {
                    UserId = UserId,
                    UserName = UserName,
                    UserRole = UserRole,
                    CreatedOn = DateTime.Now,
                    TaskMails = taskMailReportObject
                };
                taskMailAc.Add(taskMailUserAc);
            }
            return taskMailAc;
        }

        /// <summary>
        /// This Method use to featch the task mail detils.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserRole"></param>
        /// <param name="UserName"></param>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportAsync(string UserId, string UserRole, string UserName, string LoginId)
        {
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            if (UserRole == _stringConstant.RoleAdmin || UserRole == _stringConstant.RoleEmployee)
            {
                taskMailAc = await GetTaskMailDetailsInformationAsync(UserId, UserRole, UserName, LoginId);
            }
            else if (UserRole == _stringConstant.RoleTeamLeader)
            {
                var user = await _user.FirstOrDefaultAsync(x => x.Id == LoginId);
                var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                var json = await _oauthCallsRepository.GetListOfEmployeeAsync(user.Id, accessToken);
                DateTime? maxDate = null;
                DateTime? minDate = null;
                foreach (var j in json)
                {
                    var employeeObject = await _userManager.FindByNameAsync(j.UserName);

                    if (employeeObject != null)
                    {
                        var taskMailsForTeamLeader = await _taskMail.FetchAsync(y => y.EmployeeId == employeeObject.Id);
                        if (taskMailsForTeamLeader.Count() != 0)
                        {
                            var taskForTeamLeader = taskMailsForTeamLeader.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                            if (maxDate == null)
                            {
                                maxDate = taskForTeamLeader.CreatedOn;
                            }
                            else
                            {
                                if (maxDate < taskForTeamLeader.CreatedOn)
                                {
                                    maxDate = taskForTeamLeader.CreatedOn;
                                }
                            }
                            var taskMailMinDate = taskMailsForTeamLeader.OrderBy(x => x.CreatedOn).FirstOrDefault();
                            if (minDate == null)
                            {
                                minDate = taskMailMinDate.CreatedOn;
                            }
                            else
                            {
                                if (minDate > taskMailMinDate.CreatedOn)
                                {
                                    minDate = taskMailMinDate.CreatedOn;
                                }
                            }
                        }
                    }

                }
                foreach (var j in json)
                {
                    var employee = await _userManager.FindByNameAsync(j.UserName);
                    if (employee != null)
                    {
                        var taskMailsForTeamLeader = await _taskMail.FetchAsync(y => y.EmployeeId == employee.Id && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(maxDate));
                        if (taskMailsForTeamLeader != null && taskMailsForTeamLeader.Count() != 0)
                        {
                            var taskTL = taskMailsForTeamLeader.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                            IEnumerable<TaskMailDetails> taskMailDetailsTL = await _taskMailDetail.FetchAsync(x => x.TaskId == taskTL.Id);
                            List<TaskMailDetails> taskmailDetailsForTeamLeader = new List<TaskMailDetails>();
                            taskmailDetailsForTeamLeader = taskMailDetailsTL.ToList();
                            List<TaskMailReportAc> taskMailReport = new List<TaskMailReportAc>();
                            taskmailDetailsForTeamLeader.ForEach(taskMail =>
                            {
                                TaskMailReportAc taskmailReportAcForTeamLeader = new TaskMailReportAc
                                {
                                    Id = taskMail.Id,
                                    Description = taskMail.Description,
                                    Comment = taskMail.Comment,
                                    Status = taskMail.Status,
                                    Hours = taskMail.Hours
                                };
                                taskMailReport.Add(taskmailReportAcForTeamLeader);
                            });
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = taskTL.CreatedOn.Date,
                                TaskMails = taskMailReport,
                                IsMax = Convert.ToDateTime(maxDate).Date,
                                IsMin = Convert.ToDateTime(minDate).Date
                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                        else
                        {
                            List<TaskMailReportAc> taskMailReportObject = new List<TaskMailReportAc>();
                            TaskMailReportAc taskmailReportAcForTeamLeader = new TaskMailReportAc
                            {
                                Id = 0,
                                Description = _stringConstant.NotAvailable,
                                Comment = _stringConstant.NotAvailable,
                                Hours = 0
                            };
                            taskMailReportObject.Add(taskmailReportAcForTeamLeader);
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = Convert.ToDateTime(maxDate).Date,
                                TaskMails = taskMailReportObject,
                                IsMax = Convert.ToDateTime(maxDate).Date,
                                IsMin = Convert.ToDateTime(minDate).Date

                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                    }

                }
            }
            return taskMailAc;
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
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportInformationForSelectedDateAsync(string UserId, string UserName, string UserRole, string CreatedOn, string LoginId, string SelectedDate)
        {
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            DateTime? maxDateSelectedUser = null;
            DateTime? minDateSelectedUser = null;

            DateTime slectedDateForAdmin = Convert.ToDateTime(SelectedDate).Date;
            var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(slectedDateForAdmin));
            if (taskMails.Count() != 0)
            {
                var maxRecord = await _taskMail.FetchAsync(x => x.EmployeeId == UserId);
                var maxDate = maxRecord.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                maxDateSelectedUser = maxDate.CreatedOn;
                var minRecord = await _taskMail.FetchAsync(x => x.EmployeeId == UserId);
                var minDate = minRecord.OrderBy(x => x.CreatedOn).FirstOrDefault();
                minDateSelectedUser = minDate.CreatedOn;
                if (taskMails != null)
                {
                    var task = taskMails.FirstOrDefault();
                    IEnumerable<TaskMailDetails> taskMailDetails = await _taskMailDetail.FetchAsync(x => x.TaskId == task.Id);
                    List<TaskMailDetails> taskmailDetails = new List<TaskMailDetails>();
                    taskmailDetails = taskMailDetails.ToList();
                    taskmailDetails.ForEach(taskMail =>
                    {
                        TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                        {
                            Id = taskMail.Id,
                            Description = taskMail.Description,
                            Comment = taskMail.Comment,
                            Status = taskMail.Status,
                            Hours = taskMail.Hours
                        };
                        taskMailReportAc.Add(taskmailReportAc);
                    });

                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        UserRole = UserRole,
                        CreatedOn = task.CreatedOn.Date,
                        TaskMails = taskMailReportAc,
                        IsMax = Convert.ToDateTime(maxDateSelectedUser).Date,
                        IsMin = Convert.ToDateTime(minDateSelectedUser).Date
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
                else
                {
                    List<TaskMailReportAc> taskMailReportList = new List<TaskMailReportAc>();
                    TaskMailReportAc taskmailReportAcObject = new TaskMailReportAc
                    {
                        Id = 0,
                        Description = _stringConstant.NotAvailable,
                        Comment = _stringConstant.NotAvailable,
                        Status = TaskMailStatus.completed,
                        Hours = 0
                    };
                    taskMailReportList.Add(taskmailReportAcObject);
                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        UserRole = UserRole,
                        CreatedOn = Convert.ToDateTime(SelectedDate).Date,
                        TaskMails = taskMailReportList,
                        IsMax = Convert.ToDateTime(maxDateSelectedUser).Date,
                        IsMin = Convert.ToDateTime(minDateSelectedUser).Date
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
            }
            else
            {
                TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                {
                    Id = 0,
                    Description = _stringConstant.NotAvailable,
                    Comment = _stringConstant.NotAvailable,
                    Hours = 0
                };
                taskMailReportAc.Add(taskmailReportAc);
                TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                {
                    UserId = UserId,
                    UserName = UserName,
                    UserRole = UserRole,
                    CreatedOn = Convert.ToDateTime(SelectedDate).Date,
                    TaskMails = taskMailReportAc,
                    IsMax = Convert.ToDateTime(SelectedDate).Date,
                    IsMin = Convert.ToDateTime(SelectedDate).Date,
                };
                taskMailAc.Add(taskMailUserAc);
            }
            return taskMailAc;
        }

        /// <summary>
        /// this Method use to featch the task mail details for the selected date.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserName"></param>
        /// <param name="UserRole"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="LoginId"></param>
        /// <param name="SelectedDate"></param>
        /// <returns></returns>
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportSelectedDateAsync(string UserId, string UserName, string UserRole, string CreatedOn, string LoginId, string SelectedDate)
        {
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            if (UserRole == _stringConstant.RoleAdmin || UserRole == _stringConstant.RoleEmployee)
            { taskMailAc = await TaskMailDetailsReportInformationForSelectedDateAsync(UserId, UserName, UserRole, CreatedOn, LoginId, SelectedDate); }
            else if (UserRole == _stringConstant.RoleTeamLeader)
            {
                var user = await _user.FirstOrDefaultAsync(x => x.Id == LoginId);
                var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                var json = await _oauthCallsRepository.GetListOfEmployeeAsync(user.Id, accessToken);
                DateTime? maxDate = null;
                DateTime? minDate = null;
                foreach (var j in json)
                {
                    var employeeObject = await _userManager.FindByNameAsync(j.UserName);

                    if (employeeObject != null)
                    {
                        var taskMailsRecodes = await _taskMail.FetchAsync(y => y.EmployeeId == employeeObject.Id);
                        if (taskMailsRecodes.Count() != 0)
                        {
                            var taskMailMaximumDate = taskMailsRecodes.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                            if (taskMailMaximumDate != null)
                            {
                                if (maxDate == null)
                                {
                                    maxDate = taskMailMaximumDate.CreatedOn;
                                }
                                else
                                {
                                    if (maxDate < taskMailMaximumDate.CreatedOn)
                                    {
                                        maxDate = taskMailMaximumDate.CreatedOn;
                                    }
                                }
                            }
                            var taskMailMinimumDate = taskMailsRecodes.OrderBy(x => x.CreatedOn).FirstOrDefault();
                            if (taskMailMinimumDate != null)
                            {
                                if (minDate == null)
                                {
                                    minDate = taskMailMinimumDate.CreatedOn;
                                }
                                else
                                {
                                    if (minDate > taskMailMinimumDate.CreatedOn)
                                    {
                                        minDate = taskMailMinimumDate.CreatedOn;
                                    }
                                }
                            }

                        }
                    }

                }
                foreach (var j in json)
                {
                    var employee = await _userManager.FindByNameAsync(j.UserName);
                    if (employee != null)
                    {
                        DateTime selectedDateTime = Convert.ToDateTime(SelectedDate);
                        var taskMailsTL = await _taskMail.FetchAsync(y => y.EmployeeId == employee.Id && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(selectedDateTime));
                        if (taskMailsTL != null && taskMailsTL.Count() != 0)
                        {
                            var taskTL = taskMailsTL.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                            IEnumerable<TaskMailDetails> taskMailDetailsTL = await _taskMailDetail.FetchAsync(x => x.TaskId == taskTL.Id);
                            List<TaskMailDetails> listTaskmailDetailsReport = new List<TaskMailDetails>();
                            listTaskmailDetailsReport = taskMailDetailsTL.ToList();
                            List<TaskMailReportAc> listTaskMailReport = new List<TaskMailReportAc>();
                            listTaskmailDetailsReport.ForEach(taskMail =>
                            {
                                TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                                {
                                    Id = taskMail.Id,
                                    Description = taskMail.Description,
                                    Comment = taskMail.Comment,
                                    Status = taskMail.Status,
                                    Hours = taskMail.Hours
                                };
                                listTaskMailReport.Add(taskmailReportAc);
                            });
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = taskTL.CreatedOn,
                                TaskMails = listTaskMailReport,
                                IsMax = Convert.ToDateTime(maxDate).Date,
                                IsMin = Convert.ToDateTime(minDate).Date

                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                        else
                        {
                            List<TaskMailReportAc> taskMailReport = new List<TaskMailReportAc>();
                            TaskMailReportAc taskmailReportAcTL = new TaskMailReportAc
                            {
                                Id = 0,
                                Description = _stringConstant.NotAvailable,
                                Comment = _stringConstant.NotAvailable,
                                Status = TaskMailStatus.completed,
                                Hours = 0
                            };
                            taskMailReport.Add(taskmailReportAcTL);
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = Convert.ToDateTime(SelectedDate).Date,
                                TaskMails = taskMailReport,
                                IsMax = Convert.ToDateTime(maxDate).Date,
                                IsMin = Convert.ToDateTime(minDate).Date
                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                    }


                }
            }
            return taskMailAc;
        }


        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportInformationForNextPreviousDateAsync(string UserId, string UserName, string UserRole, DateTime CreatedOn, string LoginId)
        {
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            DateTime? minDate = null;
            DateTime? maxDate = null;
            var listOfTask = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);
            if (listOfTask != null)
            {
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(CreatedOn));
                //var taskMailDetail = taskMails.Where(x => DbFunctions.TruncateTime(x.CreatedOn) == DbFunctions.TruncateTime(CreatedOn));
                var task = taskMails.OrderByDescending(y => y.CreatedOn).FirstOrDefault();
                var taskMailDates = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);
                var taskMinDate = taskMailDates.OrderBy(y => y.CreatedOn).FirstOrDefault();
                minDate = taskMinDate.CreatedOn;
                var taskMaxDate = taskMailDates.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                maxDate = taskMaxDate.CreatedOn;
                if (task != null)
                {
                    IEnumerable<TaskMailDetails> taskMailDetails = await _taskMailDetail.FetchAsync(x => x.TaskId == task.Id);
                    List<TaskMailDetails> taskmailDetails = new List<TaskMailDetails>();
                    taskmailDetails = taskMailDetails.ToList();
                    taskmailDetails.ForEach(taskMail =>
                    {
                        TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                        {
                            Id = taskMail.Id,
                            Description = taskMail.Description,
                            Comment = taskMail.Comment,
                            Status = taskMail.Status,
                            Hours = taskMail.Hours
                        };
                        taskMailReportAc.Add(taskmailReportAc);
                    });

                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        UserRole = UserRole,
                        CreatedOn = task.CreatedOn.Date,
                        TaskMails = taskMailReportAc,
                        IsMin = Convert.ToDateTime(minDate).Date,
                        IsMax = Convert.ToDateTime(maxDate).Date
                    };
                    taskMailAc.Add(taskMailUserAc);

                }
                else
                {
                    List<TaskMailReportAc> taskMailReport = new List<TaskMailReportAc>();
                    TaskMailReportAc taskmailReportAcTL = new TaskMailReportAc
                    {
                        Id = 0,
                        Description = _stringConstant.NotAvailable,
                        Comment = _stringConstant.NotAvailable,
                        Hours = 0
                    };
                    taskMailReport.Add(taskmailReportAcTL);
                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        UserRole = UserRole,
                        CreatedOn = Convert.ToDateTime(CreatedOn).Date,
                        TaskMails = taskMailReport,
                        IsMin = Convert.ToDateTime(minDate).Date,
                        IsMax = Convert.ToDateTime(maxDate).Date

                    };
                    taskMailAc.Add(taskMailUserAc);
                }
            }
            else
            {
                List<TaskMailReportAc> taskMailReport = new List<TaskMailReportAc>();
                TaskMailReportAc taskmailReportAcTL = new TaskMailReportAc
                {
                    Id = 0,
                    Description = _stringConstant.NotAvailable,
                    Comment = _stringConstant.NotAvailable,
                    Hours = 0
                };
                taskMailReport.Add(taskmailReportAcTL);
                TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                {
                    UserId = UserId,
                    UserName = UserName,
                    UserRole = UserRole,
                    CreatedOn = Convert.ToDateTime(CreatedOn).Date,
                    TaskMails = taskMailReport,
                    IsMin = Convert.ToDateTime(CreatedOn).Date,
                    IsMax = Convert.ToDateTime(CreatedOn).Date

                };
                taskMailAc.Add(taskMailUserAc);
            }
            return taskMailAc;
        }


        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportNextPreviousDateAsync(string UserId, string UserName, string UserRole, string CreatedOn, string LoginId, string Type)
        {
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            DateTime? CreatedDate = null;
            if (Type == _stringConstant.NextPage)
            { CreatedDate = Convert.ToDateTime(CreatedOn).AddDays(+1); }
            else
            { CreatedDate = Convert.ToDateTime(CreatedOn).AddDays(-1); }

            if (UserRole == _stringConstant.RoleAdmin || UserRole == _stringConstant.RoleEmployee)
            {
                taskMailAc = await TaskMailDetailsReportInformationForNextPreviousDateAsync(UserId, UserName, UserRole, Convert.ToDateTime(CreatedDate), LoginId);
            }
            else if (UserRole == _stringConstant.RoleTeamLeader)
            {
                var user = await _user.FirstOrDefaultAsync(x => x.Id == LoginId);
                var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                var json = await _oauthCallsRepository.GetListOfEmployeeAsync(user.Id, accessToken);
                DateTime? minDate = null;
                DateTime? maxDate = null;

                foreach (var j in json)
                {
                    var employeeObject = await _userManager.FindByNameAsync(j.UserName);

                    if (employeeObject != null)
                    {
                        var employeesTaskMails = await _taskMail.FetchAsync(y => y.EmployeeId == employeeObject.Id);
                        if (employeesTaskMails.Count() != 0)
                        {
                            var taskMailMinDate = employeesTaskMails.OrderBy(x => x.CreatedOn).FirstOrDefault();
                            if (taskMailMinDate != null)
                            {
                                if (minDate == null)
                                {
                                    minDate = taskMailMinDate.CreatedOn;
                                }
                                else
                                {
                                    if (minDate > taskMailMinDate.CreatedOn)
                                    {
                                        minDate = taskMailMinDate.CreatedOn;
                                    }
                                }
                            }

                            var taskMailMaxate = employeesTaskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                            if (taskMailMaxate != null)
                            {
                                if (maxDate == null)
                                {
                                    maxDate = taskMailMaxate.CreatedOn;
                                }
                                else
                                {
                                    if (maxDate < taskMailMaxate.CreatedOn)
                                    {
                                        maxDate = taskMailMaxate.CreatedOn;
                                    }
                                }
                            }

                        }
                    }

                }

                foreach (var j in json)
                {
                    var employee = await _userManager.FindByNameAsync(j.UserName);
                    if (employee != null)
                    {
                        var employeeTaskMails = await _taskMail.FetchAsync(y => y.EmployeeId == employee.Id && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(CreatedDate));
                        if (employeeTaskMails != null && employeeTaskMails.Count() != 0)
                        {
                            var taskMails = employeeTaskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                            IEnumerable<TaskMailDetails> employeeTaskMailDetails = await _taskMailDetail.FetchAsync(x => x.TaskId == taskMails.Id);
                            List<TaskMailDetails> taskmailDetails = new List<TaskMailDetails>();
                            taskmailDetails = employeeTaskMailDetails.ToList();
                            List<TaskMailReportAc> taskMailDetailsReport = new List<TaskMailReportAc>();
                            taskmailDetails.ForEach(taskMail =>
                            {
                                TaskMailReportAc taskmailDetailsReportAc = new TaskMailReportAc
                                {
                                    Id = taskMail.Id,
                                    Description = taskMail.Description,
                                    Comment = taskMail.Comment,
                                    Status = taskMail.Status,
                                    Hours = taskMail.Hours
                                };
                                taskMailDetailsReport.Add(taskmailDetailsReportAc);
                            });
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = taskMails.CreatedOn.Date,
                                TaskMails = taskMailDetailsReport,
                                IsMin = Convert.ToDateTime(minDate).Date,
                                IsMax = Convert.ToDateTime(maxDate).Date
                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                        else
                        {
                            List<TaskMailReportAc> taskMailReport = new List<TaskMailReportAc>();
                            TaskMailReportAc taskmailDetailsReportAc = new TaskMailReportAc
                            {
                                Id = 0,
                                Description = _stringConstant.NotAvailable,
                                Comment = _stringConstant.NotAvailable,
                                Hours = 0
                            };
                            taskMailReport.Add(taskmailDetailsReportAc);
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = Convert.ToDateTime(CreatedDate).Date,
                                TaskMails = taskMailReport,
                                IsMin = Convert.ToDateTime(minDate).Date,
                                IsMax = Convert.ToDateTime(maxDate).Date
                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                    }

                }

            }
            return taskMailAc;
        }
        #endregion
    }
}
