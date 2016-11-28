using Newtonsoft.Json;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.StringConstants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.OauthCallsRepository
{
    public class OauthCallsRepository : IOauthCallsRepository
    {
        #region Private Variables
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly IStringConstantRepository _stringConstant;
        #endregion

        #region Constructor
        public OauthCallsRepository(IHttpClientRepository httpClientRepository, IStringConstantRepository stringConstant)
        {
            _httpClientRepository = httpClientRepository;
            _stringConstant = stringConstant;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their slack userId
        /// </summary>
        /// <param name="slackUserId"></param>
        /// <param name="accessToken"></param>
        /// <returns>user Details</returns>
        public async Task<User> GetUserByUserId(string slackUserId, string accessToken)
        {
            User userDetails = new User();
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, slackUserId);
            var response = await _httpClientRepository.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                userDetails = JsonConvert.DeserializeObject<User>(response);
            }
            return userDetails;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of TeamLeader's slack UserName from employee userName
        /// </summary>
        /// <param name="slackUserId"></param>
        /// <param name="accessToken"></param>
        /// <returns>teamLeader details</returns>
        public async Task<List<User>> GetTeamLeaderUserId(string slackUserId, string accessToken)
        {
            List<User> teamLeader = new List<User>();
            var requestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, slackUserId);
            var response = await _httpClientRepository.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                teamLeader = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return teamLeader;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of Management People's Slack UserName
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>management details</returns>
        public async Task<List<User>> GetManagementUserName(string accessToken)
        {
            List<User> management = new List<User>();
            var response = await _httpClientRepository.GetAsync(_stringConstant.ProjectUserUrl, _stringConstant.ManagementDetailsUrl, accessToken);
            if (response != null)
            {
                management = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return management;
        }


        /// <summary>
        /// Method to call an api from project oAuth server and get Project details of the given group 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="accessToken"></param>
        /// <returns>object of ProjectAc</returns>
        public async Task<ProjectAc> GetProjectDetails(string groupName, string accessToken)
        {
            var requestUrl = string.Format("{0}", groupName);
            var response = await _httpClientRepository.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            ProjectAc project = new ProjectAc();
            if (response != null)
            {
                project = JsonConvert.DeserializeObject<ProjectAc>(response);
            }
            return project;
        }


        /// <summary>
        /// This method is used to fetch list of users/employees of the given group name
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="accessToken"></param>
        /// <returns>list of object of User</returns>
        public async Task<List<User>> GetUsersByGroupName(string groupName, string accessToken)
        {
            var requestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, groupName);
            var response = await _httpClientRepository.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            List<User> users = new List<User>();
            if (response != null)
            {
                users = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return users;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their Id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="accessToken"></param>
        /// <returns>User Details</returns>
        public async Task<User> GetUserByEmployeeIdAsync(string employeeId, string accessToken)
        {
            User userDetails = new User();
            var requestUrl = string.Format("{0}{1}", employeeId, _stringConstant.UserDetailUrl);
            var response = await _httpClientRepository.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            if (response != null)
            {
                userDetails = JsonConvert.DeserializeObject<User>(response);
            }
            return userDetails;
        }

        /// <summary>
        /// Method to call an api of oAuth server and get Casual leave allowed to user by user slackName
        /// </summary>
        /// <param name="slackUserId"></param>
        /// <param name="accessToken"></param>
        /// <returns>Number of casual leave allowed</returns>
        public async Task<LeaveAllowed> CasualLeave(string slackUserId, string accessToken)
        {
            LeaveAllowed casualLeave = new LeaveAllowed();
            var requestUrl = string.Format("{0}{1}", _stringConstant.CasualLeaveUrl, slackUserId);
            var response = await _httpClientRepository.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                casualLeave = JsonConvert.DeserializeObject<LeaveAllowed>(response);
            }
            return casualLeave;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get logged in user details by their username
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns>User Details</returns>
        public async Task<User> GetUserByUserNameAsync(string userName, string accessToken)
        {
            User userDetails = new User();
            var requestUrl = string.Format("{0}{1}", userName, _stringConstant.LoginUserDetail);
            var response = await _httpClientRepository.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            if (response != null)
            {
                userDetails = JsonConvert.DeserializeObject<User>(response);
            }
            return userDetails;
        }

        /// <summary>
        /// Method to call an api from oauth server and get all the projects under a specific teamleader id along with users in it
        /// </summary>
        /// <param name="teamLeaderId"></param>
        /// <param name="accessToken"></param>
        /// <returns>list of users in a project</returns>
        public async Task<List<User>> GetProjectUsersByTeamLeaderIdAsync(string teamLeaderId, string accessToken)
        {
            List<User> projectUsers = new List<User>();
            var requestUrl = string.Format("{0}{1}", teamLeaderId,_stringConstant.ProjectUsersByTeamLeaderId);
            var response = await _httpClientRepository.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            if (response != null)
            {
                projectUsers = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return projectUsers;
        }

        /// <summary>
        /// Method to call an api from oAuth server and get whether user is admin or not
        /// </summary>
        /// <param name="slackUserId"></param>
        /// <param name="accessToken"></param>
        /// <returns>true or false</returns>
        public async Task<bool> UserIsAdmin(string slackUserId, string accessToken)
        {
            bool result = false;
            var requestUrl = string.Format("{0}{1}", _stringConstant.UserIsAdmin, slackUserId);
            var response = await _httpClientRepository.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken);
            if (response != null)
            {
                result = JsonConvert.DeserializeObject<bool>(response);
            }
            return result;
        }

        public async Task<List<UserRoleAc>> GetUserRole(string userId, string accessToken)
        {
            var requestUrl = string.Format("{0}{1}", userId, _stringConstant.UserRoleUrl);
            var response = await _httpClientRepository.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            var Json = JsonConvert.DeserializeObject<List<UserRoleAc>>(response);
            return Json;
        }

        public async Task<List<UserRoleAc>> GetListOfEmployee(string userId, string accessToken)
        {
            var requestUrl = string.Format("{0}{1}", userId,_stringConstant.TeamMembersUrl);
            var response = await _httpClientRepository.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            //var responseContent = response.Content.ReadAsStringAsync().Result;
            var Json = JsonConvert.DeserializeObject<List<UserRoleAc>>(response);
            return Json;
        }



        /// <summary>
        /// Method is used to call an api from oauth server and return list of all the projects
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>list of all the projects</returns>
        public async Task<List<ProjectAc>> GetAllProjectsAsync(string accessToken)
        {
            List<ProjectAc> projects = new List<ProjectAc>();
            var requestUrl = _stringConstant.AllProjectUrl;
            var response = await _httpClientRepository.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            if (response != null)
            {
                projects = JsonConvert.DeserializeObject<List<ProjectAc>>(response);
            }
            return projects;
        }

        /// <summary>
        /// Method to call an api from oauth server and get the details of a project using projecId
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="accessToken"></param>
        /// <returns>Details of a project</returns>
        public async Task<ProjectAc> GetProjectDetailsAsync(int projectId, string accessToken)
        {
            ProjectAc project = new ProjectAc();
            var requestUrl = string.Format("{0}{1}", projectId,_stringConstant.GetProjectDetails);
            var response = await _httpClientRepository.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            if(response != null)
            {
                project = JsonConvert.DeserializeObject<ProjectAc>(response);
            }
            return project;
        }

        #endregion
    }
}