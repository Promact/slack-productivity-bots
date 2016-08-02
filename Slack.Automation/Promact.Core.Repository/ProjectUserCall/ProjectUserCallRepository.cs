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
            client.BaseAddress = new Uri("http://localhost:6875/");
        }
        public User GetUserByUsername(string userName)
        {
            var response = client.GetAsync("project/userDetails/" + userName).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var userDetails = JsonConvert.DeserializeObject<User>(responseContent);
            return userDetails;
        }
        public List<string> GetTeamLeaderUserName(string userName)
        {
            var response = client.GetAsync("project/teamLeadersDetails/" + userName).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var teamLeader = JsonConvert.DeserializeObject<List<string>>(responseContent);
            return teamLeader;
        }
        public List<string> GetManagementUserName()
        {
            var response = client.GetAsync("project/Admin").Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var teamLeader = JsonConvert.DeserializeObject<List<string>>(responseContent);
            return teamLeader;
        }
    }
}
