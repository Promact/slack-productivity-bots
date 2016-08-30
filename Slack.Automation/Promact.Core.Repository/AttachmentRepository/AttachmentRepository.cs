using Microsoft.AspNet.Identity;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.AttachmentRepository
{
    public class AttachmentRepository:IAttachmentRepository
    {
        private readonly ApplicationUserManager _userManager;
        public AttachmentRepository(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }
        /// <summary>
        /// Method to create attchment of slack used generically
        /// </summary>
        /// <param name="leaveRequestId"></param>
        /// <param name="replyText"></param>
        /// <returns>string attachment</returns>
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
            // Adding action button on attachment
            attachmentList.Actions = ActionList;
            // Fallback as a string on attachment
            attachmentList.Fallback = StringConstant.Fallback;
            // attaching reply text as title of attachment
            attachmentList.Title = replyText;
            // assigning callbackId of attachment with leaveRequestId
            attachmentList.CallbackId = leaveRequestId;
            // assigning color of attachment as string format
            attachmentList.Color = StringConstant.Color;
            // Assigning attachment type as default
            attachmentList.AttachmentType = StringConstant.AttachmentType;
            attachment.Add(attachmentList);
            return attachment;
        }

        /// <summary>
        /// Method will create text corresponding to leave details and user which is to be send on slack as reply
        /// </summary>
        /// <param name="username"></param>
        /// <param name="leave"></param>
        /// <returns>string replyText</returns>
        public string ReplyText(string username, LeaveRequest leave)
        {
            var replyText = string.Format("Leave has been applied by {0} From {1} To {2} for Reason {3} will re-join by {4}",
                username,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.ToShortDateString());
            return replyText;
        }

        /// <summary>
        /// Way to break string by spaces only if spaces are not between quotes
        /// </summary>
        /// <param name="text"></param>
        /// <returns>List of string slackText</returns>
        public List<string> SlackText(string text)
        {
            var slackText = text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            return slackText;
        }

        /// <summary>
        /// Method to transform NameValueCollection to SlashCommand class
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SlashCommand SlashCommandTransfrom(NameValueCollection value)
        {
            SlashCommand leave = new SlashCommand()
            {
                ChannelId = value.Get(StringConstant.ChannelId),
                ChannelName = value.Get(StringConstant.ChannelName),
                Command = value.Get(StringConstant.Command),
                ResponseUrl = value.Get(StringConstant.ResponseUrl),
                TeamDomain = value.Get(StringConstant.TeamDomain),
                TeamId = value.Get(StringConstant.TeamId),
                Text = value.Get(StringConstant.Text),
                Token = value.Get(StringConstant.Token),
                UserId = value.Get(StringConstant.UserId),
                Username = value.Get(StringConstant.UserName),
            };
            return leave;
        }

        /// <summary>
        /// Method to get accessToken for Promact OAuth corresponding to username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>access token from AspNetUserLogin table</returns>
        public async Task<string> AccessToken(string username)
        {
            var providerInfo = await _userManager.GetLoginsAsync(_userManager.FindByNameAsync(username).Result.Id);
            var accessToken = "";
            foreach (var provider in providerInfo)
            {
                if(provider.LoginProvider == AppSettingsUtil.ProviderName)
                {
                    accessToken = provider.ProviderKey;
                }
            }
            return accessToken;
        }
    }
}
