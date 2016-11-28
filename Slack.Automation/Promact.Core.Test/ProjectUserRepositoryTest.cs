using Autofac;
using Moq;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.Util.StringConstants;
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
        private readonly IStringConstantRepository _stringConstant;
        public ProjectUserRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _projectUserRepository = _componentContext.Resolve<IProjectUserCallRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
        }

        /// <summary>
        /// Method GetUserByUsername Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetUserByUsername()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var user = _projectUserRepository.GetUserByUsername(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest).Result;
            Assert.Equal(user.Email, _stringConstant.ManagementEmailForTest);
        }

        /// <summary>
        /// Method GetTeamLeaderUserName Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetTeamLeaderUserName()
        {
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            string teamLeaderUsername = "";
            var teamLeader = _projectUserRepository.GetTeamLeaderUserName(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest).Result;
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
        public void GetManagementUserName()
        {
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", _stringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            string managementUsername = "";
            var management = _projectUserRepository.GetManagementUserName(_stringConstant.AccessTokenForTest).Result;
            foreach (var team in management)
            {
                managementUsername = team.FirstName;
            }
            Assert.Equal(managementUsername, _stringConstant.ManagementFirstForTest);
        }

        /// <summary>
        /// Method GetUserByUsernameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetUserByUsernameFalse()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var user = _projectUserRepository.GetUserByUsername(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest).Result;
            Assert.NotEqual(user.Email, _stringConstant.TeamLeaderEmailForTest);
        }

        /// <summary>
        /// Method GetTeamLeaderUserNameFalse Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetTeamLeaderUserNameFalse()
        {
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            string teamLeaderUsername = "";
            var teamLeader = _projectUserRepository.GetTeamLeaderUserName(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest).Result;
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
        public void GetManagementUserNameFalse()
        {
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", _stringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            string managementUsername = "";
            var management = _projectUserRepository.GetManagementUserName(_stringConstant.AccessTokenForTest).Result;
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
        public void CasualLeave()
        {
            var response = Task.FromResult(_stringConstant.CasualLeaveResponse);
            var requestUrl = string.Format("{0}{1}", _stringConstant.CasualLeaveUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var casualLeave = _projectUserRepository.CasualLeave(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest).Result;
            Assert.Equal(10, casualLeave.CasualLeave);
        }

        /// <summary>
        /// Method CasualLeave testing with false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void CasualLeaveFalse()
        {
            var response = Task.FromResult(_stringConstant.CasualLeaveResponse);
            var requestUrl = string.Format("{0}{1}", _stringConstant.CasualLeaveUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var casualLeave = _projectUserRepository.CasualLeave(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest).Result;
            Assert.NotEqual(14, casualLeave.CasualLeave);
        }

        /// <summary>
        /// Method to test GetUserByEmployeeId with correct values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetUserByEmployeeId()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl,requestUrl,_stringConstant.TestAccessToken)).Returns(response);
            var userDetails = _projectUserRepository.GetUserByEmployeeId(_stringConstant.EmployeeIdForTest,_stringConstant.TestAccessToken).Result;
            Assert.Equal(userDetails.UserName,_stringConstant.TestUserName); 
        }

        /// <summary>
        /// Method to test GetUserByEmployeeId with incorrect values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetUserByEmployeeIdFalse()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var userDetails = _projectUserRepository.GetUserByEmployeeId(_stringConstant.EmployeeIdForTest, _stringConstant.TestAccessToken).Result;
            Assert.NotEqual(userDetails.UserName, _stringConstant.TestUserNameFalse);
        }

        /// <summary>
        /// Method to test GetUserByUserName with correct values 
        /// </summary>
        [Fact, Trait("Category","Required")]
        public void GetUserByUserName()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.TestUserName,_stringConstant.LoginUserDetail);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var userDetails = _projectUserRepository.GetUserByUserName(_stringConstant.TestUserName,_stringConstant.TestAccessToken).Result;
            Assert.Equal(userDetails.UserName,_stringConstant.TestUserName);
        }

        /// <summary>
        /// Method to test GetUserByUserName with incorrect values 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetUserByUserNameFalse()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.TestUserName, _stringConstant.LoginUserDetail);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var userDetails = _projectUserRepository.GetUserByUserName(_stringConstant.TestUserName, _stringConstant.TestAccessToken).Result;
            Assert.NotEqual(userDetails.UserName, _stringConstant.TestUserNameFalse);
        }

        /// <summary>
        /// Method to test GetProjectUsersByTeamLeaderId with correct values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetProjectUsersByTeamLeaderId()
        {
            var response = Task.FromResult(_stringConstant.ProjectUsers);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest,_stringConstant.ProjectUsersByTeamLeaderId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var userName = _stringConstant.EmptyString;
            var users = _projectUserRepository.GetProjectUsersByTeamLeaderId(_stringConstant.EmployeeIdForTest, _stringConstant.TestAccessToken).Result;
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
        public void GetProjectUsersByTeamLeaderIdFalse()
        {
            var response = Task.FromResult(_stringConstant.ProjectUsers);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.ProjectUsersByTeamLeaderId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var userName = _stringConstant.EmptyString;
            var users = _projectUserRepository.GetProjectUsersByTeamLeaderId(_stringConstant.EmployeeIdForTest, _stringConstant.TestAccessToken).Result;
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
        public void UserIsAdmin()
        {
            var response = Task.FromResult(_stringConstant.True);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var result = _projectUserRepository.UserIsAdmin(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest).Result;
            Assert.Equal(true, result);
        }

        /// <summary>
        /// Method to test UserIsAdmin with correct values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void UserIsAdminWrong()
        {
            var response = Task.FromResult(_stringConstant.True);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var result = _projectUserRepository.UserIsAdmin(_stringConstant.FirstNameForTest, _stringConstant.AccessTokenForTest).Result;
            Assert.NotEqual(false, result);
        }

      

        /// <summary>
        /// Test case for conduct task mail after started
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetUserRole()
        {
            var response = Task.FromResult(_stringConstant.TaskMailReport);
            var requestUrl = string.Format("{0}{1}", _stringConstant.ProjectInformationUrl, _stringConstant.EmailForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var userRole =  _projectUserRepository.GetUserRole(_stringConstant.EmailForTest,_stringConstant.AccessTokenForTest);
            Assert.Equal(3, userRole.Result.Count);
        }

      
    }
}
