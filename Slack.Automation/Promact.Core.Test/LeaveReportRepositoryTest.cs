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
        /// Method that returns the list of employees with their leave status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveReportTest()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var requestIdUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.EmployeeIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestIdUrl, StringConstant.TestAccessToken)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = _leaveReportRepository.LeaveReport(StringConstant.TestAccessToken,StringConstant.TestUserName).Result;
            Assert.Equal(1, leaveReports.Count());
        }

        /// <summary>
        /// Method that returns the details of leave for an employee
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
        /// Method that returns the list of employees with their leave status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveReportTestFalse()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var requestIdUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.EmployeeIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestIdUrl, StringConstant.TestAccessToken)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = _leaveReportRepository.LeaveReport(StringConstant.TestAccessToken, StringConstant.TestUserName).Result;
            Assert.NotEqual(2, leaveReports.Count());
        }

        /// <summary>
        /// Method that returns the details of leave for an employee
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
            Type = StringConstant.LeaveTypeForTest,
            CreatedOn = DateTime.UtcNow,
            EmployeeId = StringConstant.EmployeeIdForTest
        };
    }
}
