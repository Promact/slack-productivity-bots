using Microsoft.AspNet.Identity.Owin;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
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
        private readonly IClient _client;
        private readonly IAttachmentRepository _attachmentRepository;

        public LeaveRequestController(ISlackRepository slackRepository, IClient client, IAttachmentRepository attachmentRepository)
        {
            _slackRepository = slackRepository;
            _client = client;
            _attachmentRepository = attachmentRepository;
        }
        /// <summary>
        /// Slack Call for Slash Command
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("leaves/slackcall")]
        public async Task<IHttpActionResult> SlackRequest()
        {
            //var request = HttpContext.Request.Form;
            var request = HttpContext.Current.Request.Form;
            var leave = _attachmentRepository.SlashCommandTransfrom(request);
            try
            {
                var slackText = _attachmentRepository.SlackText(leave.Text);
                var action = (SlackAction)Enum.Parse(typeof(SlackAction), slackText[0]);
                var accessToken = await _attachmentRepository.AccessToken(leave.Username + "@promactinfo.com");
                switch (action)
                {
                    case SlackAction.Apply:
                        {
                            var leaveRequest = await _slackRepository.LeaveApply(slackText, leave, accessToken);
                            await _client.SendMessageWithAttachmentIncomingWebhook(leave, leaveRequest, accessToken);
                        }
                        break;
                    case SlackAction.List:
                        await _slackRepository.SlackLeaveList(slackText, leave, accessToken);
                        break;
                    case SlackAction.Cancel:
                        await _slackRepository.SlackLeaveCancel(slackText, leave, accessToken);
                        break;
                    case SlackAction.Status:
                        await _slackRepository.SlackLeaveStatus(slackText, leave, accessToken);
                        break;
                    case SlackAction.Balance:
                        _slackRepository.SlackLeaveBalance(leave);
                        break;
                    default:
                        _slackRepository.SlackLeaveHelp(leave);
                        break;
                }
                return Ok();
            }
            // If throws any type of error it will give same message in slack by response_url
            catch (Exception ex)
            {
                _client.SendMessage(leave, StringConstant.SlackErrorMessage);
                return BadRequest(ex.ToString());
            }
        }

        /// <summary>
        /// Method to update the leave details. Response will be from slack interactive message button
        /// </summary>
        /// <param name="leaveResponse"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("leaves/slackbuttoncall")]
        public IHttpActionResult SlackButtonRequest(SlashChatUpdateResponse leaveResponse)
        {
            _slackRepository.UpdateLeave(leaveResponse);
            return Ok();
        }
    }
}
