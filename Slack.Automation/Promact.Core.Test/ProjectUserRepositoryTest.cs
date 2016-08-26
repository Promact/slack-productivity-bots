using Autofac;
using Promact.Core.Repository.ProjectUserCall;
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
        string accessToken = "94d56876-fb02-45a9-8b01-c56046482d17";
        public ProjectUserRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _projectUserRepository = _componentContext.Resolve<IProjectUserCallRepository>();
        }

        /// <summary>
        /// Method GetUserByUsername Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetUserByUsername()
        {
            var user = await _projectUserRepository.GetUserByUsername("siddhartha", accessToken);
            Assert.Equal(user.Email, "siddhartha@promactinfo.com");
        }

        /// <summary>
        /// Method GetTeamLeaderUserName Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetTeamLeaderUserName()
        {
            string teamLeaderUsername = "";
            var teamLeader = await _projectUserRepository.GetTeamLeaderUserName("gourav", accessToken);
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
            string managementUsername = "";
            var management = await _projectUserRepository.GetManagementUserName(accessToken);
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
            var user = await _projectUserRepository.GetUserByUsername("siddhartha", accessToken);
            Assert.NotEqual(user.Email, "admin@promactinfo.com");
        }

        /// <summary>
        /// Method GetTeamLeaderUserNameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetTeamLeaderUserNameFalse()
        {
            string teamLeaderUsername = "";
            var teamLeader = await _projectUserRepository.GetTeamLeaderUserName("gourav", accessToken);
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
            var management = await _projectUserRepository.GetManagementUserName(accessToken);
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
            var user = await _projectUserRepository.GetUserByEmployeeId("4f044cd8-5bcf-4080-b330-58eb184d79bc", accessToken);
            Assert.Equal(user.Email, "roshni@promactinfo.com");
        }

        /// <summary>
        /// Method GetUserByEmployeeId Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetUserByIdFalse()
        {
            var user = await _projectUserRepository.GetUserByEmployeeId("4f044cd8-5bcf-4080-b330-58eb184d79bc", accessToken);
            Assert.NotEqual(user.Email, "xyz@promactinfo.com");
        }
    }
}
