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

namespace Promact.Core.Repository.TaskMailRepository
{
    public class TaskMailRepository : ITaskMailRepository
    {
        private readonly IRepository<TaskMail> _taskMail;
        private readonly IRepository<TaskMailDetails> _taskMailDetail;
        private readonly IProjectUserCallRepository _projectUserRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _user;
        private readonly IEmailService _emailService;
        string questionText = "";
        public TaskMailRepository(IRepository<TaskMail> taskMail, IProjectUserCallRepository projectUserRepository, IRepository<Question> questionRepository, IRepository<TaskMailDetails> taskMailDetail, IHttpClientRepository httpClientRepository, IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> user, IEmailService emailService)
        {
            _taskMail = taskMail;
            _projectUserRepository = projectUserRepository;
            _questionRepository = questionRepository;
            _taskMailDetail = taskMailDetail;
            _httpClientRepository = httpClientRepository;
            _attachmentRepository = attachmentRepository;
            _user = user;
            _emailService = emailService;
        }
        public async Task<string> StartTaskMail(string userName)
        {
            try
            {
                var user = _user.FirstOrDefault(x => x.SlackUserName == userName);
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                TaskMailQuestion question = new TaskMailQuestion();
                Question previousQuestion = new Question();
                TaskMailDetails taskMailDetail = new TaskMailDetails();
                var oAuthUser = await _projectUserRepository.GetUserByUsername(userName, accessToken);
                TaskMail taskMail;
                try
                {
                    taskMail = _taskMail.Fetch(x => x.EmployeeId == oAuthUser.Id && x.CreatedOn.Date == DateTime.UtcNow.Date).Last();
                }
                catch (Exception)
                {
                    taskMail = null;
                }
                if (taskMail != null)
                {
                    taskMailDetail = _taskMailDetail.FirstOrDefault(x => x.TaskId == taskMail.Id);
                    previousQuestion = _questionRepository.FirstOrDefault(x => x.Id == taskMailDetail.QuestionId);
                    question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                }
                if (taskMail == null || question == TaskMailQuestion.TaskMailSend)
                {
                    if (taskMailDetail != null && taskMailDetail.SendEmailConfirmation == SendEmailConfirmation.yes)
                    {
                        questionText = StringConstant.AlreadyMailSend;
                    }
                    else
                    {
                        taskMail = new TaskMail();
                        taskMail.CreatedOn = DateTime.UtcNow;
                        taskMail.EmployeeId = oAuthUser.Id;
                        _taskMail.Insert(taskMail);
                        _taskMail.Save();
                        var firstQuestion = _questionRepository.FirstOrDefault(x => x.Type == 2);
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
        public async Task<string> QuestionAndAnswer(string userName, string answer)
        {
            try
            {
                var user = _user.FirstOrDefault(x => x.SlackUserName == userName);
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                List<TaskMailDetails> taskList = new List<TaskMailDetails>();
                TaskMail taskMail = new TaskMail();
                var oAuthUser = await _projectUserRepository.GetUserByUsername(userName, accessToken);
                try
                {
                    taskMail = _taskMail.Fetch(x => x.EmployeeId == oAuthUser.Id && x.CreatedOn.Date == DateTime.UtcNow.Date).Last();
                    var taskDetails = _taskMailDetail.FirstOrDefault(x => x.TaskId == taskMail.Id);
                    var previousQuestion = _questionRepository.FirstOrDefault(x => x.Id == taskDetails.QuestionId);
                    if (previousQuestion.OrderNumber <= 7)
                    {
                        var nextQuestion = _questionRepository.FirstOrDefault(x => x.OrderNumber == (previousQuestion.OrderNumber + 1));
                        var question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                        switch (question)
                        {
                            case TaskMailQuestion.YourTask:
                                {
                                    if (answer != null)
                                    {
                                        taskDetails.Description = answer.ToLower();
                                        questionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    else
                                    {
                                        questionText = previousQuestion.QuestionStatement;
                                    }
                                }
                                break;
                            case TaskMailQuestion.HoursSpent:
                                {
                                    try
                                    {
                                        taskDetails.Hours = Convert.ToDecimal(answer);
                                        questionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    catch (Exception)
                                    {
                                        questionText = string.Format("{0}{1}{2}", StringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
                                }
                                break;
                            case TaskMailQuestion.Status:
                                {
                                    try
                                    {
                                        var status = (TaskMailStatus)Enum.Parse(typeof(TaskMailStatus), answer.ToLower());
                                        taskDetails.Status = status;
                                        questionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    catch (Exception)
                                    {
                                        questionText = string.Format("{0}{1}{2}", StringConstant.TaskMailBotStatusErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
                                }
                                break;
                            case TaskMailQuestion.Comment:
                                {
                                    if (answer != null)
                                    {
                                        taskDetails.Comment = answer.ToLower();
                                        questionText = nextQuestion.QuestionStatement;
                                        taskDetails.QuestionId = nextQuestion.Id;
                                    }
                                    else
                                    {
                                        questionText = previousQuestion.QuestionStatement;
                                    }
                                }
                                break;
                            case TaskMailQuestion.SendEmail:
                                {
                                    try
                                    {
                                        var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation), answer.ToLower().ToString());
                                        switch (confirmation)
                                        {
                                            case SendEmailConfirmation.yes:
                                                {
                                                    taskDetails.SendEmailConfirmation = SendEmailConfirmation.yes;
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    questionText = nextQuestion.QuestionStatement;
                                                }
                                                break;
                                            case SendEmailConfirmation.no:
                                                {
                                                    taskDetails.SendEmailConfirmation = SendEmailConfirmation.no;
                                                    nextQuestion = _questionRepository.FirstOrDefault(x => x.Type == 2 && x.OrderNumber == 7);
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    questionText = StringConstant.ThankYou;
                                                }
                                                break;
                                            default:
                                                {
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
                                        var confirmation = (SendEmailConfirmation)Enum.Parse(typeof(SendEmailConfirmation), answer.ToLower().ToString());
                                        questionText = StringConstant.ThankYou;
                                        switch (confirmation)
                                        {
                                            case SendEmailConfirmation.yes:
                                                {
                                                    taskDetails.SendEmailConfirmation = SendEmailConfirmation.yes;
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                    var taskMailList = _taskMail.Fetch(x => x.EmployeeId == oAuthUser.Id && x.CreatedOn.Date == DateTime.UtcNow.Date);
                                                    foreach (var item in taskMailList)
                                                    {
                                                        var taskDetail = _taskMailDetail.FirstOrDefault(x => x.TaskId == item.Id);
                                                        taskList.Add(taskDetail);
                                                    }
                                                    var emailBody = EmailServiceTemplateTaskMail(taskList);
                                                    EmailApplication email = new EmailApplication();
                                                    email.Body = emailBody;
                                                    email.From = "rajdeep@promactinfo.com";
                                                    email.To = "siddhartha@promactinfo.com";
                                                    email.Subject = "Daily Task Mail";
                                                    _emailService.Send(email);
                                                }
                                                break;
                                            case SendEmailConfirmation.no:
                                                {
                                                    taskDetails.SendEmailConfirmation = SendEmailConfirmation.no;
                                                    taskDetails.QuestionId = nextQuestion.Id;
                                                }
                                                break;
                                            default:
                                                {
                                                    questionText = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                                }
                                                break;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        questionText = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, previousQuestion.QuestionStatement);
                                    }
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
                catch (Exception)
                {
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
