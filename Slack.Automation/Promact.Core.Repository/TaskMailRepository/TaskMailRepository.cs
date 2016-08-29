using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.Util;
using System.Collections.Generic;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.Util.Email;
using Promact.Core.Repository.BotQuestionRepository;

namespace Promact.Core.Repository.TaskMailRepository
{
    public class TaskMailRepository : ITaskMailRepository
    {
        private readonly IRepository<TaskMail> _taskMail;
        private readonly IRepository<TaskMailDetails> _taskMailDetail;
        private readonly IProjectUserCallRepository _projectUserRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _user;
        private readonly IEmailService _emailService;
        string questionText = "";
        public TaskMailRepository(IRepository<TaskMail> taskMail, IProjectUserCallRepository projectUserRepository, IRepository<TaskMailDetails> taskMailDetail, IHttpClientRepository httpClientRepository, IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> user, IEmailService emailService, IBotQuestionRepository botQuestionRepository)
        {
            _taskMail = taskMail;
            _projectUserRepository = projectUserRepository;
            _taskMailDetail = taskMailDetail;
            _httpClientRepository = httpClientRepository;
            _attachmentRepository = attachmentRepository;
            _user = user;
            _emailService = emailService;
            _botQuestionRepository = botQuestionRepository;
        }

        /// <summary>
        /// Method to start task mail
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>questionText in string format containing question statement</returns>
        public async Task<string> StartTaskMail(string userName)
        {
            try
            {
                // getting user name from user's slack name
                var user = _user.FirstOrDefault(x => x.SlackUserName == userName);
                // getting access token for that user
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                TaskMailQuestion question = new TaskMailQuestion();
                Question previousQuestion = new Question();
                TaskMailDetails taskMailDetail = new TaskMailDetails();
                // getting user information from Promact Oauth Server
                var oAuthUser = await _projectUserRepository.GetUserByUsername(userName, accessToken);
                TaskMail taskMail;
                try
                {
                    // checking for previous task mail exist or not for today
                    taskMail = _taskMail.Fetch(x => x.EmployeeId == oAuthUser.Id && x.CreatedOn.Date == DateTime.UtcNow.Date).Last();
                }
                catch (Exception)
                {
                    taskMail = null;
                }
                if (taskMail != null)
                {
                    // if exist then check the whether the task mail was completed or not
                    taskMailDetail = _taskMailDetail.FirstOrDefault(x => x.TaskId == taskMail.Id);
                    previousQuestion = _botQuestionRepository.FindById(taskMailDetail.QuestionId);
                    //previousQuestion = _questionRepository.FirstOrDefault(x => x.Id == taskMailDetail.QuestionId);
                    question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                }
                // if previous task mail completed then it will start a new one
                if (taskMail == null || question == TaskMailQuestion.TaskMailSend)
                {
                    // Before going to create new task it will check whether the user has send mail for today or not.
                    if (taskMailDetail != null && taskMailDetail.SendEmailConfirmation == SendEmailConfirmation.yes)
                    {
                        // If mail is send then user will not be able to add task mail for that day
                        questionText = StringConstant.AlreadyMailSend;
                    }
                    else
                    {
                        // If mail is not send then user will be able to add task mail for that day
                        taskMail = new TaskMail();
                        taskMail.CreatedOn = DateTime.UtcNow;
                        taskMail.EmployeeId = oAuthUser.Id;
                        _taskMail.Insert(taskMail);
                        _taskMail.Save();
                        var firstQuestion = _botQuestionRepository.FindByQuestionType(2);
                        //var firstQuestion = _questionRepository.FirstOrDefault(x => x.Type == 2);
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
                return questionText;
            }
            catch (Exception ex)
            {
                questionText = ex.ToString();
                throw;
            }
        }

        /// <summary>
        /// Method to conduct task mail after started
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public async Task<string> QuestionAndAnswer(string userName, string answer)
        {
            try
            {
                // getting user name from user's slack name
                var user = _user.FirstOrDefault(x => x.SlackUserName == userName);
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
                    //var previousQuestion = _questionRepository.FirstOrDefault(x => x.Id == taskDetails.QuestionId);
                    // checking if precious question was last and answer by user and previous task report was completed then asked for new task mail
                    if (previousQuestion.OrderNumber <= 7)
                    {
                        // getting next question to be asked to user
                        var nextQuestion = _botQuestionRepository.FindByTypeAndOrderNumber((previousQuestion.OrderNumber + 1), 2);
                        //var nextQuestion = _questionRepository.FirstOrDefault(x => x.OrderNumber == (previousQuestion.OrderNumber + 1) && x.Type == 2);
                        // Converting question Ordr to enum
                        var question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                        switch (question)
                        {
                            case TaskMailQuestion.YourTask:
                                {
                                    // if previous question was description of task and it was not null/wrong vale then it will ask next question
                                    if (answer != null)
                                    {
                                        taskDetails.Description = answer.ToLower();
                                        questionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    else
                                    {
                                        // if previous question was description of task and it was null then it will ask for description again
                                        questionText = previousQuestion.QuestionStatement;
                                    }
                                }
                                break;
                            case TaskMailQuestion.HoursSpent:
                                {
                                    try
                                    {
                                        // if previous question was hour of task and it was not null/wrong value then it will ask next question
                                        var hour = Convert.ToDecimal(answer);
                                        // checking range of hours
                                        if (hour>0 && hour<8)
                                        {
                                            taskDetails.Hours = hour;
                                            questionText = nextQuestion.QuestionStatement;
                                            taskDetails.QuestionId = nextQuestion.Id;
                                        }
                                        else
                                        {
                                            // if previous question was hour of task and it was null or wrong value then it will ask for hour again
                                            questionText = string.Format("{0}{1}{2}", StringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // if previous question was hour of task and it was null or wrong value then it will ask for hour again
                                        questionText = string.Format("{0}{1}{2}", StringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
                                }
                                break;
                            case TaskMailQuestion.Status:
                                {
                                    try
                                    {
                                        // if previous question was status of task and it was not null/wrong value then it will ask next question
                                        var status = (TaskMailStatus)Enum.Parse(typeof(TaskMailStatus), answer.ToLower());
                                        taskDetails.Status = status;
                                        questionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    catch (Exception)
                                    {
                                        // if previous question was status of task and it was null or wrong value then it will ask for status again
                                        questionText = string.Format("{0}{1}{2}", StringConstant.TaskMailBotStatusErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
                                }
                                break;
                            case TaskMailQuestion.Comment:
                                {
                                    if (answer != null)
                                    {
                                        // if previous question was comment of task and it was not null/wrong value then it will ask next question
                                        taskDetails.Comment = answer.ToLower();
                                        questionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    else
                                    {
                                        // if previous question was comment of task and it was null or wrong value then it will ask for comment again
                                        questionText = previousQuestion.QuestionStatement;
                                    }
                                }
                                break;
                            case TaskMailQuestion.SendEmail:
                                {
                                    try
                                    {
                                        // if previous question was send email of task and it was not null/wrong value then it will ask next question
                                        var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation), answer.ToLower().ToString());
                                        switch (confirmation)
                                        {
                                            case SendEmailConfirmation.yes:
                                                {
                                                    // if previous question was send email of task and it was not null/wrong value then it will ask next question
                                                    taskDetails.SendEmailConfirmation = SendEmailConfirmation.yes;
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    questionText = nextQuestion.QuestionStatement;
                                                }
                                                break;
                                            case SendEmailConfirmation.no:
                                                {
                                                    // if previous question was send email of task and it was not null/wrong value then it will say thank you and task mail stopped
                                                    taskDetails.SendEmailConfirmation = SendEmailConfirmation.no;
                                                    nextQuestion = _botQuestionRepository.FindByTypeAndOrderNumber(7, 2);
                                                    //nextQuestion = _questionRepository.FirstOrDefault(x => x.Type == 2 && x.OrderNumber == 7);
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    questionText = StringConstant.ThankYou;
                                                }
                                                break;
                                            default:
                                                {
                                                    // if previous question was send email of task and it was null or wrong value then it will ask for comment again
                                                    questionText = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                                }
                                                break;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        questionText = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
                                    break;
                                }
                            case TaskMailQuestion.ConfirmSendEmail:
                                {
                                    try
                                    {
                                        // if previous question was confirm send email of task and it was not null/wrong value then it will ask next question
                                        var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation), answer.ToLower().ToString());
                                        questionText = StringConstant.ThankYou;
                                        switch (confirmation)
                                        {
                                            case SendEmailConfirmation.yes:
                                                {
                                                    // if previous question was confirm send email of task and it was not null/wrong value then it will send email and reply back with thank you and task mail stopped
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
                                                    // if previous question was confirm send email of task and it was not null/wrong value then it will say thank you and task mail stopped
                                                    taskDetails.SendEmailConfirmation = SendEmailConfirmation.no;
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                }
                                                break;
                                            default:
                                                {
                                                    // if previous question was send email of task and it was null or wrong value then it will ask for send task mail confirm again
                                                    questionText = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                                }
                                                break;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // if previous question was send email of task and it was null or wrong value then it will ask for send task mail confirm again
                                        questionText = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
                                }
                                break;
                            default:
                                // if previous task mail was completed then ask user to start task mail
                                questionText = StringConstant.RequestToStartTaskMail;
                                break;

                        }
                        _taskMailDetail.Update(taskDetails);
                        _taskMail.Save();
                    }
                }
                catch (Exception)
                {
                    // if previous task mail doesnot exist then ask user to start task mail
                    questionText = StringConstant.RequestToStartTaskMail;
                }
                return questionText;
            }
            catch (Exception ex)
            {
                questionText = ex.ToString();
                throw;
            }
        }
        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">TaskMail template object</param>
        /// <returns>template emailBody as string</returns>
        private string EmailServiceTemplateTaskMail(List<TaskMailDetails> taskMail)
        {
            ;
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
    }
}
