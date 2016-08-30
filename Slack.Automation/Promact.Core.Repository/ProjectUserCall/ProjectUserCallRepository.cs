using Newtonsoft.Json;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;

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
            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.UserUrl, requestUrl, accessToken);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != HttpStatusCode.Forbidden)
            {
                userDetails = JsonConvert.DeserializeObject<User>(responseContent);
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
            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, accessToken);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != HttpStatusCode.Forbidden)
            {
                teamLeader = JsonConvert.DeserializeObject<List<User>>(responseContent);
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
            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUserUrl, StringConstant.ManagementDetailsUrl, accessToken);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != HttpStatusCode.Forbidden)
            {
                management = JsonConvert.DeserializeObject<List<User>>(responseContent);
            }
            return management;
        }


        /// <summary>
        /// Method to call an api from project oAuth server and get Project details of the given group - JJ
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns>object of ProjectAc</returns>
        public async Task<ProjectAc> GetProjectDetails(string groupName, string accessToken)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, groupName);
                var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUrl, requestUrl, accessToken);
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var project = JsonConvert.DeserializeObject<ProjectAc>(responseContent);
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
        /// <param name="accessToken"></param>
        /// <returns>list of object of User</returns>
        public async Task<List<User>> GetUsersByGroupName(string groupName,string accessToken)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, groupName);
                var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUrl, requestUrl, accessToken);
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<List<User>>(responseContent);
                return user;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }


        public async Task<User> GetUserById(string EmployeeId, string accessToken)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsByIdUrl, EmployeeId);
                var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUrl, requestUrl, accessToken);
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<User>(responseContent);
                return user;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public async Task<ProjectAc> GetProjectDetailsByUserName(string userName, string accessToken)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsByUserNameUrl, userName);
                var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUrl, requestUrl, accessToken);
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var project = JsonConvert.DeserializeObject<ProjectAc>(responseContent);
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
        public async Task<User> GetUserByEmployeeId(string employeeId)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailUrl, employeeId);
                var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl, null);
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var userDetails = JsonConvert.DeserializeObject<User>(responseContent);
                return userDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<User> GetUserBySlackUserName(string userName,string accessToken)
        {
            try
            {
                var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailByUserNameUrl, userName);
                var response = await _httpClientRepository.GetAsync(AppSettingsUtil.UserUrl, requestUrl, accessToken);
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var userDetails = JsonConvert.DeserializeObject<User>(responseContent);
                return userDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
