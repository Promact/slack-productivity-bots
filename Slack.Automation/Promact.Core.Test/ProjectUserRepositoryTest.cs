using Autofac;
using Promact.Core.Repository.ProjectUserCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class ProjectUserRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IProjectUserCallRepository _projectUserRepository;
        public ProjectUserRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _projectUserRepository = _componentContext.Resolve<IProjectUserCallRepository>();
        }

        /// <summary>
        /// Testing with True Value
        /// </summary>
        [Fact]
        public async void GetUserByUsername()
        {
            var user = await _projectUserRepository.GetUserByUsername("siddhartha");
            Assert.Equal(user.Email, "siddhartha@promactinfo.com");
        }

        [Fact]
        public async void GetTeamLeaderUserName()
        {
            string teamLeaderUsername = "";
            var teamLeader = await _projectUserRepository.GetTeamLeaderUserName("rajdeep");
            foreach (var team in teamLeader)
            {
                teamLeaderUsername = team.Username;
            }
            Assert.Equal(teamLeaderUsername, "siddhartha");
        }

        [Fact]
        public async void GetManagementUserName()
        {
            string managementUsername = "";
            var management = await _projectUserRepository.GetManagementUserName();
            foreach (var team in management)
            {
                managementUsername = team.Username;
            }
            Assert.Equal(managementUsername, "rinkesh");
        }

        /// <summary>
        /// Testing with True Value
        /// </summary>
        [Fact]
        public async void GetUserByUsernameFalse()
        {
            var user = await _projectUserRepository.GetUserByUsername("siddhartha");
            Assert.NotEqual(user.Email, "admin@promactinfo.com");
        }

        [Fact]
        public async void GetTeamLeaderUserNameFalse()
        {
            string teamLeaderUsername = "";
            var teamLeader = await _projectUserRepository.GetTeamLeaderUserName("rajdeep");
            foreach (var team in teamLeader)
            {
                teamLeaderUsername = team.Username;
            }
            Assert.NotEqual(teamLeaderUsername, "roshni");
        }

        [Fact]
        public async void GetManagementUserNameFalse()
        {
            string managementUsername = "";
            var management = await _projectUserRepository.GetManagementUserName();
            foreach (var team in management)
            {
                managementUsername = team.Username;
            }
            Assert.NotEqual(managementUsername, "divyangi");
        }
    }
}
