using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.Util;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Newtonsoft.Json;
using Promact.Core.Repository.Client;
using System.Web;
using Promact.Core.Repository.Bot;
using System.Collections.Generic;
using Promact.Erp.DomainModel.ApplicationClass;

namespace Promact.Core.Repository.TaskMailRepository
{
    public class TaskMailRepository : ITaskMailRepository
    {
        private readonly IRepository<TaskMail> _taskMail;
        private readonly IRepository<TaskMailDetails> _taskMailDetail;
        private readonly IProjectUserCallRepository _projectUserRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IHttpClientRepository _httpClientRepository;
        string questionText = "";
        public TaskMailRepository(IRepository<TaskMail> taskMail, IProjectUserCallRepository projectUserRepository, IRepository<Question> questionRepository, IRepository<TaskMailDetails> taskMailDetail, IHttpClientRepository httpClientRepository)
        {
            _taskMail = taskMail;
            _projectUserRepository = projectUserRepository;
            _questionRepository = questionRepository;
            _taskMailDetail = taskMailDetail;
            _httpClientRepository = httpClientRepository;
        }
        public async Task<string> StartTaskMail(string userName, string accessToken)
        {
            try
            {
                TaskMailQuestion question = new TaskMailQuestion();
                var user = await _projectUserRepository.GetUserByUsername(userName, accessToken);
                TaskMail taskMail;
                try
                {
                    taskMail = _taskMail.Fetch(x => x.EmployeeId == user.Id && x.CreatedOn.Date == DateTime.UtcNow.Date).Last();
                }
                catch (Exception)
                {
                    taskMail = null;
                }

                if (taskMail != null)
                {
                    var taskMailDetail = _taskMailDetail.FirstOrDefault(x => x.TaskId == taskMail.Id);
                    var previousQuestion = _questionRepository.FirstOrDefault(x => x.Id == taskMailDetail.QuestionId);
                    question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                }
                if (taskMail == null || question == TaskMailQuestion.SendEmail)
                {
                    taskMail = new TaskMail();
                    taskMail.CreatedOn = DateTime.UtcNow;
                    taskMail.EmployeeId = user.Id;
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
                else
                {
                    var questionText = await QuestionAndAnswer(userName, accessToken, "");
                }
                return questionText;
            }
            catch (Exception ex)
            {
                questionText = ex.ToString();
                throw;
            }
        }
        public async Task<string> QuestionAndAnswer(string userName, string accessToken, string answer)
        {
            try
            {
                List<TaskMailDetails> taskList = new List<TaskMailDetails>();
                var user = await _projectUserRepository.GetUserByUsername(userName, accessToken);
                var taskMail = _taskMail.Fetch(x => x.EmployeeId == user.Id && x.CreatedOn.Date == DateTime.UtcNow.Date).Last();
                var taskDetails = _taskMailDetail.FirstOrDefault(x => x.TaskId == taskMail.Id);
                var previousQuestion = _questionRepository.FirstOrDefault(x => x.Id == taskDetails.QuestionId);
                if (previousQuestion.OrderNumber <= 5)
                {
                    var nextQuestion = _questionRepository.FirstOrDefault(x => x.OrderNumber == (previousQuestion.OrderNumber + 1));

                    var question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                    switch (question)
                    {
                        case TaskMailQuestion.YourTask:
                            {
                                taskDetails.Description = answer;
                                questionText = nextQuestion.QuestionStatement;
                                taskDetails.QuestionId = nextQuestion.Id;
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
                                    questionText = previousQuestion.QuestionStatement;
                                }
                            }

                            break;
                        case TaskMailQuestion.Status:
                            {
                                try
                                {
                                    var status = (TaskMailStatus)Enum.Parse(typeof(TaskMailStatus), answer);
                                    taskDetails.Status = status;
                                    questionText = nextQuestion.QuestionStatement;
                                    taskDetails.QuestionId = nextQuestion.Id;
                                }
                                catch (Exception)
                                {
                                    questionText = previousQuestion.QuestionStatement;
                                }
                            }
                            break;
                        case TaskMailQuestion.Comment:
                            {
                                taskDetails.Comment = answer;
                                questionText = nextQuestion.QuestionStatement;
                                taskDetails.QuestionId = nextQuestion.Id;
                            }
                            break;
                        case TaskMailQuestion.SendEmail:
                            {
                                answer = answer.ToLower();
                                questionText = StringConstant.ThankYou;
                                if (answer == "yes")
                                {

                                    var taskMailList = _taskMail.Fetch(x => x.EmployeeId == user.Id && x.CreatedOn.Date == DateTime.UtcNow.Date);
                                    foreach (var item in taskMailList)
                                    {
                                        var taskDetail = _taskMailDetail.FirstOrDefault(x => x.TaskId == item.Id);
                                        taskList.Add(taskDetail);
                                    }
                                    //SendMail
                                }
                            }
                            break;
                        default:
                            questionText = StringConstant.InternalError;
                            break;
                    }
                    _taskMailDetail.Update(taskDetails);
                    _taskMail.Save();
                }
                return questionText;
            }
            catch (Exception ex)
            {
                questionText = ex.ToString();
                throw;
            }
        }
    }
}
