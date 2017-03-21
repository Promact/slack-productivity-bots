using Newtonsoft.Json;
using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class RedmineUserResponse
    {
        [JsonProperty("memberships")]
        public List<RedmineUser> Members { get; set; }
    }
}
