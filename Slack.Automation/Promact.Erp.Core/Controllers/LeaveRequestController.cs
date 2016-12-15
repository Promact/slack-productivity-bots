using Autofac.Extras.NLog;
using Newtonsoft.Json;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util.StringConstants;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class LeaveRequestController : WebApiBaseController
    {
        private readonly ISlackRepository _slackRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ILogger _logger;
        private readonly IStringConstantRepository _stringConstant;
        public LeaveRequestController(ISlackRepository slackRepository, IAttachmentRepository attachmentRepository, ILogger logger, IStringConstantRepository stringConstant)
        {
            _slackRepository = slackRepository;
            _attachmentRepository = attachmentRepository;
            _logger = logger;
            _stringConstant = stringConstant;
        }

        /**
        * @api {post} leaverequest/leaveslashcommand
        * @apiVersion 1.0.0
        * @apiName SlackRequestAsync
        * @apiGroup LeaveRequest
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"A message of leave will be send user using incoming webhook"
        * }
        * @apiErrorExample {json} Error-Response:
        * HTTP/1.1 400 OK 
        * {
        *     "error":"A message of error will be send user using incoming webhook"
        * }
        */
        [HttpPost]
        [Route("leaverequest/leaveslashcommand")]
        public async Task<IHttpActionResult> SlackRequestAsync()
        {
            var request = HttpContext.Current.Request.Form;
            var leave = _attachmentRepository.SlashCommandTransfrom(request);
            try
            {
                await _slackRepository.LeaveRequestAsync(leave);
                return Ok();
            }
            // If throws any type of error it will give same message in slack by response_url
            catch (Exception ex)
            {
                await _slackRepository.ErrorAsync(leave.ResponseUrl, ex.Message);
                var errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageLeaveRequestControllerSlackRequest, ex.ToString());
                _logger.Error(errorMessage, ex);
                return BadRequest();
            }
        }

        /**
        * @api {post} leaverequest/leaveupdatebyslackbutton
        * @apiVersion 1.0.0
        * @apiName SlackButtonRequestAsync
        * @apiGroup LeaveRequest
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"A message of leave will be update in slack using slack chat update method"
        * }
        * @apiErrorExample {json} Error-Response:
        * HTTP/1.1 400 OK 
        * {
        *     "error":"A message of error will be update in slack using slack chat update method"
        * }
        */
        [HttpPost]
        [Route("leaverequest/leaveupdatebyslackbutton")]
        public async Task<IHttpActionResult> SlackButtonRequestAsync()
        {
            var request = HttpContext.Current.Request.Form;
            var leaveResponse = _attachmentRepository.SlashChatUpdateResponseTransfrom(request);
            try
            {
                await _slackRepository.UpdateLeaveAsync(leaveResponse);
                return Ok();
            }
            catch (Exception ex)
            {
                await _slackRepository.ErrorAsync(leaveResponse.ResponseUrl, ex.Message);
                var errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageLeaveRequestControllerSlackButtonRequest, ex.ToString());
                _logger.Error(errorMessage, ex);
                throw;
            }
        }
    }
}
