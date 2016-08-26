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
using System.Web.Mvc;

namespace Promact.Erp.Core.Controllers
{
    public class LeaveRequestController : MVCBaseController
    {
        private readonly ISlackRepository _slackRepository;
        private readonly IClient _client;
        private readonly IAttachmentRepository _attachmentRepository;
        private ApplicationUserManager _userManager;

        public LeaveRequestController(ISlackRepository slackRepository, IClient client, IAttachmentRepository attachmentRepository, ApplicationUserManager userManager)
        {
            _slackRepository = slackRepository;
            _client = client;
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
        }
        /// <summary>
        /// Slack Call for Slash Command
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("leaves/slackcall")]
        public async Task<ActionResult> SlackRequest()
        {
            var request = HttpContext.Request.Form;
            //var request = HttpContext.Current.Request.Form;
            var leave = _attachmentRepository.SlashCommandTransfrom(request);
            try
            {
                var slackText = _attachmentRepository.SlackText(leave.Text);
                var action = (SlackAction)Enum.Parse(typeof(SlackAction), slackText[0]);
                var providerInfo = await _userManager.GetLoginsAsync(_userManager.FindByNameAsync(leave.Username + "@promactinfo.com").Result.Id);
                var accessToken = _attachmentRepository.AccessToken(providerInfo);
                switch (action)
                {
                    case SlackAction.Apply:
                        {
                            var leaveRequest = await _slackRepository.LeaveApply(slackText, leave, accessToken);
                            await _client.SendMessageWithAttachmentIncomingWebhook(leave, leaveRequest,accessToken);
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
                return View();
            }
            // If throws any type of error it will give same message in slack by response_url
            catch (Exception ex)
            {
                _client.SendMessage(leave, StringConstant.SlackErrorMessage);
                return View(ex.ToString());
            }
        }

        /// <summary>
        /// Method to update the leave details. Response will be from slack interactive message button
        /// </summary>
        /// <param name="leaveResponse"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("leaves/slackbuttoncall")]
        public ActionResult SlackButtonRequest(SlashChatUpdateResponse leaveResponse)
        {
            _slackRepository.UpdateLeave(leaveResponse);
            return View();
        }
    }
}
