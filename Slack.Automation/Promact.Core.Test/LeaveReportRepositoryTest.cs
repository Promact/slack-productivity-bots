using Autofac;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Erp.DomainModel.Models;
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

        public LeaveReportRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _leaveReportRepository = _componentContext.Resolve<ILeaveReportRepository>();
        }

        /// <summary>
        /// Method that returns the list of employees with their leave status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportTest()
        {
            var leaveReports = await _leaveReportRepository.LeaveReport();
            Assert.Equal(1,leaveReports.Count());
        }

        /// <summary>
        /// Method that returns the details of leave for an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportDetailTest()
        {
            var leaveReport = await  _leaveReportRepository.LeaveReportDetails("4f044cd8-5bcf-4080-b330-58eb184d79bc");
            Assert.NotNull(leaveReport);
        }

        /// <summary>
        /// Method that returns the list of employees with their leave status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportTestFalse()
        {
            var leaveReports = await _leaveReportRepository.LeaveReport();
            Assert.NotEqual(5,leaveReports.Count());
        }

        /// <summary>
        /// Method that returns the details of leave for an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveReportDetailTestFalse()
        {
            var leaveReport = await _leaveReportRepository.LeaveReportDetails("4f044cd8-5bcf-4080-b330-58eb184d79bc");
            Assert.Null(leaveReport);
        }
    }
}
