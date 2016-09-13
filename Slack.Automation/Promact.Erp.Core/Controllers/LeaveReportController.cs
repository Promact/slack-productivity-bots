using Promact.Core.Repository.AttachmentRepository;
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

        /**
       * @api {get} leaveReport
       * @apiVersion 1.0.0
       * @apiName LeaveReport
       * @apiGroup LeaveReport    
       * @apiSuccessExample {json} Success-Response:
       * HTTP/1.1 200 OK 
       * {
       *     "Description":"A report will be generated based on the leave status of all the employees"
       *     "EmployeeId" : "2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf"
       *     "EmployeeName" : "Gourav Agarwal"
       *     "EmployeeUserName" : "gourav@promactinfo.com"
       *     "TotalSickLeave" : "7"
       *     "TotalCasualLeave" : "14"
       *     "UtilisedCasualLeave" : "5"
       *     "BalanceCasualLeave" :  "9"
       *     "Role" : "Employee"
       * }
       */
        [HttpGet]
        [Route("leaveReport")]
        public async Task<IHttpActionResult> LeaveReport()
        {
            var accessToken = await _attachmentRepository.AccessToken(User.Identity.Name);
            var loginUser = await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok(await _leaveReport.LeaveReport(accessToken, loginUser.UserName));
        }


        /**
        * @api {get} leaveReportDetails/{employeeId}
        * @apiVersion 1.0 
        * @apiName LeaveReport
        * @apiGroup LeaveReport
        * @apiParam {string} Name  EmployeeId    
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"A report will be generated based on the leaves of the employees "
        *     "EmployeeName" : "Gourav Agarwal"
        *     "EmployeeUserName" : "gourav@promactinfo.com"
        *     "LeaveFrom" : "21/5/16"
        *     "StartDay" : "Monday"
        *     "LeaveUpto" : "23/5/16"
        *     "EndDay" : "Wednesday"
        *     "Reason" : "Marriage"
        * }
        */
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
