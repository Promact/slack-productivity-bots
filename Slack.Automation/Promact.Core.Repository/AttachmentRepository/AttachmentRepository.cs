using Newtonsoft.Json;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Promact.Core.Repository.AttachmentRepository
{
    public class AttachmentRepository:IAttachmentRepository
    {
        #region Private Variables
        private readonly ApplicationUserManager _userManager;
        private readonly IStringConstantRepository _stringConstant;
        #endregion

        #region Constructor
        public AttachmentRepository(ApplicationUserManager userManager,IStringConstantRepository stringConstant)
        {
            _userManager = userManager;
            _stringConstant = stringConstant;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to create attchment of slack with a text as reply, can be used generically
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
                Name = _stringConstant.Approved,
                Text = _stringConstant.Approved,
                Type = _stringConstant.Button,
                Value = _stringConstant.Approved,
            };
            ActionList.Add(Approved);
            SlashAttachmentAction Rejected = new SlashAttachmentAction()
            {
                Name = _stringConstant.Rejected,
                Text = _stringConstant.Rejected,
                Type = _stringConstant.Button,
                Value = _stringConstant.Rejected,
            };
            ActionList.Add(Rejected);
            // Adding action button on attachment
            attachmentList.Actions = ActionList;
            // Fallback as a string on attachment
            attachmentList.Fallback = _stringConstant.Fallback;
            // attaching reply text as title of attachment
            attachmentList.Title = replyText;
            // assigning callbackId of attachment with leaveRequestId
            attachmentList.CallbackId = leaveRequestId;
            // assigning color of attachment as string format
            attachmentList.Color = _stringConstant.Color;
            // Assigning attachment type as default
            attachmentList.AttachmentType = _stringConstant.AttachmentType;
            attachment.Add(attachmentList);
            return attachment;
        }

        /// <summary>
        /// Method will create text corresponding to leave details and user, which will to be send on slack as reply
        /// </summary>
        /// <param name="username"></param>
        /// <param name="leave"></param>
        /// <returns>string replyText</returns>
        public string ReplyText(string username, LeaveRequest leave)
        {
            var replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveApplied,
                username,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.Value.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.Value.ToShortDateString());
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
        /// <returns>leave of class SlashCommand</returns>
        public SlashCommand SlashCommandTransfrom(NameValueCollection value)
        {
            SlashCommand leave = new SlashCommand()
            {
                ChannelId = value.Get(_stringConstant.ChannelId),
                ChannelName = value.Get(_stringConstant.ChannelName),
                Command = value.Get(_stringConstant.Command),
                ResponseUrl = value.Get(_stringConstant.ResponseUrl),
                TeamDomain = value.Get(_stringConstant.TeamDomain),
                TeamId = value.Get(_stringConstant.TeamId),
                Text = value.Get(_stringConstant.Text),
                Token = value.Get(_stringConstant.Token),
                UserId = value.Get(_stringConstant.UserId),
                Username = value.Get(_stringConstant.UserName),
            };
            return leave;
        }

        /// <summary>
        /// Method to get accessToken for Promact OAuth corresponding to username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>access token from AspNetUserLogin table</returns>
        public async Task<string> UserAccessTokenAsync(string username)
        {
            var providerInfo = await _userManager.GetLoginsAsync(_userManager.FindByNameAsync(username).Result.Id);
            var accessToken = _stringConstant.EmptyString;
            foreach (var provider in providerInfo)
            {
                if(provider.LoginProvider == _stringConstant.PromactStringName)
                {
                    accessToken = provider.ProviderKey;
                }
            }
            return accessToken;
        }

        /// <summary>
        /// Method will create text corresponding to sick leave details and user, which will to be send on slack as reply
        /// </summary>
        /// <param name="username"></param>
        /// <param name="leave"></param>
        /// <returns>string replyText</returns>
        public string ReplyTextSick(string username, LeaveRequest leave)
        {
            var replyText = string.Format(_stringConstant.ReplyTextForSickLeaveApplied,
                username,
                leave.FromDate.ToShortDateString(),
                leave.Reason);
            return replyText;
        }

        /// <summary>
        /// Attachment created to be send in slack without any interactive button
        /// </summary>
        /// <param name="leaveRequestId"></param>
        /// <param name="replyText"></param>
        /// <returns></returns>
        public List<SlashAttachment> SlackResponseAttachmentWithoutButton(string leaveRequestId, string replyText)
        {
            List<SlashAttachment> attachment = new List<SlashAttachment>();
            SlashAttachment attachmentList = new SlashAttachment();
            // Fallback as a string on attachment
            attachmentList.Fallback = _stringConstant.Fallback;
            // attaching reply text as title of attachment
            attachmentList.Title = replyText;
            // assigning callbackId of attachment with leaveRequestId
            attachmentList.CallbackId = leaveRequestId;
            // assigning color of attachment as string format
            attachmentList.Color = _stringConstant.Color;
            // Assigning attachment type as default
            attachmentList.AttachmentType = _stringConstant.AttachmentType;
            attachment.Add(attachmentList);
            return attachment;
        }

        /// <summary>
        /// Method to convert slash response to SlashChatUpdateResponse
        /// </summary>
        /// <param name="value"></param>
        /// <returns>SlashChatUpdateResponse</returns>
        public SlashChatUpdateResponse SlashChatUpdateResponseTransfrom(NameValueCollection value)
        {
            var decodeResponse = HttpUtility.UrlDecode(value[_stringConstant.Payload]);
            var response = JsonConvert.DeserializeObject<SlashChatUpdateResponse>(decodeResponse);
            return response;
        }
        #endregion
    }
}
