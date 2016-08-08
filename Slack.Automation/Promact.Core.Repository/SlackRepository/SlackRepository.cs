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

namespace Promact.Core.Repository.SlackRepository
{
    public class SlackRepository : ISlackRepository
    {
        private readonly IProjectUserCallRepository _projectUser;
        private readonly ILeaveRequestRepository _leaveRepository;
        string replyText = "";
        public SlackRepository(ILeaveRequestRepository leaveRepository, IProjectUserCallRepository projectUser)
        {
            _projectUser = projectUser;
            _leaveRepository = leaveRepository;
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
        public async Task<string> LeaveList(string userName)
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
        public async Task<string> CancelLeave(int leaveId, string userName)
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
        public async Task<string> LeaveStatus(string userName)
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
        public string ChatPostAttachment(string text, string leaveRequestId)
        {
            List<SlashAttachmentAction> ActionList = new List<SlashAttachmentAction>();
            List<SlashAttachment> attachmentList = new List<SlashAttachment>();
            SlashAttachment attachment = new SlashAttachment();
            SlashAttachmentAction Approved = new SlashAttachmentAction()
            {
                name = StringConstant.Approved,
                text = StringConstant.Approved,
                type = StringConstant.Button,
                value = StringConstant.Approved,
            };
            ActionList.Add(Approved);
            SlashAttachmentAction Rejected = new SlashAttachmentAction()
            {
                name = StringConstant.Rejected,
                text = StringConstant.Rejected,
                type = StringConstant.Button,
                value = StringConstant.Rejected,
            };
            ActionList.Add(Rejected);
            attachment.fallback = StringConstant.LeaveTitle;
            attachment.title = StringConstant.LeaveTitle;
            attachment.callback_id = leaveRequestId;
            attachment.color = StringConstant.Color;
            attachment.attachment_type = StringConstant.AttachmentType;
            attachment.actions = ActionList;
            attachmentList.Add(attachment);
            attachment.title = text;
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
                name = StringConstant.Approved,
                text = StringConstant.Approved,
                type = StringConstant.Button,
                value = StringConstant.Approved,
            };
            ActionList.Add(Approved);
            SlashAttachmentAction Rejected = new SlashAttachmentAction()
            {
                name = StringConstant.Rejected,
                text = StringConstant.Rejected,
                type = StringConstant.Button,
                value = StringConstant.Rejected,
            };
            ActionList.Add(Rejected);
            attachmentList.actions = ActionList;
            attachmentList.fallback = StringConstant.LeaveTitle;
            attachmentList.title = replyText;
            attachmentList.callback_id = leaveRequestId;
            attachmentList.color = StringConstant.Color;
            attachmentList.attachment_type = StringConstant.AttachmentType;
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
    }
}

