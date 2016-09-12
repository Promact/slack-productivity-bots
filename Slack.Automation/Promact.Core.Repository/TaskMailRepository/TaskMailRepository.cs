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
        ///Fetches the task mail details of the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReport(int id)
        {
            List<TaskMailReportAc> taskMailReportAc = new List<TaskMailReportAc>();
            IEnumerable<TaskMailDetails> taskMailDetails =await _taskMailDetail.FetchAsync(x => x.TaskId == id);
            List<TaskMailDetails> taskmailDetails = new List<TaskMailDetails>();
            taskmailDetails = taskMailDetails.ToList();
            taskmailDetails.ForEach(taskMail =>
            {
                TaskMailReportAc taskmailReportAc = new TaskMailReportAc
                {
                    Id = taskMail.Id,
                    Description = taskMail.Description,
                    Comment=taskMail.Comment,
                    Status = taskMail.Status,
                    Hours = taskMail.Hours
                };
                taskMailReportAc.Add(taskmailReportAc);
            });
            return taskMailReportAc;
        }
    }
}
