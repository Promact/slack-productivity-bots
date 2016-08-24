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
        public async Task StartTaskMail(string userName, string accessToken)
        {
            var user = await _projectUserRepository.GetUserByUsername(userName,accessToken);
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
            //var text = string.Format("?token=xoxp-4652768210-17616325616-62499499863-cfbd7e4114&channel={0}&text={1}&username={2}&as_user=true&pretty=1", AppSettingsUtil.tsakmailId,HttpUtility.UrlEncode(questionText), userName);
            //await _httpClientRepository.GetAsync(AppSettingsUtil.ChatPostUrl, text, accessToken);
        }

        public async Task SendFirstQuestion(string userName,string accessToken)
        {
            var user = await _projectUserRepository.GetUserByUsername(userName, accessToken);
            //var task = _taskMail.FirstOrDefault(x => x.EmployeeId == user.Id && x.CreatedOn.ToShortDateString() == DateTime.UtcNow.ToShortDateString());
            //var question = _questionRepository.FirstOrDefault(x => x.Type == 2);
            //TaskMailDetails taskDetails = new TaskMailDetails();
            //taskDetails.QuestionId = question.Id;
            //taskDetails.TaskId = task.Id;
            //var questionText = question.QuestionStatement;
            //var text = string.Format("&channel={0}&text={1}&username={2}&as_user=true&pretty =1", AppSettingsUtil.tsakmailId, questionText, userName);
            //await _httpClientRepository.GetAsync(AppSettingsUtil.ChatPostUrl, text, accessToken);

            //var taskDetails = _taskMailDetail.Fetch(x => x.TaskId == task.Id);

            //if(taskDetails==null)
            //{
            //    var recentQuestion = question.ElementAt(taskDetails.Count());
            //    TaskMailDetails taskDeatil = new TaskMailDetails();
            //    taskDeatil.QuestionId = recentQuestion.Id;
                
            //}
            
        }
    }
}
