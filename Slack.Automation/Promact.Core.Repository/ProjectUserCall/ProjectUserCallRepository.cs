using Newtonsoft.Json;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ProjectUserCall
{
    public class ProjectUserCallRepository : IProjectUserCallRepository
    {
        private readonly IHttpClientRepository _httpClientRepository;
        public ProjectUserCallRepository(IHttpClientRepository httpClientRepository)
        {
            _httpClientRepository = httpClientRepository;
        }
        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their slack userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>user Details</returns>
        public async Task<User> GetUserByUsername(string userName, string accessToken)
        {
            User userDetails = new User();
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, userName);
            var response = await _httpClientRepository.GetAsync(StringConstant.ProjectUserUrl, requestUrl,accessToken);
            if (response != null)
            {
                userDetails = JsonConvert.DeserializeObject<User>(response);
            }
            return userDetails;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of TeamLeader's slack UserName from employee userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>teamLeader details</returns>
        public async Task<List<User>> GetTeamLeaderUserName(string userName, string accessToken)
        {
            List<User> teamLeader = new List<User>();
            var requestUrl = string.Format("{0}{1}", StringConstant.TeamLeaderDetailsUrl, userName);
            var response = await _httpClientRepository.GetAsync(StringConstant.ProjectUserUrl, requestUrl,accessToken);
            if (response != null)
            {
                teamLeader = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return teamLeader;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of Management People's Slack UserName
        /// </summary>
        /// <returns>management details</returns>
        public async Task<List<User>> GetManagementUserName(string accessToken)
        {
            List<User> management = new List<User>();
            var response = await _httpClientRepository.GetAsync(StringConstant.ProjectUserUrl, StringConstant.ManagementDetailsUrl,accessToken);
            if (response != null)
            {
                management = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return management;
        }


        /// <summary>
        /// Method to call an api from project oAuth server and get Project details of the given group - JJ
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns>object of ProjectAc</returns>
        public async Task<ProjectAc> GetProjectDetails(string groupName)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, groupName);
                var response = await _httpClientRepository.GetAsync(StringConstant.ProjectUrl, requestUrl,null);
                var project = JsonConvert.DeserializeObject<ProjectAc>(response);
                return project;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// This method is used to fetch list of users/employees of the given group name. - JJ
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns>list of object of User</returns>
        public async Task<List<User>> GetUsersByGroupName(string groupName)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, groupName);
                var response = await _httpClientRepository.GetAsync(StringConstant.ProjectUrl, requestUrl,null);
                var user = JsonConvert.DeserializeObject<List<User>>(response);
                return user;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        public async Task<User> GetUserById(string EmployeeId)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsByIdUrl, EmployeeId);
                var response = await _httpClientRepository.GetAsync(StringConstant.ProjectUrl, requestUrl, null);
                var user = JsonConvert.DeserializeObject<User>(response);
                return user;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ProjectAc> GetProjectDetailsByUserName(string userName, string accessToken)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsByUserNameUrl, userName);
                var response = await _httpClientRepository.GetAsync(StringConstant.ProjectUrl, requestUrl, accessToken);
                var project = JsonConvert.DeserializeObject<ProjectAc>(response);
                return project;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their Id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>user Details</returns>
        public async Task<User> GetUserByEmployeeId(string employeeId,string accessToken)
        {
            User userDetails = new User();
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, employeeId);
            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, accessToken);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != HttpStatusCode.Forbidden)
            {
                userDetails = JsonConvert.DeserializeObject<User>(responseContent);
            }
            return userDetails;
        }

        /// <summary>
        /// Method to call an api of oAuth server and get Casual leave allowed to user by user slackName
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns>Number of casual leave allowed</returns>
        public async Task<double> CasualLeave(string slackUserName, string accessToken)
        {
            var requestUrl = string.Format("{0}{1}", StringConstant.CasualLeaveUrl, slackUserName);
            var response = await _httpClientRepository.GetAsync(StringConstant.ProjectUserUrl, requestUrl, accessToken);
            var responseContent = JsonConvert.DeserializeObject<string>(response);
            var casualLeave = Convert.ToDouble(responseContent);
            return casualLeave;
        }
    }
}
