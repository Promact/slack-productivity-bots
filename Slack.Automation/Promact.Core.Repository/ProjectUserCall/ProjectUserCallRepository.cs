using Newtonsoft.Json;
using Promact.Erp.DomainModel.ApplicationClass;
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
            client.BaseAddress = new Uri("http://localhost:6875/");
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their slack userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User GetUserByUsername(string userName)
        {
            var response = client.GetAsync("project/userDetails/" + userName).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var userDetails = JsonConvert.DeserializeObject<User>(responseContent);
            return userDetails;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of TeamLeader's slack UserName from employee userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<string> GetTeamLeaderUserName(string userName)
        {
            var response = client.GetAsync("project/teamLeadersDetails/" + userName).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var teamLeader = JsonConvert.DeserializeObject<List<string>>(responseContent);
            return teamLeader;
        }

        /// <summary>
        /// Method to call an api from project oAuth server and get List of Management People's Slack UserName
        /// </summary>
        /// <returns></returns>
        public List<string> GetManagementUserName()
        {
            var response = client.GetAsync("project/Admin").Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var teamLeader = JsonConvert.DeserializeObject<List<string>>(responseContent);
            return teamLeader;
        }
    }
}
