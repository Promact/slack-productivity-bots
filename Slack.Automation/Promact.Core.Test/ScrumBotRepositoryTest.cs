using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class ScrumBotRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<Scrum> _scrumDataRepository;
        private readonly IScrumBotRepository _scrumBotRepository;

        private SlackUserDetails slackUserDetails = new SlackUserDetails()
        {
            Id = StringConstant.StringIdForTest,
            Name = StringConstant.UserNameForTest,
            TeamId = StringConstant.PromactStringName
        };

        private ApplicationUser user = new ApplicationUser()
        {
            Email = StringConstant.EmailForTest,
            UserName = StringConstant.EmailForTest,
            SlackUserName = StringConstant.UserNameForTest,
        };

        private Question question = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 1,
            QuestionStatement = StringConstant.ScrumQuestionForTest,
            Type = 1
        };

        private Question question1 = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 2,
            QuestionStatement = StringConstant.ScrumQuestionForTest,
            Type = 1
        };

        private Scrum scrum = new Scrum()
        {
            CreatedOn = DateTime.UtcNow,
            ProjectId = Convert.ToInt32(StringConstant.ProjectIdForTest),
            GroupName = StringConstant.GroupName,
            ScrumDate = DateTime.UtcNow,
            TeamLeaderId = StringConstant.TeamLeaderIdForTest
        };


        public ScrumBotRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumBotRepository = _componentContext.Resolve<IScrumBotRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _scrumDataRepository = _componentContext.Resolve<IRepository<Scrum>>();
        }

        /// <summary>
        /// Method StartScrum Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoQuestion()
        {
            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(question);
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserNameForTest);
            Assert.Equal(StringConstant.GoodDay + "<@>!\n" + StringConstant.ScrumQuestionForTest, msg);
        }

        /// <summary>
        /// Method AddScrumAnswer Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddScrumAnswer()
        {
            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            var userResponse = Task.FromResult(StringConstant.EmployeeDetailsFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailsByIdUrl, StringConstant.UserIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            var userDetailResponse = Task.FromResult(StringConstant.UserBySlackUserName);
            var userDetailRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailByUserNameUrl, StringConstant.UserNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, userDetailRequestUrl, StringConstant.AccessTokenForTest)).Returns(userDetailResponse);

            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);

            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.NotEqual(null, msg);
        }

        /// <summary>
        /// Method Leave Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void Leave()
        {
            {
                var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
                var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
                _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

                var userResponse = Task.FromResult(StringConstant.EmployeeDetailsFromOauth);
                var userRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailsByIdUrl, StringConstant.UserIdForTest);
                _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

                var userDetailResponse = Task.FromResult(StringConstant.UserBySlackUserName);
                var userDetailRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailByUserNameUrl, StringConstant.LeaveApplicant);
                _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, userDetailRequestUrl, StringConstant.AccessTokenForTest)).Returns(userDetailResponse);

                _slackUserRepository.AddSlackUser(slackUserDetails);
                _botQuestionRepository.AddQuestion(question);

                _scrumDataRepository.Insert(scrum);
                _scrumDataRepository.Save();

                UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user.Id, info);

                var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.LeaveApplicant);
                //Assert.NotEqual(string.Empty, msg);
                Assert.Equal(StringConstant.GoodDay + "<@>!\n" + StringConstant.ScrumQuestionForTest, msg);
            }
        }
    }
}