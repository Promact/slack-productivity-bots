using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
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
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly Mock<IClient> _mockClient;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ApplicationUserManager _userManager;
        private readonly EnvironmentVariableStore _envVariableStore;
        public SlackRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _mockClient = _componentContext.Resolve<Mock<IClient>>();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _envVariableStore = _componentContext.Resolve<EnvironmentVariableStore>();
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForCL()
        {
            await AddUser();
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _attachmentRepository.ReplyText(StringConstant.FirstNameForTest, leave);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyForNoUser()
        {
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(Task.FromResult(""));
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), StringConstant.SorryYouCannotApplyLeave));
            var replyText = StringConstant.EmptyString;
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), StringConstant.SorryYouCannotApplyLeave), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method UpdateLeave casual from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void UpdateLeave()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            leaveResponse.CallbackId = leave.Id;
            var replyText = string.Format("You had {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}",
                leave.Status,
                leaveResponse.User.Name,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.Value.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.Value.ToShortDateString());
            _mockClient.Setup(x => x.UpdateMessage(It.IsAny<SlashChatUpdateResponse>(), replyText));
            _slackRepository.UpdateLeave(leaveResponse);
            var leaveUpdated = _leaveRequestRepository.LeaveById(leave.Id);
            Assert.Equal(Condition.Approved, leaveUpdated.Status);
        }

        /// <summary>
        /// Test cases for checking method UpdateLeave casual from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void UpdateLeaveFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            leaveResponse.CallbackId = leave.Id;
            var replyText = string.Format("You had {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}",
                leave.Status,
                leaveResponse.User.Name,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.Value.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.Value.ToShortDateString());
            _mockClient.Setup(x => x.UpdateMessage(It.IsAny<SlashChatUpdateResponse>(), replyText));
            leaveResponse.Actions.Name = StringConstant.Rejected;
            leaveResponse.Actions.Value = StringConstant.Rejected;
            _slackRepository.UpdateLeave(leaveResponse);
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
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format("{0} {1} {2} {3} {4} {5}", leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, System.Environment.NewLine);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveListCommandForTest;
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
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.SlashCommandLeaveListErrorMessage;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveListCommandForTest;
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
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format("{0} {1} {2} {3} {4} {5}", leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, System.Environment.NewLine);
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveListTestForOwn;
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
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.SlashCommandLeaveListErrorMessage;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveListTestForOwn;
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
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format("Your leave Id no: {0} From {1} To {2} has been {3}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), StringConstant.Cancel);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveCancelCommandForTest;
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
            newUser.SlackUserName = StringConstant.FalseStringNameForTest;
            var result = await _userManager.CreateAsync(newUser);
            result = await _userManager.AddLoginAsync(newUser.Id, info);
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format("{0}{1}{2}", StringConstant.LeaveDoesnotExist, StringConstant.OrElseString, StringConstant.CancelLeaveError);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveCancelCommandForTest;
            slackLeave.Username = StringConstant.FalseStringNameForTest;
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
            newUser.SlackUserName = StringConstant.FalseStringNameForTest;
            var result = await _userManager.CreateAsync(newUser);
            result = await _userManager.AddLoginAsync(newUser.Id, info);
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.SlashCommandLeaveCancelErrorMessage;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.WrongLeaveCancelCommandForTest;
            slackLeave.Username = StringConstant.FalseStringNameForTest;
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
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format("Your leave Id no: {0} From {1} To {2} for {3} is {4}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.Status);
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveStatusCommandForTest;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
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
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.SlashCommandLeaveStatusErrorMessage;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveStatusCommandForTest;
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
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format("Your leave Id no: {0} From {1} To {2} for {3} is {4}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.Status);
            slackLeave.Text = StringConstant.LeaveStatusTestForOwn;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
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
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.SlashCommandLeaveStatusErrorMessage;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveStatusTestForOwn;
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
            _leaveRequestRepository.ApplyLeave(leave);
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveBalanceTestForOwn;
            var replyText = StringConstant.LeaveBalanceReplyTextForTest;
            replyText += string.Format("{0}{1}", Environment.NewLine, StringConstant.LeaveBalanceSickReplyTextForTest);
            var response = Task.FromResult(StringConstant.CasualLeaveResponse);
            var requestUrl = string.Format("{0}{1}", StringConstant.CasualLeaveUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var userResponse = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);
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
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.LeaveHelpTestForOwn;
            var replyText = StringConstant.SlackHelpMessage;
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
            slackLeave.Text = StringConstant.LeaveHelpTestForOwn;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            await AddUser();
            var replyText = StringConstant.InternalError;
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
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.SlashCommandTextSick;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _attachmentRepository.ReplyTextSick(StringConstant.NameForTest, leave);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.SlashCommandTextSick;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = StringConstant.SorryYouCannotApplyLeave;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.SlashCommandTextSickForUser;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var adminResponse = Task.FromResult(StringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", StringConstant.UserIsAdmin, StringConstant.ManagementEmailForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, adminrequestUrl, StringConstant.AccessTokenForTest)).Returns(adminResponse);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = _attachmentRepository.ReplyTextSick(StringConstant.NameForTest, leave);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
            _mockClient.Setup(x => x.SendSickLeaveMessageToUserIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.ManagementEmailForTest, replyText, user));
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
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.SlashCommandTextErrorLeaveType;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = StringConstant.NotTypeOfLeave;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.SlashCommandTextErrorDateFormatSick;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = StringConstant.DateFormatErrorMessage;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.SlashCommandTextErrorDateFormatCasual;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = StringConstant.DateFormatErrorMessage;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.SlashCommandTextCasual;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = StringConstant.SorryYouCannotApplyLeave;
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
            var replyText = string.Format("{0}{1}{2}{1}{3}", StringConstant.LeaveNoUserErrorMessage, Environment.NewLine, StringConstant.OrElseString, StringConstant.SlackErrorMessage);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
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
            _leaveRequestRepository.ApplyLeave(leave);
            var adminResponse = Task.FromResult(StringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", StringConstant.UserIsAdmin, StringConstant.EmailForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, adminrequestUrl, StringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = string.Format("Sick leave of {0} from {1} to {2} for reason {3} has been updated, will rejoin on {4}"
                            , user.SlackUserName, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(),
                            leave.Reason, leave.RejoinDate.Value.ToShortDateString());
            slackLeave.Text = StringConstant.SlashCommandUpdate;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var employeeResponse = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var employeeRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, StringConstant.StringIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, employeeRequestUrl, StringConstant.AccessTokenForTest)).Returns(employeeResponse);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithoutButtonAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
            _mockClient.Setup(x => x.SendSickLeaveMessageToUserIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.ManagementEmailForTest, replyText, user));
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
            _leaveRequestRepository.ApplyLeave(leave);
            var adminResponse = Task.FromResult(StringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", StringConstant.UserIsAdmin, StringConstant.EmailForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, adminrequestUrl, StringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = StringConstant.DateFormatErrorMessage;
            slackLeave.Text = StringConstant.SlashCommandUpdateDateError;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            _leaveRequestRepository.ApplyLeave(leave);
            var adminResponse = Task.FromResult(StringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", StringConstant.UserIsAdmin, StringConstant.EmailForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, adminrequestUrl, StringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = StringConstant.SickLeaveDoesnotExist;
            slackLeave.Text = StringConstant.SlashCommandUpdateWrongId;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            _leaveRequestRepository.ApplyLeave(leave);
            var adminResponse = Task.FromResult(StringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", StringConstant.UserIsAdmin, StringConstant.EmailForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, adminrequestUrl, StringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = StringConstant.SickLeaveDoesnotExist;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.SlashCommandUpdate;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            _leaveRequestRepository.ApplyLeave(leave);
            var adminResponse = Task.FromResult(StringConstant.True);
            var adminrequestUrl = string.Format("{0}{1}", StringConstant.UserIsAdmin, StringConstant.EmailForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, adminrequestUrl, StringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = StringConstant.UpdateEnterAValidLeaveId;
            slackLeave.Text = StringConstant.SlashCommandUpdateInValidId;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.AdminErrorMessageUpdateSickLeave;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            slackLeave.Text = StringConstant.SlashCommandUpdate;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyText, StringConstant.FirstNameForTest)).Returns(Task.FromResult(StringConstant.EmptyString));
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
            _leaveRequestRepository.ApplyLeave(leave);
            slackLeave.Text = StringConstant.LeaveBalanceTestForOwn;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var replyText = StringConstant.LeaveNoUserErrorMessage;
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
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.Text = StringConstant.SlashCommandTextCasual;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = string.Format("{0}. {1}", StringConstant.ErrorWhileSendingEmail, ex.Message.ToString());
            var replyTextSecond = _attachmentRepository.ReplyText(StringConstant.FirstNameForTest, leave);
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            _mockClient.Setup(x => x.SendMessageWithAttachmentIncomingWebhook(It.IsAny<LeaveRequest>(), StringConstant.AccessTokenForTest, replyTextSecond, StringConstant.FirstNameForTest)).Throws<SmtpException>();
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
            slackLeave.Text = StringConstant.SlashCommandTextSickForUser;
            slackLeave.ResponseUrl = _envVariableStore.FetchEnvironmentVariableValues(StringConstant.IncomingWebHookUrl);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var replyText = StringConstant.AdminErrorMessageApplySickLeave + StringConstant.SorryYouCannotApplyLeave;
            _mockClient.Setup(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText));
            await _slackRepository.LeaveRequest(slackLeave);
            _mockClient.Verify(x => x.SendMessage(It.IsAny<SlashCommand>(), replyText), Times.Once);
        }

        /// <summary>
        /// Private User variable to be used in test cases
        /// </summary>
        private User user = new User()
        {
            Id = StringConstant.StringIdForTest,
            Email = StringConstant.EmailForTest,
            FirstName = StringConstant.FirstNameForTest,
            IsActive = true,
            LastName = StringConstant.LastNameForTest,
            UserName = StringConstant.EmailForTest,
            SlackUserName = StringConstant.FirstNameForTest
        };

        /// <summary>
        /// Private leave response to be used for test cases
        /// </summary>
        private SlashChatUpdateResponse leaveResponse = new SlashChatUpdateResponse()
        {
            MessageTs = StringConstant.MessageTsForTest,
            Token = StringConstant.AccessTokenForTest,
            Actions = new SlashChatUpdateResponseAction() { Name = StringConstant.Approved, Value = StringConstant.Approved },
            User = new SlashChatUpdateResponseChannelUser() { Id = StringConstant.StringIdForTest, Name = StringConstant.FirstNameForTest }
        };

        /// <summary>
        /// Private leave details to be used in test cases
        /// </summary>
        private LeaveRequest leave = new LeaveRequest()
        {
            FromDate = DateTime.ParseExact("14-09-2016", "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN")),
            EndDate = DateTime.ParseExact("14-09-2016", "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN")),
            Reason = StringConstant.LeaveReasonForTest,
            RejoinDate = DateTime.ParseExact("14-09-2016", "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN")),
            Status = Condition.Pending,
            Type = LeaveType.cl,
            CreatedOn = DateTime.UtcNow,
            EmployeeId = StringConstant.StringIdForTest
        };

        /// <summary>
        /// Private slack command to be used in test cases
        /// </summary>
        private SlashCommand slackLeave = new SlashCommand()
        {
            Text = StringConstant.SlashCommandText,
            Username = StringConstant.FirstNameForTest
        };

        private void UserMock()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
        }

        private ApplicationUser newUser = new ApplicationUser()
        {
            UserName = StringConstant.EmailForTest,
            Email = StringConstant.EmailForTest,
            SlackUserName = StringConstant.FirstNameForTest
        };

        private UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);

        private async Task AddUser()
        {
            await _userManager.CreateAsync(newUser);
            await _userManager.AddLoginAsync(newUser.Id, info);
        }
    }
}
