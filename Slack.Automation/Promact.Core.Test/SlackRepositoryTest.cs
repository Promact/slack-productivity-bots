using Autofac;
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
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IClient _client;
        private readonly ISlackRepository _slackRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly Mock<IClient> _mockClient;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        public SlackRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _client = _componentContext.Resolve<IClient>();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _mockClient = _componentContext.Resolve<Mock<IClient>>();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApply()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            LeaveRequest leaveRequest = new LeaveRequest();
            leaveRequest.Reason = StringConstant.Reason;
            var replyText = _attachmentRepository.ReplyText(StringConstant.FirstNameForTest, leaveRequest);
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            var leaveDetails = await _slackRepository.LeaveApply(slackText, slackLeave, StringConstant.AccessTokenForTest);
            Assert.Equal(leaveDetails.Status, Condition.Pending);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplyFalse()
        {
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(Task.FromResult(""));
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            LeaveRequest leaveRequest = new LeaveRequest();
            leaveRequest.Reason = StringConstant.Reason;
            var replyText = _attachmentRepository.ReplyText(StringConstant.FirstNameForTest, leaveRequest);
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            var leaveDetails = await _slackRepository.LeaveApply(slackText, slackLeave, StringConstant.AccessTokenForTest);
            Assert.Equal(leaveDetails.Id, 0);
        }

        /// <summary>
        /// Test cases for checking method UpdateLeave from Slack respository with True value
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
                leave.EndDate.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.ToShortDateString());
            _mockClient.Setup(x => x.UpdateMessage(leaveResponse, replyText));
            _slackRepository.UpdateLeave(leaveResponse);
            var leaveUpdated = _leaveRequestRepository.LeaveById(leave.Id);
            Assert.Equal(Condition.Approved, leaveUpdated.Status);
        }

        /// <summary>
        /// Test cases for checking method UpdateLeave from Slack respository with False value
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
                leave.EndDate.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.ToShortDateString());
            _mockClient.Setup(x => x.UpdateMessage(leaveResponse, replyText));
            _slackRepository.UpdateLeave(leaveResponse);
            var leaveUpdated = _leaveRequestRepository.LeaveById(leave.Id);
            Assert.NotEqual(Condition.Rejected, leaveUpdated.Status);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackLeaveList()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format("{0} {1} {2} {3} {4} {5}", leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.ToShortDateString(), leave.Status, System.Environment.NewLine);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.Text = StringConstant.LeaveListCommandForTest;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            _slackRepository.SlackLeaveList(slackText, slackLeave, StringConstant.AccessTokenForTest);
            _mockClient.Verify(x => x.SendMessage(slackLeave, replyText), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackLeaveListFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.SlashCommandLeaveListErrorMessage;
            slackLeave.Text = StringConstant.LeaveListCommandForTest;
            var slackText = slackLeave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            _slackRepository.SlackLeaveList(slackText, slackLeave, StringConstant.AccessTokenForTest);
            _mockClient.Verify(x => x.SendMessage(slackLeave, replyText), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveCancel from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackLeaveCancel()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format("Your leave Id no: {0} From {1} To {2} has been {3}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.ToShortDateString(), StringConstant.Cancel);
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.Text = StringConstant.LeaveCancelCommandForTest;
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            _slackRepository.SlackLeaveCancel(slackText, slackLeave, StringConstant.AccessTokenForTest);
            _mockClient.Verify(x => x.SendMessage(slackLeave, replyText), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveCancel from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackLeaveCancelFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.CancelLeaveError;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.Text = StringConstant.LeaveCancelCommandForTest;
            slackLeave.Username = StringConstant.FalseStringNameForTest;
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            _slackRepository.SlackLeaveCancel(slackText, slackLeave, StringConstant.AccessTokenForTest);
            _mockClient.Verify(x => x.SendMessage(slackLeave, replyText), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveStatus from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackLeaveStatus()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format("Your leave Id no: {0} From {1} To {2} for {3} is {4}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.ToShortDateString(), leave.Reason, leave.Status);
            slackLeave.Text = StringConstant.LeaveListCommandForTest;
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            _slackRepository.SlackLeaveStatus(slackText, slackLeave, StringConstant.AccessTokenForTest);
            _mockClient.Verify(x => x.SendMessage(slackLeave, replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveStatus from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackLeaveStatusFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.SlashCommandLeaveStatusErrorMessage;
            slackLeave.Text = StringConstant.LeaveListCommandForTest;
            var slackText = slackLeave.Text.Split('"')
                .Select((element, index) => index % 2 == 0 ? element
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                .SelectMany(element => element).ToList();
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            _slackRepository.SlackLeaveStatus(slackText, slackLeave, StringConstant.AccessTokenForTest);
            _mockClient.Verify(x => x.SendMessage(slackLeave, replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveBalance from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackLeaveBalance()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.LeaveBalanceReplyTextForTest;
            var response = Task.FromResult(StringConstant.CasualLeaveResponse);
            var requestUrl = string.Format("{0}{1}", StringConstant.CasualLeaveUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            var userResponse = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            _slackRepository.SlackLeaveBalance(slackLeave, StringConstant.AccessTokenForTest);
            _mockClient.Verify(x => x.SendMessage(slackLeave, replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveBalance from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackLeaveBalanceFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = StringConstant.LeaveBalanceErrorMessage;
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            _slackRepository.SlackLeaveBalance(slackLeave, StringConstant.AccessTokenForTest);
            _mockClient.Verify(x => x.SendMessage(slackLeave, replyText), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveHelp from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackLeaveHelp()
        {
            var replyText = StringConstant.SlackHelpMessage;
            _mockClient.Setup(x => x.SendMessage(slackLeave, replyText));
            _slackRepository.SlackLeaveHelp(slackLeave);
            _mockClient.Verify(x => x.SendMessage(slackLeave, replyText), Times.Once);
        }

        private User user = new User()
        {
            Id = StringConstant.StringIdForTest,
            Email = StringConstant.EmailForTest,
            FirstName = StringConstant.FirstNameForTest,
            IsActive = true,
            LastName = StringConstant.LastNameForTest,
            UserName = StringConstant.EmailForTest
        };

        private SlashChatUpdateResponse leaveResponse = new SlashChatUpdateResponse()
        {
            MessageTs = StringConstant.MessageTsForTest,
            Token = StringConstant.AccessTokenForTest,
            Actions = new SlashChatUpdateResponseAction() { Name = StringConstant.Approved, Value = StringConstant.Approved },
            User = new SlashChatUpdateResponseChannelUser() { Id = StringConstant.StringIdForTest, Name = StringConstant.FirstNameForTest }
        };

        private LeaveRequest leave = new LeaveRequest()
        {
            FromDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow,
            Reason = StringConstant.LeaveReasonForTest,
            RejoinDate = DateTime.UtcNow,
            Status = Condition.Pending,
            Type = StringConstant.LeaveTypeForTest,
            CreatedOn = DateTime.UtcNow,
            EmployeeId = StringConstant.StringIdForTest };
        private SlashCommand slackLeave = new SlashCommand()
        {
            Text = StringConstant.SlashCommandText,
            Username = StringConstant.FirstNameForTest,
            ResponseUrl = Environment.GetEnvironmentVariable(StringConstant.IncomingWebHookUrl, EnvironmentVariableTarget.User)
        };


    }
}
