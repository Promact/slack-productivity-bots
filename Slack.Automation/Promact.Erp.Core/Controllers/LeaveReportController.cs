using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Erp.DomainModel.Models;
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
            var accessToken = await _attachmentRepository.AccessToken(User.Identity.Name);
            var loginUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var userName = loginUser.UserName;
            return Ok(await _leaveReport.LeaveReport(accessToken, userName));
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
                var accessToken = await _attachmentRepository.AccessToken(User.Identity.Name);
                return Ok(await _leaveReport.LeaveReportDetails(employeeId,accessToken));
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
