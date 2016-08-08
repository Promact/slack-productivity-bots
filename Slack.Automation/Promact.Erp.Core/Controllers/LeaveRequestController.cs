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
                var slackText = leave.Text.Split('"').Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element }).SelectMany(element => element).ToList();
                var action = slackText[0];
                switch (action)
                {
                    case "apply":
                        {
                            var leaveRequest = await _slackRepository.LeaveApply(slackText, leave.Username);
                            var replyText = string.Format("Leave has been applied by {0} From {1} To {2} for Reason {3} will re-join by {4}", leave.Username, leaveRequest.FromDate.ToShortDateString(), leaveRequest.EndDate.ToShortDateString(), leaveRequest.Reason, leaveRequest.RejoinDate.ToShortDateString());
                            _client.SendMessage(leave, replyText);
                            // Assigning the Incoming Web-Hook Url in response url to send message to all TL and management in their personal message by LeaveBot
                            leave.ResponseUrl = "https://hooks.slack.com/services/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";
                            _client.SendMessageWithAttachmentIncomingWebhook(leave, replyText, leaveRequest);
                        }
                        break;
                    case "list":
                        {
                            var replyText = "";
                            if (slackText.Count > 1)
                            {
                                var userName = slackText[1];
                                replyText = await _slackRepository.LeaveList(userName);
                                _client.SendMessage(leave, replyText);
                            }
                            else
                            {
                                replyText = await _slackRepository.LeaveList(leave.Username);
                                _client.SendMessage(leave, replyText);
                            }
                        }
                        break;
                    case "cancel":
                        {
                            var leaveId = Convert.ToInt32(slackText[1]);
                            var replyText = await _slackRepository.CancelLeave(leaveId, leave.Username);
                            _client.SendMessage(leave, replyText);
                        }
                        break;
                    case "status":
                        {
                            if (slackText.Count > 1)
                            {
                                var userName = slackText[1];
                                var replyText = await _slackRepository.LeaveStatus(userName);
                                _client.SendMessage(leave, replyText);
                            }
                            else
                            {
                                var replyText = await _slackRepository.LeaveStatus(leave.Username);
                                _client.SendMessage(leave, replyText);
                            }
                        }
                        break;
                    case "balance":
                        {
                            var replyText = "Still on Construction";
                            _client.SendMessage(leave, replyText);
                        }
                        break;
                    case "help":
                        {
                            var replyText = StringConstant.SlackHelpMessage;
                            _client.SendMessage(leave, replyText);
                        }
                        break;
                }
                return Ok();
            }
            // If throws any type of error it will give same message in slack by response_url
            catch (Exception ex)
            {
                var replyText = StringConstant.SlackErrorMessage;
                _client.SendMessage(leave, replyText);
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
            var replyText = string.Format("You had {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}", leave.Status, leaveResponse.User.Name, leave.FromDate.ToShortDateString(), leave.EndDate.ToShortDateString(), leave.Reason, leave.RejoinDate.ToShortDateString());
            _client.UpdateMessage(leaveResponse, replyText);
            return Ok();
        }
    }
}
