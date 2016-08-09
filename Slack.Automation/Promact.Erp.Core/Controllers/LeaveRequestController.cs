using Promact.Core.Repository.Client;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class LeaveRequestController : ApiController
    {
        private readonly ILeaveRequestRepository _leaveRequest;
        private readonly ISlackRepository _slackRepository;
        private readonly IClient _client;
        public LeaveRequestController(ILeaveRequestRepository leaveRequest, ISlackRepository slackRepository, IClient client)
        {
            _leaveRequest = leaveRequest;
            _slackRepository = slackRepository;
            _client = client;
        }
        /// <summary>
        /// Slack Call for Slash Command
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("leaves/slackcall")]
        public async Task<IHttpActionResult> SlackRequest(SlashCommand leave)
        {
            try
            {
                // Way to break string by spaces only if spaces are not between quotes
                var slackText = leave.Text.Split('"')
                    .Select((element, index) => index % 2 == 0 ? element
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                    .SelectMany(element => element).ToList();
                //var action = slackText[0];
                var action = (SlackAction)Enum.Parse(typeof(SlackAction), slackText[0]);
                switch (action)
                {
                    case SlackAction.Apply:
                        {
                            var leaveRequest = await _slackRepository.LeaveApply(slackText, leave.Username);
                            var replyText = string.Format("Leave has been applied by {0} From {1} To {2} for Reason {3} will re-join by {4}",
                                leave.Username,
                                leaveRequest.FromDate.ToShortDateString(),
                                leaveRequest.EndDate.ToShortDateString(),
                                leaveRequest.Reason,
                                leaveRequest.RejoinDate.ToShortDateString());
                            _client.SendMessage(leave, replyText);
                            // Assigning the Incoming Web-Hook Url in response url to send message to all TL 
                            // and management in their personal message by LeaveBot
                            leave.ResponseUrl = AppSettingsUtil.IncomingWebHookUrl;
                            _client.SendMessageWithAttachmentIncomingWebhook(leave, replyText, leaveRequest);
                        }
                        break;
                    case SlackAction.List:
                        {
                            _slackRepository.SlackLeaveList(slackText, leave);
                        }
                        break;
                    case SlackAction.Cancel:
                        {
                            _slackRepository.SlackLeaveCancel(slackText, leave);
                        }
                        break;
                    case SlackAction.Status:
                        {
                            _slackRepository.SlackLeaveStatus(slackText, leave);
                        }
                        break;
                    case SlackAction.Balance:
                        {
                            _slackRepository.SlackLeaveBalance(leave);
                        }
                        break;
                    case SlackAction.Help:
                        {
                            _slackRepository.SlackLeaveHelp(leave);
                        }
                        break;
                }
                return Ok();
            }
            // If throws any type of error it will give same message in slack by response_url
            catch (Exception ex)
            {
                _client.SendMessage(leave, StringConstant.SlackErrorMessage);
                return BadRequest();
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
            var leave = _slackRepository.UpdateLeave(leaveResponse.CallbackId, leaveResponse.Actions.Value);
            var replyText = string.Format("You had {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}",
                leave.Status,
                leaveResponse.User.Name,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.ToShortDateString());
            _client.UpdateMessage(leaveResponse, replyText);
            return Ok();
        }
    }
}
