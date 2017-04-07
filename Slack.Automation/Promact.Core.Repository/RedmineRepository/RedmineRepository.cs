using System;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.Redmine;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.Client;
using Newtonsoft.Json;
using System.Globalization;
using System.Collections.Generic;
using Promact.Core.Repository.AppCredentialRepository;

namespace Promact.Core.Repository.RedmineRepository
{
    public class RedmineRepository : IRedmineRepository
    {
        #region Private Variables
        private readonly IRepository<ApplicationUser> _userDataRepository;
        private readonly IHttpClientService _httpClientService;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IClient _clientRepository;
        private string replyText = null;
        private readonly IAppCredentialRepository _appCredentialRepository;
        #endregion

        #region Constructor
        public RedmineRepository(IRepository<ApplicationUser> userDataRepository, IHttpClientService httpClientService,
            IStringConstantRepository stringConstant, IAttachmentRepository attachmentRepository, IClient clientRepository,
            IAppCredentialRepository appCredentialRepository)
        {
            _userDataRepository = userDataRepository;
            _httpClientService = httpClientService;
            _stringConstant = stringConstant;
            _attachmentRepository = attachmentRepository;
            _clientRepository = clientRepository;
            _appCredentialRepository = appCredentialRepository;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Method to handle Redmine Slash command
        /// </summary>
        /// <param name="slashCommand">slash command</param>
        /// <returns>reply message</returns>
        public async Task SlackRequestAsync(SlashCommand slashCommand)
        {
            if ((await _appCredentialRepository.FetchAppCredentialByModuleAsync(_stringConstant.RedmineModule)).BotToken != null)
            {
                // Way to break string by spaces only if spaces are not between quotes
                var text = _attachmentRepository.SlackText(slashCommand.Text);
                // Get user details from SlackUserId
                var user = await _userDataRepository.FirstOrDefaultAsync(x => x.SlackUserId == slashCommand.UserId);
                if (user != null)
                {
                    SlackAction action;
                    if (SlackAction.TryParse(text[0], out action))
                    {
                        if (action != SlackAction.apikey)
                        {
                            if (!string.IsNullOrEmpty(user.RedmineApiKey))
                            {
                                switch (action)
                                {
                                    // To get redmine project list
                                    #region Project list
                                    case SlackAction.projects:
                                        await GetRedmineProjectList(user);
                                        break;
                                    #endregion

                                    // Redmine issue related
                                    #region Issues
                                    case SlackAction.issues:
                                        {
                                            RedmineAction redmineAction;
                                            if (RedmineAction.TryParse(text[1], out redmineAction))
                                            {
                                                switch (redmineAction)
                                                {
                                                    // To get redmine issue list assignee to me
                                                    #region Issues List
                                                    case RedmineAction.list:
                                                        await GetRedmineIssueList(user, text);
                                                        break;
                                                    #endregion

                                                    // To create redmine issue
                                                    #region Issue Create
                                                    case RedmineAction.create:
                                                        await CreateRedmineIssue(user, text);
                                                        break;
                                                    #endregion

                                                    // To change assignee in redmine issue
                                                    #region Change Assignee
                                                    case RedmineAction.changeassignee:
                                                        await UpdateChangeAssignee(user, text);
                                                        break;
                                                    #endregion

                                                    // To close the issue of redmine
                                                    #region Issue close
                                                    case RedmineAction.close:
                                                        await UpdateByPropertyAsync(true, 0, text[2], user.RedmineApiKey);
                                                        break;
                                                    #endregion

                                                    // To add time entry in redmine issue
                                                    #region Issue Time Entry
                                                    case RedmineAction.timeentry:
                                                        await AddTimeEntryToRedmineIssue(user, text);
                                                        break;
                                                        #endregion
                                                }
                                            }
                                            else
                                                // If command action is not in format
                                                replyText = string.Format(_stringConstant.ProperRedmineIssueAction, RedmineAction.list.ToString(),
                                                    RedmineAction.create.ToString(), RedmineAction.changeassignee.ToString(), RedmineAction.close.ToString(),
                                                    RedmineAction.timeentry.ToString());
                                        }
                                        break;
                                    #endregion

                                    // To get help in redmine slash command
                                    #region Help
                                    case SlackAction.help:
                                        replyText = _stringConstant.RedmineHelp;
                                        break;
                                        #endregion
                                }
                            }
                            else
                                // If user's redmine API key is not yet set
                                replyText = _stringConstant.RedmineApiKeyIsNull;
                        }
                        #region Api Key
                        else
                            await AddRedmineAPIKey(user, text);
                        #endregion
                    }
                    else
                        // If command action is not in format
                        replyText = string.Format(_stringConstant.RequestToEnterProperRedmineAction, SlackAction.list.ToString(),
                            SlackAction.projects.ToString(), SlackAction.help.ToString());
                }
                else
                    // If user not found
                    replyText = _stringConstant.SlackUserNotFound;
            }
            else
                replyText = _stringConstant.RequestToReInstallSlackApp;
            await _clientRepository.SendMessageAsync(slashCommand.ResponseUrl, replyText);
        }
        #endregion

        #region Private Region
        /// <summary>
        /// Method which try to convert string to Priority
        /// </summary>
        /// <param name="priority">priority in string</param>
        /// <param name="priorityId">priorityId</param>
        /// <returns>True or False</returns>
        private bool CheckPriority(string priority, out Priority priorityId)
        {
            bool valueConverted = false;
            valueConverted = Priority.TryParse(priority, out priorityId);
            if (!valueConverted)
                // If create command Priority is not in format
                replyText = string.Format(_stringConstant.RedminePriorityErrorMessage,
                    Priority.Low.ToString(), Priority.Normal.ToString(), Priority.High.ToString(),
                    Priority.Urgent.ToString(), Priority.Immediate.ToString());
            return valueConverted;
        }

        /// <summary>
        /// Method which try to convert string to Status
        /// </summary>
        /// <param name="status">status in string</param>
        /// <param name="statusId">statusId</param>
        /// <returns>True or False</returns>
        private bool CheckStatus(string status, out Status statusId)
        {
            bool valueConverted = Status.TryParse(status, out statusId);
            if (!valueConverted)
                // If create command Status is not in format
                replyText = string.Format(_stringConstant.RedmineStatusErrorMessage,
                    Status.New.ToString(), Status.InProgess.ToString(), Status.Confirmed.ToString(),
                    Status.Resolved.ToString(), Status.Hold.ToString(), Status.Feedback.ToString(),
                    Status.Closed.ToString(), Status.Rejected.ToString());
            return valueConverted;
        }

        /// <summary>
        /// Method which try to convert string to Tracker
        /// </summary>
        /// <param name="text">tracker in string</param>
        /// <param name="trackerId">trackerId</param>
        /// <returns>True or False</returns>
        private bool CheckTracker(string text, out Tracker trackerId)
        {
            bool valueConverted = false;
            valueConverted = Tracker.TryParse(text, out trackerId);
            if (!valueConverted)
                // If create command Tracker is not in format
                replyText = string.Format(_stringConstant.RedmineTrackerErrorMessage,
                    Tracker.Bug.ToString(), Tracker.Feature.ToString(), Tracker.Support.ToString(),
                    Tracker.Tasks.ToString());
            return valueConverted;
        }

        /// <summary>
        /// Method to send request to update the redmine issue
        /// </summary>
        /// <param name="issueId">issue Id</param>
        /// <param name="redmineApiKey">redmine api key</param>
        /// <param name="issue">issue details</param>
        private async Task UpdateIssueAsync(int issueId, string redmineApiKey, PostRedmineResponse issue)
        {
            var updateRequestUrl = string.Format(_stringConstant.RedmineIssueUpdateUrl,
                _stringConstant.RedmineBaseUrl, _stringConstant.IssueUrl, issueId);
            var issueInJsonText = JsonConvert.SerializeObject(issue);
            // To update issue in redmine
            var updateResult = await _httpClientService.PutAsync(updateRequestUrl, issueInJsonText,
                _stringConstant.JsonApplication, redmineApiKey, _stringConstant.RedmineApiKey);
            if (updateResult == null)
                replyText = _stringConstant.ErrorInUpdateIssue;
            else
                replyText = string.Format(_stringConstant.IssueSuccessfullUpdated, issueId);
        }

        /// <summary>
        /// Method to check the where issue exist or not to update if exist then exist
        /// </summary>
        /// <param name="isClose">update to close the issue</param>
        /// <param name="assignTo">update to assign the issue</param>
        /// <param name="issueStringId">issue id in string</param>
        /// <param name="userRedmineApiKey">redmine api key</param>
        private async Task UpdateByPropertyAsync(bool isClose, int assignTo, string issueStringId, string userRedmineApiKey)
        {
            int issueId;
            string result;
            if (ToCheckIssueExistOrNot(issueStringId, userRedmineApiKey, out issueId, out result))
            {
                var response = JsonConvert.DeserializeObject<RedmineResponseSingleProject>(result);
                var updateIssue = new PostRedmineResponse()
                {
                    Issue = new PostRedminIssue()
                    {
                        ProjectId = response.Issue.Project.Id,
                        PriorityId = (Priority)response.Issue.Priority.Id,
                        TrackerId = (Tracker)response.Issue.Tracker.Id,
                        StatusId = (Status)response.Issue.Status.Id,
                        Description = response.Issue.Description,
                        Subject = response.Issue.Subject
                    }
                };
                // If true update assigneeTo
                if (isClose)
                {
                    updateIssue.Issue.AssignTo = response.Issue.AssignTo.Id;
                    updateIssue.Issue.StatusId = Status.Closed;
                }
                // else close the issue
                else
                {
                    updateIssue.Issue.AssignTo = assignTo;
                    updateIssue.Issue.StatusId = (Status)response.Issue.Status.Id;

                }
                // To update issue in redmine
                await UpdateIssueAsync(issueId, userRedmineApiKey, updateIssue);
            }
        }

        /// <summary>
        /// Method to check whether redmine issue is exist or not
        /// </summary>
        /// <param name="issueStringId">issue Id in string</param>
        /// <param name="userRedmineApiKey">user's redmine api key</param>
        /// <param name="issueId">issue Id</param>
        /// <param name="response">response from redmine server</param>
        /// <returns>true or false</returns>
        private bool ToCheckIssueExistOrNot(string issueStringId, string userRedmineApiKey, out int issueId, out string response)
        {
            response = string.Empty;
            if (int.TryParse(issueStringId, out issueId))
            {
                var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, issueId);
                response = _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl, requestUrl, userRedmineApiKey, _stringConstant.RedmineApiKey).Result;
                if (string.IsNullOrEmpty(response))
                {
                    // If issue not found in redmine
                    replyText = string.Format(_stringConstant.IssueDoesNotExist, issueId);
                    return false;
                }
                return true;
            }
            else
                // If project Id is not proper
                replyText = _stringConstant.ProperProjectId;
            return false;
        }

        /// <summary>
        /// Method to get list of user in a particular project
        /// </summary>
        /// <param name="name">name of user</param>
        /// <param name="projectId">project Id</param>
        /// <param name="redmineApiKey">redmine api key</param>
        /// <returns>user Id</returns>
        private async Task<int> GetUserRedmineIdByNameAsync(string name, int projectId, string redmineApiKey)
        {
            int userId = 0;
            var requestUrl = string.Format(_stringConstant.UserByProjectIdUrl, projectId);
            // To get redmine user details
            var response = await _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl, requestUrl, redmineApiKey, _stringConstant.RedmineApiKey);
            if (!string.IsNullOrEmpty(response))
            {
                var users = JsonConvert.DeserializeObject<RedmineUserResponse>(response);
                foreach (var user in users.Members)
                {
                    if (user.User.Name == name)
                    {
                        userId = user.User.Id;
                        return userId;
                    }
                }
            }
            return userId;
        }

        /// <summary>
        /// Method to add redmine api key in database
        /// </summary>
        /// <param name="user">user details</param>
        /// <param name="text">list of string containing parameter to add redmine api key</param>
        private async Task AddRedmineAPIKey(ApplicationUser user, List<string> text)
        {
            var response = await _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl,
                _stringConstant.RedmineIssueUrl, text[1], _stringConstant.RedmineApiKey);
            if (!string.IsNullOrEmpty(response))
            {
                // If gets response from redmine then will add else not
                user.RedmineApiKey = text[1];
                _userDataRepository.Update(user);
                await _userDataRepository.SaveChangesAsync();
                replyText = _stringConstant.RedmineKeyAddSuccessfully;
            }
            else
                // If get nothing in response then invalid api key error
                replyText = _stringConstant.PleaseEnterValidAPIKey;
        }

        /// <summary>
        /// Method to get list of project list for me
        /// </summary>
        /// <param name="user">user details</param>
        private async Task GetRedmineProjectList(ApplicationUser user)
        {
            var result = await _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl,
                _stringConstant.RedmineProjectListAssignToMeUrl, user.RedmineApiKey, _stringConstant.RedmineApiKey);
            if (!string.IsNullOrEmpty(result))
            {
                var projectList = JsonConvert.DeserializeObject<GetRedmineProjectsResponse>(result);
                foreach (var project in projectList.Projects)
                {
                    // Project list in string format
                    replyText += string.Format(_stringConstant.RedmineProjectListFormat, project.Id,
                        project.Name, Environment.NewLine);
                }
            }
            else
                replyText = _stringConstant.NoProjectFoundForUser;
        }

        /// <summary>
        /// Method to get list of redmine issue assignee to me
        /// </summary>
        /// <param name="user">user details</param>
        /// <param name="text">list of string containing parameter to get issue list</param>
        private async Task GetRedmineIssueList(ApplicationUser user, List<string> text)
        {
            int projectId;
            if (int.TryParse(text[2], out projectId))
            {
                var requestUrl = string.Format(_stringConstant.RedmineIssueListAssignToMeByProjectIdUrl, projectId);
                var result = await _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl, requestUrl, user.RedmineApiKey, _stringConstant.RedmineApiKey);
                if (!string.IsNullOrEmpty(result))
                {
                    var issues = JsonConvert.DeserializeObject<GetRedmineResponse>(result);
                    foreach (var issue in issues.Issues)
                    {
                        // Issue list in string format
                        replyText += string.Format(_stringConstant.RedmineIssueMessageFormat, issue.Project.Name, issue.IssueId,
                            issue.Subject, issue.Status.Name, issue.Priority.Name, issue.Tracker.Name, Environment.NewLine);
                    }
                }
                else
                    // If project not found in redmine
                    replyText = string.Format(_stringConstant.ProjectDoesNotExistForThisId, projectId);
            }
            else
                // If project Id is not valid, i.e., not a integer
                replyText = _stringConstant.ProperProjectId;
        }

        /// <summary>
        /// Method to create red mine issue
        /// </summary>
        /// <param name="user">user details</param>
        /// <param name="text">list of string containing parameter to create issue</param>
        private async Task CreateRedmineIssue(ApplicationUser user, List<string> text)
        {
            Priority priorityId;
            Status statusId;
            Tracker trackerId;
            if (CheckPriority(text[5], out priorityId) && CheckStatus(text[6], out statusId) && CheckTracker(text[7], out trackerId))
            {
                int projectId;
                if (int.TryParse(text[2], out projectId))
                {
                    // To get user Id
                    var redmineUserId = await GetUserRedmineIdByNameAsync(text[8], projectId, user.RedmineApiKey);
                    if (redmineUserId != 0)
                    {
                        var issue = new PostRedmineResponse()
                        {
                            Issue = new PostRedminIssue()
                            {
                                ProjectId = projectId,
                                PriorityId = priorityId,
                                TrackerId = trackerId,
                                StatusId = statusId,
                                Subject = text[3],
                                Description = text[4],
                                AssignTo = redmineUserId
                            }
                        };
                        var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat,
                            _stringConstant.RedmineBaseUrl, _stringConstant.RedmineIssueUrl);
                        var issueInJsonText = JsonConvert.SerializeObject(issue);
                        // Post call to create issue in redmine
                        var result = await _httpClientService.PostAsync(requestUrl, issueInJsonText,
                            _stringConstant.JsonApplication, user.RedmineApiKey, _stringConstant.RedmineApiKey);
                        if (string.IsNullOrEmpty(result))
                            // If issue is not created in redmine
                            replyText = _stringConstant.ErrorInCreatingIssue;
                        else
                        {
                            // If issue is created on redmine
                            var createdIssue = JsonConvert.DeserializeObject<RedmineResponseSingleProject>(result);
                            replyText = string.Format(_stringConstant.IssueSuccessfullyCreatedMessage, createdIssue.Issue.IssueId);
                        }
                    }
                    else
                        // If user not found in redmine
                        replyText = string.Format(_stringConstant.NoUserFoundInProject, text[8], projectId);
                }
                else
                    // If project Id is not valid, i.e., not a integer
                    replyText = _stringConstant.ProperProjectId;
            }
        }

        /// <summary>
        /// Method to change assignee in redmine issue
        /// </summary>
        /// <param name="user">user details</param>
        /// <param name="text">list of string containing parameter to update assignee</param>
        private async Task UpdateChangeAssignee(ApplicationUser user, List<string> text)
        {
            var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, text[2]);
            var response = await _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl, requestUrl, user.RedmineApiKey, _stringConstant.RedmineApiKey);
            if (!string.IsNullOrEmpty(response))
            {
                var issue = JsonConvert.DeserializeObject<RedmineResponseSingleProject>(response);
                // To get user Id from redmine
                var redmineUserId = await GetUserRedmineIdByNameAsync(text[3], issue.Issue.Project.Id, user.RedmineApiKey);
                if (redmineUserId != 0)
                    await UpdateByPropertyAsync(false, redmineUserId, text[2], user.RedmineApiKey);
                else
                    // If user not found in redmine
                    replyText = string.Format(_stringConstant.NoUserFoundInProject, text[3], issue.Issue.Project.Id);
            }
            else
                // If issue doesn't exist in redmine
                replyText = string.Format(_stringConstant.IssueDoesNotExist, text[2]);
        }

        /// <summary>
        /// Method to add time entry in redmine issues
        /// </summary>
        /// <param name="user">user details</param>
        /// <param name="text">list of string containing parameter to add time entry</param>
        private async Task AddTimeEntryToRedmineIssue(ApplicationUser user, List<string> text)
        {
            int issueId;
            string result;
            if (ToCheckIssueExistOrNot(text[2], user.RedmineApiKey, out issueId, out result))
            {
                double hour;
                var hourConvertor = double.TryParse(text[3], out hour);
                if (hourConvertor)
                {
                    DateTime date;
                    if (DateTime.TryParseExact(text[4], _stringConstant.RedmineTimeEntryDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        TimeEntryActivity timeEntryActivity;
                        if (TimeEntryActivity.TryParse(text[5], out timeEntryActivity))
                        {
                            var timeEntry = new RedmineTimeEntryApplicationClass()
                            {
                                TimeEntry = new RedmineTimeEntries()
                                {
                                    ActivityId = timeEntryActivity,
                                    IssueId = issueId,
                                    Date = date.ToString(_stringConstant.RedmineTimeEntryDateFormat),
                                    Hours = hour
                                }
                            };
                            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat,
                                _stringConstant.RedmineBaseUrl, _stringConstant.TimeEntryUrl);
                            var jsonText = JsonConvert.SerializeObject(timeEntry);
                            // Post call to add time entry
                            var response = await _httpClientService.PostAsync(requestUrl, jsonText,
                                _stringConstant.JsonApplication, user.RedmineApiKey, _stringConstant.RedmineApiKey);
                            if (!string.IsNullOrEmpty(response))
                                // If added time entry in redmine issue
                                replyText = string.Format(_stringConstant.TimeEnrtyAddSuccessfully, issueId);
                            else
                                // If error in adding time entry in redmine issue
                                replyText = string.Format(_stringConstant.ErrorInAddingTimeEntry, issueId);
                        }
                        else
                            // If TimeEntryActivity is not valid
                            replyText = string.Format(_stringConstant.TimeEntryActivityErrorMessage, TimeEntryActivity.Analysis.ToString(),
                                TimeEntryActivity.Design.ToString(), TimeEntryActivity.Development.ToString(), TimeEntryActivity.Roadblock.ToString(),
                                TimeEntryActivity.Testing.ToString());
                    }
                    else
                        // If date format is not valid
                        replyText = string.Format(_stringConstant.DateFormatErrorMessage, _stringConstant.RedmineTimeEntryDateFormat);
                }
                else
                    // If hour is not in numeric
                    replyText = _stringConstant.HourIsNotNumericMessage;
            }
        }
        #endregion
    }
}
