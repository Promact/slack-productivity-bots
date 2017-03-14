using Newtonsoft.Json;
using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class RedmineUser
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("project")]
        public RedmineBase Project { get; set; }
        [JsonProperty("user")]
        public RedmineBase User { get; set; }
        [JsonProperty("roles")]
        public List<RedmineBase> Roles { get; set; }
    }
}
