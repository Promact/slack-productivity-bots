using Newtonsoft.Json;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.OauthCallsRepository
{
    public class OauthCallsRepository : IOauthCallsRepository
    {

        #region Private Variables

        private readonly IHttpClientService _httpClientService;
        private readonly IStringConstantRepository _stringConstant;


        #endregion


        #region Constructor

        public OauthCallsRepository(IHttpClientService httpClientService, IStringConstantRepository stringConstant)
        {
            _httpClientService = httpClientService;
            _stringConstant = stringConstant;
        }

        #endregion


        #region Public Methods


        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their slack userId. - SS
        /// </summary>
        /// <param name="slackUserId">userId of slack user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>user Details.Object of User</returns>
        public async Task<User> GetUserByUserIdAsync(string slackUserId, string accessToken)
        {
            User userDetails = new User();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, slackUserId);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                userDetails = JsonConvert.DeserializeObject<User>(response);
            }
            return userDetails;
        }


        /// <summary>
        /// Method to call an api from project oAuth server and get List of TeamLeader's slack UserName from employee userName. - SS
        /// </summary>
        /// <param name="slackUserId">userId of slack user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>teamLeader details.List of object of User</returns>
        public async Task<List<User>> GetTeamLeaderUserIdAsync(string slackUserId, string accessToken)
        {
            List<User> teamLeader = new List<User>();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl, slackUserId);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                teamLeader = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return teamLeader;
        }


        /// <summary>
        /// Method to call an api from project oAuth server and get List of Management People's Slack UserName. - SS
        /// </summary>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>management details.List of object of User</returns>
        public async Task<List<User>> GetManagementUserNameAsync(string accessToken)
        {
            List<User> management = new List<User>();
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUserUrl, _stringConstant.ManagementDetailsUrl, accessToken);
            if (response != null)
            {
                management = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return management;
        }


        /// <summary>
        /// Method to call an api from project oAuth server and get Project details of the given channel. - JJ 
        /// </summary>
        /// <param name="channelName">slack channel name</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>object of ProjectAc</returns>
        public async Task<ProjectAc> GetProjectDetailsAsync(string channelName, string accessToken)
        {
            var requestUrl = channelName;
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            ProjectAc project = new ProjectAc();
            if (!string.IsNullOrEmpty(response))
            {
                project = JsonConvert.DeserializeObject<ProjectAc>(response);
            }
            return project;
        }


        /// <summary>
        /// This method is used to fetch list of users/employees of the given channel name from OAuth server. - JJ
        /// </summary>
        /// <param name="channelName">slack channel name</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>list of object of User</returns>
        public async Task<List<User>> GetUsersByChannelNameAsync(string channelName, string accessToken)
        {
            string requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UsersDetailByChannelNameUrl, channelName);
            string response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            List<User> users = new List<User>();
            if (!string.IsNullOrEmpty(response))
            {
                users = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return users;
        }


        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their Id. - GA
        /// </summary>
        /// <param name="employeeId">id of employee</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>User Details. Object of User</returns>
        public async Task<User> GetUserByEmployeeIdAsync(string employeeId, string accessToken)
        {
            User userDetails = new User();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, employeeId, _stringConstant.UserDetailUrl);
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            if (response != null)
            {
                userDetails = JsonConvert.DeserializeObject<User>(response);
            }
            return userDetails;
        }


        /// <summary>
        /// Method to call an api of oAuth server and get Casual leave allowed to user by user slackName. - SS
        /// </summary>
        /// <param name="slackUserId">userId of slack user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>Number of casual leave allowed. Object of LeaveAllowed</returns>
        public async Task<LeaveAllowed> CasualLeaveAsync(string slackUserId, string accessToken)
        {
            LeaveAllowed casualLeave = new LeaveAllowed();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.CasualLeaveUrl, slackUserId);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                casualLeave = JsonConvert.DeserializeObject<LeaveAllowed>(response);
            }
            return casualLeave;
        }


        /// <summary>
        /// Method to call an api from oauth server and get all the projects under a specific teamleader id along with users in it. - GA
        /// </summary>
        /// <param name="teamLeaderId">id of the team leader</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>list of users in a project.List of object of User</returns>
        public async Task<List<User>> GetProjectUsersByTeamLeaderIdAsync(string teamLeaderId, string accessToken)
        {
            List<User> projectUsers = new List<User>();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, teamLeaderId, _stringConstant.ProjectUsersByTeamLeaderId);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            if (response != null)
            {
                projectUsers = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return projectUsers;
        }


        /// <summary>
        /// Method to call an api from oAuth server and get whether user is admin or not. - SS
        /// </summary>
        /// <param name="slackUserId">userId of slack user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>true if user has admin role else false</returns>
        public async Task<bool> UserIsAdminAsync(string slackUserId, string accessToken)
        {
            bool result = false;
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserIsAdmin, slackUserId);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                result = JsonConvert.DeserializeObject<bool>(response);
            }
            return result;
        }


        /// <summary>
        /// Used to get user role. - RS
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>user details. List of object of UserRoleAc</returns>
        public async Task<List<UserRoleAc>> GetUserRoleAsync(string userId, string accessToken)
        {
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, userId, _stringConstant.UserRoleUrl);
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            var userRoleListAc = JsonConvert.DeserializeObject<List<UserRoleAc>>(response);
            return userRoleListAc;
        }


        /// <summary>
        /// List of employee under this employee. - RS
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>List of user. List of object of UserRoleAc</returns>
        public async Task<List<UserRoleAc>> GetListOfEmployeeAsync(string userId, string accessToken)
        {
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, userId,_stringConstant.TeamMembersUrl);
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            var userRoleListAc = JsonConvert.DeserializeObject<List<UserRoleAc>>(response);
            return userRoleListAc;
        }

        
        /// <summary>
        /// Method is used to call an api from oauth server and return list of all the projects. - GA
        /// </summary>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>list of all the projects</returns>
        public async Task<List<ProjectAc>> GetAllProjectsAsync(string accessToken)
        {
            List<ProjectAc> projects = new List<ProjectAc>();
            var requestUrl = _stringConstant.AllProjectUrl;
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            if (response != null)
            {
                projects = JsonConvert.DeserializeObject<List<ProjectAc>>(response);
            }
            return projects;
        }


        /// <summary>
        /// Method to call an api from oauth server and get the details of a project using projecId. - GA
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>Details of a project</returns>
        public async Task<ProjectAc> GetProjectDetailsAsync(int projectId, string accessToken)
        {
            ProjectAc project = new ProjectAc();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, projectId,_stringConstant.GetProjectDetails);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            if(response != null)
            {
                project = JsonConvert.DeserializeObject<ProjectAc>(response);
            }
            return project;
        }
      

        #endregion

    }
}