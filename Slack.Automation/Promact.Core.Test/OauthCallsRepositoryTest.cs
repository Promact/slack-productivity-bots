using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Promact.Core.Test
{
    /// <summary>
    /// Test Cases of Project User Call Repository
    /// </summary>
    public class OauthCallsRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly Mock<IHttpClientService> _mockHttpClient;
        private readonly IStringConstantRepository _stringConstant;
        private readonly Mock<HttpContextBase> _mockHttpContextBase;
        private readonly ApplicationUserManager _userManager;
        #endregion

        #region Constructor
        public OauthCallsRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _oauthCallsRepository = _componentContext.Resolve<IOauthCallsRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _mockHttpContextBase = _componentContext.Resolve<Mock<HttpContextBase>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Method GetUserByUserId Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetUserByUserIdAsync()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var user = await _oauthCallsRepository.GetUserByUserIdAsync(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest);
            Assert.Equal(user.Email, _stringConstant.ManagementEmailForTest);
        }

        /// <summary>
        /// Method GetTeamLeaderUserName Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetTeamLeaderUserNameAsync()
        {
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            string teamLeaderUsername = _stringConstant.EmptyString;
            var teamLeader = await _oauthCallsRepository.GetTeamLeaderUserIdAsync(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest);
            foreach (var team in teamLeader)
            {
                teamLeaderUsername = team.Email;
            }
            Assert.Equal(teamLeaderUsername, _stringConstant.TeamLeaderEmailForTest);
        }

        /// <summary>
        /// Method GetManagementUserName Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetManagementUserNameAsync()
        {
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = _stringConstant.ManagementDetailsUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            string managementUsername = _stringConstant.EmptyString;
            var management = await _oauthCallsRepository.GetManagementUserNameAsync(_stringConstant.AccessTokenForTest);
            foreach (var team in management)
            {
                managementUsername = team.FirstName;
            }
            Assert.Equal(managementUsername, _stringConstant.ManagementFirstForTest);
        }

        /// <summary>
        /// Method GetUserByUserIdFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetUserByUserIdFalseAsync()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var user = await _oauthCallsRepository.GetUserByUserIdAsync(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest);
            Assert.NotEqual(user.Email, _stringConstant.TeamLeaderEmailForTest);
        }

        /// <summary>
        /// Method GetTeamLeaderUserIdFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetTeamLeaderUserIdFalseAsync()
        {
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            string teamLeaderUsername = _stringConstant.EmptyString;
            var teamLeader = await _oauthCallsRepository.GetTeamLeaderUserIdAsync(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest);
            foreach (var team in teamLeader)
            {
                teamLeaderUsername = team.FirstName;
            }
            Assert.NotEqual(teamLeaderUsername, _stringConstant.ManagementFirstForTest);
        }

        /// <summary>
        /// Method GetManagementUserNameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetManagementUserNameFalseAsync()
        {
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = _stringConstant.ManagementDetailsUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            string managementUsername = _stringConstant.EmptyString;
            var management = await _oauthCallsRepository.GetManagementUserNameAsync(_stringConstant.AccessTokenForTest);
            foreach (var team in management)
            {
                managementUsername = team.FirstName;
            }
            Assert.NotEqual(managementUsername, _stringConstant.FirstNameForTest);
        }

        /// <summary>
        /// Method CasualLeave testing with true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task CasualLeaveAsync()
        {
            var response = Task.FromResult(_stringConstant.CasualLeaveResponse);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.CasualLeaveUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var casualLeave = await _oauthCallsRepository.CasualLeaveAsync(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest);
            Assert.Equal(10, casualLeave.CasualLeave);
        }

        /// <summary>
        /// Method CasualLeave testing with false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task CasualLeaveFalseAsync()
        {
            var response = Task.FromResult(_stringConstant.CasualLeaveResponse);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.CasualLeaveUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var casualLeave = await _oauthCallsRepository.CasualLeaveAsync(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest);
            Assert.NotEqual(14, casualLeave.CasualLeave);
        }

        /// <summary>
        /// Method to test GetUserByEmployeeId with correct values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetUserByEmployeeIdAsync()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl );
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var userDetails = await _oauthCallsRepository.GetUserByEmployeeIdAsync(_stringConstant.EmployeeIdForTest, _stringConstant.TestAccessToken);
            Assert.Equal(userDetails.UserName, _stringConstant.TestUserName);
        }

        /// <summary>
        /// Method to test GetUserByEmployeeId with incorrect values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetUserByEmployeeIdFalseAsync()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var userDetails = await _oauthCallsRepository.GetUserByEmployeeIdAsync(_stringConstant.EmployeeIdForTest, _stringConstant.TestAccessToken);
            Assert.NotEqual(userDetails.UserName, _stringConstant.TestUserNameFalse);
        }

        /// <summary>
        /// Method to test GetProjectUsersByTeamLeaderId with correct values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetProjectUsersByTeamLeaderIdAsync()
        {
            var response = Task.FromResult(_stringConstant.ProjectUsers);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.EmployeeIdForTest, _stringConstant.ProjectUsersByTeamLeaderId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var userName = _stringConstant.EmptyString;
            var users = await _oauthCallsRepository.GetProjectUsersByTeamLeaderIdAsync(_stringConstant.EmployeeIdForTest, _stringConstant.TestAccessToken);
            foreach (var user in users)
            {
                userName = user.UserName;
            }
            Assert.Equal(userName, _stringConstant.FirstUserName);
        }

        /// <summary>
        /// Method to test GetProjectUsersByTeamLeaderId with incorrect values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetProjectUsersByTeamLeaderIdFalseAsync()
        {
            var response = Task.FromResult(_stringConstant.ProjectUsers);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.EmployeeIdForTest, _stringConstant.ProjectUsersByTeamLeaderId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var userName = _stringConstant.EmptyString;
            var users = await _oauthCallsRepository.GetProjectUsersByTeamLeaderIdAsync(_stringConstant.EmployeeIdForTest, _stringConstant.TestAccessToken);
            foreach (var user in users)
            {
                userName = user.UserName;
            }
            Assert.NotEqual(userName, _stringConstant.FirstUserNameFalse);
        }

        /// <summary>
        /// Method to test UserIsAdmin with correct values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UserIsAdminAsync()
        {
            var response = Task.FromResult(_stringConstant.True);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var result = await _oauthCallsRepository.UserIsAdminAsync(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest);
            Assert.Equal(true, result);
        }

        /// <summary>
        /// Method to test UserIsAdmin with correct values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UserIsAdminWrongAsync()
        {
            var response = Task.FromResult(_stringConstant.True);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var result = await _oauthCallsRepository.UserIsAdminAsync(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest);
            Assert.NotEqual(false, result);
        }

      

        /// <summary>
        /// Test case for conduct task mail after started
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetUserRoleAsync()
        {
            var response = Task.FromResult(_stringConstant.TaskMailReport);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.EmailForTest, _stringConstant.UserRoleUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var userRole = await _oauthCallsRepository.GetUserRoleAsync(_stringConstant.EmailForTest,_stringConstant.AccessTokenForTest);
            Assert.Equal(3, userRole.Count);
        }

      

        /// <summary>
        /// Method to test GetALlProjects with correct values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetAllProjectsTrueAsync()
        {
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForAdminFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.TestAccessToken)).Returns(responseProjects);
            var projects = await _oauthCallsRepository.GetAllProjectsAsync(_stringConstant.TestAccessToken);
            Assert.Equal(1, projects.Count);
        }

        /// <summary>
        /// Method to test GetProjectDetails with correct values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetProjectDetailsTrueAsync()
        {
            await AccessTokenSetUp();
            Mocking();
            int testProjectId = 1012;
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            var project = await _oauthCallsRepository.GetProjectDetailsAsync(testProjectId);
            Assert.Equal(2, project.ApplicationUsers.Count);
        }
        #endregion
        private void Mocking()
        {
            _mockHttpContextBase.Setup(x => x.User.Identity.Name).Returns(_stringConstant.EmailForTest);
        }

        private async Task AccessTokenSetUp()
        {
            var user = new ApplicationUser() { Email = _stringConstant.EmailForTest, UserName = _stringConstant.EmailForTest, SlackUserId = _stringConstant.FirstNameForTest };
            var result = await _userManager.CreateAsync(user);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            var secondResult = await _userManager.AddLoginAsync(user.Id, info);
        }
    }
}
