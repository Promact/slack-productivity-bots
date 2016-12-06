using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Newtonsoft.Json;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.EmailServiceTemplateRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.Email;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly ISlackRepository _slackRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly Mock<IHttpClientService> _mockHttpClient;
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
        private SlackUserDetails firstUser = new SlackUserDetails();
        private SlackUserDetails secondUser = new SlackUserDetails();
        private SlackUserDetails thirdUser = new SlackUserDetails();
        private EmailApplication email = new EmailApplication();
        private readonly IEmailServiceTemplateRepository _emailTemplateRepository;
        private readonly Mock<IEmailService> _mockEmail;
        #endregion

        #region Constructor
        public SlackRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _envVariableRepository = _componentContext.Resolve<IEnvironmentVariableRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            incomingWebhookURL = _envVariableRepository.IncomingWebHookUrl;
            _emailTemplateRepository = _componentContext.Resolve<IEmailServiceTemplateRepository>();
            _mockEmail = _componentContext.Resolve<Mock<IEmailService>>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForCLAsync()
        {
            await AddUser();
            await AddSlackThreeUsersAsync();
            MockingOfUserDetails();
            MockingOfTeamLeaderDetails();
            MockingOfManagementDetails();
            var replyText = _attachmentRepository.ReplyText(_stringConstant.FirstNameForTest, leave);
            var attachment = _attachmentRepository.SlackResponseAttachment(Convert.ToString(1), replyText);
            PostAsyncMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            var text = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
            var textJson = JsonConvert.SerializeObject(text);
            PostAsyncMethodMocking(_envVariableRepository.IncomingWebHookUrl, textJson, _stringConstant.JsonContentString);
            var textTransform = new SlashResponse() { ResponseType = _stringConstant.ResponseTypeEphemeral, Text = replyText };
            var textJsonTransform = JsonConvert.SerializeObject(textTransform);
            PostAsyncMethodMocking(_envVariableRepository.IncomingWebHookUrl, textJsonTransform, _stringConstant.JsonContentString);
            MockingEmailService(_emailTemplateRepository.EmailServiceTemplate(leave));
            MockingUserDetialFromSlackUserId();
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJsonTransform, _stringConstant.JsonContentString), Times.Once);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.AtLeastOnce());
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
            _mockEmail.Verify(x => x.Send(It.IsAny<EmailApplication>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForNoUserAsync()
        {
            var replyText = _stringConstant.SorryYouCannotApplyLeave;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method UpdateLeave casual from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateLeaveAsync()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            leaveResponse.CallbackId = leave.Id;
            var replyText = string.Format(_stringConstant.ReplyTextForUpdateLeave,
                leave.Status,
                leaveResponse.User.Name,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.Value.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.Value.ToShortDateString());
            var responseUrl = string.Format(_stringConstant.UpdateMessageUrl, HttpUtility.UrlEncode(leaveResponse.Token), 
                HttpUtility.UrlEncode(leaveResponse.Channel.Id), HttpUtility.UrlEncode(replyText), 
                HttpUtility.UrlEncode(leaveResponse.MessageTs));
            GetAsyncMethodMocking(_stringConstant.Ok, _stringConstant.SlackChatUpdateUrl, responseUrl, leaveResponse.Token);
            await _slackRepository.UpdateLeaveAsync(leaveResponse);
            var leaveUpdated = await _leaveRequestRepository.LeaveByIdAsync(leave.Id);
            Assert.Equal(Condition.Approved, leaveUpdated.Status);
        }

        /// <summary>
        /// Test cases for checking method UpdateLeave casual from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateLeaveFalseAsync()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            leaveResponse.CallbackId = leave.Id;
            var replyText = string.Format(_stringConstant.ReplyTextForUpdateLeave,
                leave.Status,
                leaveResponse.User.Name,
                leave.FromDate.ToShortDateString(),
                leave.EndDate.Value.ToShortDateString(),
                leave.Reason,
                leave.RejoinDate.Value.ToShortDateString());
            var responseUrl = string.Format(_stringConstant.UpdateMessageUrl, HttpUtility.UrlEncode(leaveResponse.Token), HttpUtility.UrlEncode(leaveResponse.Channel.Id), HttpUtility.UrlEncode(replyText), HttpUtility.UrlEncode(leaveResponse.MessageTs));
            GetAsyncMethodMocking(_stringConstant.Ok, _stringConstant.SlackChatUpdateUrl, responseUrl, leaveResponse.Token);
            leaveResponse.Actions.Name = _stringConstant.Rejected;
            leaveResponse.Actions.Value = _stringConstant.Rejected;
            await _slackRepository.UpdateLeaveAsync(leaveResponse);
            var leaveUpdated = await _leaveRequestRepository.LeaveByIdAsync(leave.Id);
            Assert.Equal(Condition.Rejected, leaveUpdated.Status);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository with True value for casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveListAsync()
        {
            await AddUser();
            await AddSlackThreeUsersAsync();
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveList, leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, System.Environment.NewLine);
            slackLeave.Text = _stringConstant.LeaveListCommandForTest;
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveListFalseAsync()
        {
            await AddUser();
            await AddSlackThreeUsersAsync();
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = _stringConstant.SlashCommandLeaveStatusErrorMessage;
            slackLeave.Text = _stringConstant.LeaveListCommandForTest;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository for Own with true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveListForOwnAsync()
        {
            await AddUser();
            MockingOfUserDetails();
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveList, leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, System.Environment.NewLine);
            slackLeave.Text = _stringConstant.LeaveListTestForOwn;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            MockingUserDetialFromSlackUserId();
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository for Own with false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveListForOwnFalseAsync()
        {
            await AddUser();
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = _stringConstant.SlashCommandLeaveStatusErrorMessage;
            slackLeave.Text = _stringConstant.LeaveListTestForOwn;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
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
            var replyText = string.Format(_stringConstant.ReplyTextForCancelLeave, leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), _stringConstant.Cancel);
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            MockingUserDetialFromSlackUserId();
            slackLeave.Text = _stringConstant.LeaveCancelCommandForTest;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
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
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.LeaveDoesnotExist, _stringConstant.OrElseString, _stringConstant.CancelLeaveError);
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            slackLeave.Text = _stringConstant.LeaveCancelCommandForTest;
            slackLeave.Username = _stringConstant.FalseStringNameForTest;
            slackLeave.UserId = _stringConstant.FalseStringNameForTest;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
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
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = _stringConstant.SlashCommandLeaveCancelErrorMessage;
            slackLeave.Text = _stringConstant.WrongLeaveCancelCommandForTest;
            slackLeave.Username = _stringConstant.FalseStringNameForTest;
            slackLeave.UserId = _stringConstant.FalseStringNameForTest;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveStatus from Slack respository with True value for casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveStatus()
        {
            await AddUser();
            await AddSlackThreeUsersAsync();
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveStatus, leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.Status);
            slackLeave.Text = _stringConstant.LeaveStatusCommandForTest;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveStatus from Slack respository with False value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveStatusFalse()
        {
            await AddUser();
            await AddSlackThreeUsersAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUser);
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = _stringConstant.SlashCommandLeaveStatusErrorMessage;
            slackLeave.Text = _stringConstant.LeaveStatusCommandForTest;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
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
            var replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveStatus, leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.Status);
            slackLeave.Text = _stringConstant.LeaveStatusTestForOwn;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
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
            var replyText = _stringConstant.SlashCommandLeaveStatusErrorMessage;
            slackLeave.Text = _stringConstant.LeaveStatusTestForOwn;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveBalance from Slack respository with True value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveBalance()
        {
            await AddUser();
            _leaveRequestRepository.ApplyLeave(leave);
            slackLeave.Text = _stringConstant.LeaveBalanceTestForOwn;
            var replyText = _stringConstant.LeaveBalanceReplyTextForTest;
            replyText += string.Format(_stringConstant.FirstAndSecondIndexStringFormat, Environment.NewLine, _stringConstant.LeaveBalanceSickReplyTextForTest);
            var response = Task.FromResult(_stringConstant.CasualLeaveResponse);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.CasualLeaveUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var allowedLeaveRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.CasualLeaveUrl, _stringConstant.UserSlackId);
            GetAsyncMethodMocking(_stringConstant.LeaveAllowed, _stringConstant.ProjectUserUrl, allowedLeaveRequestUrl, _stringConstant.AccessTokenForTest);
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
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
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForSL()
        {
            await AddUser();
            await AddSlackThreeUsersAsync();
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            MockingOfTeamLeaderDetails();
            MockingOfManagementDetails();
            slackLeave.Text = _stringConstant.SlashCommandTextSick;
            var replyText = _attachmentRepository.ReplyTextSick(_stringConstant.FirstNameForTest, leave);
            var userTextJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(1), replyText);
            var text = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + _stringConstant.ManagementFirstForTest, Username = _stringConstant.LeaveBot, Attachments = attachment };
            var textJson = JsonConvert.SerializeObject(text);
            PostAsyncMethodMocking(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString);
            MockingEmailService(_emailTemplateRepository.EmailServiceTemplate(leave));
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, userTextJson, _stringConstant.JsonContentString), Times.Once);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
            _mockEmail.Verify(x => x.Send(It.IsAny<EmailApplication>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value sick leave, not user error
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForSLForNoUser()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextSick;
            var replyText = _stringConstant.SorryYouCannotApplyLeave;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value sick leave for other user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForSLForUser()
         {
            await AddUser();
            await AddSlackThreeUsersAsync();
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            MockingOfTeamLeaderDetails();
            MockingOfManagementDetails();
            slackLeave.Text = _stringConstant.SlashCommandTextSickForUser;
            var adminResponse = Task.FromResult(_stringConstant.True);
            var adminrequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserIsAdmin, _stringConstant.UserSlackId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, adminrequestUrl, _stringConstant.AccessTokenForTest)).Returns(adminResponse);
            var replyText = _attachmentRepository.ReplyTextSick(_stringConstant.FirstNameForTest, leave);
            var userTextJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(1), replyText);
            var text = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
            var teamLeaderTextJson = JsonConvert.SerializeObject(text);
            PostAsyncMethodMocking(_envVariableRepository.IncomingWebHookUrl, teamLeaderTextJson, _stringConstant.JsonContentString);
            attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(1), replyText);
            text = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
            var managementTextJson = JsonConvert.SerializeObject(text);
            PostAsyncMethodMocking(_envVariableRepository.IncomingWebHookUrl, managementTextJson, _stringConstant.JsonContentString);
            MockingEmailService(_emailTemplateRepository.EmailServiceTemplateSickLeave(leave));
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, userTextJson, _stringConstant.JsonContentString), Times.Once);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, teamLeaderTextJson, _stringConstant.JsonContentString), Times.Once);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, managementTextJson, _stringConstant.JsonContentString), Times.Once);
            _mockEmail.Verify(x => x.Send(It.IsAny<EmailApplication>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with leave type error value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForErrorLeaveType()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextErrorLeaveType;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var replyText = _stringConstant.NotTypeOfLeave;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with date type error value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForErrorDateFormat()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextErrorDateFormatSick;
            MockingOfUserDetails();
            var replyText = _stringConstant.DateFormatErrorMessage;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with date type error value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForErrorDateFormatForCasual()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextErrorDateFormatCasual;
            MockingOfUserDetails();
            var replyText = _stringConstant.DateFormatErrorMessage;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value casual leave, for no user error
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForCLForNoUser()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextCasual;
            var replyText = _stringConstant.SorryYouCannotApplyLeave;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test case to check method Error of slack repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void Error()
        {
            var replyText = _stringConstant.SlashCommandErrorMessage;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            _slackRepository.Error(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdate()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            MockingUserIsAdminBySlackId();
            var replyText = string.Format(_stringConstant.ReplyTextForSickLeaveUpdate
                            , user.FirstName, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(),
                            leave.Reason, leave.RejoinDate.Value.ToShortDateString());
            slackLeave.Text = _stringConstant.SlashCommandUpdate;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            MockingOfTeamLeaderDetails();
            MockingOfManagementDetails();
            await AddSlackThreeUsersAsync();
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailUrl, _stringConstant.StringIdForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            var attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(1), replyText);
            var teamLeaderText = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
            var teamLeaderTextJson = JsonConvert.SerializeObject(teamLeaderText);
            PostAsyncMethodMocking(_envVariableRepository.IncomingWebHookUrl, teamLeaderTextJson, _stringConstant.JsonContentString);
            attachment = _attachmentRepository.SlackResponseAttachmentWithoutButton(Convert.ToString(1), replyText);
            var userText = new SlashIncomingWebhook() { Channel = _stringConstant.AtTheRate + slackUser.Name, Username = _stringConstant.LeaveBot, Attachments = attachment };
            var userTextJson = JsonConvert.SerializeObject(userText);
            PostAsyncMethodMocking(_envVariableRepository.IncomingWebHookUrl, userTextJson, _stringConstant.JsonContentString);
            MockingEmailService(_emailTemplateRepository.EmailServiceTemplate(leave));
            MockingEmailService(_emailTemplateRepository.EmailServiceTemplateSickLeave(leave));
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, teamLeaderTextJson, _stringConstant.JsonContentString), Times.Once);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, userTextJson, _stringConstant.JsonContentString), Times.Once);
            _mockEmail.Verify(x => x.Send(It.IsAny<EmailApplication>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateDateFormatError()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            MockingUserIsAdminBySlackId();
            var replyText = _stringConstant.DateFormatErrorMessage;
            slackLeave.Text = _stringConstant.SlashCommandUpdateDateError;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateWrongId()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            MockingUserIsAdminBySlackId();
            var replyText = _stringConstant.SickLeaveDoesnotExist;
            slackLeave.Text = _stringConstant.SlashCommandUpdateWrongId;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateTryToUpdateCL()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.cl;
            _leaveRequestRepository.ApplyLeave(leave);
            MockingUserIsAdminBySlackId();
            var replyText = _stringConstant.SickLeaveDoesnotExist;
            slackLeave.Text = _stringConstant.SlashCommandUpdate;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateInValidID()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.cl;
            _leaveRequestRepository.ApplyLeave(leave);
            MockingUserIsAdminBySlackId();
            var replyText = _stringConstant.UpdateEnterAValidLeaveId;
            slackLeave.Text = _stringConstant.SlashCommandUpdateInValidId;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateNotAdmin()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = _stringConstant.AdminErrorMessageUpdateSickLeave;
            slackLeave.Text = _stringConstant.SlashCommandUpdate;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveBalance from Slack respository with false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveBalanceWrong()
        {
            await AddUser();
            _leaveRequestRepository.ApplyLeave(leave);
            slackLeave.Text = _stringConstant.LeaveBalanceTestForOwn;
            var replyText = _stringConstant.LeaveNoUserErrorMessage;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForCLForEmailError()
        {
            SmtpException ex = new SmtpException();
            await AddUser();
            await AddSlackThreeUsersAsync();
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            MockingOfTeamLeaderDetails();
            MockingOfManagementDetails();
            slackLeave.Text = _stringConstant.SlashCommandTextCasual;
            var replyText = string.Format(_stringConstant.ReplyTextForSMTPExceptionErrorMessage, _stringConstant.ErrorWhileSendingEmail, ex.Message.ToString());
            var replyTextSecond = _attachmentRepository.ReplyText(_stringConstant.FirstNameForTest, leave);
            email.Body = _emailTemplateRepository.EmailServiceTemplate(leave);
            _mockEmail.Setup(x => x.Send(It.IsAny<EmailApplication>())).Throws(ex);
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with True value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForSLOtherUserNotAdmin()
        {
            await AddUser();
            slackLeave.Text = _stringConstant.SlashCommandTextSickForUser;
            var replyText = _stringConstant.AdminErrorMessageApplySickLeave + _stringConstant.SorryYouCannotApplyLeave;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveList from Slack respository with True value for sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveListSick()
        {
            await AddUser();
            await AddSlackThreeUsersAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUser);
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format(_stringConstant.ReplyTextForSickLeaveList, leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.Status, System.Environment.NewLine);
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            slackLeave.Text = _stringConstant.LeaveListCommandForTest;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method SlackLeaveStatus from Slack respository with True value for sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveStatusSick()
        {
            await AddUser();
            await AddSlackThreeUsersAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUser);
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var replyText = string.Format(_stringConstant.ReplyTextForSickLeaveStatus, leave.Id, leave.FromDate.ToShortDateString(), leave.Reason, leave.Status);
            slackLeave.Text = _stringConstant.LeaveStatusCommandForTest;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test case for invalid action of leave slash command
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SlackLeaveProperActionError()
        {
            await AddUser();
            await _slackUserRepository.AddSlackUserAsync(slackUser);
            var replyText = _stringConstant.RequestToEnterProperAction;
            slackLeave.Text = _stringConstant.WrongActionSlashCommand;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with back date value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForCLWithBackDate()
        {
            await AddUser();
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var replyText = _stringConstant.BackDateErrorMessage;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            slackLeave.Text = _stringConstant.LeaveWrongCommandForBackDateCL;
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with end date beyond start date value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForCLWithWrongDateFirstTest()
        {
            await AddUser();
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var replyText = _stringConstant.InValidDateErrorMessage;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            slackLeave.Text = _stringConstant.LeaveWrongCommandForBeyondDateFirstExample;
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with rejoin date beyond end date value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForCLWithWrongDateSecondTest()
        {
            await AddUser();
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var replyText = _stringConstant.InValidDateErrorMessage;
            slackLeave.Text = _stringConstant.LeaveWrongCommandForBeyondDateSecondExample;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with back date value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForSLWithBackDate()
        {
            await AddUser();
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var replyText = _stringConstant.BackDateErrorMessage;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            slackLeave.Text = _stringConstant.LeaveWrongCommandForBackDateSL;
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository with end date beyond start date value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateForBeyondStartDate()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            MockingUserIsAdminBySlackId();
            var replyText = _stringConstant.InValidDateErrorMessage;
            slackLeave.Text = _stringConstant.SlashCommandUpdateForBeyondStartDateFirstExample;
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveUpdate from Slack respository with rejoin date beyond end date value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateForBeyondEndDate()
        {
            await AddUser();
            leave.Status = Condition.Approved;
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            MockingUserIsAdminBySlackId();
            var replyText = _stringConstant.InValidDateErrorMessage;
            slackLeave.Text = _stringConstant.SlashCommandUpdateForBeyondStartDateSecondExample;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with rejoin date beyond end date value casual leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForCLWithAlreadyExistDate()
        {
            await AddUser();
            _leaveRequestRepository.ApplyLeave(leave);
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var replyText = _stringConstant.LeaveAlreadyExistOnSameDate;
            slackLeave.Text = _stringConstant.SlashCommandText;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository with rejoin date beyond end date value sick leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyForSLWithAlreadyExistDate()
        {
            await AddUser();
            _leaveRequestRepository.ApplyLeave(leave);
            MockingOfUserDetails();
            MockingUserDetialFromSlackUserId();
            var replyText = _stringConstant.LeaveAlreadyExistOnSameDate;
            slackLeave.Text = _stringConstant.SlashCommandTextSick;
            var textJson = SlackReplyMethodMocking(slackLeave.ResponseUrl, replyText, _stringConstant.JsonContentString);
            await _slackRepository.LeaveRequestAsync(slackLeave);
            _mockHttpClient.Verify(x => x.PostAsync(slackLeave.ResponseUrl, textJson, _stringConstant.JsonContentString), Times.Once);
        }
        #endregion

        #region Initialisation
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
            slackUser.Name = _stringConstant.ManagementFirstForTest;
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
            var dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            leave.FromDate = DateTime.ParseExact(DateTime.UtcNow.ToShortDateString(), dateFormat, CultureInfo.InvariantCulture);
            leave.EndDate = DateTime.ParseExact(DateTime.UtcNow.ToShortDateString(), dateFormat, CultureInfo.InvariantCulture);
            leave.Reason = _stringConstant.LeaveReasonForTest;
            leave.RejoinDate = DateTime.ParseExact(DateTime.UtcNow.AddDays(1).ToShortDateString(), dateFormat, CultureInfo.InvariantCulture);
            leave.Status = Condition.Pending;
            leave.Type = LeaveType.cl;
            leave.CreatedOn = DateTime.UtcNow;
            leave.EmployeeId = _stringConstant.StringIdForTest;

            slackLeave.Text = _stringConstant.SlashCommandText;
            slackLeave.Username = _stringConstant.FirstNameForTest;
            slackLeave.ResponseUrl = incomingWebhookURL;
            slackLeave.UserId = _stringConstant.UserSlackId;
            slackLeave.ChannelId = _stringConstant.ChannelId;
            slackLeave.ChannelName = _stringConstant.ChannelName;
            slackLeave.Command = _stringConstant.Command;
            slackLeave.TeamDomain = _stringConstant.TeamDomain;
            slackLeave.TeamId = _stringConstant.TeamId;
            slackLeave.Token = _stringConstant.Token;

            newUser.UserName = _stringConstant.EmailForTest;
            newUser.Email = _stringConstant.EmailForTest;
            newUser.SlackUserId = _stringConstant.UserSlackId;
            firstUser.CreatedOn = DateTime.UtcNow;
            firstUser.Deleted = false;
            firstUser.Email = _stringConstant.EmailForTest;
            firstUser.FirstName = _stringConstant.UserName;
            firstUser.UserId = _stringConstant.UserSlackId;
            firstUser.Name = _stringConstant.FirstNameForTest;
            secondUser.CreatedOn = DateTime.UtcNow;
            secondUser.Deleted = false;
            secondUser.Email = _stringConstant.TeamLeaderEmailForTest;
            secondUser.FirstName = _stringConstant.TeamLeader;
            secondUser.UserId = _stringConstant.TeamLeaderSlackId;
            secondUser.Name = _stringConstant.TeamLeader;
            thirdUser.CreatedOn = DateTime.UtcNow;
            thirdUser.Deleted = false;
            thirdUser.Email = _stringConstant.ManagementEmailForTest;
            thirdUser.FirstName = _stringConstant.ManagementFirstForTest;
            thirdUser.UserId = _stringConstant.ManagementSlackId;
            thirdUser.Name = _stringConstant.ManagementFirstForTest;
            email.From = _stringConstant.EmailForTest;
            leaveResponse.Channel = new SlashChatUpdateResponseChannelUser() { Id = _stringConstant.ChannelId };
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Common method used to get user details from user name using moq
        /// </summary>
        private void UserMock()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
        }

        /// <summary>
        /// Method to add user in database
        /// </summary>
        private async Task AddUser()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(newUser);
            await _userManager.AddLoginAsync(newUser.Id, info);
        }

        /// <summary>
        /// Mocking of Slack message sending common method
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentString"></param>
        /// <param name="contentHeader"></param>
        /// <returns>Json text</returns>
        private string SlackReplyMethodMocking(string baseUrl, string contentString, string contentHeader)
        {
            var text = new SlashResponse() { ResponseType = _stringConstant.ResponseTypeEphemeral, Text = contentString };
            var textJson = JsonConvert.SerializeObject(text);
            PostAsyncMethodMocking(baseUrl, textJson, contentHeader);
            return textJson;
        }

        /// <summary>
        /// Mocking of PostAsync method of HttpClientRepository
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="contentString"></param>
        /// <param name="contentHeader"></param>
        private void PostAsyncMethodMocking(string baseUrl, string contentString, string contentHeader)
        {
            var responseString = Task.FromResult(_stringConstant.Ok);
            _mockHttpClient.Setup(x => x.PostAsync(baseUrl, contentString, contentHeader)).Returns(responseString);
        }

        /// <summary>
        /// Mocking of GetAsync method of HttpClientRepository
        /// </summary>
        /// <param name="toReturn"></param>
        /// <param name="baseUrl"></param>
        /// <param name="contentUrl"></param>
        /// <param name="accessToken"></param>
        private void GetAsyncMethodMocking(string toReturn, string baseUrl, string contentUrl, string accessToken)
        {
            var responseString = Task.FromResult(toReturn);
            _mockHttpClient.Setup(x => x.GetAsync(baseUrl, contentUrl, accessToken)).Returns(responseString);
        }

        /// <summary>
        /// Common method used to get user details from user name using moq
        /// </summary>
        private void MockingOfUserDetails()
        {
            var response = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);
        }

        /// <summary>
        /// Common method used to get teamLeader details from user name using moq
        /// </summary>
        private void MockingOfTeamLeaderDetails()
        {
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
        }

        /// <summary>
        /// Common method used to get management details from user name using moq
        /// </summary>
        private void MockingOfManagementDetails()
        {
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = _stringConstant.ManagementDetailsUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
        }

        /// <summary>
        /// Common method used to add user details in slack table
        /// </summary>
        private async Task AddSlackThreeUsersAsync()
        {
            await _slackUserRepository.AddSlackUserAsync(firstUser);
            await _slackUserRepository.AddSlackUserAsync(secondUser);
            await _slackUserRepository.AddSlackUserAsync(thirdUser);
        }

        /// <summary>
        /// Common method used to moq email service
        /// </summary>
        /// <param name="body"></param>
        private void MockingEmailService(string body)
        {
            email.Body = body;
            email.Subject = _stringConstant.EmailSubject;
            email.To = _stringConstant.TeamLeaderEmailForTest;
            _mockEmail.Setup(x => x.Send(It.IsAny<EmailApplication>()));
        }

        /// <summary>
        /// Common method used to get user details from user slackId using moq
        /// </summary>
        private void MockingUserDetialFromSlackUserId()
        {
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.UserSlackId);
            GetAsyncMethodMocking(_stringConstant.UserDetailsFromOauthServer, _stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest);
        }

        /// <summary>
        /// Common method used to get user is Admin or not from user slackId using moq
        /// </summary>
        private void MockingUserIsAdminBySlackId()
        {
            var adminResponse = Task.FromResult(_stringConstant.True);
            var adminrequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserIsAdmin, _stringConstant.UserSlackId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, adminrequestUrl, _stringConstant.AccessTokenForTest)).Returns(adminResponse);
        }
        #endregion
    }
}
