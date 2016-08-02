using Promact.Core.Repository.Client;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.SlackRepository;
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
        public IHttpActionResult SlackRequest(SlashCommand leave)
        {
            try
            {
                var slackText = leave.text.Split('"').Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element }).SelectMany(element => element).ToList();
                var action = slackText[0];
                switch (action)
                {
                    case "apply":
                        {
                            var leaveRequest = _slackRepository.LeaveApply(slackText, leave.user_name);
                            var replyText = "Leave has been applied by " + leave.user_name + " From " + leaveRequest.FromDate.ToShortDateString() + " To " + leaveRequest.EndDate.ToShortDateString() + " for Reason " + leaveRequest.Reason + " will re-join by " + leaveRequest.RejoinDate.ToShortDateString();
                            //_client.SendMessageWithAttachment(leave, replyText, Convert.ToString(leaveRequest.Id));
                            _client.SendMessage(leave, replyText);
                            leave.response_url = "https://hooks.slack.com/services/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";
                            _client.SendMessageWithAttachmentIncomingWebhook(leave, replyText, Convert.ToString(leaveRequest.Id));
                        }
                        break;
                    case "list":
                        {
                            var replyText = "";
                            if (slackText.Count > 1)
                            {
                                var userName = slackText[1];
                                replyText = _slackRepository.LeaveList(userName);
                                _client.SendMessage(leave, replyText);
                            }
                            else
                            {
                                replyText = _slackRepository.LeaveList(leave.user_name);
                                _client.SendMessage(leave, replyText);
                            }
                        }
                        break;
                    case "cancel":
                        {
                            var leaveId = Convert.ToInt32(slackText[1]);
                            var replyText = _slackRepository.CancelLeave(leaveId, leave.user_name);
                            _client.SendMessage(leave, replyText);
                        }
                        break;
                    case "status":
                        {
                            if (slackText.Count > 1)
                            {
                                var userName = slackText[1];
                                var replyText = _slackRepository.LeaveStatus(userName);
                                _client.SendMessage(leave, replyText);
                            }
                            else
                            {
                                var replyText = _slackRepository.LeaveStatus(leave.user_name);
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
                            var replyText = "For leave apply: /leaves apply [Reason] [FromDate] [EndDate] [LeaveType] [RejoinDate]" + Environment.NewLine + "For leave list of Yours : /leaves list" + Environment.NewLine + "For leave list of others : /leaves list [@user]" + Environment.NewLine + "For leave Cancel : /leaves cancel [leave Id number]" + Environment.NewLine + "For leave status of Yours : /leaves status" + Environment.NewLine + "For leave status of others : /leaves status [@user]" + Environment.NewLine + "For leaves balance: /leaves balance";
                            _client.SendMessage(leave, replyText);
                        }
                        break;
                }
                return Ok();
            }
            catch (Exception ex)
            {
                var replyText = "Sorry, I didn't quite get that. I'm easily confused. Perhaps try the words in a different order. For help : /leaves help";
                _client.SendMessage(leave, replyText);
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("leaves/slackbuttoncall")]
        public IHttpActionResult SlackButtonRequest(SlashChatUpdateResponse leaveResponse)
        {
            var leave = _slackRepository.UpdateLeave(leaveResponse.callback_id, leaveResponse.actions.value);
            var replyText = "You had " + leave.Status + " Leave for " + leaveResponse.user.name + " From " + leave.FromDate.ToShortDateString() + " To " + leave.EndDate.ToShortDateString() + " for Reason " + leave.Reason + " will re-join by " + leave.RejoinDate.ToShortDateString();
            _client.UpdateMessage(leaveResponse, replyText);
            return Ok(replyText);
        }
    }
}
