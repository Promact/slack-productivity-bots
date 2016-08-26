using Promact.Core.Repository.AttachmentRepository;
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
    public class LeaveReportController : WebApiBaseController
    {
        private readonly ILeaveReportRepository _leaveReport;
        private readonly IAttachmentRepository _attachmentRepository;
        private ApplicationUserManager _userManager;

        public LeaveReportController(ILeaveReportRepository leaveReport, IAttachmentRepository attachmentRepository, ApplicationUserManager userManager)
        {
            _leaveReport = leaveReport;
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Method that returns the list of employees with leave details
        /// </summary>
        /// <returns>list of employees with their leave details</returns>
        [HttpGet]
        [Route("leaveReport")]
        public async Task<IHttpActionResult> LeaveReport()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var login = await _userManager.GetLoginsAsync(user.Id);
            var acccessToken = _attachmentRepository.AccessToken(login);
            return Ok(await _leaveReport.LeaveReport(acccessToken));
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
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var login = await _userManager.GetLoginsAsync(user.Id);
                var acccessToken = _attachmentRepository.AccessToken(login);
                return Ok(await _leaveReport.LeaveReportDetails(employeeId,acccessToken));
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
