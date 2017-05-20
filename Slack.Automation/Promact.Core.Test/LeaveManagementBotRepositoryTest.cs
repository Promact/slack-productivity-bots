using Autofac;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Moq;
using Newtonsoft.Json;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.LeaveManagementBotRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.ServiceRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringLiteral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class LeaveManagementBotRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly ILeaveManagementBotRepository _leaveManagementBotRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly ApplicationUserManager _userManager;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly Mock<IHttpClientService> _httpClientMock;
        private readonly AppStringLiteral _stringConstant;
        private readonly IRepository<TemporaryLeaveRequestDetail> _temporaryLeaveRequestDetailDataRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly Mock<IServiceRepository> _serviceRepositoryMock;
        private readonly IMapper _mapper;
        private readonly IRepository<IncomingWebHook> _incomingWebHookRepository;
        private SlackUserDetails slackUser = new SlackUserDetails();
        private Question firstQuestionLeaveManagement = new Question();
        private Question secondQuestionLeaveManagement = new Question();
        private Question thirdQuestionLeaveManagement = new Question();
        private Question fourthQuestionLeaveManagement = new Question();
        private Question fifthQuestionLeaveManagement = new Question();
        private Question sixthQuestionLeaveManagement = new Question();
        private ApplicationUser user = new ApplicationUser();
        private TemporaryLeaveRequestDetail temporaryLeaveDetail = new TemporaryLeaveRequestDetail();
        private LeaveRequest leaveRequest = new LeaveRequest();
        private LeaveRequest secondLeaveRequest = new LeaveRequest();
        private LeaveRequest thirdLeaveRequest = new LeaveRequest();
        #endregion

        #region Constructor
        public LeaveManagementBotRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _leaveManagementBotRepository = _componentContext.Resolve<ILeaveManagementBotRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _httpClientMock = _componentContext.Resolve<Mock<IHttpClientService>>();
            _stringConstant = _componentContext.Resolve<ISingletonStringLiteral>().StringConstant;
            _temporaryLeaveRequestDetailDataRepository = _componentContext.Resolve<IRepository<TemporaryLeaveRequestDetail>>();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
            _serviceRepositoryMock = _componentContext.Resolve<Mock<IServiceRepository>>();
            _mapper = _componentContext.Resolve<IMapper>();
            _incomingWebHookRepository = _componentContext.Resolve<IRepository<IncomingWebHook>>();
            Initialize();
        }
        #endregion

        #region Test cases
        /// <summary>
        /// Method to start leave apply process but user not found - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UserNotFoundProcessLeaveAsync()
        {
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                _stringConstant.LeaveApplyCommand);
            Assert.Equal(result, _stringConstant.SorryYouCannotApplyLeave);
        }

        /// <summary>
        /// Method to start leave apply process improper action - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ImProperActionProcessLeaveAsync()
        {
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, 
                _stringConstant.LeaveProcessWrongCommand);
            Assert.Equal(result, _stringConstant.ProperActionErrorMessage);
        }

        /// <summary>
        /// Method to start leave apply process - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task StartLeaveApplyProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                _stringConstant.LeaveApplyCommand);
            Assert.Equal(result, firstQuestionLeaveManagement.QuestionStatement);
        }

        /// <summary>
        /// Method to start leave apply process - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task StartLeaveApplyAlreadtStartedProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.LeaveType)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                _stringConstant.LeaveApplyCommand);
            var previousQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.LeaveType);
            var expectedResult = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.NotTypeOfLeave,
                    Environment.NewLine, previousQuestion.QuestionStatement);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave apply, answer of first question - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyFirstAnswerProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.LeaveType)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, LeaveType.cl.ToString());
            Assert.Equal(result, secondQuestionLeaveManagement.QuestionStatement);
        }

        /// <summary>
        /// Method to process leave apply, answer of second question - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplySecondAnswerProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.Reason)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.Reason);
            Assert.Equal(result, thirdQuestionLeaveManagement.QuestionStatement);
        }

        /// <summary>
        /// Method to process leave apply, answer of second question expected but user fired leave apply command- SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplySecondAnswerExpectedProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.Reason)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                _stringConstant.LeaveApplyCommand);
            Assert.Equal(result, secondQuestionLeaveManagement.QuestionStatement);
        }

        /// <summary>
        /// Method to process leave apply, answer of third question for cl - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyThirdAnswerForCLFromDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.FromDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, DateTime.UtcNow.ToShortDateString());
            Assert.Equal(result, fourthQuestionLeaveManagement.QuestionStatement);
        }

        /// <summary>
        /// Method to process leave apply, answer of third question for sl - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyThirdAnswerForSLFromDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.FromDate)).Id;
            temporaryLeaveDetail.Type = LeaveType.sl;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, DateTime.UtcNow.ToShortDateString());
            Assert.Equal(result, sixthQuestionLeaveManagement.QuestionStatement);
        }

        /// <summary>
        /// Method to process leave apply, answer of third question expected but user fired leave apply command and date format error - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyThirdAnswerDateFormatErrorFromDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.FromDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                _stringConstant.LeaveApplyCommand);
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            var expectedResult = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave apply, answer of third question expected but error found for back date- SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyThirdAnswerBackDateFromDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.FromDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            Assert.Equal(result, _stringConstant.BackDateErrorMessage);
        }

        /// <summary>
        /// Method to process leave apply, answer of third question expected but error found for leave already exist - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyThirdAnswerLeaveAlreadyExistFromDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.FromDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                DateTime.UtcNow.ToShortDateString());
            Assert.Equal(result, _stringConstant.LeaveAlreadyExistOnSameDate);
        }

        /// <summary>
        /// Method to process leave apply, answer of fourth question expected - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyFourthAnswerEndDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.FromDate = DateTime.UtcNow;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                DateTime.UtcNow.ToShortDateString());
            Assert.Equal(result, fifthQuestionLeaveManagement.QuestionStatement);
        }

        /// <summary>
        /// Method to process leave apply, answer of fourth question expected but user fired leave apply command and date format error - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyFourthAnswerDateFormatEndDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.FromDate = DateTime.UtcNow;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                _stringConstant.LeaveApplyCommand);
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            var dateErrorMessage = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
            var expectedResult = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, dateErrorMessage, Environment.NewLine,
                fourthQuestionLeaveManagement.QuestionStatement);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave apply, answer of fourth question expected but got error leave already exist - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyFourthAnswerEndDateLeaveAlreadyExistProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            temporaryLeaveDetail.FromDate = DateTime.UtcNow;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                DateTime.UtcNow.ToShortDateString());
            Assert.Equal(result, _stringConstant.LeaveAlreadyExistOnSameDate);
        }

        /// <summary>
        /// Method to process leave apply, answer of fourth question expected but got error back date - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyFourthAnswerEndDateBackDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            temporaryLeaveDetail.FromDate = DateTime.UtcNow;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var expectedResult = string.Format(_stringConstant.EndDateBeyondStartDateErrorMessage, Environment.NewLine,
                        fourthQuestionLeaveManagement.QuestionStatement);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave apply, answer of fifth question expected - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyFifthAnswerEndDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.EndDate = DateTime.UtcNow;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.RejoinDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                DateTime.UtcNow.ToShortDateString());
            Assert.Equal(result, sixthQuestionLeaveManagement.QuestionStatement);
        }

        /// <summary>
        /// Method to process leave apply, answer of fifth question expected but user fired leave apply command and date format error - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyFifthAnswerDateFormatEndDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.EndDate = DateTime.UtcNow;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.RejoinDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                _stringConstant.LeaveApplyCommand);
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            var dateErrorMessage = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
            var expectedResult = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, dateErrorMessage, Environment.NewLine,
                fifthQuestionLeaveManagement.QuestionStatement);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave apply, answer of fourth question expected but got error back date - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyFifthhAnswerEndDateBackDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            temporaryLeaveDetail.EndDate = DateTime.UtcNow;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.RejoinDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var expectedResult = string.Format(_stringConstant.RejoinDateBeyondEndDateErrorMessage, Environment.NewLine,
                        fifthQuestionLeaveManagement.QuestionStatement);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave apply, answer of sixth question expected for cl - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplySixthAnswerForCLProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockPostAsync();
            MockingGetListOfProjectsEnrollmentOfUserByUserId();
            await MockingGetTeamLeaderUserId();
            await MockingGetManagementUserName();
            temporaryLeaveDetail.FromDate = DateTime.UtcNow;
            temporaryLeaveDetail.EndDate = DateTime.UtcNow;
            temporaryLeaveDetail.RejoinDate = DateTime.UtcNow;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.SendLeaveMail)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                SendEmailConfirmation.yes.ToString());
            Assert.Equal(result, _stringConstant.ThankYou);
        }

        /// <summary>
        /// Method to process leave apply, answer of sixth question expected for sl - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplySixthAnswerForSLProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockPostAsync();
            MockingGetListOfProjectsEnrollmentOfUserByUserId();
            await MockingGetTeamLeaderUserId();
            await MockingGetManagementUserName();
            temporaryLeaveDetail.FromDate = DateTime.UtcNow;
            temporaryLeaveDetail.EndDate = DateTime.UtcNow;
            temporaryLeaveDetail.RejoinDate = DateTime.UtcNow;
            temporaryLeaveDetail.Type = LeaveType.sl;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.SendLeaveMail)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                SendEmailConfirmation.yes.ToString());
            Assert.Equal(result, _stringConstant.ThankYou);
        }

        /// <summary>
        /// Method to process leave apply, answer of sixth question expected for sl - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplySixthAnswerWrongAnswerProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            temporaryLeaveDetail.FromDate = DateTime.UtcNow;
            temporaryLeaveDetail.EndDate = DateTime.UtcNow;
            temporaryLeaveDetail.RejoinDate = DateTime.UtcNow;
            temporaryLeaveDetail.Type = LeaveType.sl;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.SendLeaveMail)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.Ok);
            var expectedResult = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                    _stringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, sixthQuestionLeaveManagement.QuestionStatement);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave apply, answer of sixth question expected for sl - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveErrorMessageProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.Ok);
            Assert.Equal(result, _stringConstant.LeaveBotDoesNotUnderStandErrorMessage);
        }

        /// <summary>
        /// Method to process leave list for own - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveListForOwnProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveListCommand);
            var expectedResult = GetLeaveListMessageByLeaveList(_leaveRequestRepository.LeaveListByUserId(_stringConstant.StringIdForTest).ToList());
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave list for own for no record found - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveListForOwnNoLeaveExistProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveListCommand);
            Assert.Equal(result, _stringConstant.LeaveDoesNotExistErrorMessage);
        }

        /// <summary>
        /// Method to process leave list for other - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveListForOtherProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await AddEmployeeDetailsAsync();
            MockingUserIsAdmin();
            leaveRequest.EmployeeId = _stringConstant.TeamLeaderIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            secondLeaveRequest.EmployeeId = _stringConstant.TeamLeaderIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(secondLeaveRequest);
            thirdLeaveRequest.EmployeeId = _stringConstant.TeamLeaderIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(thirdLeaveRequest);
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveListCommand
                + _stringConstant.TeamLeader);
            var expectedResult = GetLeaveListMessageByLeaveList(_leaveRequestRepository.LeaveListByUserId(_stringConstant.TeamLeaderIdForTest).ToList());
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave list for other but no record found - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveListForOtherNoFoundProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await AddEmployeeDetailsAsync();
            MockingUserIsAdmin();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveListCommand
                + _stringConstant.TeamLeader);
            var expectedResult = string.Format(_stringConstant.LeaveListForOtherErrorMessage, _stringConstant.TeamLeader);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave list for other but unauthorize - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveListForOtherUnAuthorizeProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await AddEmployeeDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveListCommand
                + _stringConstant.TeamLeader);
            Assert.Equal(result, _stringConstant.UserIsNotAllowedToListOtherLeaveDetailsMessage);
        }

        /// <summary>
        /// Method to process leave list for other but employee not found - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveListForOtherEmployeeNotFoundProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            await AddEmployeeSlackDetailAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveListCommand
                + _stringConstant.TeamLeader);
            Assert.Equal(result, _stringConstant.MessageToRequestToAddToSlackOtherUser);
        }

        /// <summary>
        /// Method to process leave list for other but employee not found - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveListForOtherEmployeeAddToSlackErrorProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveListCommand
                + _stringConstant.TeamLeader);
            var expectedResult = string.Format(_stringConstant.UserNotFoundRequestToAddToSlackOtherUser, _stringConstant.TeamLeader);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave cancel - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveCancelProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            leaveRequest.Status = Condition.Pending;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveCancelCommand
                + leaveRequest.Id);
            Assert.Equal(result, _stringConstant.LeaveCancelSuccessfulMessage);
        }

        /// <summary>
        /// Method to process leave cancel, status already changed - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveCancelProcessLeaveStatusAlreadyChangeProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            leaveRequest.Status = Condition.Approved;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveCancelCommand
                + leaveRequest.Id);
            var expectedResult = string.Format(_stringConstant.LeaveStatusAlreadyUpdatedErrorMessge, leaveRequest.Id, leaveRequest.Reason,
                leaveRequest.Status);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave cancel, unauthorize - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveCancelProcessUnAuthorizeProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            leaveRequest.EmployeeId = _stringConstant.TeamLeaderIdForTest;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveCancelCommand
                + leaveRequest.Id);
            var expectedResult = string.Format(_stringConstant.LeaveCancelUnAuthorizeErrorMessage, leaveRequest.Id);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave cancel, no record found - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveCancelProcessNoRecordFoundProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveCancelCommand 
                + leaveRequest.Id);
            var expectedResult = string.Format(_stringConstant.LeaveDoesNotExistErrorMessageWithLeaveIdFormat, leaveRequest.Id);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave cancel, leave id is not numeric - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveCancelProcessLeaveIdFormatErrorProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveCancelCommand 
                + _stringConstant.Ok);
            Assert.Equal(result, _stringConstant.LeaveCancelCommandErrorFormatMessage);
        }

        /// <summary>
        /// Method to process leave cancel, leave format error - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveCancelProcessLeaveFormatErrorProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveCancelCommand);
            Assert.Equal(result, _stringConstant.IncorrectLeaveCancelCommandMessage);
        }

        /// <summary>
        /// Method to process leave status - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveStatusProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveStatusCommand);
            var expectedResult = string.Format(_stringConstant.ReplyTextForCasualLeaveList, leaveRequest.Id, leaveRequest.Reason,
                leaveRequest.FromDate.ToShortDateString(), leaveRequest.EndDate.Value.ToShortDateString(), leaveRequest.Status, Environment.NewLine);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave status, but no record found - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveStatusNoRecordFoundProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveStatusCommand);
            Assert.Equal(result, _stringConstant.LeaveDoesNotExistErrorMessage);
        }

        /// <summary>
        /// Method to process leave balance - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveBalanceProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingAllowedLeave();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveBalanceCommand);
            var expectedResult = string.Format(_stringConstant.ReplyTextForCasualLeaveBalance, 0, 14, Environment.NewLine, 14);
            expectedResult += string.Format(_stringConstant.ReplyTextForSickLeaveBalance, 0, 7, Environment.NewLine, 7);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave help for admin - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveHelpForAdminProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveHelpCommand);
            var expectedResult = string.Format(_stringConstant.FirstAndSecondIndexStringFormat,_stringConstant.LeaveHelpBotCommands, 
                _stringConstant.LeaveUpdateFormatMessage);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave help for user - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveHelpForUserProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, _stringConstant.LeaveHelpCommand);;
            Assert.Equal(result, _stringConstant.LeaveHelpBotCommands);
        }

        /// <summary>
        /// Method to process sick leave update - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            leaveRequest.Type = LeaveType.sl;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, leaveRequest.Id, DateTime.UtcNow.AddDays(1).ToShortDateString(),
                DateTime.UtcNow.AddDays(2).ToShortDateString());
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            var expectedResult = string.Format(_stringConstant.LeaveUpdateMessage, leaveRequest.Id, Environment.NewLine);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process sick leave update rejoin date beyond error - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateRejoinDateErrorProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            leaveRequest.Type = LeaveType.sl;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, leaveRequest.Id, DateTime.UtcNow.AddDays(1).ToShortDateString(),
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            var expectedResult = string.Format(_stringConstant.RejoinDateBeyondEndDateErrorMessage, Environment.NewLine,
                _stringConstant.LeaveUpdateFormatMessage);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process sick leave update rejoin date format error - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateRejoinDateFormatErrorProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            leaveRequest.Type = LeaveType.sl;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, leaveRequest.Id, DateTime.UtcNow.AddDays(1).ToShortDateString(),
                _stringConstant.Ok);
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            var dateErrorMessage = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
            var expectedResult = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, dateErrorMessage,
                Environment.NewLine, _stringConstant.LeaveUpdateFormatMessage);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process sick leave update end date beyond error - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateEndDateDateErrorProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            leaveRequest.Type = LeaveType.sl;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, leaveRequest.Id, DateTime.UtcNow.AddDays(-1).ToShortDateString(),
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            var expectedResult = string.Format(_stringConstant.EndDateBeyondStartDateErrorMessage, Environment.NewLine,
                _stringConstant.LeaveUpdateFormatMessage);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process sick leave update end date format error - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateEndDateFormatErrorProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            leaveRequest.Type = LeaveType.sl;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, leaveRequest.Id, _stringConstant.Ok,
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            var dateErrorMessage = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
            var expectedResult = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, dateErrorMessage,
                Environment.NewLine, _stringConstant.LeaveUpdateFormatMessage);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process sick leave update end date but leave on that day already exist - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateEndDateAlreadyLeaveExistProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            leaveRequest.Type = LeaveType.sl;
            leaveRequest.EndDate = null;
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, leaveRequest.Id, DateTime.UtcNow.ToShortDateString(),
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            Assert.Equal(result, _stringConstant.LeaveAlreadyExistOnSameDate);
        }

        /// <summary>
        /// Method to process leave update but not sl - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateNotSickErrorProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, leaveRequest.Id, DateTime.UtcNow.ToShortDateString(),
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            var expectedResult = string.Format(_stringConstant.SickLeaveDoesnotExist, leaveRequest.Id);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave update for leave not found - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateLeaveNotFoundProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, 0, DateTime.UtcNow.ToShortDateString(),
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            var expectedResult = string.Format(_stringConstant.LeaveDoesNotExistErrorMessageWithLeaveIdFormat, 0);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave update for wrong leave Id - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateLeaveIdImProperProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, _stringConstant.Ok, DateTime.UtcNow.ToShortDateString(),
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            var expectedResult = string.Format(_stringConstant.LeaveUpdateLeaveIdErrorFormatErrorMessage, Environment.NewLine);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave update error for unautorize user - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUpdateUnAuthorizeProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            await _leaveRequestRepository.ApplyLeaveAsync(leaveRequest);
            var message = string.Format(_stringConstant.LeaveUpdatedMessage, _stringConstant.Ok, DateTime.UtcNow.ToShortDateString(),
                DateTime.UtcNow.AddDays(-1).ToShortDateString());
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, message);
            Assert.Equal(result, _stringConstant.AdminErrorMessageUpdateSickLeave);
        }

        /// <summary>
        /// Method to convert slack user id to slack user's name for user found - SS
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task ProcessToConvertSlackIdToSlackUserNameForUserFoundAsync()
        {
            await AddEmployeeSlackDetailAsync();
            bool result;
            var message = _leaveManagementBotRepository.ProcessToConvertSlackIdToSlackUserName(_stringConstant.Ok + " <@" +
                _stringConstant.TeamLeaderSlackId + ">", out result);
            Assert.False(result);
        }

        /// <summary>
        /// Method to convert slack user id to slack user's name for user not found - SS
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public void ProcessToConvertSlackIdToSlackUserNameForUserNotFound()
        {
            bool result;
            var message = _leaveManagementBotRepository.ProcessToConvertSlackIdToSlackUserName(_stringConstant.Ok + " <@" +
                _stringConstant.TeamLeaderSlackId + ">", out result);
            Assert.True(result);
        }

        /// <summary>
        /// Method to convert slack user id to slack user's name for local string - SS
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public void ProcessToConvertSlackIdToSlackUserNameNormalString()
        {
            bool result;
            var message = _leaveManagementBotRepository.ProcessToConvertSlackIdToSlackUserName(_stringConstant.Ok, out result);
            Assert.False(result);
        }

        /// <summary>
        /// Method to leave process for in-active user - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task InActiveUserProcessLeaveAsync()
        {
            await AddUserAndMockAccessTokenReturnAsync();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl,
                _stringConstant.StringIdForTest);
            var expectedUserDetail = new User()
            {
                Email = _stringConstant.EmailForTest,
                UserName = _stringConstant.EmailForTest,
                NumberOfCasualLeave = 14,
                NumberOfSickLeave = 7,
                FirstName = _stringConstant.NameForTest,
                SlackUserId = _stringConstant.SlackUserID,
                IsActive = false,
                Id = _stringConstant.StringIdForTest
            };
            _httpClientMock.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest, 
                _stringConstant.Bearer)).Returns(Task.FromResult(JsonConvert.SerializeObject(expectedUserDetail)));
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                _stringConstant.LeaveApplyCommand);
            Assert.Equal(result, _stringConstant.InActiveUserErrorMessage);
        }

        /// <summary>
        /// Method to update leave but got error of format error - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateLeaveCommandErrorProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID,
                _stringConstant.LeaveUpdateCommandWrongFormatForTest);
            var expectedResult = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.LeaveUpdateFormatErrorMessage,
                    Environment.NewLine, _stringConstant.LeaveUpdateFormatMessage);
            Assert.Equal(result, expectedResult);
        }

        /// <summary>
        /// Method to process leave sick leave already exist on same date - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task SickLeaveExistOnSameDateProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            secondLeaveRequest.EndDate = null;
            await _leaveRequestRepository.ApplyLeaveAsync(secondLeaveRequest);
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.FromDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, DateTime.UtcNow.ToShortDateString());
            Assert.Equal(result, _stringConstant.LeaveAlreadyExistOnSameDate);
        }

        /// <summary>
        /// Method to process advanced leave exist then leave leave of today - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyBeforeAdvancedLeaveProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            secondLeaveRequest.FromDate = DateTime.UtcNow.AddDays(5);
            secondLeaveRequest.EndDate = DateTime.UtcNow.AddDays(5);
            secondLeaveRequest.RejoinDate = DateTime.UtcNow.AddDays(5);
            await _leaveRequestRepository.ApplyLeaveAsync(secondLeaveRequest);
            secondLeaveRequest.EndDate = null;
            await _leaveRequestRepository.ApplyLeaveAsync(secondLeaveRequest);
            temporaryLeaveDetail.FromDate = DateTime.UtcNow;
            temporaryLeaveDetail.EndDate = null;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, DateTime.UtcNow.ToShortDateString());
            Assert.Equal(result, fifthQuestionLeaveManagement.QuestionStatement);
        }

        /// <summary>
        /// Method to process advanced leave exist then add end date that cover the advanced leave - SS
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplyBeforeAdvancedAndEndDateCoveringTheAdvancedLeaveLeaveProcessLeaveAsync()
        {
            await AddQuestionAsync();
            await AddUserAndMockAccessTokenReturnAsync();
            MockGetUserDetails();
            MockingUserIsAdmin();
            secondLeaveRequest.FromDate = DateTime.UtcNow.AddDays(5);
            secondLeaveRequest.EndDate = DateTime.UtcNow.AddDays(5);
            secondLeaveRequest.RejoinDate = DateTime.UtcNow.AddDays(5);
            await _leaveRequestRepository.ApplyLeaveAsync(secondLeaveRequest);
            secondLeaveRequest.EndDate = null;
            await _leaveRequestRepository.ApplyLeaveAsync(secondLeaveRequest);
            temporaryLeaveDetail.FromDate = DateTime.UtcNow;
            temporaryLeaveDetail.EndDate = null;
            temporaryLeaveDetail.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate)).Id;
            await AddTemporaryLeaveDetailsAsync();
            var result = await _leaveManagementBotRepository.ProcessLeaveAsync(_stringConstant.SlackUserID, 
                DateTime.UtcNow.AddDays(6).ToShortDateString());
            Assert.Equal(result, _stringConstant.LeaveAlreadyExistOnSameDate);
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// Initialization - SS
        /// </summary>
        public void Initialize()
        {
            firstQuestionLeaveManagement.CreatedOn = DateTime.UtcNow;
            firstQuestionLeaveManagement.OrderNumber = QuestionOrder.LeaveType;
            firstQuestionLeaveManagement.QuestionStatement = _stringConstant.FirstQuestionLeaveManagement;
            firstQuestionLeaveManagement.Type = BotQuestionType.LeaveManagement;
            secondQuestionLeaveManagement.CreatedOn = DateTime.UtcNow;
            secondQuestionLeaveManagement.OrderNumber = QuestionOrder.Reason;
            secondQuestionLeaveManagement.QuestionStatement = _stringConstant.SecondQuestionLeaveManagement;
            secondQuestionLeaveManagement.Type = BotQuestionType.LeaveManagement;
            thirdQuestionLeaveManagement.CreatedOn = DateTime.UtcNow;
            thirdQuestionLeaveManagement.OrderNumber = QuestionOrder.FromDate;
            thirdQuestionLeaveManagement.QuestionStatement = _stringConstant.ThirdQuestionLeaveManagement;
            thirdQuestionLeaveManagement.Type = BotQuestionType.LeaveManagement;
            fourthQuestionLeaveManagement.CreatedOn = DateTime.UtcNow;
            fourthQuestionLeaveManagement.OrderNumber = QuestionOrder.EndDate;
            fourthQuestionLeaveManagement.QuestionStatement = _stringConstant.FourthQuestionLeaveManagement;
            fourthQuestionLeaveManagement.Type = BotQuestionType.LeaveManagement;
            fifthQuestionLeaveManagement.CreatedOn = DateTime.UtcNow;
            fifthQuestionLeaveManagement.OrderNumber = QuestionOrder.RejoinDate;
            fifthQuestionLeaveManagement.QuestionStatement = _stringConstant.FifthQuestionLeaveManagement;
            fifthQuestionLeaveManagement.Type = BotQuestionType.LeaveManagement;
            sixthQuestionLeaveManagement.CreatedOn = DateTime.UtcNow;
            sixthQuestionLeaveManagement.OrderNumber = QuestionOrder.SendLeaveMail;
            sixthQuestionLeaveManagement.QuestionStatement = _stringConstant.SixthQuestionLeaveManagement;
            sixthQuestionLeaveManagement.Type = BotQuestionType.LeaveManagement;
            user.Email = _stringConstant.EmailForTest;
            user.UserName = _stringConstant.EmailForTest;
            user.SlackUserId = _stringConstant.SlackUserID;
            user.Id = _stringConstant.StringIdForTest;
            slackUser.CreatedOn = DateTime.UtcNow;
            slackUser.Email = _stringConstant.EmailForTest;
            slackUser.UserId = _stringConstant.SlackUserID;
            slackUser.Deleted = false;
            slackUser.FirstName = _stringConstant.NameForTest;
            slackUser.Name = _stringConstant.NameForTest;
            temporaryLeaveDetail.CreatedOn = DateTime.UtcNow;
            temporaryLeaveDetail.EmployeeId = _stringConstant.StringIdForTest;
            temporaryLeaveDetail.Reason = _stringConstant.Reason;
            leaveRequest.CreatedOn = DateTime.UtcNow;
            leaveRequest.EmployeeId = _stringConstant.StringIdForTest;
            leaveRequest.EndDate = DateTime.UtcNow;
            leaveRequest.FromDate = DateTime.UtcNow;
            leaveRequest.RejoinDate = DateTime.UtcNow;
            leaveRequest.Reason = _stringConstant.Reason;
            leaveRequest.Status = Condition.Approved;
            leaveRequest.Type = LeaveType.cl;
            secondLeaveRequest.CreatedOn = DateTime.UtcNow;
            secondLeaveRequest.EmployeeId = _stringConstant.StringIdForTest;
            secondLeaveRequest.EndDate = DateTime.UtcNow;
            secondLeaveRequest.FromDate = DateTime.UtcNow;
            secondLeaveRequest.RejoinDate = DateTime.UtcNow;
            secondLeaveRequest.Reason = _stringConstant.Reason;
            secondLeaveRequest.Status = Condition.Approved;
            secondLeaveRequest.Type = LeaveType.sl;
            thirdLeaveRequest.CreatedOn = DateTime.UtcNow;
            thirdLeaveRequest.EmployeeId = _stringConstant.StringIdForTest;
            thirdLeaveRequest.FromDate = DateTime.UtcNow;
            thirdLeaveRequest.Reason = _stringConstant.Reason;
            thirdLeaveRequest.Status = Condition.Approved;
            thirdLeaveRequest.Type = LeaveType.sl;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Method to add questions for leave management - SS
        /// </summary>
        private async Task AddQuestionAsync()
        {
            await _botQuestionRepository.AddQuestionAsync(firstQuestionLeaveManagement);
            await _botQuestionRepository.AddQuestionAsync(secondQuestionLeaveManagement);
            await _botQuestionRepository.AddQuestionAsync(thirdQuestionLeaveManagement);
            await _botQuestionRepository.AddQuestionAsync(fourthQuestionLeaveManagement);
            await _botQuestionRepository.AddQuestionAsync(fifthQuestionLeaveManagement);
            await _botQuestionRepository.AddQuestionAsync(sixthQuestionLeaveManagement);
        }

        /// <summary>
        /// Method to add user - SS
        /// </summary>
        private async Task AddUserAndMockAccessTokenReturnAsync()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUser);
            await _userManager.CreateAsync(user);
            UserLoginInfo userLoginInfo = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            var success = await _userManager.AddLoginAsync(user.Id, userLoginInfo);
            var result = Task.FromResult(_stringConstant.AccessTokenForTest);
            _serviceRepositoryMock.Setup(x => x.GerAccessTokenByRefreshToken(It.IsAny<string>())).Returns(result);
        }

        /// <summary>
        /// Method to get question of leave management by order number - SS
        /// </summary>
        /// <param name="orderNumber">order number</param>
        /// <returns>question details</returns>
        private async Task<Question> GetLeaveQuestionDetailsByOrderAsync(QuestionOrder orderNumber)
        {
            return await _botQuestionRepository.FindByTypeAndOrderNumberAsync((int)orderNumber, (int)BotQuestionType.LeaveManagement);
        }

        /// <summary>
        /// Method to add temporary leave detail - SS
        /// </summary>
        private async Task AddTemporaryLeaveDetailsAsync()
        {
            _temporaryLeaveRequestDetailDataRepository.Insert(temporaryLeaveDetail);
            await _temporaryLeaveRequestDetailDataRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Method to tranform leave list to string message - SS
        /// </summary>
        /// <param name="leaves">leave list</param>
        /// <returns>leave list in message format</returns>
        private string GetLeaveListMessageByLeaveList(List<LeaveRequest> leaves)
        {
            string replyText = string.Empty;
            foreach (var leave in leaves)
                if (leave.Type == LeaveType.cl)
                    replyText += string.Format(_stringConstant.ReplyTextForCasualLeaveList, leave.Id,
                                leave.Reason, leave.FromDate.ToShortDateString(),
                                leave.EndDate.Value.ToShortDateString(), leave.Status, Environment.NewLine);
                else
                {
                    if (leave.EndDate.HasValue)
                        replyText += string.Format(_stringConstant.ReplyTextForSickLeaveList, leave.Id, leave.Reason,
                            leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, Environment.NewLine);
                    else
                        replyText += string.Format(_stringConstant.ReplyTextForSickLeaveListWithoutEndDate, leave.Id, leave.Reason,
                            leave.FromDate.ToShortDateString(), leave.Status, Environment.NewLine);
                }
            return replyText;
        }

        /// <summary>
        /// Method to add employee details in user and slack user table - SS
        /// </summary>
        private async Task AddEmployeeDetailsAsync()
        {
            var employee = new User()
            {
                Email = _stringConstant.TeamLeaderEmailForTest,
                UserName = _stringConstant.TeamLeaderEmailForTest,
                SlackUserId = _stringConstant.TeamLeaderSlackId,
                Id = _stringConstant.TeamLeaderIdForTest
            };
            await _userManager.CreateAsync(_mapper.Map<User, ApplicationUser>(employee));
            await AddEmployeeSlackDetailAsync();
        }

        /// <summary>
        /// Method to add employee detail in slack user table - SS
        /// </summary>
        private async Task AddEmployeeSlackDetailAsync()
        {
            var employeeSlackUser = new SlackUserDetails()
            {
                CreatedOn = DateTime.UtcNow,
                UserId = _stringConstant.TeamLeaderSlackId,
                Name = _stringConstant.TeamLeader
            };
            await _slackUserRepository.AddSlackUserAsync(employeeSlackUser);
        }
        #endregion

        #region Mock
        /// <summary>
        /// Method to mock post get of http client - SS
        /// </summary>
        private void MockPostAsync()
        {
            var result = Task.FromResult(_stringConstant.Ok);
            _httpClientMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(result);
        }

        /// <summary>
        /// Method to mock GetUserByUserIdAsync of IOauthCallsRepository - SS
        /// </summary>
        private void MockGetUserDetails()
        {
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl,
                _stringConstant.StringIdForTest);
            var expectedUserDetail = new User()
            {
                Email = _stringConstant.EmailForTest,
                UserName = _stringConstant.EmailForTest,
                NumberOfCasualLeave = 14,
                NumberOfSickLeave = 7,
                FirstName = _stringConstant.NameForTest,
                SlackUserId = _stringConstant.SlackUserID,
                IsActive = true,
                Id = _stringConstant.StringIdForTest
            };
            var result = Task.FromResult(JsonConvert.SerializeObject(expectedUserDetail));
            _httpClientMock.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(result);
        }

        /// <summary>
        /// Mock of GetListOfProjectsEnrollmentOfUserByUserIdAsync of IOauthCallsRepository - SS
        /// </summary>
        private void MockingGetListOfProjectsEnrollmentOfUserByUserId()
        {
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl,
                _stringConstant.StringIdForTest);
            List<ProjectAc> projects = new List<ProjectAc>()
            {
                new ProjectAc()
                {
                    Id = 1,
                    Name = _stringConstant.ProjectDetail
                }
            };
            _httpClientMock.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(projects)));
        }

        /// <summary>
        /// Method to create team leader and mock of GetTeamLeaderUserIdAsync of IOauthCallsRepository - SS
        /// </summary>
        private async Task MockingGetTeamLeaderUserId()
        {
            var teamLeader = new User()
            {
                Email = _stringConstant.TeamLeaderEmailForTest,
                Id = _stringConstant.TeamLeaderIdForTest,
                UserName = _stringConstant.TeamLeaderEmailForTest,
                SlackUserId = _stringConstant.TeamLeaderSlackId
            };
            await _userManager.CreateAsync(_mapper.Map<User, ApplicationUser>(teamLeader));
            var teamLeaderSlackDetails = new SlackUserDetails()
            {
                Name = _stringConstant.NameForTest,
                UserId = _stringConstant.TeamLeaderSlackId,
                CreatedOn = DateTime.UtcNow
            };
            await _slackUserRepository.AddSlackUserAsync(teamLeaderSlackDetails);
            var teamLeaderIncomingWebHook = new IncomingWebHook()
            {
                IncomingWebHookUrl = _stringConstant.IncomingWebHookUrl,
                UserId = _stringConstant.TeamLeaderSlackId
            };
            _incomingWebHookRepository.Insert(teamLeaderIncomingWebHook);
            await _incomingWebHookRepository.SaveChangesAsync();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl,
                _stringConstant.StringIdForTest);
            List<User> teamLeaders = new List<User>();
            teamLeaders.Add(teamLeader);
            _httpClientMock.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(teamLeaders)));
        }

        /// <summary>
        /// Method to create team leader and mock of GetManagementUserNameAsync of IOauthCallsRepository - SS
        /// </summary>
        private async Task MockingGetManagementUserName()
        {
            var management = new User()
            {
                Email = _stringConstant.ManagementEmailForTest,
                Id = _stringConstant.ManagementIdForTest,
                UserName = _stringConstant.ManagementEmailForTest,
                SlackUserId = _stringConstant.ManagementSlackId
            };
            await _userManager.CreateAsync(_mapper.Map<User, ApplicationUser>(management));
            var managementSlackDetails = new SlackUserDetails()
            {
                Name = _stringConstant.NameForTest,
                UserId = _stringConstant.ManagementSlackId,
                CreatedOn = DateTime.UtcNow
            };
            await _slackUserRepository.AddSlackUserAsync(managementSlackDetails);
            var managementIncomingWebHook = new IncomingWebHook()
            {
                IncomingWebHookUrl = _stringConstant.IncomingWebHookUrl,
                UserId = _stringConstant.ManagementSlackId
            };
            _incomingWebHookRepository.Insert(managementIncomingWebHook);
            await _incomingWebHookRepository.SaveChangesAsync();
            List<User> managements = new List<User>();
            managements.Add(management);
            _httpClientMock.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, _stringConstant.ManagementDetailsUrl,
                _stringConstant.AccessTokenForTest, _stringConstant.Bearer)).Returns(Task.FromResult(JsonConvert.SerializeObject(managements)));
        }

        /// <summary>
        /// Method to mock UserIsAdminAsync of IOauthCallsRepository - SS
        /// </summary>
        private void MockingUserIsAdmin()
        {
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserIsAdmin, _stringConstant.StringIdForTest);
            _httpClientMock.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer))
                .Returns(Task.FromResult(_stringConstant.True));
        }

        /// <summary>
        /// Method to mock AllowedLeave of IOauthCallsRepository - SS
        /// </summary>
        private void MockingAllowedLeave()
        {
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.CasualLeaveUrl,
                _stringConstant.StringIdForTest);
            LeaveAllowed allowedLeave = new LeaveAllowed()
            {
                CasualLeave = 14,
                SickLeave = 7
            };
            _httpClientMock.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, _stringConstant.AccessTokenForTest, _stringConstant.Bearer))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(allowedLeave)));
        }
        #endregion
    }
}