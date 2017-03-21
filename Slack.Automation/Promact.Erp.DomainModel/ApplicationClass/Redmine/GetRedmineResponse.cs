using Newtonsoft.Json;
using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class GetRedmineResponse
    {
        [JsonProperty("issues")]
        public List<GetRedmineIssue> Issues { get; set; }
        [JsonProperty("total_count")]
        public int IssueCount { get; set; }
        [JsonProperty("offset")]
        public int OffSet { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
    }
}
