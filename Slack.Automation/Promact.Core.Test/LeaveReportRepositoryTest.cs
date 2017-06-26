using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Newtonsoft.Json;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.ServiceRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringLiteral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Promact.Core.Test
{
    public class LeaveReportRepositoryTest
    {
        #region Private Variables
        private IComponentContext _componentContext;
        private ILeaveReportRepository _leaveReportRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly Mock<IHttpClientService> _mockHttpClient;
        private readonly AppStringLiteral _stringConstant;
        private LeaveRequest leave = new LeaveRequest();
        private readonly Mock<HttpContextBase> _mockHttpContextBase;
        private readonly ApplicationUserManager _userManager;
        private readonly Mock<IServiceRepository> _mockServiceRepository;
        #endregion

        #region Constructor
        public LeaveReportRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _leaveReportRepository = _componentContext.Resolve<ILeaveReportRepository>();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _stringConstant = _componentContext.Resolve<ISingletonStringLiteral>().StringConstant;
            _mockHttpContextBase = _componentContext.Resolve<Mock<HttpContextBase>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _mockServiceRepository = _componentContext.Resolve<Mock<IServiceRepository>>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Method to test LeaveReport when the logged in person is admin
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportAdminTestAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(response);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReports = _leaveReportRepository.LeaveReportAsync(_stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(true, leaveReports.Any());
        }

        /// <summary>
        /// Method to test LeaveReport when the logged in person is employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportEmployeeTestAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            MockingGetListOfEmployeeAsync();
            var response = Task.FromResult(_stringConstant.EmployeeDetailFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(response);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReports = _leaveReportRepository.LeaveReportAsync(_stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(true, leaveReports.Any());
        }

        /// <summary>
        /// Method to test LeaveReport when the logged in person is teamleader
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportTeamLeaderTestAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            MockingGetListOfEmployeeAsync();
            var response = Task.FromResult(_stringConstant.TeamLeaderDetailFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectUsers);
            var requestProjectUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.ProjectUsersByTeamLeaderId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(responseProject);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReports = _leaveReportRepository.LeaveReportAsync(_stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(true, leaveReports.Any());
        }

        /// <summary>
        /// Method to test LeaveReportDetails that returns the details of leave for an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportDetailTestAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            var response = Task.FromResult(_stringConstant.EmployeeDetailFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(response);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReport = _leaveReportRepository.LeaveReportDetailsAsync(_stringConstant.EmployeeIdForTest).Result;
            Assert.NotNull(leaveReport);
        }

        /// <summary>
        /// Method to test LeaveReport that returns the list of employees with their leave status for incorrect values
        /// </summary> 
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportTestFalseAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            MockingGetListOfEmployeeAsync();
            var response = Task.FromResult(_stringConstant.TeamLeaderDetailFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(response);
            leave.EmployeeId = _stringConstant.StringIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReports = _leaveReportRepository.LeaveReportAsync(_stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(false, leaveReports.Any());
        }



        /// <summary>
        /// Method to test LeaveReportDetails that returns the details of leave for an employee with incorrect values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportDetailTestFalseAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            var response = Task.FromResult(_stringConstant.EmptyString);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(response);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveDetail = _leaveReportRepository.LeaveReportDetailsAsync(_stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(false, leaveDetail.Any());
        }
        #endregion

        #region Initialization
        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialize()
        {
            leave.FromDate = DateTime.UtcNow;
            leave.EndDate = DateTime.UtcNow;
            leave.Reason = _stringConstant.LeaveReasonForTest;
            leave.RejoinDate = DateTime.UtcNow;
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.cl;
            leave.CreatedOn = DateTime.UtcNow;
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
            _mockServiceRepository.Setup(x => x.GerAccessTokenByRefreshToken(It.IsAny<string>(), It.IsAny<string>())).Returns(accessToken);
        }

        /// <summary>
        /// Mocking of GetListOfEmployeeAsync of OAuthCallsRepository
        /// </summary>
        private void MockingGetListOfEmployeeAsync()
        {
            var userRoleAc = new List<UserRoleAc>();
            userRoleAc.Add(new UserRoleAc() { Name = _stringConstant.EmailForTest, Role = _stringConstant.Employee, UserId = _stringConstant.EmployeeIdForTest, UserName = _stringConstant.EmailForTest });
            var result = JsonConvert.SerializeObject(userRoleAc);
            var response = Task.FromResult(result);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.EmployeeIdForTest, _stringConstant.TeamMembersUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(response);
        }
        #endregion
    }
}
