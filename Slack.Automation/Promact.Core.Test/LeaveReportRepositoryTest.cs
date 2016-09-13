using Autofac;
using Moq;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class LeaveReportRepositoryTest
    {
        private IComponentContext _componentContext;
        private ILeaveReportRepository _leaveReportRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;

        public LeaveReportRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _leaveReportRepository = _componentContext.Resolve<ILeaveReportRepository>();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
        }

        /// <summary>
        /// Method to test LeaveReport when the logged in person is admin
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveReportAdminTest()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var requestIdUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.EmployeeIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestIdUrl, StringConstant.TestAccessToken)).Returns(response);
            leave.EmployeeId = StringConstant.EmployeeIdForTest;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = _leaveReportRepository.LeaveReport(StringConstant.TestAccessToken,StringConstant.TestUserName).Result;
            Assert.Equal(1, leaveReports.Count());
        }

        /// <summary>
        /// Method to test LeaveReport when the logged in person is employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveReportEmployeeTest()
        {
            var response = Task.FromResult(StringConstant.EmployeeDetailFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var requestIdUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.IdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestIdUrl, StringConstant.TestAccessToken)).Returns(response);
            leave.EmployeeId = StringConstant.IdForTest;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = _leaveReportRepository.LeaveReport(StringConstant.TestAccessToken, StringConstant.TestUserName).Result;
            Assert.Equal(1, leaveReports.Count());
        }

        /// <summary>
        /// Method to test LeaveReport when the logged in person is teamleader
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveReportTeamLeaderTest()
        {
            var response = Task.FromResult(StringConstant.TeamLeaderDetailFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var requestIdUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.IdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestIdUrl, StringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(StringConstant.ProjectUsers);
            var requestProjectUrl = string.Format("{0}{1}", StringConstant.ProjectUsersByTeamLeaderId, StringConstant.IdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, requestProjectUrl, StringConstant.TestAccessToken)).Returns(responseProject);
            leave.EmployeeId = StringConstant.IdForTest;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = _leaveReportRepository.LeaveReport(StringConstant.TestAccessToken, StringConstant.TestUserName).Result;
            Assert.Equal(1, leaveReports.Count());
        }

        /// <summary>
        /// Method to test LeaveReportDetails that returns the details of leave for an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveReportDetailTest()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.EmployeeIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReport = _leaveReportRepository.LeaveReportDetails(StringConstant.EmployeeIdForTest, StringConstant.TestAccessToken).Result;
            Assert.NotNull(leaveReport);
        }

        /// <summary>
        /// Method to test LeaveReport that returns the list of employees with their leave status for incorrect values
        /// </summary> 
        [Fact, Trait("Category", "Required")]
        public void LeaveReportTestFalse()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var requestIdUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.EmployeeIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestIdUrl, StringConstant.TestAccessToken)).Returns(response);
            leave.EmployeeId = StringConstant.EmployeeIdForTest;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = _leaveReportRepository.LeaveReport(StringConstant.TestAccessToken, StringConstant.TestUserName).Result;
            Assert.NotEqual(2, leaveReports.Count());
        }



        /// <summary>
        /// Method to test LeaveReportDetails that returns the details of leave for an employee with incorrect values
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveReportDetailTestFalse()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.EmployeeIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReport = _leaveReportRepository.LeaveReportDetails(StringConstant.EmployeeIdForTest, StringConstant.TestAccessToken).Result;
            Assert.NotEqual(2, leaveReport.Count());
        }

        /// <summary>
        /// A mock leave request object
        /// </summary>
        private LeaveRequest leave = new LeaveRequest()
        {
            FromDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow,
            Reason = StringConstant.LeaveReasonForTest,
            RejoinDate = DateTime.UtcNow,
            Status = Condition.Approved,
            Type = LeaveType.cl,
            CreatedOn = DateTime.UtcNow,
        };
    }
}
