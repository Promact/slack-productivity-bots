using Autofac.Extras.NLog;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.StringConstants;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    [Authorize]
    [RoutePrefix("api/leaveReport")]
    public class LeaveReportController : WebApiBaseController
    {
        #region Private Variables 
        private readonly ILeaveReportRepository _leaveReport;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ApplicationUserManager _userManager;
        private readonly ILogger _logger;
        private readonly IStringConstantRepository _stringConstant;
        #endregion

        #region Constructor
        public LeaveReportController(ILeaveReportRepository leaveReport, IAttachmentRepository attachmentRepository, ILogger logger, ApplicationUserManager userManager, IStringConstantRepository stringConstant)
        {
            _leaveReport = leaveReport;
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
            _logger = logger;
            _stringConstant = stringConstant;
        }
        #endregion

        #region Public methods
        /**
       * @api {get} api/leaveReport
       * @apiVersion 1.0.0
       * @apiName GetLeaveReport
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
       *     "UtilisedSickLeave" : "2"
       *     "BalanceSickLeave" :  "5"
       *     "Role" : "Employee"
       * }
       */
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> LeaveReportAsync()
        {
            try
            {
                var accessToken = await _attachmentRepository.UserAccessTokenAsync(User.Identity.Name);
                var loginUser = await _userManager.FindByNameAsync(User.Identity.Name);
                return Ok(await _leaveReport.LeaveReportAsync(accessToken, loginUser.Id));
            }
            catch (ForbiddenUserException)
            {
                _logger.Error(_stringConstant.ForbiddenUser);
                return StatusCode(HttpStatusCode.Forbidden);
            }

        }


        /**
        * @api {get} api/leaveReport/{id}
        * @apiVersion 1.0 
        * @apiName GetLeaveReportDetails
        * @apiGroup LeaveReport
        * @apiParam {string} id    
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"A report will be generated based on the leaves taken by a particular employee "
        *     "EmployeeName" : "Gourav Agarwal"
        *     "EmployeeUserName" : "gourav@promactinfo.com"
        *     "LeaveFrom" : "21/5/16"
        *     "StartDay" : "Monday"
        *     "LeaveUpto" : "23/5/16"
        *     "EndDay" : "Wednesday"
        *     "Reason" : "Marriage"
        *     "Type" : "cl"
        * }
        */
        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> LeaveReportDetailsAsync(string id)
        {
            try
            {
                if (id != null)
                {
                    var accessToken = await _attachmentRepository.UserAccessTokenAsync(User.Identity.Name);
                    return Ok(await _leaveReport.LeaveReportDetailsAsync(id, accessToken));
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (ForbiddenUserException)
            {
                _logger.Error(_stringConstant.ForbiddenUser);
                return StatusCode(HttpStatusCode.Forbidden);
            }
        }
        #endregion
    }
}
