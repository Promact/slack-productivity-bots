using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.AttachmentRepository
{
    public class AttachmentRepository:IAttachmentRepository
    {
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
    }
}
