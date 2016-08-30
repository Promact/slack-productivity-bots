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

namespace Promact.Core.Repository.SlackRepository
{
    public class SlackRepository : ISlackRepository
    {
        private readonly IProjectUserCallRepository _projectUser;
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IClient _client;
        private readonly IAttachmentRepository _attachmentRepository;
        string replyText = null;
        public SlackRepository(ILeaveRequestRepository leaveRepository, IProjectUserCallRepository projectUser, IClient client, IAttachmentRepository attachmentRepository)
        {
            _projectUser = projectUser;
            _leaveRepository = leaveRepository;
            _client = client;
            _attachmentRepository = attachmentRepository;
        }

        /// <summary>
        /// Method to convert List of string to leaveRequest object and call leave apply method to save the leave details
        /// </summary>
        /// <param name="slackRequest"></param>
        /// <param name="userName"></param>
        /// <returns>leaveRequest</returns>
        public async Task<LeaveRequest> LeaveApply(List<string> slackRequest, SlashCommand leave, string accessToken)
        {
            LeaveRequest leaveRequest = new LeaveRequest();
            leaveRequest.Reason = slackRequest[1];
            leaveRequest.FromDate = DateTime.ParseExact(slackRequest[2], "dd-MM-yyyy", null);
            leaveRequest.EndDate = DateTime.ParseExact(slackRequest[3], "dd-MM-yyyy", null);
            leaveRequest.Type = slackRequest[4];
            leaveRequest.RejoinDate = Convert.ToDateTime(slackRequest[5]);
            leaveRequest.Status = Condition.Pending;
            var user = await _projectUser.GetUserByUsername(leave.Username, accessToken);
            leaveRequest.EmployeeId = user.Id;
            leaveRequest.CreatedOn = DateTime.UtcNow;
            _leaveRepository.ApplyLeave(leaveRequest);
            replyText = _attachmentRepository.ReplyText(leave.Username, leaveRequest);
            _client.SendMessage(leave, replyText);
            return leaveRequest;
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
                    replyText += string.Format("{0} {1} {2} {3} {4} {5}", leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.ToShortDateString(), leave.Status, System.Environment.NewLine);
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
                replyText = string.Format("Your leave Id no: {0} From {1} To {2} has been {3}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.ToShortDateString(), leave.Status);
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
                if (leave != null)
                {
                    replyText = string.Format("Your leave Id no: {0} From {1} To {2} for {3} is {4}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.ToShortDateString(), leave.Reason, leave.Status);
                }
                else
                {
                    replyText = StringConstant.SlashCommandLeaveStatusErrorMessage;
                }
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
            if (leaveResponse.Actions.Value == StringConstant.Approved)
            {
                leave.Status = Condition.Approved;
            }
            else
            {
                leave.Status = Condition.Rejected;
            }
            if (leave.Status == Condition.Pending)
            {
                _leaveRepository.UpdateLeave(leave);
            }
            var replyText = string.Format("You had {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}",
                            leave.Status,
                            leaveResponse.User.Name,
                            leave.FromDate.ToShortDateString(),
                            leave.EndDate.ToShortDateString(),
                            leave.Reason,
                            leave.RejoinDate.ToShortDateString());
            _client.UpdateMessage(leaveResponse, replyText);
        }

        /// <summary>
        /// Method to Get Leave List on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        public async Task SlackLeaveList(List<string> slackText, SlashCommand leave, string accessToken)
        {
            if (slackText.Count > 1)
            {
                try
                {
                    var userName = slackText[1];
                    replyText = await LeaveList(userName, accessToken);
                }
                catch (Exception)
                {
                    replyText = StringConstant.SlashCommandLeaveListErrorMessage;
                }
            }
            else
            {
                replyText = await LeaveList(leave.Username, accessToken);
            }
            _client.SendMessage(leave, replyText);
        }

        /// <summary>
        /// Method to cancel leave by its Id from slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        public async Task SlackLeaveCancel(List<string> slackText, SlashCommand leave, string accessToken)
        {
            try
            {
                var leaveId = Convert.ToInt32(slackText[1]);
                var replyText = await CancelLeave(leaveId, leave.Username, accessToken);
            }
            catch (Exception)
            {
                replyText = StringConstant.SlashCommandLeaveCancelErrorMessage;
            }
            _client.SendMessage(leave, replyText);
        }

        /// <summary>
        /// Method to get last leave status and details on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        public async Task SlackLeaveStatus(List<string> slackText, SlashCommand leave, string accessToken)
        {
            if (slackText.Count > 1)
            {
                try
                {
                    var userName = slackText[1];
                    var replyText = await LeaveStatus(userName, accessToken);

                }
                catch (Exception)
                {
                    replyText = StringConstant.SlashCommandLeaveStatusErrorMessage;
                }
            }
            else
            {
                var replyText = await LeaveStatus(leave.Username, accessToken);
            }
            _client.SendMessage(leave, replyText);
        }

        /// <summary>
        /// Method to check leave Balance from slack
        /// </summary>
        /// <param name="leave"></param>
        public void SlackLeaveBalance(SlashCommand leave)
        {
            var replyText = StringConstant.UnderConstruction;
            _client.SendMessage(leave, replyText);
        }

        /// <summary>
        /// Method for gettin help on slack regards Leave slash command
        /// </summary>
        /// <param name="leave"></param>
        public void SlackLeaveHelp(SlashCommand leave)
        {
            var replyText = StringConstant.SlackHelpMessage;
            _client.SendMessage(leave, replyText);
        }
    }
}



