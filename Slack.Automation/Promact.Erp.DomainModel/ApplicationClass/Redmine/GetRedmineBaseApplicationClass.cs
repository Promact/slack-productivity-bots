using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class GetRedmineBaseApplicationClass
    {
        [JsonProperty("total_count")]
        public int IssueCount { get; set; }
        [JsonProperty("offset")]
        public int OffSet { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
    }
}
