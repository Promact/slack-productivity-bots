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
                var user = await _projectUserRepository.GetUserByUsername(userName, accessToken);
                TaskMail taskMail = new TaskMail();
                taskMail.CreatedOn = DateTime.UtcNow;
                taskMail.EmployeeId = user.Id;
                _taskMail.Insert(taskMail);
                _taskMail.Save();
                var question = _questionRepository.FirstOrDefault(x => x.Type == 2);
                TaskMailDetails taskDetails = new TaskMailDetails();
                taskDetails.QuestionId = question.Id;
                taskDetails.TaskId = taskMail.Id;
                var questionText = question.QuestionStatement;
                _taskMailDetail.Insert(taskDetails);
                _taskMailDetail.Save();
                return questionText;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<string> QuestionAndAnswer(string userName, string accessToken, string answer)
        {
            try
            {
                string questionText;
                List<TaskMailDetails> taskList = new List<TaskMailDetails>();
                var user = await _projectUserRepository.GetUserByUsername(userName, accessToken);
                var taskMail = _taskMail.Fetch(x => x.EmployeeId == user.Id && x.CreatedOn.Date == DateTime.UtcNow.Date).Last();
                var taskDetails = _taskMailDetail.FirstOrDefault(x => x.TaskId == taskMail.Id);
                var previousQuestion = _questionRepository.FirstOrDefault(x => x.Id == taskDetails.QuestionId);
                if (previousQuestion.OrderNumber < 5)
                {
                    var nextQuestion = _questionRepository.FirstOrDefault(x => x.OrderNumber == (previousQuestion.OrderNumber + 1));
                    questionText = nextQuestion.QuestionStatement;
                    var question = (TaskMailQuestion)Enum.Parse(typeof(TaskMailQuestion), previousQuestion.OrderNumber.ToString());
                    switch (question)
                    {
                        case TaskMailQuestion.YourTask:
                            taskDetails.Description = answer;
                            break;
                        case TaskMailQuestion.HoursSpent:
                            taskDetails.Hours = Convert.ToDecimal(answer);
                            break;
                        case TaskMailQuestion.Status:
                            var status = (TaskMailStatus)Enum.Parse(typeof(TaskMailStatus), answer);
                            taskDetails.Status = status;
                            break;
                        case TaskMailQuestion.Comment:
                            taskDetails.Comment = answer;
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

                    taskDetails.QuestionId = nextQuestion.Id;
                    _taskMailDetail.Update(taskDetails);
                    _taskMail.Save();

                }
                questionText = StringConstant.InternalError;
                return questionText;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
