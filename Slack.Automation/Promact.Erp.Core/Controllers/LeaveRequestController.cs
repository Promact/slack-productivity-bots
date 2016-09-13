using NLog;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util;
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

        public LeaveRequestController(ISlackRepository slackRepository, IAttachmentRepository attachmentRepository,  ILogger logger)
        {
            _slackRepository = slackRepository;
            _attachmentRepository = attachmentRepository;
            _logger = logger;
        }

        /**
        * @api {post} leaves/slackcall
        * @apiVersion 1.0.0
        * @apiName SlackRequest
        * @apiGroup LeaveRequest  
        * @apiParam {string} Name  Token
        * @apiParam {string} Name  TeamId
        * @apiParam {string} Name  TeamDomain
        * @apiParam {string} Name  ChannelId
        * @apiParam {string} Name  ChannelName
        * @apiParam {string} Name  UserId
        * @apiParam {string} Name  Username
        * @apiParam {string} Name  Command
        * @apiParam {string} Name  Text
        * @apiParam {string} Name  ResponseUrl
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"A message will be send user using incoming webhook"
        * }
        */
        [HttpPost]
        [Route("leaves/slackcall")]
        public async Task<IHttpActionResult> SlackRequest()
        {
            var request = HttpContext.Current.Request.Form;
            var leave = _attachmentRepository.SlashCommandTransfrom(request);
            try
            {
                await _slackRepository.Leave(leave);
                return Ok();
            }
            // If throws any type of error it will give same message in slack by response_url
            catch (Exception ex)
            {
                _slackRepository.Error(leave);
                _logger.Error(ex, StringConstant.LoggerErrorMessageLeaveRequestControllerSlackRequest);
                return BadRequest();
            }
        }

        /**
        * @api {post} leaves/slackbuttoncall
        * @apiVersion 1.0.0
        * @apiName SlackButtonRequest
        * @apiGroup SlackButtonRequest    
        * @apiParam {SlashChatUpdateResponse} Name  leaveResponse
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"A message will be update in slack using slack chat update method"
        * }
        */
        [HttpPost]
        [Route("leaves/slackbuttoncall")]
        public IHttpActionResult SlackButtonRequest(SlashChatUpdateResponse leaveResponse)
        {
            try
            {
                _slackRepository.UpdateLeave(leaveResponse);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, StringConstant.LoggerErrorMessageLeaveRequestControllerSlackButtonRequest);
                throw;
            }
        }
    }
}
