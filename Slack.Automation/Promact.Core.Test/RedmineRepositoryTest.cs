using Autofac;
using Moq;
using Newtonsoft.Json;
using Promact.Core.Repository.RedmineRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.Redmine;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class RedmineRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IRedmineRepository _redmineRepository;
        private readonly ApplicationUserManager _userManager;
        private readonly Mock<IHttpClientService> _mockHttpClient;
        private SlashCommand slashCommand = new SlashCommand();
        private GetRedmineProjectsResponse project = new GetRedmineProjectsResponse();
        private GetRedmineResponse issueResponse = new GetRedmineResponse();
        private RedmineBase redmineBase = new RedmineBase();
        private RedmineUserResponse user = new RedmineUserResponse();
        private RedmineResponseSingleProject singleIssue = new RedmineResponseSingleProject();
        private PostRedmineResponse redmineIssue = new PostRedmineResponse();
        private RedmineTimeEntryApplicationClass timeEntry = new RedmineTimeEntryApplicationClass();
        private readonly IRepository<AppCredential> _appCredentialDataRepository;
        private AppCredential appCredential = new AppCredential();
        #endregion

        #region Constructor
        public RedmineRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _redmineRepository = _componentContext.Resolve<IRedmineRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _appCredentialDataRepository = _componentContext.Resolve<IRepository<AppCredential>>();
            Initialize();
            AddRedmineAppAsync().Wait();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test case for User Not Found
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestUserNotFoundAsync()
        {
            slashCommand.Text = _stringConstant.Text;
            var replyText = _stringConstant.SlackUserNotFound;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Wrong command
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestWrongActionAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineWrongActionCommand;
            var replyText = string.Format(_stringConstant.RequestToEnterProperRedmineAction, SlackAction.list.ToString(),
                        SlackAction.projects.ToString(), SlackAction.help.ToString());
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for No Project Found
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestNoProjectFoundAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandProjectList;
            var replyText = _stringConstant.NoProjectFoundForUser;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Project Found
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestProjectFoundAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandProjectList;
            var replyText = MessageForProject(project.Projects[0]);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            GetAsyncMethodMocking(GetRedmineProjectsResponseInJson(), _stringConstant.RedmineBaseUrl,
                _stringConstant.RedmineProjectListAssignToMeUrl, _stringConstant.AccessTokenForTest);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Wrong Issue Action
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestWrongIssueActionAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineWrongIssueActionCommand;
            var replyText = string.Format(_stringConstant.ProperRedmineIssueAction, RedmineAction.list.ToString(),
                                        RedmineAction.create.ToString(), RedmineAction.changeassignee.ToString(), RedmineAction.close.ToString(),
                                        RedmineAction.timeentry.ToString());
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue List Wrong Project Id
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueListWrongProjectIdAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandIssueListWrongProjectId;
            var replyText = _stringConstant.ProperProjectId;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue List Project Not Found
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueListProjectNotFoundAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandIssueList;
            var replyText = string.Format(_stringConstant.ProjectDoesNotExistForThisId, 1);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue List Project Found
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueListProjectFoundAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandIssueList;
            var replyText = MessageForIssue(issueResponse.Issues[0]);
            var requestUrl = string.Format(_stringConstant.RedmineIssueListAssignToMeByProjectIdUrl, 1);
            GetAsyncMethodMocking(GetRedmineResponseInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Create Wrong Priority
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueCreateWrongPriorityAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandCreateWrongPriority;
            var replyText = string.Format(_stringConstant.RedminePriorityErrorMessage,
                    Priority.Low.ToString(), Priority.Normal.ToString(), Priority.High.ToString(),
                    Priority.Urgent.ToString(), Priority.Immediate.ToString());
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Create Wrong Status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueCreateWrongStatusAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandCreateWrongStatus;
            var replyText = string.Format(_stringConstant.RedmineStatusErrorMessage,
                    Status.New.ToString(), Status.InProgess.ToString(), Status.Confirmed.ToString(),
                    Status.Resolved.ToString(), Status.Hold.ToString(), Status.Feedback.ToString(),
                    Status.Closed.ToString(), Status.Rejected.ToString());
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Create Wrong Tracker
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueCreateWrongTrackerAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandCreateWrongTracker;
            var replyText = string.Format(_stringConstant.RedmineTrackerErrorMessage,
                    Tracker.Bug.ToString(), Tracker.Feature.ToString(), Tracker.Support.ToString(),
                    Tracker.Tasks.ToString());
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Create Wrong Project Id
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueCreateWrongProjectIdAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandCreateWrongProjectId;
            var replyText = _stringConstant.ProperProjectId;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Create Assign user not found
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueCreateWrongAssignUserNotFoundAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandCreate;
            var replyText = string.Format(_stringConstant.NoUserFoundInProject, _stringConstant.FirstNameForTest, 1);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Create Error
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueCreateErrorAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandCreate;
            var requestUrl = string.Format(_stringConstant.UserByProjectIdUrl, 1);
            GetAsyncMethodMocking(RedmineUserResponseInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var replyText = _stringConstant.ErrorInCreatingIssue;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Create
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueCreateAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandCreate;
            var requestUrl = string.Format(_stringConstant.UserByProjectIdUrl, 1);
            GetAsyncMethodMocking(RedmineUserResponseInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var postRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.RedmineBaseUrl, _stringConstant.RedmineIssueUrl);
            var issueInJsonText = JsonConvert.SerializeObject(redmineIssue);
            _mockHttpClient.Setup(x => x.PostAsync(postRequestUrl, issueInJsonText, _stringConstant.JsonApplication, _stringConstant.AccessTokenForTest, _stringConstant.RedmineApiKey)).Returns(Task.FromResult(RedmineResponseSingleProjectInJson()));
            var replyText = string.Format(_stringConstant.IssueSuccessfullyCreatedMessage, singleIssue.Issue.IssueId);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Change Assign no issue
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueChangeAssignNoIssueAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandChangeAssign;
            var replyText = string.Format(_stringConstant.IssueDoesNotExist, 1);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Change Assign
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueChangeAssignAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandChangeAssign;
            var replyText = string.Format(_stringConstant.IssueSuccessfullUpdated, 1);
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, 1);
            GetAsyncMethodMocking(RedmineResponseSingleProjectInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            requestUrl = string.Format(_stringConstant.UserByProjectIdUrl, 1); ;
            GetAsyncMethodMocking(RedmineUserResponseInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var postRequestUrl = string.Format(_stringConstant.RedmineIssueUpdateUrl,_stringConstant.RedmineBaseUrl, _stringConstant.IssueUrl, 1);
            redmineIssue.Issue.TrackerId = Tracker.Bug;
            redmineIssue.Issue.PriorityId = Priority.Normal;
            var issueInJsonText = JsonConvert.SerializeObject(redmineIssue);
            _mockHttpClient.Setup(x => x.PutAsync(postRequestUrl, issueInJsonText, _stringConstant.JsonApplication, _stringConstant.AccessTokenForTest, _stringConstant.RedmineApiKey)).Returns(Task.FromResult(_stringConstant.Ok));
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Change Assign for user not found
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueChangeAssignUserNotFouundAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandChangeAssign;
            var replyText = string.Format(_stringConstant.NoUserFoundInProject, _stringConstant.FirstNameForTest, 1);
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, 1);
            GetAsyncMethodMocking(RedmineResponseSingleProjectInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var postRequestUrl = string.Format(_stringConstant.RedmineIssueUpdateUrl, _stringConstant.RedmineBaseUrl, _stringConstant.IssueUrl, 1);
            redmineIssue.Issue.TrackerId = Tracker.Bug;
            redmineIssue.Issue.PriorityId = Priority.Normal;
            var issueInJsonText = JsonConvert.SerializeObject(redmineIssue);
            _mockHttpClient.Setup(x => x.PutAsync(postRequestUrl, issueInJsonText, _stringConstant.JsonApplication, _stringConstant.AccessTokenForTest, _stringConstant.RedmineApiKey)).Returns(Task.FromResult(_stringConstant.Ok));
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Change Assign No updated
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueChangeAssignNoUpdatedAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandChangeAssign;
            var replyText = _stringConstant.ErrorInUpdateIssue;
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, 1);
            GetAsyncMethodMocking(RedmineResponseSingleProjectInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            requestUrl = string.Format(_stringConstant.UserByProjectIdUrl, 1); ;
            GetAsyncMethodMocking(RedmineUserResponseInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var postRequestUrl = string.Format(_stringConstant.RedmineIssueUpdateUrl, _stringConstant.RedmineBaseUrl, _stringConstant.IssueUrl, 1);
            redmineIssue.Issue.TrackerId = Tracker.Bug;
            redmineIssue.Issue.PriorityId = Priority.Normal;
            var issueInJsonText = JsonConvert.SerializeObject(redmineIssue);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Close
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueCloseAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandIssueClose;
            var replyText = string.Format(_stringConstant.IssueSuccessfullUpdated, 1);
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, 1);
            GetAsyncMethodMocking(RedmineResponseSingleProjectInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            requestUrl = string.Format(_stringConstant.UserByProjectIdUrl, 1); ;
            GetAsyncMethodMocking(RedmineUserResponseInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var postRequestUrl = string.Format(_stringConstant.RedmineIssueUpdateUrl, _stringConstant.RedmineBaseUrl, _stringConstant.IssueUrl, 1);
            redmineIssue.Issue.TrackerId = Tracker.Bug;
            redmineIssue.Issue.PriorityId = Priority.Normal;
            redmineIssue.Issue.StatusId = Status.Closed;
            var issueInJsonText = JsonConvert.SerializeObject(redmineIssue);
            _mockHttpClient.Setup(x => x.PutAsync(postRequestUrl, issueInJsonText, _stringConstant.JsonApplication, _stringConstant.AccessTokenForTest, _stringConstant.RedmineApiKey)).Returns(Task.FromResult(_stringConstant.Ok));
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Time Entry Wrong data type Issue Id
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueTimeEntryWrongTypeIssueIdAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineTimeEntryWrongIssueId;
            var replyText = _stringConstant.ProperProjectId;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Time Entry server response null
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueTimeEntryServerResponseNullAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineTimeEntry;
            var replyText = string.Format(_stringConstant.IssueDoesNotExist, 1);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Time Entry hour format issue
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueTimeEntryHourFormatIssueAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineTimeEntryHourFormatCommand;
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, 1);
            GetAsyncMethodMocking(RedmineResponseSingleProjectInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var replyText = _stringConstant.HourIsNotNumericMessage;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Time Entry date format issue
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueTimeEntryDateFormatIssueAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineTimeEntryDateFormatCommand;
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, 1);
            GetAsyncMethodMocking(RedmineResponseSingleProjectInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var replyText = string.Format(_stringConstant.DateFormatErrorMessage, _stringConstant.RedmineTimeEntryDateFormat);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Time Entry TimeEntryActivity format issue
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueTimeEntryTimeEntryActivityFormatIssueAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineTimeEntryTimeEntryActivityFormatCommand;
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, 1);
            GetAsyncMethodMocking(RedmineResponseSingleProjectInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var replyText = string.Format(_stringConstant.TimeEntryActivityErrorMessage, TimeEntryActivity.Analysis.ToString(),
                TimeEntryActivity.Design.ToString(), TimeEntryActivity.Development.ToString(), TimeEntryActivity.Roadblock.ToString(),
                TimeEntryActivity.Testing.ToString());
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Time Entry no added
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueTimeEntryNoAddedAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineTimeEntry;
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, 1);
            GetAsyncMethodMocking(RedmineResponseSingleProjectInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var replyText = string.Format(_stringConstant.ErrorInAddingTimeEntry, 1);
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Time Entry
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestIssueTimeEntryAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineTimeEntry;
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, 1);
            GetAsyncMethodMocking(RedmineResponseSingleProjectInJson(), _stringConstant.RedmineBaseUrl, requestUrl, _stringConstant.AccessTokenForTest);
            var replyText = string.Format(_stringConstant.TimeEnrtyAddSuccessfully, 1);
            var postRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat,
                _stringConstant.RedmineBaseUrl, _stringConstant.TimeEntryUrl);
            var jsonText = JsonConvert.SerializeObject(timeEntry);
            _mockHttpClient.Setup(x => x.PostAsync(postRequestUrl, jsonText, _stringConstant.JsonApplication,
                _stringConstant.AccessTokenForTest, _stringConstant.RedmineApiKey)).Returns(Task.FromResult(RedmineTimeEntryApplicationClassInJson()));
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for Issue Change Assign no issue
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestHelpAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineCommandHelp;
            var replyText = _stringConstant.RedmineHelp;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for redmine api key not found
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestRedmineKeyNullAsync()
        {
            var user = new ApplicationUser()
            {
                Id = _stringConstant.StringIdForTest,
                UserName = _stringConstant.EmailForTest,
                Email = _stringConstant.EmailForTest,
                SlackUserId = _stringConstant.UserSlackId,
            };
            await _userManager.CreateAsync(user);
            slashCommand.Text = _stringConstant.RedmineCommandHelp;
            var replyText = _stringConstant.RedmineApiKeyIsNull;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for redmine api key in-valid
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestAPIKeyInValidAsync()
        {
            await CreateUserAsync();
            slashCommand.Text = _stringConstant.RedmineAPIKeyCommand;
            var replyText = _stringConstant.PleaseEnterValidAPIKey;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for redmine api key
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RedmineSlackRequestAPIKeyAsync()
        {
            await CreateUserAsync();
            GetAsyncMethodMocking(_stringConstant.Ok, _stringConstant.RedmineBaseUrl,_stringConstant.RedmineIssueUrl, 
                _stringConstant.AccessTokenForTest);
            slashCommand.Text = _stringConstant.RedmineAPIKeyCommand;
            var replyText = _stringConstant.RedmineKeyAddSuccessfully;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }

        /// <summary>
        /// Test case for RequestToReInstallSlackApp
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task RequestToReInstallSlackAppAsync()
        {
            await CreateUserAsync();
            GetAsyncMethodMocking(_stringConstant.Ok, _stringConstant.RedmineBaseUrl, _stringConstant.RedmineIssueUrl,
                _stringConstant.AccessTokenForTest);
            slashCommand.Text = _stringConstant.RedmineAPIKeyCommand;
            var replyText = _stringConstant.RequestToReInstallSlackApp;
            var slashResponseJsonText = MockingSendMessageAsync(replyText);
            appCredential.BotToken = null;
            _appCredentialDataRepository.Update(appCredential);
            await _appCredentialDataRepository.SaveChangesAsync();
            await _redmineRepository.SlackRequestAsync(slashCommand);
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString, null, null), Times.Once);
        }
        #endregion

        #region Initialization
        public void Initialize()
        {
            slashCommand.UserId = _stringConstant.UserSlackId;
            project.Projects = new List<RedmineProject>();
            project.IssueCount = 25;
            project.Limit = 25;
            project.Projects.Add(new RedmineProject()
            {
                Id = 1,
                CreateOn = DateTime.UtcNow,
                Description = _stringConstant.Admin,
                Indentifier = _stringConstant.Admin,
                Name = _stringConstant.NameForTest,
                Status = (int)Status.InProgess,
                UpdatedOn = DateTime.UtcNow
            });
            redmineBase.Id = 1;
            redmineBase.Name = _stringConstant.FirstNameForTest;
            issueResponse.IssueCount = 25;
            issueResponse.Limit = 25;
            issueResponse.Issues = new List<GetRedmineIssue>();
            issueResponse.Issues.Add(new GetRedmineIssue()
            {
                AssignTo = redmineBase,
                Description = _stringConstant.Admin,
                IssueId = 123,
                Priority = new RedmineBase() { Id = 4, Name = Priority.Normal.ToString() },
                Project = redmineBase,
                Status = redmineBase,
                Subject = _stringConstant.Admin,
                Tracker = redmineBase
            });
            user.Members = new List<RedmineUser>();
            user.Members.Add(new RedmineUser()
            {
                Id = 1,
                Project = redmineBase,
                Roles = new List<RedmineBase>(),
                User = redmineBase
            });
            singleIssue.Issue = new GetRedmineIssue()
            {
                AssignTo = redmineBase,
                Description = _stringConstant.Admin,
                IssueId = 123,
                Priority = new RedmineBase() {Id = 4, Name = Priority.Normal.ToString() },
                Project = redmineBase,
                Status = redmineBase,
                Subject = _stringConstant.Admin,
                Tracker = redmineBase
            };
            redmineIssue.Issue = new PostRedminIssue()
            {
                AssignTo = 1,
                Description = _stringConstant.Admin,
                StatusId = Status.New,
                PriorityId = Priority.Normal,
                ProjectId = 1,
                Subject = _stringConstant.Admin,
                TrackerId = Tracker.Feature
            };
            timeEntry.TimeEntry = new RedmineTimeEntries()
            {
                ActivityId = TimeEntryActivity.Development,
                Date = DateTime.UtcNow.ToString(_stringConstant.RedmineTimeEntryDateFormat),
                Hours = 2.0,
                IssueId = 1
            };
            appCredential.BotToken = _stringConstant.AccessTokenForTest;
            appCredential.BotUserId = _stringConstant.IdForTest;
            appCredential.ClientId = _stringConstant.TestSlackClientId;
            appCredential.ClientSecret = _stringConstant.TestSlackClientSecret;
            appCredential.CreatedOn = DateTime.UtcNow;
            appCredential.Module = _stringConstant.RedmineModule;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Private method to create a user add login info and mocking of Identity and return access token
        /// </summary>
        private async Task CreateUserAsync()
        {
            var user = new ApplicationUser()
            {
                Id = _stringConstant.StringIdForTest,
                UserName = _stringConstant.EmailForTest,
                Email = _stringConstant.EmailForTest,
                SlackUserId = _stringConstant.UserSlackId,
                RedmineApiKey = _stringConstant.AccessTokenForTest
            };
            await _userManager.CreateAsync(user);
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
            _mockHttpClient.Setup(x => x.PostAsync(baseUrl, contentString, contentHeader, _stringConstant.AccessTokenForTest, _stringConstant.RedmineApiKey)).Returns(responseString);
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
            _mockHttpClient.Setup(x => x.GetAsync(baseUrl, contentUrl, accessToken, _stringConstant.RedmineApiKey)).Returns(responseString);
        }

        /// <summary>
        /// Method to setup for Send Message of client
        /// </summary>
        /// <param name="replyText">reply text</param>
        /// <returns>json string</returns>
        private string MockingSendMessageAsync(string replyText)
        {
            var slashResponseText = new SlashResponse() { ResponseType = _stringConstant.ResponseTypeEphemeral, Text = replyText };
            var slashResponseJsonText = JsonConvert.SerializeObject(slashResponseText);
            PostAsyncMethodMocking(It.IsAny<string>(), slashResponseJsonText, _stringConstant.JsonContentString);
            return slashResponseJsonText;
        }

        /// <summary>
        /// Method to generate json text for project of redmine
        /// </summary>
        /// <returns>json string containing project details</returns>
        private string GetRedmineProjectsResponseInJson()
        {
            return JsonConvert.SerializeObject(project);
        }

        /// <summary>
        /// Method to create message for redmine issue
        /// </summary>
        /// <param name="issue">redmine issue</param>
        /// <returns>string message</returns>
        private string MessageForIssue(GetRedmineIssue issue)
        {
            return string.Format(_stringConstant.RedmineIssueMessageFormat, issue.Project.Name, issue.IssueId,
                issue.Subject, issue.Status.Name, issue.Priority.Name, issue.Tracker.Name, Environment.NewLine);
        }

        /// <summary>
        /// Method to get string project details from project object
        /// </summary>
        /// <param name="project">project details</param>
        /// <returns>project detail string</returns>
        private string MessageForProject(RedmineProject project)
        {
            return string.Format("{0}. Project - {1},{2}", project.Id, project.Name, Environment.NewLine);
        }

        /// <summary>
        /// Method to generate json text for issue of redmine
        /// </summary>
        /// <returns>json string containing issue details</returns>
        private string GetRedmineResponseInJson()
        {
            return JsonConvert.SerializeObject(issueResponse);
        }

        /// <summary>
        /// Method to generate json text for user of redmine
        /// </summary>
        /// <returns>json string containing issue details</returns>
        private string RedmineUserResponseInJson()
        {
            return JsonConvert.SerializeObject(user);
        }

        /// <summary>
        /// Method to generate json text for issue of redmine
        /// </summary>
        /// <returns>json string containing issue details</returns>
        private string RedmineResponseSingleProjectInJson()
        {
            return JsonConvert.SerializeObject(singleIssue);
        }

        /// <summary>
        /// Method to generate json text for time entry of redmine
        /// </summary>
        /// <returns>json string containing time entry details</returns>
        private string RedmineTimeEntryApplicationClassInJson()
        {
            return JsonConvert.SerializeObject(timeEntry);
        }

        /// <summary>
        /// Method to add redmine app
        /// </summary>
        private async Task AddRedmineAppAsync()
        {
            _appCredentialDataRepository.Insert(appCredential);
            await _appCredentialDataRepository.SaveChangesAsync();
        }
        #endregion
    }
}
