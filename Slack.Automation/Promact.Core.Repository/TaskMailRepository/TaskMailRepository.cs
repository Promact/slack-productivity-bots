using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Promact.Erp.Util;
using System.Collections.Generic;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.Util.Email;
using Newtonsoft.Json;
using System.Data.Entity;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Erp.DomainModel.DataRepository;
using System.Net.Mail;

namespace Promact.Core.Repository.TaskMailRepository
{
    public class TaskMailRepository : ITaskMailRepository
    {
        private readonly IRepository<TaskMail> _taskMail;
        private readonly IRepository<TaskMailDetails> _taskMailDetail;
        private readonly IProjectUserCallRepository _projectUserRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _user;
        private readonly IEmailService _emailService;
        private readonly ApplicationUserManager _userManager;
        
        string questionText = "";
        public TaskMailRepository(IRepository<TaskMail> taskMail, IProjectUserCallRepository projectUserRepository, IRepository<TaskMailDetails> taskMailDetail, IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> user, IEmailService emailService, IBotQuestionRepository botQuestionRepository, ApplicationUserManager userManager)
        {
            _taskMail = taskMail;
            _projectUserRepository = projectUserRepository;
            _taskMailDetail = taskMailDetail;
            _attachmentRepository = attachmentRepository;
            _user = user;
            _emailService = emailService;
            _botQuestionRepository = botQuestionRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Method to start task mail
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>questionText in string format containing question statement</returns>
        public async Task<string> StartTaskMail(string userName)
        {
            // getting user name from user's slack name
            var user = _user.FirstOrDefault(x => x.SlackUserName == userName);
            // getting access token for that user
            if (user != null)
            {
                // get access token of user for promact oauth server
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                // get user details from
                var oAuthUser = await _projectUserRepository.GetUserByUsername(userName, accessToken);
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
                    taskMailDetail = _taskMailDetail.FirstOrDefault(x => x.TaskId == taskMail.Id);
                    previousQuestion = _botQuestionRepository.FindById(taskMailDetail.QuestionId);
                    question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                }
                // if previous task mail completed then it will start a new one
                if (taskMail == null || question == TaskMailQuestion.TaskMailSend)
                {
                    // Before going to create new task it will check whether the user has send mail for today or not.
                    if (taskMailDetail != null && taskMailDetail.SendEmailConfirmation == SendEmailConfirmation.yes)
                        // If mail is send then user will not be able to add task mail for that day
                        questionText = StringConstant.AlreadyMailSend;
                    else
                    {
                        // If mail is not send then user will be able to add task mail for that day
                        taskMail = new TaskMail();
                        taskMail.CreatedOn = DateTime.UtcNow;
                        taskMail.EmployeeId = oAuthUser.Id;
                        _taskMail.Insert(taskMail);
                        _taskMail.Save();
                        // getting first question of type 2
                        var firstQuestion = _botQuestionRepository.FindByQuestionType(2);
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
                    var questionText = await QuestionAndAnswer(userName, null);
                }
            }
            else
                // if user doesn't exist then this message will be shown to user
                questionText = StringConstant.YouAreNotInExistInOAuthServer;
            return questionText;
        }

        /// <summary>
        /// Method to conduct task mail after started
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="answer"></param>
        /// <returns>questionText in string format containing question statement</returns>
        public async Task<string> QuestionAndAnswer(string userName, string answer)
        {
            // getting user name from user's slack name
            var user = _user.FirstOrDefault(x => x.SlackUserName == userName);
            if (user != null)
            {
                // getting access token for that user
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                List<TaskMailDetails> taskList = new List<TaskMailDetails>();
                TaskMail taskMail = new TaskMail();
                // getting user information from Promact Oauth Server
                var oAuthUser = await _projectUserRepository.GetUserByUsername(userName, accessToken);
                try
                {
                    // checking for previous task mail exist or not for today
                    taskMail = _taskMail.Fetch(x => x.EmployeeId == oAuthUser.Id && x.CreatedOn.Date == DateTime.UtcNow.Date).Last();
                    // getting task mail details for pervious started task mail
                    var taskDetails = _taskMailDetail.FirstOrDefault(x => x.TaskId == taskMail.Id);
                    // getting inform of previous question asked to user
                    var previousQuestion = _botQuestionRepository.FindById(taskDetails.QuestionId);
                    // checking if precious question was last and answer by user and previous task report was completed then asked for new task mail
                    if (previousQuestion.OrderNumber <= 7)
                    {
                        // getting next question to be asked to user
                        var nextQuestion = _botQuestionRepository.FindByTypeAndOrderNumber((previousQuestion.OrderNumber + 1), 2);
                        // Converting question Ordr to enum
                        var question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
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
                                        // checking range of hours
                                        if (hour > 0 && hour < 8)
                                        {
                                            taskDetails.Hours = hour;
                                            questionText = nextQuestion.QuestionStatement;
                                            taskDetails.QuestionId = nextQuestion.Id;
                                        }
                                        else
                                            // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                                            questionText = string.Format("{0}{1}{2}", StringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
                                    else
                                        // if previous question was hour of task and it was null or wrong value then answer will ask for hour again
                                        questionText = string.Format("{0}{1}{2}", StringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
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
                                        questionText = string.Format("{0}{1}{2}", StringConstant.TaskMailBotStatusErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
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
                                        var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation), answer.ToLower().ToString());
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
                                                    nextQuestion = _botQuestionRepository.FindByTypeAndOrderNumber(7, 2);
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    questionText = StringConstant.ThankYou;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                        // if previous question was send email of task and answer was null/wrong value then answer will say thank you and task mail stopped
                                        questionText = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
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
                                        var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation), answer.ToLower().ToString());
                                        questionText = StringConstant.ThankYou;
                                        switch (confirmation)
                                        {
                                            case SendEmailConfirmation.yes:
                                                {
                                                    // if previous question was confirm send email of task and it was not null/wrong value then answer will send email and reply back with thank you and task mail stopped
                                                    taskDetails.SendEmailConfirmation = SendEmailConfirmation.yes;
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    // get list of task done and register for today for that user
                                                    var taskMailList = _taskMail.Fetch(x => x.EmployeeId == oAuthUser.Id && x.CreatedOn.Date == DateTime.UtcNow.Date);
                                                    foreach (var item in taskMailList)
                                                    {
                                                        var taskDetail = _taskMailDetail.FirstOrDefault(x => x.TaskId == item.Id);
                                                        taskList.Add(taskDetail);
                                                    }
                                                    var teamLeaders = await _projectUserRepository.GetTeamLeaderUserName(userName, accessToken);
                                                    var managements = await _projectUserRepository.GetManagementUserName(accessToken);
                                                    foreach (var management in managements)
                                                    {
                                                        teamLeaders.Add(management);
                                                    }
                                                    foreach (var teamLeader in teamLeaders)
                                                    {
                                                        // transforming task mail details to template page and getting as string
                                                        var emailBody = EmailServiceTemplateTaskMail(taskList);
                                                        EmailApplication email = new EmailApplication();
                                                        email.Body = emailBody;
                                                        email.From = oAuthUser.Email;
                                                        email.To = teamLeader.Email;
                                                        email.Subject = StringConstant.TaskMailSubject;
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
                                        questionText = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                }
                                break;
                            default:
                                    questionText = StringConstant.RequestToStartTaskMail;
                                break;
                        }
                        _taskMailDetail.Update(taskDetails);
                        _taskMail.Save();
                    }
                }
                catch (SmtpException ex)
                {
                    // error message will be send to email. But leave will be applied
                    questionText = string.Format("{0}. {1}", StringConstant.ErrorOfEmailServiceFailureTaskMail, ex.Message.ToString());
                }
                catch (Exception)
                {
                    // if previous task mail doesnot exist then ask user to start task mail
                    questionText = StringConstant.RequestToStartTaskMail;
                }
            }
            else
                // if user doesn't exist in oAuth server
                questionText = StringConstant.YouAreNotInExistInOAuthServer;
            return questionText;
        }

        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">TaskMail template object</param>
        /// <returns>template emailBody as string</returns>
        private string EmailServiceTemplateTaskMail(List<TaskMailDetails> taskMail)
        {
            Erp.Util.Email_Templates.TaskMail leaveTemplate = new Erp.Util.Email_Templates.TaskMail();
            // Assigning Value in template page
            leaveTemplate.Session = new Dictionary<string, object>
            {
                {StringConstant.TaskMailDescription, taskMail},
            };
            leaveTemplate.Initialize();
            var emailBody = leaveTemplate.TransformText();
            return emailBody;
        }

        /// <summary>
        /// Method geting the user/users information 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailReportAc>> TaskMailReport(string userId,int currentPage,int itemsPerPage)
        {
            var user = _user.FirstOrDefault(x => x.Id == userId);
            var accessToken = await _attachmentRepository.AccessToken(user.UserName);
            var Json = await _projectUserRepository.GetUserRole(user.UserName, accessToken);
            var role = Json.FirstOrDefault(x => x.UserName == user.UserName);
            var skip=0;
            if (currentPage == 1)
            { skip = 0; }
            else { skip = (currentPage * itemsPerPage) - itemsPerPage; }
            var take = itemsPerPage;
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            if (role.Role == StringConstant.RoleAdmin)
            {
                List<TaskMail> taskMails = new List<TaskMail>();
                foreach (var j in Json)
                {
                    var employeeId = await _userManager.FindByNameAsync(j.UserName);//_user.FirstOrDefault(x => x.UserName == j.UserName);
                    if (employeeId != null)
                    {
                        taskMails = _taskMail.Fetch(x => x.EmployeeId == employeeId.Id).OrderByDescending(o => o.CreatedOn).ToList();
                        taskMails.ForEach(taskMail =>
                        {
                            TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                            {
                                Id = taskMail.Id,
                                UserName = j.Name,
                                CreatedOn = taskMail.CreatedOn
                            };
                            taskMailReportAc.Add(taskmailReportAc);
                        });
                    }
                }
            }
            else if (role.Role == StringConstant.RoleTeamLeader)
            {
                List<TaskMail> taskMails = new List<TaskMail>();
                foreach (var t in Json)
                {
                    //var employeeId = _user.FirstOrDefault(x => x.UserName == t.UserName);
                    var employeeIds = await _userManager.FindByNameAsync(t.UserName);
                    if (employeeIds != null)
                    {
                        taskMails = _taskMail.Fetch(x => x.EmployeeId == employeeIds.Id).OrderByDescending(o => o.CreatedOn).ToList();
                        taskMails.ForEach(taskMail =>
                        {
                            TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                            {
                                Id = taskMail.Id,
                                UserName = t.Name,
                                CreatedOn = taskMail.CreatedOn
                            };
                            taskMailReportAc.Add(taskmailReportAc);
                        });
                    }
                }
               
            }
            else
            {
                List<TaskMail> taskMails = new List<TaskMail>();
                var employee = await _userManager.FindByNameAsync(role.UserName);//_user.FirstOrDefault(x => x.UserName == role.UserName);
                taskMails = _taskMail.Fetch(x => x.EmployeeId == employee.Id).OrderByDescending(o => o.CreatedOn).ToList();
                taskMails.ForEach(taskMail =>
                {
                    TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                    {
                        Id = taskMail.Id,
                        UserName = role.Name,
                        CreatedOn = taskMail.CreatedOn
                    };
                    taskMailReportAc.Add(taskmailReportAc);
                });

              
            }
            foreach (var taskMail in taskMailReportAc)
            {
                taskMail.TotalItems = taskMailReportAc.Count;
            }
            return taskMailReportAc.OrderByDescending(o => o.CreatedOn).Skip(skip).Take(take).ToList();
            //return taskMailReportAc.OrderByDescending(o => o.CreatedOn).ToList();
        }
        
        /// <summary>
        ///Method geting list of Employee 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailUserAc>> getAllEmployee(string userId)
        {
            var user = _user.FirstOrDefault(x => x.Id == userId);
            var accessToken = await _attachmentRepository.AccessToken(user.UserName);
            var jsonResult = await _projectUserRepository.GetUserRole(user.UserName, accessToken);
            var role = jsonResult.FirstOrDefault(x => x.UserName == user.UserName);
            List<TaskMailUserAc> taskMailUsertAc = new List<TaskMailUserAc>();
            if (role.Role == StringConstant.RoleAdmin)
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
                            UserEmail=j.UserName
                        };
                        taskMailUsertAc.Add(taskmailUserAc);
                    }
                }
            }
            else if (role.Role == StringConstant.RoleTeamLeader)
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
            else if(role.Role == StringConstant.RoleEmployee)
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
        /// This Method use to featch the task mail detils.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserRole"></param>
        /// <param name="UserName"></param>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReport(string UserId,string UserRole,string UserName,string LoginId)
        {
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            if (UserRole == StringConstant.RoleAdmin)
            {
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);
                var task = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                var taskMailMinDate = taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault();
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
                    CreatedOn = task.CreatedOn,
                    TaskMails = taskMailReportAc,
                    isMax=task.CreatedOn,
                    isMin= taskMailMinDate.CreatedOn
                };
                taskMailAc.Add(taskMailUserAc);

            }
            else if (UserRole == StringConstant.RoleTeamLeader)
            {
                var user = _user.FirstOrDefault(x => x.Id == LoginId);
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                var json = await _projectUserRepository.GetListOfEmployee(user.UserName, accessToken);
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
                        var taskMailsForTeamLeader = await _taskMail.FetchAsync(y => y.EmployeeId == employee.Id && y.CreatedOn == maxDate);
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
                                UserRole= UserRole,
                                CreatedOn = taskTL.CreatedOn,
                                TaskMails = taskMailReport,
                                isMax=Convert.ToDateTime(maxDate),
                                isMin=Convert.ToDateTime(minDate)
                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                        else {
                            List<TaskMailReportAc> taskMailReportObject = new List<TaskMailReportAc>();
                            TaskMailReportAc taskmailReportAcForTeamLeader = new TaskMailReportAc
                            {
                                Id = 0,
                                Description = "Not Available",
                                Comment = "Not Available",
                                Status = TaskMailStatus.completed,
                                Hours = 0
                            };
                            taskMailReportObject.Add(taskmailReportAcForTeamLeader);
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = Convert.ToDateTime(maxDate),
                                TaskMails = taskMailReportObject,
                                isMax = Convert.ToDateTime(maxDate),
                                isMin = Convert.ToDateTime(minDate)

                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                    }

                }
            }
            else if (UserRole == StringConstant.RoleEmployee)
            {
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);
                if (taskMails.Count() != 0)
                {
                    var task = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                    var taskMIn = taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault();
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
                        CreatedOn = task.CreatedOn,
                        TaskMails = taskMailReportAc,
                        isMax=task.CreatedOn,
                        isMin= taskMIn.CreatedOn
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
                else {
                    TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                    {
                        Id = 0,
                        Description = "Not Available",
                        Comment = "Not Available",
                        Status = TaskMailStatus.completed,
                        Hours = 0
                    };
                    taskMailReportAc.Add(taskmailReportAc);
                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        CreatedOn = DateTime.Now,
                        TaskMails = taskMailReportAc

                    };
                    taskMailAc.Add(taskMailUserAc);
                }
            }
            return taskMailAc;
        }

        /// <summary>
        /// This Method use to featch the task mail details for the Previous Date.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserName"></param>
        /// <param name="UserRole"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportPreviousDate(string UserId, string UserName, string UserRole, string CreatedOn, string LoginId)
        {
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            if (UserRole == StringConstant.RoleAdmin)
            {
                DateTime? isMinSelectedUser = null;
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);
                DateTime datedesc = Convert.ToDateTime(CreatedOn);
                var t = taskMails.Where(x => x.CreatedOn < datedesc);
                var task = t.OrderByDescending(y => y.CreatedOn).FirstOrDefault();
                var taskmindate= taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault();
                isMinSelectedUser = taskmindate.CreatedOn;
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
                    UserRole=UserRole,
                    CreatedOn = task.CreatedOn,
                    TaskMails = taskMailReportAc,
                    isMin = Convert.ToDateTime(isMinSelectedUser)
                };
                taskMailAc.Add(taskMailUserAc);

            }
            else if (UserRole == StringConstant.RoleTeamLeader)
            {
                var user = _user.FirstOrDefault(x => x.Id == LoginId);
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                var json = await _projectUserRepository.GetListOfEmployee(user.UserName, accessToken);
                DateTime? maxDate = null;
                DateTime? isMin= null;
                foreach (var j in json)
                {
                    var employeeObject = await _userManager.FindByNameAsync(j.UserName);

                    if (employeeObject != null)
                    {
                        var taskMailsTLs = await _taskMail.FetchAsync(y => y.EmployeeId == employeeObject.Id);
                        if (taskMailsTLs.Count() != 0)
                        {
                            var taskTLisMin = taskMailsTLs.OrderBy(x => x.CreatedOn).FirstOrDefault();
                            if (taskTLisMin != null)
                            {
                                if (isMin == null)
                                {
                                    isMin = taskTLisMin.CreatedOn;
                                }
                                else
                                {
                                    if (isMin > taskTLisMin.CreatedOn)
                                    {
                                        isMin = taskTLisMin.CreatedOn;
                                    }
                                }
                            }

                            var taskTLs = taskMailsTLs.Where(y=>y.CreatedOn<Convert.ToDateTime(CreatedOn)).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                            if (taskTLs != null)
                            {
                                if (maxDate == null)
                                {
                                    maxDate = taskTLs.CreatedOn;
                                }
                                else
                                {
                                    if (maxDate < taskTLs.CreatedOn)
                                    {
                                        maxDate = taskTLs.CreatedOn;
                                    }
                                }
                            }
                        }
                    }

                }
                //DateTime d = new DateTime();
                //d=Convert.ToDateTime(maxDate);
                if (maxDate != null)
                {
                    foreach (var j in json)
                    {
                        var employee = await _userManager.FindByNameAsync(j.UserName);
                        if (employee != null)
                        {
                            var taskMailsTL = await _taskMail.FetchAsync(y => y.EmployeeId == employee.Id && y.CreatedOn == maxDate);
                            if (taskMailsTL != null && taskMailsTL.Count() != 0)
                            {
                                var taskTL = taskMailsTL.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                                IEnumerable<TaskMailDetails> taskMailDetailsTL = await _taskMailDetail.FetchAsync(x => x.TaskId == taskTL.Id);
                                List<TaskMailDetails> taskmailDetailsTL = new List<TaskMailDetails>();
                                taskmailDetailsTL = taskMailDetailsTL.ToList();
                                List<TaskMailReportAc> taskMailReportAc1 = new List<TaskMailReportAc>();
                                taskmailDetailsTL.ForEach(taskMail =>
                                {
                                    TaskMailReportAc taskmailReportAcTL = new TaskMailReportAc
                                    {
                                        Id = taskMail.Id,
                                        Description = taskMail.Description,
                                        Comment = taskMail.Comment,
                                        Status = taskMail.Status,
                                        Hours = taskMail.Hours
                                    };
                                    taskMailReportAc1.Add(taskmailReportAcTL);
                                });
                                TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                                {
                                    UserId = employee.Id,
                                    UserName = j.Name,
                                    UserRole = UserRole,
                                    CreatedOn = taskTL.CreatedOn,
                                    TaskMails = taskMailReportAc1,
                                    isMin = Convert.ToDateTime(isMin)
                                };
                                taskMailAc.Add(taskMailUserAc);
                            }
                            else
                            {
                                List<TaskMailReportAc> taskMailReportAc2 = new List<TaskMailReportAc>();
                                TaskMailReportAc taskmailReportAcTL = new TaskMailReportAc
                                {
                                    Id = 0,
                                    Description = "Not Available",
                                    Comment = "Not Available",
                                    Status = TaskMailStatus.completed,
                                    Hours = 0
                                };
                                taskMailReportAc2.Add(taskmailReportAcTL);
                                TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                                {
                                    UserId = employee.Id,
                                    UserName = j.Name,
                                    UserRole = UserRole,
                                    CreatedOn = Convert.ToDateTime(maxDate),
                                    TaskMails = taskMailReportAc2,
                                    isMin = Convert.ToDateTime(isMin)

                                };
                                taskMailAc.Add(taskMailUserAc);
                            }
                        }

                    }
                }
                else
                {
                    foreach (var j in json)
                    {
                        var employee = await _userManager.FindByNameAsync(j.UserName);
                        if (employee != null)
                        {
                            List<TaskMailReportAc> taskMailReportAc2 = new List<TaskMailReportAc>();
                            TaskMailReportAc taskmailReportAcTL = new TaskMailReportAc
                            {
                                Id = 0,
                                Description = "Not Available",
                                Comment = "Not Available",
                                Status = TaskMailStatus.completed,
                                Hours = 0
                            };
                            taskMailReportAc2.Add(taskmailReportAcTL);
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = Convert.ToDateTime(maxDate),
                                TaskMails = taskMailReportAc2

                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                    }
                }
            }
            else if (UserRole == StringConstant.RoleEmployee)
            {
                DateTime? isMinEmployee = null;
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);
                if (taskMails.Count() != 0)
                {
                    DateTime datedesc = Convert.ToDateTime(CreatedOn);
                    var taskEmployee = taskMails.Where(x => x.CreatedOn < datedesc);
                    var task = taskEmployee.OrderByDescending(y => y.CreatedOn).FirstOrDefault();
                    //var task = taskMails.OrderByDescending(x => Convert.ToDateTime(x.CreatedOn) < Convert.ToDateTime(CreatedOn)).FirstOrDefault();
                    var taskmindate = taskMails.OrderBy(x => x.CreatedOn).FirstOrDefault();
                    isMinEmployee = taskmindate.CreatedOn;
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
                        CreatedOn = task.CreatedOn,
                        TaskMails = taskMailReportAc,
                        isMin = Convert.ToDateTime(isMinEmployee)
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
                else
                {
                    //List<TaskMailDetails> taskmailDetails = new List<TaskMailDetails>();
                    TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                    {
                        Id = 0,
                        Description = "Not Available",
                        Comment = "Not Available",
                        Status = TaskMailStatus.completed,
                        Hours = 0
                    };
                    taskMailReportAc.Add(taskmailReportAc);
                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        CreatedOn = DateTime.Now,
                        TaskMails = taskMailReportAc

                    };
                    taskMailAc.Add(taskMailUserAc);
                }
            }
            return taskMailAc;
        }
        
        /// <summary>
        /// This Method use to featch the task mail details for the Next Date.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserName"></param>
        /// <param name="UserRole"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportNextDate(string UserId, string UserName, string UserRole, string CreatedOn, string LoginId)
        {
           
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            if (UserRole == StringConstant.RoleAdmin)
            {
                DateTime? maxDateSelectedUser = null;
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);
                var task = taskMails.OrderByDescending(x => Convert.ToDateTime(x.CreatedOn) > Convert.ToDateTime(CreatedOn)).FirstOrDefault();
                var taskmaxdate = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                maxDateSelectedUser = taskmaxdate.CreatedOn;
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
                    CreatedOn = task.CreatedOn,
                    TaskMails = taskMailReportAc,
                    isMax = Convert.ToDateTime(maxDateSelectedUser)
                };
                taskMailAc.Add(taskMailUserAc);

            }
            else if (UserRole == StringConstant.RoleTeamLeader)
            {
                var user = _user.FirstOrDefault(x => x.Id == LoginId);
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                var json = await _projectUserRepository.GetListOfEmployee(user.UserName, accessToken);
                DateTime? maxDate = null;
                DateTime? isMax = null;
                foreach (var j in json)
                {
                    var employeeObject = await _userManager.FindByNameAsync(j.UserName);

                    if (employeeObject != null)
                    {
                        var taskMailsObject = await _taskMail.FetchAsync(y => y.EmployeeId == employeeObject.Id);
                        if (taskMailsObject.Count() != 0)
                        {
                            var taskMailMaxDate = taskMailsObject.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                            if (taskMailMaxDate != null)
                            {
                                if (isMax == null)
                                {
                                    isMax = taskMailMaxDate.CreatedOn;
                                }
                                else
                                {
                                    if (isMax < taskMailMaxDate.CreatedOn)
                                    {
                                        isMax = taskMailMaxDate.CreatedOn;
                                    }
                                }
                            }

                            var taskMailRecode = taskMailsObject.Where(y => y.CreatedOn > Convert.ToDateTime(CreatedOn)).OrderBy(x => x.CreatedOn).FirstOrDefault();
                            if (taskMailRecode != null)
                            {
                                if (maxDate == null)
                                {
                                    maxDate = taskMailRecode.CreatedOn;
                                }
                                else
                                {
                                    if (maxDate > taskMailRecode.CreatedOn)
                                    {
                                        maxDate = taskMailRecode.CreatedOn;
                                    }
                                }
                            }
                        }
                    }

                }
                //DateTime d = new DateTime();
                //d=Convert.ToDateTime(maxDate);
                if (maxDate != null)
                {
                    foreach (var j in json)
                    {
                        var employee = await _userManager.FindByNameAsync(j.UserName);
                        if (employee != null)
                        {
                            var listTaskMails = await _taskMail.FetchAsync(y => y.EmployeeId == employee.Id && y.CreatedOn == maxDate);
                            if (listTaskMails != null && listTaskMails.Count() != 0)
                            {
                                var taskTL = listTaskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                                IEnumerable<TaskMailDetails> taskMailDetailsTL = await _taskMailDetail.FetchAsync(x => x.TaskId == taskTL.Id);
                                List<TaskMailDetails> listTaskmailDetails = new List<TaskMailDetails>();
                                listTaskmailDetails = taskMailDetailsTL.ToList();
                                List<TaskMailReportAc> listTaskMailReportAc = new List<TaskMailReportAc>();
                                listTaskmailDetails.ForEach(taskMail =>
                                {
                                    TaskMailReportAc taskMailReportAcObject = new TaskMailReportAc
                                    {
                                        Id = taskMail.Id,
                                        Description = taskMail.Description,
                                        Comment = taskMail.Comment,
                                        Status = taskMail.Status,
                                        Hours = taskMail.Hours
                                    };
                                    listTaskMailReportAc.Add(taskMailReportAcObject);
                                });
                                TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                                {
                                    UserId = employee.Id,
                                    UserName = j.Name,
                                    UserRole = UserRole,
                                    CreatedOn = taskTL.CreatedOn,
                                    TaskMails = listTaskMailReportAc,
                                    isMax = Convert.ToDateTime(isMax)

                                };
                                taskMailAc.Add(taskMailUserAc);
                            }
                            else
                            {
                                List<TaskMailReportAc> listsTaskMailReportAc = new List<TaskMailReportAc>();
                                TaskMailReportAc taskMailReportObject = new TaskMailReportAc
                                {
                                    Id = 0,
                                    Description = "Not Available",
                                    Comment = "Not Available",
                                    Status = TaskMailStatus.completed,
                                    Hours = 0
                                };
                                listsTaskMailReportAc.Add(taskMailReportObject);
                                TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                                {
                                    UserId = employee.Id,
                                    UserName = j.Name,
                                    UserRole = UserRole,
                                    CreatedOn = Convert.ToDateTime(maxDate),
                                    TaskMails = listsTaskMailReportAc,
                                    isMax = Convert.ToDateTime(isMax)

                                };
                                taskMailAc.Add(taskMailUserAc);
                            }
                        }

                    }
                }
                else
                {
                    foreach (var j in json)
                    {
                        var employee = await _userManager.FindByNameAsync(j.UserName);
                        if (employee != null)
                        {
                            List<TaskMailReportAc> listTaskMailReportAc = new List<TaskMailReportAc>();
                            TaskMailReportAc taskMailReportObject = new TaskMailReportAc
                            {
                                Id = 0,
                                Description = "Not Available",
                                Comment = "Not Available",
                                Status = TaskMailStatus.completed,
                                Hours = 0
                            };
                            listTaskMailReportAc.Add(taskMailReportObject);
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = Convert.ToDateTime(maxDate),
                                TaskMails = listTaskMailReportAc

                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                    }
                }
            }
            else if (UserRole == StringConstant.RoleEmployee)
            {
                DateTime? maxDateEmployee = null;
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);
                
                if (taskMails.Count() != 0)
                {
                    var task = taskMails.OrderByDescending(x => Convert.ToDateTime(x.CreatedOn) > Convert.ToDateTime(CreatedOn)).FirstOrDefault();
                    var taskmaxdate = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                    maxDateEmployee = taskmaxdate.CreatedOn;
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
                        UserRole=UserRole,
                        CreatedOn = task.CreatedOn,
                        TaskMails = taskMailReportAc,
                        isMax=Convert.ToDateTime(maxDateEmployee)
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
                else
                {
                    TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                    {
                        Id = 0,
                        Description = "Not Available",
                        Comment = "Not Available",
                        Status = TaskMailStatus.completed,
                        Hours = 0
                    };
                    taskMailReportAc.Add(taskmailReportAc);
                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        UserRole=UserRole,
                        CreatedOn = DateTime.Now,
                        TaskMails = taskMailReportAc
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
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
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportSelectedDate(string UserId, string UserName, string UserRole, string CreatedOn,  string LoginId, string SelectedDate)
        {
            List<TaskMailUserAc> taskMailAc = new List<TaskMailUserAc>();
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            if (UserRole == StringConstant.RoleAdmin)
            {
                DateTime? maxDateSelectedUser = null;
                DateTime? minDateSelectedUser = null;

                DateTime slectedDateForAdmin=Convert.ToDateTime(SelectedDate).Date;
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId && y.CreatedOn.Day== slectedDateForAdmin.Day && y.CreatedOn.Month == slectedDateForAdmin.Month && y.CreatedOn.Year == slectedDateForAdmin.Year);
                //var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId && y.Date == slectedDateForAdmin );
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
                        CreatedOn = task.CreatedOn,
                        TaskMails = taskMailReportAc,
                        isMax = Convert.ToDateTime(maxDateSelectedUser),
                        isMin=Convert.ToDateTime(minDateSelectedUser)
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
                else {
                    List<TaskMailReportAc> taskMailReportList = new List<TaskMailReportAc>();
                    TaskMailReportAc taskmailReportAcObject = new TaskMailReportAc
                    {
                        Id = 0,
                        Description = "Not Available",
                        Comment = "Not Available",
                        Status = TaskMailStatus.completed,
                        Hours = 0
                    };
                    taskMailReportList.Add(taskmailReportAcObject);
                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        UserRole = UserRole,
                        CreatedOn = Convert.ToDateTime(SelectedDate),
                        TaskMails = taskMailReportList,
                        isMax = Convert.ToDateTime(maxDateSelectedUser),
                        isMin = Convert.ToDateTime(minDateSelectedUser)
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
                

            }
            else if (UserRole == StringConstant.RoleTeamLeader)
            {
                var user = _user.FirstOrDefault(x => x.Id == LoginId);
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                var json = await _projectUserRepository.GetListOfEmployee(user.UserName, accessToken);
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
                        var taskMailsTL = await _taskMail.FetchAsync(y => y.EmployeeId == employee.Id && y.CreatedOn == selectedDateTime);
                        if (taskMailsTL != null && taskMailsTL.Count() != 0)
                        {
                            var taskTL = taskMailsTL.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                            IEnumerable<TaskMailDetails> taskMailDetailsTL = await _taskMailDetail.FetchAsync(x => x.TaskId == taskTL.Id);
                            List<TaskMailDetails> listTaskmailDetailsReport = new List<TaskMailDetails>();
                            listTaskmailDetailsReport = taskMailDetailsTL.ToList();
                            List<TaskMailReportAc> listTaskMailReport = new List<TaskMailReportAc>();
                            listTaskmailDetailsReport.ForEach(taskMail =>
                            {
                                TaskMailReportAc taskmailReportAcTL = new TaskMailReportAc
                                {
                                    Id = taskMail.Id,
                                    Description = taskMail.Description,
                                    Comment = taskMail.Comment,
                                    Status = taskMail.Status,
                                    Hours = taskMail.Hours
                                };
                                listTaskMailReport.Add(taskmailReportAcTL);
                            });
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = taskTL.CreatedOn,
                                TaskMails = listTaskMailReport,
                                isMax = Convert.ToDateTime(maxDate),
                                isMin=Convert.ToDateTime(minDate)

                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                        else
                        {
                            List<TaskMailReportAc> taskMailReportAc2 = new List<TaskMailReportAc>();
                            TaskMailReportAc taskmailReportAcTL = new TaskMailReportAc
                            {
                                Id = 0,
                                Description = "Not Available",
                                Comment = "Not Available",
                                Status = TaskMailStatus.completed,
                                Hours = 0
                            };
                            taskMailReportAc2.Add(taskmailReportAcTL);
                            TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                            {
                                UserId = employee.Id,
                                UserName = j.Name,
                                UserRole = UserRole,
                                CreatedOn = Convert.ToDateTime(SelectedDate),
                                TaskMails = taskMailReportAc2,
                                isMax = Convert.ToDateTime(maxDate),
                                isMin=Convert.ToDateTime(minDate)
                            };
                            taskMailAc.Add(taskMailUserAc);
                        }
                    }

                }
            }
            else if (UserRole == StringConstant.RoleEmployee)
            {
                DateTime? isMaxEmployee = null;
                var taskMails = await _taskMail.FetchAsync(y => y.EmployeeId == UserId);

                if (taskMails.Count() != 0)
                {
                    var task = taskMails.OrderByDescending(x => Convert.ToDateTime(x.CreatedOn) > Convert.ToDateTime(CreatedOn)).FirstOrDefault();
                    var taskmaxdate = taskMails.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                    isMaxEmployee = taskmaxdate.CreatedOn;
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
                        CreatedOn = task.CreatedOn,
                        TaskMails = taskMailReportAc,
                        isMax = Convert.ToDateTime(isMaxEmployee)
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
                else
                {
                    TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                    {
                        Id = 0,
                        Description = "Not Available",
                        Comment = "Not Available",
                        Status = TaskMailStatus.completed,
                        Hours = 0
                    };
                    taskMailReportAc.Add(taskmailReportAc);
                    TaskMailUserAc taskMailUserAc = new TaskMailUserAc
                    {
                        UserId = UserId,
                        UserName = UserName,
                        UserRole = UserRole,
                        CreatedOn = DateTime.Now,
                        TaskMails = taskMailReportAc
                    };
                    taskMailAc.Add(taskMailUserAc);
                }
            }
            return taskMailAc;
        }

    }
}
