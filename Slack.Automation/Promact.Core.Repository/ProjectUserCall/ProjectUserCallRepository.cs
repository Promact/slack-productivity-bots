using Newtonsoft.Json;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ProjectUserCall
{
    public class ProjectUserCallRepository:IProjectUserCallRepository
    {
        HttpClient client;
        public ProjectUserCallRepository()
        {
            client = new HttpClient();
            //Setting baseAddress in client
            client.BaseAddress = new Uri(AppSettingsUtil.ProjectUserUrl);
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their slack userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetUserByUsername(string userName)
        {
            var requestUrl = string.Format("project/userDetails/{0}",userName);
            var response = await client.GetAsync(requestUrl);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var userDetails = JsonConvert.DeserializeObject<User>(responseContent);
            return userDetails;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of TeamLeader's slack UserName from employee userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<List<ProjectUserDetailsApplicationClass>> GetTeamLeaderUserName(string userName)
        {
            var requestUrl = string.Format("project/teamLeadersDetails/{0}", userName);
            var response = await client.GetAsync(requestUrl);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var teamLeader = JsonConvert.DeserializeObject<List<ProjectUserDetailsApplicationClass>>(responseContent);
            return teamLeader;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of Management People's Slack UserName
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProjectUserDetailsApplicationClass>> GetManagementUserName()
        {
            var requestUrl = string.Format("project/teamLeadersDetails/");
            var response = await client.GetAsync(requestUrl);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var teamLeader = JsonConvert.DeserializeObject<List<ProjectUserDetailsApplicationClass>>(responseContent);
            return teamLeader;
        }
    }
}
