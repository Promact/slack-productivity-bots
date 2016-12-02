using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.StringConstants;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ISlackRepository _slackRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly Mock<IClient> _mockClient;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ApplicationUserManager _userManager;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IStringConstantRepository _stringConstant;
        private static string incomingWebhookURL;
        private User user = new User();
        private SlackUserDetails slackUser = new SlackUserDetails();
        private LeaveRequest leave = new LeaveRequest();
        private SlashCommand slackLeave = new SlashCommand();
        private ApplicationUser newUser = new ApplicationUser();
        private SlashChatUpdateResponse leaveResponse = new SlashChatUpdateResponse();
        public SlackRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _mockClient = _componentContext.Resolve<Mock<IClient>>();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _envVariableRepository = _componentContext.Resolve<IEnvironmentVariableRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            incomingWebhookURL = _envVariableRepository.IncomingWebHookUrl;
            Initialize();
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForCL()
        {
            await AddUser();
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                  .Select((element, index) => index % 2 == 0 ? element
                  .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                  .SelectMany(element => element).ToList();
            var replyText = _attachmentRepository.ReplyText(_stringConstant.FirstNameForTest, leave);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForNoUser()
        {
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(Task.FromResult(""));
            var slackText = slackLeave.Text.Split('"')
     .Select((element, index) => index % 2 == 0 ? element
     .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
     .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), _stringConstant.SorryYouCannotApplyLeave));
            var replyText = _stringConstant.EmptyString;
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), _stringConstant.SorryYouCannotApplyLeave), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method UpdateLeave casual from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void UpdateLeave()
        {
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            leaveResponse.CallbackId = leave.Id;
            var replyText = string.Format("You had {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}",
                leave.Status,
                leaveResponse.User.Name,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.Value.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.Value.ToShortDateString());
            _mockClient.Setup(x => x.UpdateMessage(It.IsAny<SlashChatUpdateResponse>(), replyText));
            await _slackRepository.UpdateLeaveAsync(leaveResponse);
            var leaveUpdated = _leaveRequestRepository.LeaveById(leave.Id);
            Assert.Equal(Condition.Approved, leaveUpdated.Status);
        }

        /// <summary>
        /// Test cases for checking method UpdateLeave casual from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void UpdateLeaveFalse()
        {
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            leaveResponse.CallbackId = leave.Id;
            var replyText = string.Format("You had {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}",
                leave.Status,
                leaveResponse.User.Name,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.Value.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.Value.ToShortDateString());
            _mockClient.Setup(x => x.UpdateMessage(It.IsAny<SlashChatUpdateResponse>(), replyText));
            leaveResponse.Actions.Name = _stringConstant.Rejected;
            leaveResponse.Actions.Value = _stringConstant.Rejected;
            await _slackRepository.UpdateLeaveAsync(leaveResponse);
            var leaveUpdated = _leaveRequestRepository.LeaveById(leave.Id);
            Assert.Equal(Condition.Rejected, leaveUpdated.Status);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveList()
        {
            await AddUser();
            await _slackUserRepository.AddSlackUserAsync(slackUser);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = string.Format("{0} {1} {2} {3} {4} {5}", leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, System.Environment.NewLine);
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.Text = _stringConstant.LeaveListCommandForTest;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveListFalse()
        {
            await AddUser();
            await _slackUserRepository.AddSlackUserAsync(slackUser);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = _stringConstant.SlashCommandLeaveListErrorMessage;
            slackLeave.Text = _stringConstant.LeaveListCommandForTest;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository for Own with true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveListForOwn()
        {
            await AddUser();
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = string.Format("{0} {1} {2} {3} {4} {5}", leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, System.Environment.NewLine);
            slackLeave.Text = _stringConstant.LeaveListTestForOwn;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository for Own with false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveListForOwnFalse()
        {
            await AddUser();
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = _stringConstant.SlashCommandLeaveListErrorMessage;
            slackLeave.Text = _stringConstant.LeaveListTestForOwn;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveCancel from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveCancel()
        {
            await AddUser();
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = string.Format("Your leave Id no: {0} From {1} To {2} has been {3}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), _stringConstant.Cancel);
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.Text = _stringConstant.LeaveCancelCommandForTest;
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveCancel from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveCancelFalse()
        {
            newUser.SlackUserId = _stringConstant.FalseStringNameForTest;
            var result = await _userManager.CreateAsync(newUser);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            result = await _userManager.AddLoginAsync(newUser.Id, info);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = string.Format("{0}{1}{2}", _stringConstant.LeaveDoesnotExist, _stringConstant.OrElseString, _stringConstant.CancelLeaveError);
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.Text = _stringConstant.LeaveCancelCommandForTest;
            slackLeave.Username = _stringConstant.FalseStringNameForTest;
            slackLeave.UserId = _stringConstant.FalseStringNameForTest;
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveCancel from Slack respository with wrong value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveCancelWrong()
        {
            newUser.SlackUserId = _stringConstant.FalseStringNameForTest;
            var result = await _userManager.CreateAsync(newUser);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            result = await _userManager.AddLoginAsync(newUser.Id, info);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = _stringConstant.SlashCommandLeaveCancelErrorMessage;
            slackLeave.Text = _stringConstant.WrongLeaveCancelCommandForTest;
            slackLeave.Username = _stringConstant.FalseStringNameForTest;
            slackLeave.UserId = _stringConstant.FalseStringNameForTest;
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveStatus from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveStatus()
        {
            await AddUser();
            await _slackUserRepository.AddSlackUserAsync(slackUser);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = string.Format("Your leave Id no: {0} From {1} To {2} for {3} is {4}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.Status);
            slackLeave.Text = _stringConstant.LeaveStatusCommandForTest;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveStatus from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveStatusFalse()
        {
            await AddUser();
            await _slackUserRepository.AddSlackUserAsync(slackUser);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = _stringConstant.SlashCommandLeaveStatusErrorMessage;
            slackLeave.Text = _stringConstant.LeaveStatusCommandForTest;
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveStatus from Slack respository for Own with true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveStatusForOwn()
        {
            await AddUser();
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = string.Format("Your leave Id no: {0} From {1} To {2} for {3} is {4}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.Status);
            slackLeave.Text = _stringConstant.LeaveStatusTestForOwn;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveStatus from Slack respository for Own with false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveStatusOwnFalse()
        {
            await AddUser();
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = _stringConstant.SlashCommandLeaveStatusErrorMessage;
            slackLeave.Text = _stringConstant.LeaveStatusTestForOwn;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveBalance from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveBalance()
        {
            await AddUser();
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            slackLeave.Text = _stringConstant.LeaveBalanceTestForOwn;
            var replyText = _stringConstant.LeaveBalanceReplyTextForTest;
            replyText += string.Format("{0}{1}", Environment.NewLine, _stringConstant.LeaveBalanceSickReplyTextForTest);
            var response = Task.FromResult(_stringConstant.CasualLeaveResponse);
            var requestUrl = string.Format("{0}{1}", _stringConstant.CasualLeaveUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveHelp from Slack respository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveHelp()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.LeaveHelpTestForOwn;
            var replyText = _stringConstant.SlackHelpMessage;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveHelp from Slack respository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveHelpFalse()
        {
            slackLeave.Text = _stringConstant.LeaveHelpTestForOwn;
            await AddUser();
            var replyText = _stringConstant.InternalError;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Never);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForSL()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextSick;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _attachmentRepository.ReplyTextSick(_stringConstant.FirstNameForTest, leave);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForSLForNoUser()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextSick;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _stringConstant.SorryYouCannotApplyLeave;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForSLForUser()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextSickForUser;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var adminResponse = Task.FromResult(_stringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, adminrequestUrl, _stringConstant.AccessTokenForTest)).Returns(adminResponse);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _attachmentRepository.ReplyTextSick(_stringConstant.FirstNameForTest, leave);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            _mockClient.Setup(x =>  x.SendSickLeaveMessageToUserIncomingWebhookAsync(It.IsAny<LeaveRequest>(), _stringConstant.ManagementEmailForTest, replyText, user));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForErrorLeaveType()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextErrorLeaveType;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _stringConstant.NotTypeOfLeave;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForErrorDateFormat()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextErrorDateFormatSick;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _stringConstant.DateFormatErrorMessage;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForErrorDateFormatForCasual()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextErrorDateFormatCasual;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _stringConstant.DateFormatErrorMessage;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForCLForNoUser()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextCasual;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _stringConstant.SorryYouCannotApplyLeave;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test case to check method Error of slack repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void Error()
        {
            var replyText = string.Format("{0}{1}{2}{1}{3}", _stringConstant.LeaveNoUserErrorMessage, Environment.NewLine, _stringConstant.OrElseString, _stringConstant.SlackErrorMessage);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _slackRepository.Error(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveUpdate()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var adminResponse = Task.FromResult(_stringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, adminrequestUrl, _stringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = string.Format("Sick leave of {0} from {1} to {2} for reason {3} has been updated, will rejoin on {4}"
                            , user.FirstName, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(),
                            leave.Reason, leave.RejoinDate.Value.ToShortDateString());
            slackLeave.Text = _stringConstant.SlashCommandUpdate;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var employeeResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var employeeRequestUrl = string.Format("{0}{1}", _stringConstant.UserDetailUrl, _stringConstant.StringIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, employeeRequestUrl, _stringConstant.AccessTokenForTest)).Returns(employeeResponse);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithoutButtonAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            _mockClient.Setup(x => x.SendSickLeaveMessageToUserIncomingWebhookAsync(It.IsAny<LeaveRequest>(), _stringConstant.ManagementEmailForTest, replyText, user));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveUpdateDateFormatError()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var adminResponse = Task.FromResult(_stringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, adminrequestUrl, _stringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = _stringConstant.DateFormatErrorMessage;
            slackLeave.Text = _stringConstant.SlashCommandUpdateDateError;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveUpdateWrongId()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var adminResponse = Task.FromResult(_stringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, adminrequestUrl, _stringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = _stringConstant.SickLeaveDoesnotExist;
            slackLeave.Text = _stringConstant.SlashCommandUpdateWrongId;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveUpdateTryToUpdateCL()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.cl;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var adminResponse = Task.FromResult(_stringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, adminrequestUrl, _stringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = _stringConstant.SickLeaveDoesnotExist;
            slackLeave.Text = _stringConstant.SlashCommandUpdate;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveUpdateInValidID()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.cl;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var adminResponse = Task.FromResult(_stringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", _stringConstant.UserIsAdmin, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, adminrequestUrl, _stringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = _stringConstant.UpdateEnterAValidLeaveId;
            slackLeave.Text = _stringConstant.SlashCommandUpdateInValidId;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveUpdateNotAdmin()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            var replyText = _stringConstant.AdminErrorMessageUpdateSickLeave;
            slackLeave.Text = _stringConstant.SlashCommandUpdate;
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyText, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Returns(Task.FromResult(_stringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveBalance from Slack respository with false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveBalanceWrong()
        {
            await AddUser();
            await _leaveRequestRepository.ApplyLeaveAsync(leave);
            slackLeave.Text = _stringConstant.LeaveBalanceTestForOwn;
            var replyText = _stringConstant.LeaveNoUserErrorMessage;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForCLForEmailError()
        {
            SmtpException ex = new SmtpException();
            await AddUser();
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.Text = _stringConstant.SlashCommandTextCasual;
            var slackText = slackLeave.Text.Split('"')
                  .Select((element, index) => index % 2 == 0 ? element
                  .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                  .SelectMany(element => element).ToList();
            var replyText = string.Format("{0}. {1}", _stringConstant.ErrorWhileSendingEmail, ex.Message.ToString());
            var replyTextSecond = _attachmentRepository.ReplyText(_stringConstant.FirstNameForTest, leave);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), _stringConstant.AccessTokenForTest, replyTextSecond, _stringConstant.FirstNameForTest, _stringConstant.FirstNameForTest)).Throws<SmtpException>();
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForSLOtherUserNotAdmin()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextSickForUser;
            var slackText = slackLeave.Text.Split('"')
                   .Select((element, index) => index % 2 == 0 ? element
                   .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                   .SelectMany(element => element).ToList();
            var replyText = _stringConstant.AdminErrorMessageApplySickLeave + _stringConstant.SorryYouCannotApplyLeave;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialize()
        {
            user.Id = _stringConstant.StringIdForTest;
            user.Email = _stringConstant.EmailForTest;
            user.FirstName = _stringConstant.FirstNameForTest;
            user.IsActive = true;
            user.LastName = _stringConstant.LastNameForTest;
            user.UserName = _stringConstant.EmailForTest;
            user.SlackUserId = _stringConstant.FirstNameForTest;

            slackUser.UserId = _stringConstant.FirstNameForTest;
            slackUser.Name = _stringConstant.FirstNameForTest;
            slackUser.FirstName = _stringConstant.FirstNameForTest;
            slackUser.IsBot = false;

            leaveResponse.MessageTs = _stringConstant.MessageTsForTest;
            leaveResponse.Token = _stringConstant.AccessTokenForTest;
            leaveResponse.Actions = new SlashChatUpdateResponseAction()
            {
                Name = _stringConstant.Approved,
                Value = _stringConstant.Approved
            };
            leaveResponse.User = new SlashChatUpdateResponseChannelUser()
            {
                Id = _stringConstant.StringIdForTest,
                Name = _stringConstant.FirstNameForTest
            };
            
            leave.FromDate = DateTime.ParseExact("14-09-2016", "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
            leave.EndDate = DateTime.ParseExact("14-09-2016", "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
            leave.Reason = _stringConstant.LeaveReasonForTest;
            leave.RejoinDate = DateTime.ParseExact("14-09-2016", "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
            leave.Status = Condition.Pending;
            leave.Type = LeaveType.cl;
            leave.CreatedOn = DateTime.UtcNow;
            leave.EmployeeId = _stringConstant.StringIdForTest;

            slackLeave.Text = _stringConstant.SlashCommandText;
            slackLeave.Username = _stringConstant.FirstNameForTest;
            slackLeave.ResponseUrl = incomingWebhookURL;
            slackLeave.UserId = _stringConstant.FirstNameForTest;

            newUser.UserName = _stringConstant.EmailForTest;
            newUser.Email = _stringConstant.EmailForTest;
            newUser.SlackUserId = _stringConstant.FirstNameForTest;

        }


        private void UserMock()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
        }


        private async Task AddUser()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(newUser);
            await _userManager.AddLoginAsync(newUser.Id, info);
        }
    }
}
