using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Erp.Util;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.AttachmentRepository;
using System.Linq;
using System.Globalization;
using Promact.Erp.DomainModel.DataRepository;

namespace Promact.Core.Repository.SlackRepository
{
    public class SlackRepository : ISlackRepository
    {
        private readonly IProjectUserCallRepository _projectUser;
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IClient _client;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _userManager;
        string replyText = null;
        public SlackRepository(ILeaveRequestRepository leaveRepository, IProjectUserCallRepository projectUser, IClient client, IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> userManager)
        {
            _projectUser = projectUser;
            _leaveRepository = leaveRepository;
            _client = client;
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Method to convert List of string to leaveRequest object and call leave apply method to save the leave details
        /// </summary>
        /// <param name="slackRequest"></param>
        /// <param name="userName"></param>
        /// <returns>leaveRequest</returns>
        public async Task<string> LeaveApply(List<string> slackRequest, SlashCommand leave, string accessToken)
        {
            try
            {
                User user = new User();
                var startDate = DateTime.ParseExact(slackRequest[3], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
                LeaveRequest leaveRequest = new LeaveRequest();
                try
                {
                    var leaveType = (LeaveType)Enum.Parse(typeof(LeaveType), slackRequest[1]);
                    switch (leaveType)
                    {
                        case LeaveType.cl:
                            {
                                try
                                {
                                    var endDate = DateTime.ParseExact(slackRequest[4], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
                                    var reJoinDate = DateTime.ParseExact(slackRequest[5], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
                                    user = await _projectUser.GetUserByUsername(leave.Username, accessToken);
                                    leaveRequest.EndDate = endDate;
                                    leaveRequest.RejoinDate = reJoinDate;
                                    leaveRequest.Status = Condition.Pending;
                                    leaveRequest.Type = leaveType;
                                    leaveRequest.Reason = slackRequest[2];
                                    leaveRequest.FromDate = startDate;
                                    leaveRequest.CreatedOn = DateTime.UtcNow;
                                    if (user != null)
                                    {
                                        try
                                        {
                                            leaveRequest.EmployeeId = user.Id;
                                            _leaveRepository.ApplyLeave(leaveRequest);
                                            replyText = _attachmentRepository.ReplyText(leave.Username, leaveRequest);
                                            await _client.SendMessageWithAttachmentIncomingWebhook(leave, leaveRequest, accessToken);
                                        }
                                        catch (Exception)
                                        {
                                            replyText = StringConstant.ErrorWhileApplyingLeaveAndSendingEmail;
                                        }
                                    }
                                    else
                                    {
                                        replyText = StringConstant.SorryYouCannotApplyLeave;
                                    }
                                }
                                catch (Exception)
                                {
                                    replyText = StringConstant.DateFormatErrorMessage;
                                }
                            }
                            break;
                        case LeaveType.sl:
                            {
                                if (slackRequest.Count > 4)
                                {
                                    user = await _projectUser.GetUserByUsername(slackRequest[4], accessToken);
                                }
                                else
                                {
                                    user = await _projectUser.GetUserByUsername(leave.Username, accessToken);
                                }
                                leaveRequest.Status = Condition.Approved;
                                leaveRequest.Type = leaveType;
                                leaveRequest.Reason = slackRequest[2];
                                leaveRequest.FromDate = startDate;
                                leaveRequest.CreatedOn = DateTime.UtcNow;
                                if (user != null)
                                {
                                    try
                                    {
                                        leaveRequest.EmployeeId = user.Id;
                                        _leaveRepository.ApplyLeave(leaveRequest);
                                        replyText = _attachmentRepository.ReplyText(leave.Username, leaveRequest);
                                        await _client.SendMessageWithAttachmentIncomingWebhook(leave, leaveRequest, accessToken);
                                    }
                                    catch (Exception)
                                    {
                                        replyText = StringConstant.ErrorWhileApplyingLeaveAndSendingEmail;
                                    }
                                }
                                else
                                {
                                    replyText = StringConstant.SorryYouCannotApplyLeave;
                                }
                            }
                            break;
                        default:
                            {
                                replyText = StringConstant.NotTypeOfLeave;
                            }
                            break;
                    }
                }
                catch (Exception)
                {
                    replyText = StringConstant.NotTypeOfLeave;
                }
            }
            catch (Exception)
            {
                replyText = StringConstant.DateFormatErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method to get Employee Id from its userName and from its employeeId, to get list of leave
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>replyText as string</returns>
        private async Task<string> LeaveList(string userName, string accessToken)
        {
            var user = await _projectUser.GetUserByUsername(userName, accessToken);
            var userId = user.Id;
            var leaveList = _leaveRepository.LeaveListByUserId(userId);
            if (leaveList.Count() != 0)
            {
                foreach (var leave in leaveList)
                {
                    replyText += string.Format("{0} {1} {2} {3} {4} {5}", leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, System.Environment.NewLine);
                }
            }
            else
            {
                replyText = StringConstant.SlashCommandLeaveListErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method to Cancel leave, only allow the applier of the leave to cancel the leave
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="userName"></param>
        /// <returns>replyText as string</returns>
        private async Task<string> CancelLeave(int leaveId, string userName, string accessToken)
        {
            var user = await _projectUser.GetUserByUsername(userName, accessToken);
            var userId = user.Id;
            if (userId == _leaveRepository.LeaveById(leaveId).EmployeeId)
            {
                var leave = _leaveRepository.CancelLeave(leaveId);
                replyText = string.Format("Your leave Id no: {0} From {1} To {2} has been {3}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status);
            }
            else
            {
                replyText = StringConstant.CancelLeaveError;
            }
            return replyText;
        }

        /// <summary>
        /// Method to get Employee Id from its userName and from its employeeId, to get last leave status
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>replyText as string</returns>
        private async Task<string> LeaveStatus(string userName, string accessToken)
        {
            var user = await _projectUser.GetUserByUsername(userName, accessToken);
            var userId = user.Id;
            try
            {
                var leave = _leaveRepository.LeaveListStatusByUserId(userId);
                replyText = string.Format("Your leave Id no: {0} From {1} To {2} for {3} is {4}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.Status);
            }
            catch (Exception)
            {
                replyText = StringConstant.SlashCommandLeaveStatusErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method to get leave Updated from slack button response
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="status"></param>
        public void UpdateLeave(SlashChatUpdateResponse leaveResponse)
        {
            var leave = _leaveRepository.LeaveById(leaveResponse.CallbackId);
            if (leave.Status == Condition.Pending)
            {
                if (leaveResponse.Actions.Value == StringConstant.Approved)
                {
                    leave.Status = Condition.Approved;
                }
                else
                {
                    leave.Status = Condition.Rejected;
                }
            }
            _leaveRepository.UpdateLeave(leave);

            replyText = string.Format("You had {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}",
                            leave.Status,
                            leaveResponse.User.Name,
                            leave.FromDate.ToShortDateString(),
                            leave.EndDate.Value.ToShortDateString(),
                            leave.Reason,
                            leave.RejoinDate.Value.ToShortDateString());
            _client.UpdateMessage(leaveResponse, replyText);
        }

        /// <summary>
        /// Method to Get Leave List on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        public async Task<string> SlackLeaveList(List<string> slackText, SlashCommand leave, string accessToken)
        {
            if (slackText.Count > 1)
            {
                var userName = slackText[1];
                replyText = await LeaveList(userName, accessToken);
            }
            else
            {
                replyText = await LeaveList(leave.Username, accessToken);
            }
            return replyText;
        }

        /// <summary>
        /// Method to cancel leave by its Id from slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        public async Task<string> SlackLeaveCancel(List<string> slackText, SlashCommand leave, string accessToken)
        {
            try
            {
                var leaveId = Convert.ToInt32(slackText[1]);
                replyText = await CancelLeave(leaveId, leave.Username, accessToken);
            }
            catch (Exception)
            {
                replyText = StringConstant.SlashCommandLeaveCancelErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method to get last leave status and details on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        public async Task<string> SlackLeaveStatus(List<string> slackText, SlashCommand leave, string accessToken)
        {
            if (slackText.Count > 1)
            {
                var userName = slackText[1];
                replyText = await LeaveStatus(userName, accessToken);
            }
            else
            {
                replyText = await LeaveStatus(leave.Username, accessToken);
            }
            return replyText;
        }


        /// <summary>
        /// Method to check leave Balance from slack
        /// </summary>
        /// <param name="leave"></param>
        /// <param name="accessToken"></param>
        public async Task<string> SlackLeaveBalance(SlashCommand leave, string accessToken)
        {
            try
            {
                var casualLeave = await _projectUser.CasualLeave(leave.Username, accessToken);
                var user = await _projectUser.GetUserByUsername(leave.Username, accessToken);
                var casualLeaveTaken = _leaveRepository.NumberOfLeaveTaken(user.Id);
                var casualLeaveLeft = casualLeave - casualLeaveTaken;
                replyText = string.Format("You have taken {0} casual leave out of {1}{2}You have casual leave left {3}", casualLeaveTaken, casualLeave, Environment.NewLine, casualLeaveLeft);
            }
            catch (Exception)
            {
                replyText = StringConstant.LeaveBalanceErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method for gettin help on slack regards Leave slash command
        /// </summary>
        /// <param name="leave"></param>
        public string SlackLeaveHelp(SlashCommand leave)
        {
            var replyText = StringConstant.SlackHelpMessage;
            return replyText;
        }

        public async Task Leave(SlashCommand leave)
        {
            leave.Text.ToLower();
            var slackText = _attachmentRepository.SlackText(leave.Text);
            var user = _userManager.FirstOrDefault(x => x.SlackUserName == leave.Username);
            if (user != null)
            {
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                try
                {
                    var action = (SlackAction)Enum.Parse(typeof(SlackAction), slackText[0]);
                    switch (action)
                    {
                        case SlackAction.apply:
                            replyText = await LeaveApply(slackText, leave, accessToken);
                            break;
                        case SlackAction.list:
                            replyText = await SlackLeaveList(slackText, leave, accessToken);
                            break;
                        case SlackAction.cancel:
                            replyText = await SlackLeaveCancel(slackText, leave, accessToken);
                            break;
                        case SlackAction.status:
                            replyText = await SlackLeaveStatus(slackText, leave, accessToken);
                            break;
                        case SlackAction.balance:
                            replyText = await SlackLeaveBalance(leave, accessToken);
                            break;
                        case SlackAction.update:

                            break;
                        default:
                            replyText = SlackLeaveHelp(leave);
                            break;
                    }
                }
                catch (Exception)
                {
                    replyText = StringConstant.SlackErrorMessage;
                }
            }
            else
            {
                replyText = StringConstant.LeaveBalanceErrorMessage;
            }
            _client.SendMessage(leave, replyText);
        }

        public void Error(SlashCommand leave)
        {
            var replyText = string.Format("{0}{1}{2}{1}{3}", StringConstant.LeaveBalanceErrorMessage, Environment.NewLine, StringConstant.OrElseString, StringConstant.SlackErrorMessage);
            _client.SendMessage(leave, replyText);
        }
    }
}



