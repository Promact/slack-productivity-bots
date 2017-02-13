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
        public async Task<User> GetUserByUserIdAsync(string userId, string accessToken)
        {
            User userDetails = new User();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, userId);
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
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
        public async Task<List<User>> GetTeamLeaderUserIdAsync(string userId, string accessToken)
        {
            List<User> teamLeader = new List<User>();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl, userId);
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
        /// Method to call an api of oAuth server and get Casual leave allowed to user by user slackName. - SS
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>Number of casual leave allowed. Object of LeaveAllowed</returns>
        public async Task<LeaveAllowed> AllowedLeave(string userId, string accessToken)
        {
            LeaveAllowed allowedLeave = new LeaveAllowed();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.CasualLeaveUrl, userId);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                allowedLeave = JsonConvert.DeserializeObject<LeaveAllowed>(response);
            }
            return allowedLeave;
        }


        /// <summary>
        /// Method to call an api from oAuth server and get whether user is admin or not. - SS
        /// </summary>
        /// <param name="slackUserId">userId of slack user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>true if user has admin role else false</returns>
        public async Task<bool> UserIsAdminAsync(string userId, string accessToken)
        {
            bool result = false;
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserIsAdmin, userId);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                result = JsonConvert.DeserializeObject<bool>(response);
            }
            return result;
        }
        #endregion
    }
}