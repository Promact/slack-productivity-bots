using Newtonsoft.Json;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ProjectUserCall
{
    public class ProjectUserCallRepository:IProjectUserCallRepository
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
        public async Task<User> GetUserByUsername(string userName)
        {
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, userName);
            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var userDetails = JsonConvert.DeserializeObject<User>(responseContent);
            return userDetails;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of TeamLeader's slack UserName from employee userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>teamLeader details</returns>
        public async Task<List<ProjectUserDetailsApplicationClass>> GetTeamLeaderUserName(string userName)
        {
            var requestUrl = string.Format("{0}{1}", StringConstant.TeamLeaderDetailsUrl, userName);
            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var teamLeader = JsonConvert.DeserializeObject<List<ProjectUserDetailsApplicationClass>>(responseContent);
            return teamLeader;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of Management People's Slack UserName
        /// </summary>
        /// <returns>management details</returns>
        public async Task<List<ProjectUserDetailsApplicationClass>> GetManagementUserName()
        {
            var requestUrl = string.Format("{0}", StringConstant.ManagementDetailsUrl);
            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var management = JsonConvert.DeserializeObject<List<ProjectUserDetailsApplicationClass>>(responseContent);
            return management;
        }
    }
}
