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
        private  IComponentContext _componentContext;
        private  ILeaveReportRepository _leaveReportRepository;
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
        public async void LeaveReportTest()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.StringIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, null)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = await _leaveReportRepository.LeaveReport();
            Assert.Equal(1,leaveReports.Count());
        }

        /// <summary>
        /// Method that returns the details of leave for an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportDetailTest()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.StringIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, null)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReport = await  _leaveReportRepository.LeaveReportDetails(StringConstant.StringIdForTest);
            Assert.NotNull(leaveReport);
        }

        /// <summary>
        /// Method that returns the list of employees with their leave status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportTestFalse()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.StringIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, null)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReports = await _leaveReportRepository.LeaveReport();
            Assert.NotEqual(5,leaveReports.Count());
        }

        /// <summary>
        /// Method that returns the details of leave for an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportDetailTestFalse()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.StringIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, null)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaveReport = await _leaveReportRepository.LeaveReportDetails(StringConstant.StringIdForTest);
            Assert.NotNull(leaveReport);
        }
        private LeaveRequest leave = new LeaveRequest() { FromDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Reason = StringConstant.LeaveReasonForTest, RejoinDate = DateTime.UtcNow, Status = Condition.Pending, Type = StringConstant.LeaveTypeForTest, CreatedOn = DateTime.UtcNow, EmployeeId = StringConstant.StringIdForTest };
    }
}
