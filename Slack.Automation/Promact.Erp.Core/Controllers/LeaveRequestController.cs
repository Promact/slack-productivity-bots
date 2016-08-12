using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Promact.Erp.Core.Controllers
{
    public class CustomerBinder : IModelBinder
    {

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.HttpContext.Request;

            string strCustomerCode = request.Form.Get("response_url");
            string strCustomerName = request.Form.Get("team_id");

            return new SlashCommand
            {
                TeamId = strCustomerCode,
                ResponseUrl = strCustomerName
            };
        }
    }
    public class LeaveRequestController : LeaveRequestControllerBase
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
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("leaves/slackcall")]
        public async Task<IHttpActionResult> SlackRequest([ModelBinder(typeof(CustomerBinder))] SlashCommand leave)
        {
            try
            {
                var slackText = _attachmentRepository.SlackText(leave.Text);
                var action = (SlackAction)Enum.Parse(typeof(SlackAction), slackText[0]);
                switch (action)
                {
                    case SlackAction.Apply:
                        {
                            var leaveRequest = await _slackRepository.LeaveApply(slackText, leave);
                            _client.SendMessageWithAttachmentIncomingWebhook(leave, leaveRequest);
                        }
                        break;
                    case SlackAction.List:
                        _slackRepository.SlackLeaveList(slackText, leave);
                        break;
                    case SlackAction.Cancel:
                        _slackRepository.SlackLeaveCancel(slackText, leave);
                        break;
                    case SlackAction.Status:
                        _slackRepository.SlackLeaveStatus(slackText, leave);
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
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("leaves/slackbuttoncall")]
        public IHttpActionResult SlackButtonRequest(SlashChatUpdateResponse leaveResponse)
        {
            _slackRepository.UpdateLeave(leaveResponse);
            return Ok();
        }
    }
}
