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
                //var project = await _projectUserRepository.GetProjectDetailsByUserName(userName, accessToken);
                TaskMail taskMail = new TaskMail();
                taskMail.CreatedOn = DateTime.UtcNow;
                taskMail.EmployeeId = user.Id;
                //taskMail.ProjectId = project.Id;
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
            //var text = string.Format("?token=xoxp-4652768210-17616325616-62499499863-cfbd7e4114&channel={0}&text={1}&username={2}&as_user=true&pretty=1", userName,HttpUtility.UrlEncode("Hello"), "tsakmail");
            //await _httpClientRepository.GetAsync(AppSettingsUtil.ChatPostUrl, text, accessToken);
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
                    switch (previousQuestion.OrderNumber)
                    {
                        case 1:
                            taskDetails.Description = answer;
                            break;
                        case 2:
                            taskDetails.Hours = Convert.ToDecimal(answer);
                            break;
                        case 3:
                            var status = (TaskMailStatus)Enum.Parse(typeof(TaskMailStatus), answer);
                            taskDetails.Status = status;
                            break;
                        case 4:
                            taskDetails.Comment = answer;
                            break;
                        case 5:
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
