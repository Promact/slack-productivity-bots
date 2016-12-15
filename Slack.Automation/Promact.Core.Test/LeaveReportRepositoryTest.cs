using Autofac;
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
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Method to test LeaveReport when the logged in person is admin
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportAdminTest()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.TestAccessToken)).Returns(response);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReports = _leaveReportRepository.LeaveReportAsync(_stringConstant.TestAccessToken, _stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(true, leaveReports.Any());
        }

        /// <summary>
        /// Method to test LeaveReport when the logged in person is employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportEmployeeTest()
        {
            var response = Task.FromResult(_stringConstant.EmployeeDetailFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.TestAccessToken)).Returns(response);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReports = _leaveReportRepository.LeaveReportAsync(_stringConstant.TestAccessToken, _stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(true, leaveReports.Any());
        }

        /// <summary>
        /// Method to test LeaveReport when the logged in person is teamleader
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportTeamLeaderTest()
        {
            var response = Task.FromResult(_stringConstant.TeamLeaderDetailFromOauthServer);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectUsers);
            var requestProjectUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.ProjectUsersByTeamLeaderId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.TestAccessToken)).Returns(responseProject);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReports = _leaveReportRepository.LeaveReportAsync(_stringConstant.TestAccessToken, _stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(true, leaveReports.Any());
        }

        /// <summary>
        /// Method to test LeaveReportDetails that returns the details of leave for an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportDetailTest()
        {
            var response = Task.FromResult(_stringConstant.EmployeeDetailFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest, _stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReport = _leaveReportRepository.LeaveReportDetailsAsync(_stringConstant.EmployeeIdForTest, _stringConstant.TestAccessToken).Result;
            Assert.NotNull(leaveReport);
        }

        /// <summary>
        /// Method to test LeaveReport that returns the list of employees with their leave status for incorrect values
        /// </summary> 
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportTestFalse()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServerFalse);
            var requestIdUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest,_stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestIdUrl, _stringConstant.TestAccessToken)).Returns(response);
            leave.EmployeeId = _stringConstant.EmployeeIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var leaveReports = _leaveReportRepository.LeaveReportAsync(_stringConstant.TestAccessToken, _stringConstant.EmployeeIdForTest).Result;
            Assert.Equal(false, leaveReports.Any());
        }



        /// <summary>
        /// Method to test LeaveReportDetails that returns the details of leave for an employee with incorrect values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportDetailTestFalse()
        {
            var response = Task.FromResult(_stringConstant.EmptyString);
            var requestUrl = string.Format("{0}{1}", _stringConstant.EmployeeIdForTest,_stringConstant.UserDetailUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
           await  _leaveRequestRepository.ApplyLeave(leave);
            var leaveDetail = _leaveReportRepository.LeaveReportDetailsAsync(_stringConstant.EmployeeIdForTest, _stringConstant.TestAccessToken).Result;
            Assert.Equal(false, leaveDetail.Any());
        }


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
    }
}
