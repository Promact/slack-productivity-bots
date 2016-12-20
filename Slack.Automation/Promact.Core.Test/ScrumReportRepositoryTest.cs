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
using Microsoft.AspNet.Identity;
using System.Web;

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
            Initialize();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Method to mock user identity
        /// </summary>
        private void MockIdentity()
        {
            _mockHttpContextBase.Setup(x => x.User.Identity.Name).Returns(_stringConstant.TestUserName);
        }

        /// <summary>
        /// Method to mock access token 
        /// </summary>
        /// <returns>access token</returns>
        private async Task AccessTokenSetUp()
        {
            var user = new ApplicationUser() { Email = _stringConstant.TestUserName, UserName = _stringConstant.TestUserName, SlackUserId = _stringConstant.FirstNameForTest };
            var result = await _userManager.CreateAsync(user);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            var secondResult = await _userManager.AddLoginAsync(user.Id, info);
        }

        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        private void Initialize()
        {
            scrum.GroupName = _stringConstant.TestGroupName;
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
            questionOne.Type = 1;

            questionTwo.QuestionStatement = _stringConstant.ScrumSecondQuestion;
            questionTwo.Type = 1;

            questionThree.QuestionStatement = _stringConstant.ScrumThirdQuestion;
            questionThree.Type = 1;
        }
        #endregion
        
        #region Test Cases
        /// <summary>
        /// Method to test GetProjects when the logged in person is admin
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task GetProjectsAdminTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            var response = Task.FromResult(_stringConstant.AdminLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForAdminFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.AccessTokenForTest)).Returns(responseProjects);
            var projects = await _scrumReportRepository.GetProjectsAsync(_stringConstant.EmployeeIdForTest);
            Assert.Equal(true, projects.Any());

        }

        /// <summary>
        /// Method to test GetProjects when the logged in person is team leader
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task GetProjectsTeamLeaderTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            var response = Task.FromResult(_stringConstant.TeamLeaderLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForTeamLeaderFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.AccessTokenForTest)).Returns(responseProjects);
            var projects = await _scrumReportRepository.GetProjectsAsync(_stringConstant.EmployeeIdForTest);
            Assert.Equal(true, projects.Any());
        }

        /// <summary>
        /// Method to test GetProjects when the logged in person is employee
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task GetProjectsEmployeeTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            var response = Task.FromResult(_stringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForEmployeeFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.AccessTokenForTest)).Returns(responseProjects);
            var projects = await _scrumReportRepository.GetProjectsAsync(_stringConstant.EmployeeIdForTest);
            Assert.Equal(true, projects.Any());
        }


        /// <summary>
        /// Method to test ScrumReportDetails when the person is not available on the scrum date
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task ScrumReportDetailsPersonUnavailableTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 15);
            var response = Task.FromResult(_stringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            var scrumProjectDetails = await _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.EmployeeIdForTest);
            Assert.NotNull(scrumProjectDetails);
        }

        /// <summary>
        /// Method to test ScrumReportDetails when the logged in person is employee 
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task ScrumReportDetailsEmployeeTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 19);
            var response = Task.FromResult(_stringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            _questionDataRepository.Insert(questionOne);
            _questionDataRepository.Save();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Save();
            var scrumProjectDetails = await _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.EmployeeIdForTest);
            Assert.NotNull(scrumProjectDetails);
        }


        /// <summary>
        /// Method to test ScrumReportDetails when the logged in person is admin 
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task ScrumReportDetailsAdminTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 19);
            var response = Task.FromResult(_stringConstant.AdminLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            _questionDataRepository.Insert(questionTwo);
            _questionDataRepository.Save();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Save();
            var scrumProjectDetails =await _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.EmployeeIdForTest);
            Assert.NotNull(scrumProjectDetails);
        }

        /// <summary>
        /// Method to test ScrumReportDetails when the logged in person is teamLeader 
        /// </summary>
        [Fact Trait("Category", "Required")]
        public async Task ScrumReportDetailsTeamLeaderTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 19);
            var response = Task.FromResult(_stringConstant.TeamLeaderLoginDetails);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            _questionDataRepository.Insert(questionThree);
            _questionDataRepository.Save();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Save();
            var scrumProjectDetails = await _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.EmployeeIdForTest);
            Assert.NotNull(scrumProjectDetails);
        }
        #endregion


    }
}
