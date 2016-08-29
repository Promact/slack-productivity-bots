using Autofac;
using Moq;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
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

        public TaskMailRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _taskMailRepository = _componentContext.Resolve<ITaskMailRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
        }

        /// <summary>
        /// Test case for task mail start and ask first question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void StartTaskMail()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(question);
            var responses = await _taskMailRepository.StartTaskMail(StringConstant.FirstNameForTest);
            Assert.Equal(responses, question.QuestionStatement);
        }

        /// <summary>
        /// Test case for conduct task mail after started
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswer()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var firstResponse = await _taskMailRepository.StartTaskMail(StringConstant.FirstNameForTest);
            var secondResponse = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, answer);
            Assert.NotEqual(secondResponse, StringConstant.RequestToStartTaskMail);
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
    }
}
