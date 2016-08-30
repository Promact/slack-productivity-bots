using Autofac;
using Moq;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.Util;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    /// <summary>
    /// Test Cases of Project User Call Repository
    /// </summary>
    public class ProjectUserRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IProjectUserCallRepository _projectUserRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        public ProjectUserRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _projectUserRepository = _componentContext.Resolve<IProjectUserCallRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
        }

        /// <summary>
        /// Method GetUserByUsername Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetUserByUsername()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var user = await _projectUserRepository.GetUserByUsername(StringConstant.FirstNameForTest, StringConstant.AccessTokenForTest);
            Assert.NotEqual(user.Email, StringConstant.EmailForTest);
        }

        /// <summary>
        /// Method GetTeamLeaderUserName Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetTeamLeaderUserName()
        {
            var teamLeaderResponse = Task.FromResult(StringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", StringConstant.TeamLeaderDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, teamLeaderRequestUrl, StringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            string teamLeaderUsername = "";
            var teamLeader = await _projectUserRepository.GetTeamLeaderUserName(StringConstant.FirstNameForTest, StringConstant.AccessTokenForTest);
            foreach (var team in teamLeader)
            {
                teamLeaderUsername = team.Email;
            }
            Assert.Equal(teamLeaderUsername, StringConstant.TeamLeaderEmailForTest);
        }

        /// <summary>
        /// Method GetManagementUserName Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetManagementUserName()
        {
            var managementResponse = Task.FromResult(StringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", StringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, managementRequestUrl, StringConstant.AccessTokenForTest)).Returns(managementResponse);
            string managementUsername = "";
            var management = await _projectUserRepository.GetManagementUserName(StringConstant.AccessTokenForTest);
            foreach (var team in management)
            {
                managementUsername = team.FirstName;
            }
            Assert.Equal(managementUsername, StringConstant.ManagementFirstForTest);
        }

        /// <summary>
        /// Method GetUserByUsernameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetUserByUsernameFalse()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var user = await _projectUserRepository.GetUserByUsername(StringConstant.FirstNameForTest, StringConstant.AccessTokenForTest);
            Assert.NotEqual(user.Email, StringConstant.TeamLeaderEmailForTest);
        }

        /// <summary>
        /// Method GetTeamLeaderUserNameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetTeamLeaderUserNameFalse()
        {
            var teamLeaderResponse = Task.FromResult(StringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", StringConstant.TeamLeaderDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, teamLeaderRequestUrl, StringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            string teamLeaderUsername = "";
            var teamLeader = await _projectUserRepository.GetTeamLeaderUserName(StringConstant.FirstNameForTest, StringConstant.AccessTokenForTest);
            foreach (var team in teamLeader)
            {
                teamLeaderUsername = team.FirstName;
            }
            Assert.NotEqual(teamLeaderUsername, StringConstant.ManagementFirstForTest);
        }

        /// <summary>
        /// Method GetManagementUserNameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetManagementUserNameFalse()
        {
            var managementResponse = Task.FromResult(StringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", StringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, managementRequestUrl, StringConstant.AccessTokenForTest)).Returns(managementResponse);
            string managementUsername = "";
            var management = await _projectUserRepository.GetManagementUserName(StringConstant.AccessTokenForTest);
            foreach (var team in management)
            {
                managementUsername = team.FirstName;
            }
            Assert.NotEqual(managementUsername, StringConstant.FirstNameForTest);
        }

        /// <summary>
        /// Method GetUserByEmployeeId Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetUserById()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.StringIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, null)).Returns(response);
            var user = await _projectUserRepository.GetUserByEmployeeId(StringConstant.StringIdForTest);
            Assert.Equal(user.Email, StringConstant.ManagementEmailForTest);
        }

        /// <summary>
        /// Method GetUserByEmployeeId Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetUserByIdFalse()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.StringIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, null)).Returns(response);
            var user = await _projectUserRepository.GetUserByEmployeeId(StringConstant.StringIdForTest);
            Assert.NotEqual(user.Email, StringConstant.EmailForTest);
        }
    }
}
