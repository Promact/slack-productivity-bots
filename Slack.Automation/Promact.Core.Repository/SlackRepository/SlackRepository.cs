using Newtonsoft.Json;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Promact.Erp.Util;
using Promact.Erp.Util.Email_Templates;
using Promact.Erp.Util.Email;
using Promact.Core.Repository.Client;

namespace Promact.Core.Repository.SlackRepository
{
    public class SlackRepository : ISlackRepository
    {
        private readonly IProjectUserCallRepository _projectUser;
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IClient _client;
        string replyText = "";
        public SlackRepository(ILeaveRequestRepository leaveRepository, IProjectUserCallRepository projectUser, IClient client)
        {
            _projectUser = projectUser;
            _leaveRepository = leaveRepository;
            _client = client;
        }

        /// <summary>
        /// Method to convert List of string to leaveRequest object and call leave apply method to save the leave details
        /// </summary>
        /// <param name="slackRequest"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<LeaveRequest> LeaveApply(List<string> slackRequest, string userName)
        {
            LeaveRequest leaveRequest = new LeaveRequest();
            leaveRequest.Reason = slackRequest[1];
            leaveRequest.FromDate = Convert.ToDateTime(slackRequest[2]);
            leaveRequest.EndDate = Convert.ToDateTime(slackRequest[3]);
            leaveRequest.Type = slackRequest[4];
            leaveRequest.RejoinDate = Convert.ToDateTime(slackRequest[5]);
            leaveRequest.Status = Condition.Pending;
            var user = await _projectUser.GetUserByUsername(userName);
            leaveRequest.EmployeeId = user.Id;
            leaveRequest.CreatedOn = DateTime.UtcNow;
            _leaveRepository.ApplyLeave(leaveRequest);
            return leaveRequest;
        }

        /// <summary>
        /// Method to get Employee Id from its userName and from its employeeId, to get list of leave
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private async Task<string> LeaveList(string userName)
        {
            var user = await _projectUser.GetUserByUsername(userName);
            var userId = user.Id;
            var leaveList = _leaveRepository.LeaveListByUserId(userId);
            foreach (var leave in leaveList)
            {
                replyText += string.Format("{0} {1} {2} {3} {4} {5}", leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.ToShortDateString(), leave.Status, System.Environment.NewLine);
            }
            return replyText;
        }

        /// <summary>
        /// Method to Cancel leave, only allow the applier of the leave to cancel the leave
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        private async Task<string> CancelLeave(int leaveId, string userName)
        {
            var user = await _projectUser.GetUserByUsername(userName);
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
        /// <returns></returns>
        private async Task<string> LeaveStatus(string userName)
        {
            var user = await _projectUser.GetUserByUsername(userName);
            var userId = user.Id;
            var leave = _leaveRepository.LeaveListStatusByUserId(userId);
            replyText = string.Format("Your leave Id no: {0} From {1} To {2} for {3} is {4}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.ToShortDateString(), leave.Reason, leave.Status);
            return replyText;
        }

        /// <summary>
        /// Method to create attchment of slack which is used in chat.PostMessage method
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ChatPostAttachment(string text, string leaveRequestId)
        {
            List<SlashAttachmentAction> ActionList = new List<SlashAttachmentAction>();
            List<SlashAttachment> attachmentList = new List<SlashAttachment>();
            SlashAttachment attachment = new SlashAttachment();
            SlashAttachmentAction Approved = new SlashAttachmentAction()
            {
                Name = StringConstant.Approved,
                Text = StringConstant.Approved,
                Type = StringConstant.Button,
                Value = StringConstant.Approved,
            };
            ActionList.Add(Approved);
            SlashAttachmentAction Rejected = new SlashAttachmentAction()
            {
                Name = StringConstant.Rejected,
                Text = StringConstant.Rejected,
                Type = StringConstant.Button,
                Value = StringConstant.Rejected,
            };
            ActionList.Add(Rejected);
            attachment.Fallback = StringConstant.LeaveTitle;
            attachment.CallbackId = leaveRequestId;
            attachment.Color = StringConstant.Color;
            attachment.AttachmentType = StringConstant.AttachmentType;
            attachment.Actions = ActionList;
            attachmentList.Add(attachment);
            attachment.Title = text;
            var attachments = JsonConvert.SerializeObject(attachmentList);
            return attachments;
        }

        /// <summary>
        /// Method to create attchment of slack used generically
        /// </summary>
        /// <param name="leaveRequestId"></param>
        /// <param name="replyText"></param>
        /// <returns></returns>
        public List<SlashAttachment> SlackResponseAttachment(string leaveRequestId, string replyText)
        {
            List<SlashAttachmentAction> ActionList = new List<SlashAttachmentAction>();
            List<SlashAttachment> attachment = new List<SlashAttachment>();
            SlashAttachment attachmentList = new SlashAttachment();
            SlashAttachmentAction Approved = new SlashAttachmentAction()
            {
                Name = StringConstant.Approved,
                Text = StringConstant.Approved,
                Type = StringConstant.Button,
                Value = StringConstant.Approved,
            };
            ActionList.Add(Approved);
            SlashAttachmentAction Rejected = new SlashAttachmentAction()
            {
                Name = StringConstant.Rejected,
                Text = StringConstant.Rejected,
                Type = StringConstant.Button,
                Value = StringConstant.Rejected,
            };
            ActionList.Add(Rejected);
            attachmentList.Actions = ActionList;
            attachmentList.Fallback = StringConstant.LeaveTitle;
            attachmentList.Title = replyText;
            attachmentList.CallbackId = leaveRequestId;
            attachmentList.Color = StringConstant.Color;
            attachmentList.AttachmentType = StringConstant.AttachmentType;
            attachment.Add(attachmentList);
            return attachment;
        }

        /// <summary>
        /// Method to get leave Updated from slack button response
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public LeaveRequest UpdateLeave(int leaveId, string status)
        {
            var leave = _leaveRepository.LeaveById(leaveId);
            if (status == StringConstant.Approved)
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
            return leave;
        }

        /// <summary>
        /// Method to Get Leave List on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        public async void SlackLeaveList(List<string> slackText,SlashCommand leave)
        {
            var replyText = "";
            if (slackText.Count > 1)
            {
                var userName = slackText[1];
                replyText = await LeaveList(userName);
                _client.SendMessage(leave, replyText);
            }
            else
            {
                replyText = await LeaveList(leave.Username);
                _client.SendMessage(leave, replyText);
            }
        }

        /// <summary>
        /// Method to cancel leave by its Id from slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        public async void SlackLeaveCancel(List<string> slackText, SlashCommand leave)
        {
            var leaveId = Convert.ToInt32(slackText[1]);
            var replyText = await CancelLeave(leaveId, leave.Username);
            _client.SendMessage(leave, replyText);
        }

        /// <summary>
        /// Method to get last leave status and details on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        public async void SlackLeaveStatus(List<string> slackText, SlashCommand leave)
        {
            if (slackText.Count > 1)
            {
                var userName = slackText[1];
                var replyText = await LeaveStatus(userName);
                _client.SendMessage(leave, replyText);
            }
            else
            {
                var replyText = await LeaveStatus(leave.Username);
                _client.SendMessage(leave, replyText);
            }
        }

        /// <summary>
        /// Method to check leave Balance from slack
        /// </summary>
        /// <param name="leave"></param>
        public void SlackLeaveBalance(SlashCommand leave)
        {
            var replyText = "Still on Construction";
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



