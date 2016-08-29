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
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.TeamLeaderDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            string teamLeaderUsername = "";
            var teamLeader = await _projectUserRepository.GetTeamLeaderUserName("gourav", StringConstant.AccessTokenForTest);
            foreach (var team in teamLeader)
            {
                teamLeaderUsername = team.Email;
            }
            Assert.Equal(teamLeaderUsername, "siddhartha@promactinfo.com");
        }

        /// <summary>
        /// Method GetManagementUserName Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetManagementUserName()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}", StringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            string managementUsername = "";
            var management = await _projectUserRepository.GetManagementUserName(StringConstant.AccessTokenForTest);
            foreach (var team in management)
            {
                managementUsername = team.FirstName;
            }
            Assert.NotEqual(managementUsername, "rinkesh");
        }

        /// <summary>
        /// Method GetUserByUsernameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetUserByUsernameFalse()
        {
            var user = await _projectUserRepository.GetUserByUsername("siddhartha", StringConstant.AccessTokenForTest);
            Assert.NotEqual(user.Email, "admin@promactinfo.com");
        }

        /// <summary>
        /// Method GetTeamLeaderUserNameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetTeamLeaderUserNameFalse()
        {
            string teamLeaderUsername = "";
            var teamLeader = await _projectUserRepository.GetTeamLeaderUserName("gourav", StringConstant.AccessTokenForTest);
            foreach (var team in teamLeader)
            {
                teamLeaderUsername = team.FirstName;
            }
            Assert.NotEqual(teamLeaderUsername, "roshni");
        }

        /// <summary>
        /// Method GetManagementUserNameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetManagementUserNameFalse()
        {
            string managementUsername = "";
            var management = await _projectUserRepository.GetManagementUserName(StringConstant.AccessTokenForTest);
            foreach (var team in management)
            {
                managementUsername = team.FirstName;
            }
            Assert.NotEqual(managementUsername, "divyangi");
        }

        /// <summary>
        /// Method GetUserByEmployeeId Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetUserById()
        {
            var user = await _projectUserRepository.GetUserByEmployeeId("4f044cd8-5bcf-4080-b330-58eb184d79bc");
            Assert.Equal(user.Email, "roshni@promactinfo.com");
        }

        /// <summary>
        /// Method GetUserByEmployeeId Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetUserByIdFalse()
        {
            var user = await _projectUserRepository.GetUserByEmployeeId("4f044cd8-5bcf-4080-b330-58eb184d79bc");
            Assert.NotEqual(user.Email, "xyz@promactinfo.com");
        }
    }
}
