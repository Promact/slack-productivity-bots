using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class TaskMailRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ITaskMailRepository _taskMailRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<TaskMail> _taskMailDataRepository;
        private readonly IRepository<TaskMailDetails> _taskMailDetailsDataRepository;
        private readonly IProjectUserCallRepository _projectUserRepository;

        public TaskMailRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _taskMailRepository = _componentContext.Resolve<ITaskMailRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _taskMailDataRepository = _componentContext.Resolve<IRepository<TaskMail>>();
            _taskMailDetailsDataRepository = _componentContext.Resolve<IRepository<TaskMailDetails>>();
            _projectUserRepository = _componentContext.Resolve<IProjectUserCallRepository>();
        }

        /// <summary>
        /// Test case for task mail start and ask first question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void StartTaskMail()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(question);
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            var responses = await _taskMailRepository.StartTaskMail(StringConstant.FirstNameForTest);
            Assert.Equal(responses, question.QuestionStatement);
        }

        /// <summary>
        /// Test case for conduct task mail after started
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswer()
        {
            var userResponse = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(StringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", StringConstant.TeamLeaderDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, teamLeaderRequestUrl, StringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(StringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", StringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, managementRequestUrl, StringConstant.AccessTokenForTest)).Returns(managementResponse);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(question);
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = question.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, answer);
            Assert.NotEqual(response, StringConstant.QuestionForTest);
        }

        [Fact, Trait("Category", "Required")]
        public async void TaskMailDetailsReport()
        {
            var userResponse = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(StringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", StringConstant.TeamLeaderDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, teamLeaderRequestUrl, StringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(StringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", StringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, managementRequestUrl, StringConstant.AccessTokenForTest)).Returns(managementResponse);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(question);
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = question.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            var taskMailDetail = _taskMailRepository.TaskMailDetailsReport(taskMailDetails.Id);
            Assert.Equal(1,taskMailDetail.Result.Count);
        }

        private static string answer = null;
        private SlackUserDetails slackUserDetails = new SlackUserDetails()
        {
            Id = StringConstant.StringIdForTest,
            Name = StringConstant.FirstNameForTest,
            TeamId = StringConstant.PromactStringName
        };
        private Question question = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 1,
            QuestionStatement = StringConstant.QuestionForTest,
            Type = 2
        };
        private ApplicationUser user = new ApplicationUser()
        {
            Email = StringConstant.EmailForTest,
            UserName = StringConstant.EmailForTest,
            SlackUserName = StringConstant.FirstNameForTest,
        };
        private TaskMail taskMail = new TaskMail()
        {
            CreatedOn = DateTime.UtcNow,
        };
        private TaskMailDetails taskMailDetails = new TaskMailDetails()
        {
            Comment = StringConstant.CommentAndDescriptionForTest,
            Description = StringConstant.CommentAndDescriptionForTest,
            Hours = Convert.ToDecimal(StringConstant.StringHourForTest),
            SendEmailConfirmation = SendEmailConfirmation.no,
            Status = TaskMailStatus.completed,
        };
    }
}
