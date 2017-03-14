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
using System.Threading;
using System.Globalization;

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
        #endregion

        #region Constructor
        public RedmineRepository(IRepository<ApplicationUser> userDataRepository, IHttpClientService httpClientService,
            IStringConstantRepository stringConstant, IAttachmentRepository attachmentRepository, IClient clientRepository)
        {
            _userDataRepository = userDataRepository;
            _httpClientService = httpClientService;
            _stringConstant = stringConstant;
            _attachmentRepository = attachmentRepository;
            _clientRepository = clientRepository;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Method to handle Redmine Slash command
        /// </summary>
        /// <param name="slashCommand">slash command</param>
        /// <returns>reply message</returns>
        public async Task SlackRequest(SlashCommand slashCommand)
        {
            var text = _attachmentRepository.SlackText(slashCommand.Text.ToLower());
            var user = await _userDataRepository.FirstOrDefaultAsync(x => x.SlackUserId == slashCommand.UserId);
            if (user != null)
            {
                SlackAction action;
                var actionConvertorResult = SlackAction.TryParse(text[0], out action);
                if (actionConvertorResult)
                {
                    switch (action)
                    {
                        #region Project list
                        case SlackAction.projects:
                            {
                                ;
                                var result = await _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl,
                                    _stringConstant.RedmineProjectListAssignToMeUrl,
                                    user.RedmineApiKey, _stringConstant.RedmineApiKey);
                                if (!string.IsNullOrEmpty(result))
                                {
                                    var projectList = JsonConvert.DeserializeObject<GetRedmineProjectsResponse>(result);
                                    foreach (var project in projectList.Projects)
                                    {
                                        replyText += MessageForProject(project);
                                    }
                                }
                                else
                                    replyText = _stringConstant.NoProjectFoundForUser;
                            }
                            break;
                        #endregion

                        #region Issues
                        case SlackAction.issues:
                            {
                                RedmineAction redmineAction;
                                var subActionConvertor = RedmineAction.TryParse(text[1], out redmineAction);
                                if (subActionConvertor)
                                {
                                    switch (redmineAction)
                                    {
                                        #region Issues List
                                        case RedmineAction.list:
                                            {
                                                int projectId;
                                                var projectConvertor = StringToInt(text[2], out projectId);
                                                if (projectConvertor)
                                                {
                                                    var requestUrl = string.Format(_stringConstant.RedmineIssueListAssignToMeByProjectIdUrl, projectId);
                                                    var result = await _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl, requestUrl, user.RedmineApiKey, _stringConstant.RedmineApiKey);
                                                    if (!string.IsNullOrEmpty(result))
                                                    {
                                                        var issues = JsonConvert.DeserializeObject<GetRedmineResponse>(result);
                                                        foreach (var issue in issues.Issues)
                                                        {
                                                            replyText += MessageForIssue(issue);
                                                        }
                                                    }
                                                    else
                                                        replyText = string.Format(_stringConstant.ProjectDoesNotExistForThisId, projectId);
                                                }
                                                else
                                                    replyText = _stringConstant.ProperProjectId;
                                            }
                                            break;
                                        #endregion

                                        #region Issue Create
                                        case RedmineAction.create:
                                            {
                                                Priority priorityId;
                                                Status statusId;
                                                Tracker trackerId;
                                                bool priorityConvertor = CheckPriority(text[5], out priorityId);
                                                bool statusConvertor = CheckStatus(text[6], out statusId);
                                                bool trackerConvertor = CheckTracker(text[7], out trackerId);
                                                if (priorityConvertor && statusConvertor && trackerConvertor)
                                                {
                                                    int projectId;
                                                    var projectConvertor = StringToInt(text[2], out projectId);
                                                    if (projectConvertor)
                                                    {
                                                        var redmineUserId = await GetUserRedmineIdByName(text[8], projectId, user.RedmineApiKey);
                                                        if (redmineUserId !=0)
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
                                                            var result = await _httpClientService.PostAsync(requestUrl, issueInJsonText,
                                                                _stringConstant.JsonApplication, user.RedmineApiKey, _stringConstant.RedmineApiKey);
                                                            if (string.IsNullOrEmpty(result))
                                                                replyText = _stringConstant.ErrorInCreatingIssue;
                                                            else
                                                            {
                                                                var createdIssue = JsonConvert.DeserializeObject<RedmineResponseSingleProject>(result);
                                                                replyText = string.Format(_stringConstant.IssueSuccessfullyCreatedMessage, createdIssue.Issue.IssueId);
                                                            }
                                                        }
                                                        else
                                                            replyText = _stringConstant.NoUserFoundInProject;
                                                    }
                                                    else
                                                        replyText = _stringConstant.ProperProjectId;
                                                }
                                            }
                                            break;
                                        #endregion

                                        #region Change Assignee
                                        case RedmineAction.changeassignee:
                                            {
                                                var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, text[2]);
                                                var response = await _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl, requestUrl, user.RedmineApiKey, _stringConstant.RedmineApiKey);
                                                if (!string.IsNullOrEmpty(response))
                                                {
                                                    var issue = JsonConvert.DeserializeObject<RedmineResponseSingleProject>(response);
                                                    var redmineUserId = await GetUserRedmineIdByName(text[4], issue.Issue.Project.Id, user.RedmineApiKey);
                                                    await UpdateByProperty(false, redmineUserId, text[2], user.RedmineApiKey);
                                                }
                                                else
                                                    replyText = string.Format(_stringConstant.IssueDoesNotExist, text[2]);
                                            }
                                            break;
                                        #endregion

                                        #region Issue close
                                        case RedmineAction.close:
                                            {
                                                await UpdateByProperty(true, 0, text[2], user.RedmineApiKey);
                                            }
                                            break;
                                        #endregion

                                        #region Issue Time Entry
                                        case RedmineAction.timeentry:
                                            {
                                                int issueId;
                                                string result;
                                                var issueConvertor = ToCheckIssueExistOrNot(text[2], user.RedmineApiKey, out issueId, out result);
                                                if (issueConvertor)
                                                {
                                                    double hour;
                                                    var hourConvertor = double.TryParse(text[3], out hour);
                                                    if (hourConvertor)
                                                    {
                                                        var dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                                                        DateTime date;
                                                        var dateConvertor = DateTime.TryParseExact(text[4], dateFormat, CultureInfo.InvariantCulture,
                                                            DateTimeStyles.None, out date);
                                                        if (dateConvertor)
                                                        {
                                                            TimeEntryActivity timeEntryActivity;
                                                            var timeEntryActivityConvertor = TimeEntryActivity.TryParse(text[5], out timeEntryActivity);
                                                            if (timeEntryActivityConvertor)
                                                            {
                                                                var timeEntry = new RedmineTimeEntries()
                                                                {
                                                                    ActivityId = timeEntryActivity,
                                                                    IssueId = issueId,
                                                                    Date = date,
                                                                    Hours = hour
                                                                };
                                                                var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat,
                                                                    _stringConstant.RedmineBaseUrl, _stringConstant.TimeEntryUrl);
                                                                var jsonText = JsonConvert.SerializeObject(timeEntry);
                                                                var response = await _httpClientService.PostAsync(requestUrl, jsonText,
                                                                    _stringConstant.JsonApplication, user.RedmineApiKey, _stringConstant.RedmineApiKey);
                                                                if (!string.IsNullOrEmpty(response))
                                                                {
                                                                    replyText = string.Format(_stringConstant.TimeEnrtyAddSuccessfully, issueId);
                                                                }
                                                                else
                                                                    replyText = string.Format(_stringConstant.ErrorInAddingTimeEntry, issueId);
                                                            }
                                                            else
                                                                replyText = string.Format(_stringConstant.TimeEntryActivityErrorMessage, TimeEntryActivity.Analysis.ToString(),
                                                                    TimeEntryActivity.Design.ToString(), TimeEntryActivity.Development.ToString(), TimeEntryActivity.Roadblock.ToString(),
                                                                    TimeEntryActivity.Testing.ToString());
                                                        }
                                                        else
                                                            replyText = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
                                                    }
                                                    else
                                                        replyText = _stringConstant.HourIsNotNumericMessage;
                                                }
                                            }
                                            break;
                                            #endregion
                                    }
                                }
                                else
                                    replyText = string.Format(_stringConstant.ProperRedmineIssueAction, RedmineAction.list.ToString(),
                                        RedmineAction.create.ToString(), RedmineAction.changeassignee.ToString(), RedmineAction.close.ToString(),
                                        RedmineAction.timeentry.ToString());
                            }
                            break;
                        #endregion

                        #region Help
                        case SlackAction.help:
                            {
                                replyText = _stringConstant.RedmineHelp;
                            }
                            break;
                            #endregion
                    }
                }
                else
                    replyText = string.Format(_stringConstant.RequestToEnterProperRedmineAction, SlackAction.list.ToString(),
                        SlackAction.projects.ToString(), SlackAction.help.ToString());
            }
            else
                replyText = _stringConstant.SlackUserNotFound;
            await _clientRepository.SendMessageAsync(slashCommand.ResponseUrl, replyText);
        }
        #endregion

        #region Private Region
        /// <summary>
        /// Method which try to convert string to int
        /// </summary>
        /// <param name="input">string</param>
        /// <param name="output">out integer</param>
        /// <returns>True or False</returns>
        private bool StringToInt(string input, out int output)
        {
            return int.TryParse(input, out output);
        }

        /// <summary>
        /// Method to create message for redmine issue
        /// </summary>
        /// <param name="issue">redmine issue</param>
        /// <returns>string message</returns>
        private string MessageForIssue(GetRedmineIssue issue)
        {
            return string.Format(_stringConstant.RedmineIssueMessageFormat, issue.Project.Name, issue.IssueId,
                issue.Subject, issue.Status.Name, issue.Priority.Name, issue.Tracker.Name);
        }

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
            if (valueConverted)
                replyText = string.Empty;
            else
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
            if (valueConverted)
                replyText = string.Empty;
            else
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
            if (valueConverted)
                replyText = string.Empty;
            else
                string.Format(_stringConstant.RedmineTrackerErrorMessage,
                    Tracker.Bug.ToString(), Tracker.Feature.ToString(), Tracker.Support.ToString(),
                    Tracker.Tasks.ToString());
            return valueConverted;
        }

        /// <summary>
        /// Method to get string project details from project object
        /// </summary>
        /// <param name="project">project details</param>
        /// <returns>project detail string</returns>
        private string MessageForProject(RedmineProject project)
        {
            return string.Format("{0}. Project - {1}, ", project.Id, project.Name);
        }

        /// <summary>
        /// Method to send request to update the redmine issue
        /// </summary>
        /// <param name="issueId">issue Id</param>
        /// <param name="redmineApiKey">redmine api key</param>
        /// <param name="issue">issue details</param>
        private async Task UpdateIssue(int issueId, string redmineApiKey, PostRedmineResponse issue)
        {
            var updateRequestUrl = string.Format(_stringConstant.RedmineIssueUpdateUrl,
                _stringConstant.RedmineBaseUrl, _stringConstant.IssueUrl, issueId);
            var issueInJsonText = JsonConvert.SerializeObject(issue);
            var updateResult = await _httpClientService.PostAsync(updateRequestUrl, issueInJsonText,
                _stringConstant.JsonApplication, redmineApiKey, _stringConstant.RedmineApiKey);
            if (string.IsNullOrEmpty(updateResult))
                replyText = _stringConstant.ErrorInUpdateIssue;
            else
            {
                var createdIssue = JsonConvert.DeserializeObject<RedmineResponseSingleProject>(updateResult);
                replyText = string.Format(_stringConstant.IssueSuccessfullUpdated, createdIssue.Issue.IssueId);
            }
        }

        /// <summary>
        /// Method to check the where issue exist or not to update if exist then exist
        /// </summary>
        /// <param name="isClose">update to close the issue</param>
        /// <param name="assignTo">update to assign the issue</param>
        /// <param name="issueStringId">issue id in string</param>
        /// <param name="userRedmineApiKey">redmine api key</param>
        private async Task UpdateByProperty(bool isClose, int assignTo, string issueStringId, string userRedmineApiKey)
        {
            int issueId;
            string result;
            var issueConvertor = ToCheckIssueExistOrNot(issueStringId, userRedmineApiKey, out issueId, out result);
            if (issueConvertor)
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
                    }
                };
                if (isClose)
                {
                    updateIssue.Issue.AssignTo = response.Issue.AssignTo.Id;
                    updateIssue.Issue.StatusId = Status.Closed;
                }
                else
                {
                    updateIssue.Issue.AssignTo = assignTo;
                    updateIssue.Issue.StatusId = (Status)response.Issue.Status.Id;

                }
                await UpdateIssue(issueId, userRedmineApiKey, updateIssue);
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
            var issueConvertor = StringToInt(issueStringId, out issueId);
            if (issueConvertor)
            {
                var requestUrl = string.Format(_stringConstant.IssueDetailsUrl, issueId);
                response = _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl, requestUrl, userRedmineApiKey, _stringConstant.RedmineApiKey).Result;
                if (string.IsNullOrEmpty(response))
                {
                    replyText = string.Format(_stringConstant.IssueDoesNotExist, issueId);
                    return false;
                }
                return true;
            }
            else
                replyText = _stringConstant.ProperProjectId;
            return false;
        }

        /// <summary>
        /// Method to get list of user in a particular project
        /// </summary>
        /// <param name="name">name of user</param>
        /// <param name="projectId">project Id</param>
        /// <param name="redmineApiKey">redmine api key</param>
        /// <returns></returns>
        private async Task<int> GetUserRedmineIdByName(string name, int projectId, string redmineApiKey)
        {
            int userId = 0;
            var requestUrl = string.Format(_stringConstant.UserByProjectIdUrl, projectId);
            var response = await _httpClientService.GetAsync(_stringConstant.RedmineBaseUrl, requestUrl, redmineApiKey, _stringConstant.RedmineApiKey);
            if (!string.IsNullOrEmpty(response))
            {
                var users = JsonConvert.DeserializeObject<RedmineUserResponse>(response);
                foreach (var user in users.Members)
                {
                    if (user.User.Name == name)
                        userId = user.User.Id;
                }
            }
            return userId;
        }
        #endregion
    }
}
