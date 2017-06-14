using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.ServiceRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringLiteral;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class AttachmentRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ApplicationUserManager _userManager;
        private readonly AppStringLiteral _stringConstant;
        private readonly Mock<IServiceRepository> _mockServiceRepository;
        #endregion

        #region Constructor
        public AttachmentRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _stringConstant = _componentContext.Resolve<ISingletonStringLiteral>().StringConstant;
            _mockServiceRepository = _componentContext.Resolve<Mock<IServiceRepository>>();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test case to check creating attchment of slack used generically for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachment()
        {
            var response = _attachmentRepository.SlackResponseAttachment(_stringConstant.StringValueOneForTest, _stringConstant.Hello).Last();
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
            var replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveApplied,
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
        public async Task AccessTokenAsync()
        {
            var user = new ApplicationUser() { Email = _stringConstant.EmailForTest, UserName = _stringConstant.EmailForTest, SlackUserId = _stringConstant.FirstNameForTest };
            var result = await _userManager.CreateAsync(user);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            var secondResult = await _userManager.AddLoginAsync(user.Id, info);
            var accessTokenForTest = Task.FromResult(_stringConstant.AccessTokenForTest);
            _mockServiceRepository.Setup(x => x.GerAccessTokenByRefreshToken(_stringConstant.AccessTokenForTest, user.Id)).Returns(accessTokenForTest);
             var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.Email);
            Assert.Equal(accessToken, _stringConstant.AccessTokenForTest);
        }

        /// <summary>
        /// Test case to check creating attchment of slack used generically for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachmentFalse()
        {
            var response = _attachmentRepository.SlackResponseAttachment(_stringConstant.StringValueFiftyFiveForTest, _stringConstant.Hello).Last();
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
            var replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveApplied,
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
        public async Task AccessTokenFalseAsync()
        {
            var firstUser = new ApplicationUser() { Email = _stringConstant.EmailForTest, UserName = _stringConstant.EmailForTest, SlackUserId = _stringConstant.FirstNameForTest };
            var secondUser = new ApplicationUser() { Email = _stringConstant.TeamLeaderEmailForTest, UserName = _stringConstant.TeamLeaderEmailForTest, SlackUserId = _stringConstant.LastNameForTest };
            var result = await _userManager.CreateAsync(firstUser);
            result = await _userManager.CreateAsync(secondUser);
            UserLoginInfo firstInfo = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            UserLoginInfo secondInfo = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.SlackChannelIdForTest);
            result = await _userManager.AddLoginAsync(firstUser.Id, firstInfo);
            result = await _userManager.AddLoginAsync(secondUser.Id, secondInfo);
            var accessTokenForTest = Task.FromResult(_stringConstant.AccessTokenForTest);
            _mockServiceRepository.Setup(x => x.GerAccessTokenByRefreshToken(_stringConstant.AccessTokenForTest, firstUser.Id)).Returns(accessTokenForTest);
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(secondUser.Email);
            Assert.NotEqual(accessToken, _stringConstant.AccessTokenForTest);
        }

        /// <summary>
        /// Test case to check creating attchment of slack used generically for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachmentWithoutButton()
        {
            var response = _attachmentRepository.SlackResponseAttachmentWithoutButton(_stringConstant.StringValueOneForTest, _stringConstant.Hello).Last();
            Assert.Equal(response.Title, _stringConstant.Hello);
            Assert.Equal(response.Color, _stringConstant.Color);
        }

        /// <summary>
        /// Test case to check method SlashChatUpdateResponseTransfrom of Attachment Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlashChatUpdateResponseTransfrom()
        {
            NameValueCollection value = new NameValueCollection();
            value[_stringConstant.Payload] = _stringConstant.LeaveUpdateResponseJsonString;
            var response = _attachmentRepository.SlashChatUpdateResponseTransfrom(value);
            Assert.Equal(response.User.Id, _stringConstant.UserSlackId);
        }

        /// <summary>
        /// Test case to check method SlashChatUpdateResponseTransfrom of Attachment Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetTaskMailInStringFormat()
        {
            List<TaskMailDetails> taskMailDetails = new List<TaskMailDetails>()
            {
                new TaskMailDetails()
                {
                    Description = _stringConstant.TaskMailDescription,
                    Comment = _stringConstant.TaskMailComment,
                    Hours = Convert.ToDecimal(_stringConstant.HourSpentForTest),
                    Status = TaskMailStatus.completed
                }
            };
            var response = _attachmentRepository.GetTaskMailInStringFormat(taskMailDetails);
            Assert.NotEqual(response, string.Empty);
        }
        #endregion
    }
}
