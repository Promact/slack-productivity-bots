using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class LeaveReportController : ApiController
    {
        private readonly ILeaveReportRepository _leaveReport;

        public LeaveReportController(ILeaveReportRepository leaveReport)
        {
            this._leaveReport = leaveReport;
        }

        /// <summary>
        /// Method that returns the list of employees with leave details
        /// </summary>
        /// <returns>list of employees with their leave details</returns>
        [HttpGet]
        [Route("leaveReport")]
        public async Task<IHttpActionResult> LeaveReport()
        {
            return Ok(await _leaveReport.LeaveReport());
        }


        /// <summary>
        /// Method to return the details of leave for an employee
        /// </summary>
        /// <returns>Details of leave for an employee </returns>
        [HttpGet]
        [Route("leaveReportDetails/{employeeId}")]
        public async Task<IHttpActionResult> LeaveReportDetails(string employeeId)
        {
            if (employeeId != null)
            {
                return Ok(await _leaveReport.LeaveReportDetails(employeeId));
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
