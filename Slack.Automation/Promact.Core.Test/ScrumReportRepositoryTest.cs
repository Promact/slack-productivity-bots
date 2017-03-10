using Autofac;
using Moq;
using Promact.Core.Repository.ScrumReportRepository;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.DomainModel.ApplicationClass;
using System.Web;
using Promact.Core.Repository.ServiceRepository;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Promact.Core.Test
{
    public class ScrumReportRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IScrumReportRepository _scrumReportRepository;
        private readonly Mock<IHttpClientService> _mockHttpClient;
        private readonly IRepository<Scrum> _scrumDataRepository;
        private readonly IRepository<ScrumAnswer> _scrumAnswerDataRepository;
        private readonly IRepository<Question> _questionDataRepository;
        private readonly IStringConstantRepository _stringConstant;
        private readonly Mock<HttpContextBase> _mockHttpContextBase;
        private readonly ApplicationUserManager _userManager;
        private readonly Mock<IServiceRepository> _mockServiceRepository;

        private Scrum scrum = new Scrum();
        private ScrumAnswer scrumAnswer = new ScrumAnswer();
        private Question questionOne = new Question();
        private Question questionTwo = new Question();
        private Question questionThree = new Question();
        #endregion

        #region Constructor
        public ScrumReportRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumReportRepository = _componentContext.Resolve<IScrumReportRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _scrumDataRepository = _componentContext.Resolve<IRepository<Scrum>>();
            _scrumAnswerDataRepository = _componentContext.Resolve<IRepository<ScrumAnswer>>();
            _questionDataRepository = _componentContext.Resolve<IRepository<Question>>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _mockHttpContextBase = _componentContext.Resolve<Mock<HttpContextBase>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _mockServiceRepository = _componentContext.Resolve<Mock<IServiceRepository>>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Method to test GetProjects when the logged in person is admin
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task GetProjectsAdminTest()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            var response = Task.FromResult(_stringConstant.AdminLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForAdminFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.AccessTokenForTest)).Returns(responseProjects);
            var projects = _scrumReportRepository.GetProjectsAsync(_stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(true, projects.Any());
        }

        /// <summary>
        /// Method to test GetProjects when the logged in person is team leader
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task GetProjectsTeamLeaderTest()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            MockingGetListOfProjectsEnrollmentOfUserByUserIdAsync();
            var response = Task.FromResult(_stringConstant.TeamLeaderLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForTeamLeaderFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.AccessTokenForTest)).Returns(responseProjects);
            var projects = _scrumReportRepository.GetProjectsAsync(_stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(true, projects.Any());

        }

        /// <summary>
        /// Method to test GetProjects when the logged in person is employee
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task GetProjectsEmployeeTest()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            MockingGetListOfProjectsEnrollmentOfUserByUserIdAsync();
            var response = Task.FromResult(_stringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForEmployeeFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.AccessTokenForTest)).Returns(responseProjects);
            var projects = _scrumReportRepository.GetProjectsAsync(_stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(true, projects.Any());

        }


        /// <summary>
        /// Method to test ScrumReportDetails when the person is not available on the scrum date
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task ScrumReportDetailsPersonUnavailableTest()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 15);
            var response = Task.FromResult(_stringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.EmployeeIdForTest).Result;
            Assert.NotNull(scrumProjectDetails);
        }


        /// <summary>
        /// Method to test ScrumReportDetails when the logged in person is employee 
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task ScrumReportDetailsEmployeeTest()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 19);
            var response = Task.FromResult(_stringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _questionDataRepository.Insert(questionOne);
            await _questionDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.EmployeeIdForTest).Result;
            Assert.NotNull(scrumProjectDetails);
        }


        /// <summary>
        /// Method to test ScrumReportDetails when the logged in person is admin 
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task ScrumReportDetailsAdminTest()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 19);
            var response = Task.FromResult(_stringConstant.AdminLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _questionDataRepository.Insert(questionTwo);
            await _questionDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.EmployeeIdForTest).Result;
            Assert.NotNull(scrumProjectDetails);
        }


        /// <summary>
        /// Method to test ScrumReportDetails when the logged in person is teamLeader 
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task ScrumReportDetailsTeamLeaderTest()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 19);
            var response = Task.FromResult(_stringConstant.TeamLeaderLoginDetails);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _questionDataRepository.Insert(questionThree);
            await _questionDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.EmployeeIdForTest).Result;
            Assert.NotNull(scrumProjectDetails);
        }


        #endregion

        #region Initialisation
        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialize()
        {
            scrum.SlackChannelId = _stringConstant.TestGroupName;
            scrum.ScrumDate = new DateTime(2016, 9, 19);
            scrum.ProjectId = 1012;
            scrum.TeamLeaderId = _stringConstant.TestId;

            scrumAnswer.EmployeeId = _stringConstant.EmployeeIdForTest;
            scrumAnswer.ScrumId = 1;
            scrumAnswer.QuestionId = 1;
            scrumAnswer.Answer = _stringConstant.TestAnswer;
            scrumAnswer.AnswerDate = new DateTime(2016, 9, 19);
            scrumAnswer.CreatedOn = DateTime.UtcNow;
            scrumAnswer.Id = 1;

            questionOne.QuestionStatement = _stringConstant.ScrumFirstQuestion;
            questionOne.Type = BotQuestionType.Scrum;
            questionOne.OrderNumber = QuestionOrder.Yesterday;

            questionTwo.QuestionStatement = _stringConstant.ScrumSecondQuestion;
            questionTwo.Type = BotQuestionType.Scrum;

            questionThree.QuestionStatement = _stringConstant.ScrumThirdQuestion;
            questionThree.Type = BotQuestionType.Scrum;
            questionThree.OrderNumber = QuestionOrder.RoadBlock;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Private method to create a user add login info and mocking of Identity and return access token
        /// </summary>
        /// <returns></returns>
        private async Task CreateUserAndMockingHttpContextToReturnAccessToken()
        {
            var user = new ApplicationUser()
            {
                Id = _stringConstant.StringIdForTest,
                UserName = _stringConstant.EmailForTest,
                Email = _stringConstant.EmailForTest
            };
            await _userManager.CreateAsync(user);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.AddLoginAsync(user.Id, info);
            Claim claim = new Claim(_stringConstant.Sub, _stringConstant.StringIdForTest);
            var mockClaims = new Mock<ClaimsIdentity>();
            IList<Claim> claims = new List<Claim>();
            claims.Add(claim);
            mockClaims.Setup(x => x.Claims).Returns(claims);
            _mockHttpContextBase.Setup(x => x.User.Identity).Returns(mockClaims.Object);
            var accessToken = Task.FromResult(_stringConstant.AccessTokenForTest);
            _mockServiceRepository.Setup(x => x.GerAccessTokenByRefreshToken(It.IsAny<string>())).Returns(accessToken);
        }

        /// <summary>
        /// Method to check GetListOfProjectsEnrollmentOfUserByUserIdAsync
        /// </summary>
        /// <returns></returns>
        public void MockingGetListOfProjectsEnrollmentOfUserByUserIdAsync()
        {
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForAdminFromOauth);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.EmployeeIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(responseProjects);
        }
        #endregion
    }
}
