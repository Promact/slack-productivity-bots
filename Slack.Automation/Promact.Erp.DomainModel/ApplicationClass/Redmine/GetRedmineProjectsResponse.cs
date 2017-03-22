using Newtonsoft.Json;
using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class GetRedmineProjectsResponse : GetRedmineBaseApplicationClass
    {
        [JsonProperty("projects")]
        public List<RedmineProject> Projects { get; set; }
    }
}
