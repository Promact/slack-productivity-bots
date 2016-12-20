using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System;
using System.Linq;
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
        private readonly IStringConstantRepository _stringConstant;
        private readonly Mock<HttpContextBase> _mockHttpContextBase;
        private readonly ApplicationUserManager _userManager;
        private LeaveRequest leave = new LeaveRequest();
        #endregion

        #region Constructor
        public LeaveReportRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _leaveReportRepository = _componentContext.Resolve<ILeaveReportRepository>();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _mockHttpContextBase = _componentContext.Resolve<Mock<HttpContextBase>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            Initialize();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        private void Initialize()
        {
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            leave.FromDate = DateTime.UtcNow;
            leave.EndDate = DateTime.UtcNow;
            leave.Reason = _stringConstant.LeaveReasonForTest;
            leave.RejoinDate = DateTime.UtcNow;
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.cl;
            leave.CreatedOn = DateTime.UtcNow;
        }

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
        #endregion

        #region Test Cases
        /// <summary>
        /// Method to test LeaveReport when the logged in person is admin
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportAdminTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest,_stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = await _leaveReportRepository.LeaveReportAsync( _stringConstant.EmployeeIdForTest);
            Assert.Equal(true, leaveReports.Any());
        }

        /// <summary>
        /// Method to test LeaveReport when the logged in person is employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportEmployeeTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            var response = Task.FromResult(_stringConstant.EmployeeDetailFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = await _leaveReportRepository.LeaveReportAsync( _stringConstant.EmployeeIdForTest);
            Assert.Equal(true, leaveReports.Any());
        }

        /// <summary>
        /// Method to test LeaveReport when the logged in person is teamleader
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportTeamLeaderTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            var response = Task.FromResult(_stringConstant.TeamLeaderDetailFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectUsers);
            var requestProjectUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.ProjectUsersByTeamLeaderId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestProjectUrl, _stringConstant.AccessTokenForTest)).Returns(responseProject);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = await _leaveReportRepository.LeaveReportAsync( _stringConstant.EmployeeIdForTest);
            Assert.Equal(true, leaveReports.Any());
        }

        /// <summary>
        /// Method to test LeaveReportDetails that returns the details of leave for an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportDetailTest()
        {
            await AccessTokenSetUp();
            MockIdentity();
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReport = await _leaveReportRepository.LeaveReportDetailsAsync(_stringConstant.EmployeeIdForTest);
            Assert.Equal(true, leaveDetail.Any());
        }

        /// <summary>
        /// Method to test LeaveReport that returns the list of employees with their leave status for incorrect values
        /// </summary> 
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportTestFalse()
        {
            await AccessTokenSetUp();
            MockIdentity();
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = await _leaveReportRepository.LeaveReportAsync( _stringConstant.EmployeeIdForTest);
            Assert.NotEqual(false, leaveReports.Any());
        }



        /// <summary>
        /// Method to test LeaveReportDetails that returns the details of leave for an employee with incorrect values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveReportDetailTestFalse()
        {
            await AccessTokenSetUp();
            MockIdentity();
            var response = Task.FromResult(_stringConstant.EmptyString);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReport = await _leaveReportRepository.LeaveReportDetailsAsync(_stringConstant.EmployeeIdForTest);
            Assert.NotEqual(false, leaveDetail.Any());
        }
        #endregion

    }
}
