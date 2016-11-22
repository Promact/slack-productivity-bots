using Autofac;
using Microsoft.AspNet.Identity;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Specialized;
using System.Linq;
using Xunit;

namespace Promact.Core.Test
{
    public class AttachmentRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ApplicationUserManager _userManager;
        private readonly IStringConstantRepository _stringConstant;
        public AttachmentRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
        }

        /// <summary>
        /// Test case to check creating attchment of slack used generically for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachment()
        {
            var response = _attachmentRepository.SlackResponseAttachment("1", _stringConstant.Hello).Last();
            Assert.Equal(response.Title, _stringConstant.Hello);
            Assert.Equal(response.Color, _stringConstant.Color);
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
                EmployeeId = _stringConstant.StringIdForTest,
                EndDate = DateTime.UtcNow,
                FromDate = DateTime.UtcNow,
                Reason = _stringConstant.LeaveReasonForTest,
                RejoinDate = DateTime.UtcNow,
                Status = Condition.Pending,
                Type = LeaveType.cl,
                Id = 1
            };
            var replyText = string.Format("Leave has been applied by {0} From {1} To {2} for Reason {3} will re-join by {4}",
                _stringConstant.FirstNameForTest,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.Value.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.Value.ToShortDateString());
            var response = _attachmentRepository.ReplyText(_stringConstant.FirstNameForTest, leave);
            Assert.Equal(response, replyText);
        }

        /// <summary>
        /// Test case to check break string by spaces only if spaces are not between quotes for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackText()
        {
            var response = _attachmentRepository.SlackText(_stringConstant.Hello).Last();
            Assert.Equal(response, _stringConstant.All);
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
            var user = new ApplicationUser() { Email = _stringConstant.EmailForTest, UserName = _stringConstant.EmailForTest, SlackUserId = _stringConstant.FirstNameForTest };
            var result = _userManager.CreateAsync(user).Result;
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            var secondResult = _userManager.AddLoginAsync(user.Id, info).Result;
            var accessToken = _attachmentRepository.AccessToken(user.Email).Result;
            Assert.Equal(accessToken, _stringConstant.AccessTokenForTest);
        }

        /// <summary>
        /// Test case to check creating attchment of slack used generically for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachmentFalse()
        {
            var response = _attachmentRepository.SlackResponseAttachment("55", _stringConstant.Hello).Last();
            Assert.NotEqual(response.Title, _stringConstant.FirstNameForTest);
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
                EmployeeId = _stringConstant.StringIdForTest,
                EndDate = DateTime.UtcNow,
                FromDate = DateTime.UtcNow,
                Reason = _stringConstant.LeaveReasonForTest,
                RejoinDate = DateTime.UtcNow,
                Status = Condition.Pending,
                Type = LeaveType.cl,
                Id = 1
            };
            var replyText = string.Format("Leave has been applied by {0} From {1} To {2} for Reason {3} will re-join by {4}",
                _stringConstant.FirstNameForTest,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.Value.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.Value.ToShortDateString());
            leave.Reason = _stringConstant.Reason;
            var response = _attachmentRepository.ReplyText(_stringConstant.FirstNameForTest, leave);
            Assert.NotEqual(response, replyText);
        }

        /// <summary>
        /// Test case to check break string by spaces only if spaces are not between quotes for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackTextFalse()
        {
            var response = _attachmentRepository.SlackText(_stringConstant.Hello).Last();
            Assert.NotEqual(response, _stringConstant.FirstNameForTest);
        }

        /// <summary>
        /// Test case to check Method AccessToken of Attachment Repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AccessTokenFalse()
        {
            var firstUser = new ApplicationUser() { Email = _stringConstant.EmailForTest, UserName = _stringConstant.EmailForTest, SlackUserId = _stringConstant.FirstNameForTest };
            var secondUser = new ApplicationUser() { Email = _stringConstant.TeamLeaderEmailForTest, UserName = _stringConstant.TeamLeaderEmailForTest, SlackUserId = _stringConstant.LastNameForTest };
            var result = _userManager.CreateAsync(firstUser).Result;
            result = _userManager.CreateAsync(secondUser).Result;
            UserLoginInfo firstInfo = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            UserLoginInfo secondInfo = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.SlackChannelIdForTest);
            result = _userManager.AddLoginAsync(firstUser.Id, firstInfo).Result;
            result = _userManager.AddLoginAsync(secondUser.Id, secondInfo).Result;
            var accessToken = _attachmentRepository.AccessToken(secondUser.Email).Result;
            Assert.NotEqual(accessToken, _stringConstant.AccessTokenForTest);
        }

        /// <summary>
        /// Test case to check creating attchment of slack used generically for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachmentWithoutButton()
        {
            var response = _attachmentRepository.SlackResponseAttachmentWithoutButton("1", _stringConstant.Hello).Last();
            Assert.Equal(response.Title, _stringConstant.Hello);
            Assert.Equal(response.Color, _stringConstant.Color);
        }
    }
}
