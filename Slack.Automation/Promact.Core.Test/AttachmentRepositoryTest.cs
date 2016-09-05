using Autofac;
using Microsoft.AspNet.Identity;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class AttachmentRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ApplicationUserManager _userManager;
        public AttachmentRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
        }

        /// <summary>
        /// Test case to check creating attchment of slack used generically for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachment()
        {
            var response = _attachmentRepository.SlackResponseAttachment("1", StringConstant.Hello).Last();
            Assert.Equal(response.Title, StringConstant.Hello);
            Assert.Equal(response.Color, StringConstant.Color);
        }

        /// <summary>
        /// Test case to check creating text corresponding to leave details and user which is to be send on slack as reply for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void ReplyText()
        {
            LeaveRequest leave = new LeaveRequest()
            {
                CreatedOn = DateTime.UtcNow,
                EmployeeId = StringConstant.StringIdForTest,
                EndDate = DateTime.UtcNow,
                FromDate = DateTime.UtcNow,
                Reason = StringConstant.LeaveReasonForTest,
                RejoinDate = DateTime.UtcNow,
                Status = Condition.Pending,
                Type = StringConstant.LeaveTypeForTest,
                Id = 1
            };
            var replyText = string.Format("Leave has been applied by {0} From {1} To {2} for Reason {3} will re-join by {4}",
                StringConstant.FirstNameForTest,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.ToShortDateString());
            var response = _attachmentRepository.ReplyText(StringConstant.FirstNameForTest,leave);
            Assert.Equal(response, replyText);
        }

        /// <summary>
        /// Test case to check break string by spaces only if spaces are not between quotes for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackText()
        {
            var response = _attachmentRepository.SlackText(StringConstant.Hello).Last();
            Assert.Equal(response, StringConstant.All);
        }

        /// <summary>
        /// Test cases to check transform NameValueCollection to SlashCommand class for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlashCommandTransfrom()
        {
            NameValueCollection value = new NameValueCollection();
            var response = _attachmentRepository.SlashCommandTransfrom(value);
            Assert.Equal(response.ChannelName, null);
        }

        /// <summary>
        /// Test case to check Method AccessToken of Attachment Repository 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AccessToken()
        {
            var user = new ApplicationUser() { Email = StringConstant.EmailForTest, UserName = StringConstant.EmailForTest, SlackUserName = StringConstant.FirstNameForTest};
            var result = _userManager.CreateAsync(user).Result;
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            var secondResult = _userManager.AddLoginAsync(user.Id, info).Result;
            var accessToken = _attachmentRepository.AccessToken(user.Email).Result;
            Assert.Equal(accessToken, StringConstant.AccessTokenForTest);
        }

        /// <summary>
        /// Test case to check creating attchment of slack used generically for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachmentFalse()
        {
            var response = _attachmentRepository.SlackResponseAttachment("55", StringConstant.Hello).Last();
            Assert.NotEqual(response.Title, StringConstant.FirstNameForTest);
        }

        /// <summary>
        /// Test case to check creating text corresponding to leave details and user which is to be send on slack as reply for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void ReplyTextFalse()
        {
            LeaveRequest leave = new LeaveRequest()
            {
                CreatedOn = DateTime.UtcNow,
                EmployeeId = StringConstant.StringIdForTest,
                EndDate = DateTime.UtcNow,
                FromDate = DateTime.UtcNow,
                Reason = StringConstant.LeaveReasonForTest,
                RejoinDate = DateTime.UtcNow,
                Status = Condition.Pending,
                Type = StringConstant.LeaveTypeForTest,
                Id = 1
            };
            var replyText = string.Format("Leave has been applied by {0} From {1} To {2} for Reason {3} will re-join by {4}",
                StringConstant.FirstNameForTest,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.ToShortDateString());
            leave.Reason = StringConstant.Reason;
            var response = _attachmentRepository.ReplyText(StringConstant.FirstNameForTest, leave);
            Assert.NotEqual(response, replyText);
        }

        /// <summary>
        /// Test case to check break string by spaces only if spaces are not between quotes for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackTextFalse()
        {
            var response = _attachmentRepository.SlackText(StringConstant.Hello).Last();
            Assert.NotEqual(response, StringConstant.FirstNameForTest);
        }

        /// <summary>
        /// Test case to check Method AccessToken of Attachment Repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AccessTokenFalse()
        {
            var firstUser = new ApplicationUser() { Email = StringConstant.EmailForTest, UserName = StringConstant.EmailForTest, SlackUserName = StringConstant.FirstNameForTest };
            var secondUser = new ApplicationUser() { Email = StringConstant.TeamLeaderEmailForTest, UserName = StringConstant.TeamLeaderEmailForTest, SlackUserName = StringConstant.LastNameForTest };
            var result = _userManager.CreateAsync(firstUser).Result;
            result = _userManager.CreateAsync(secondUser).Result;
            UserLoginInfo firstInfo = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            UserLoginInfo secondInfo = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.SlackChannelIdForTest);
            result = _userManager.AddLoginAsync(firstUser.Id, firstInfo).Result;
            result = _userManager.AddLoginAsync(secondUser.Id, secondInfo).Result;
            var accessToken = _attachmentRepository.AccessToken(secondUser.Email).Result;
            Assert.NotEqual(accessToken, StringConstant.AccessTokenForTest);
        }
    }
}
