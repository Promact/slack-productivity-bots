using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class GetRedmineIssue
    {
        [JsonProperty("id")]
        public int IssueId { get; set; }
        [JsonProperty("project")]
        public RedmineBase Project { get; set; }
        [JsonProperty("tracker")]
        public RedmineBase Tracker { get; set; }
        [JsonProperty("status")]
        public RedmineBase Status { get; set; }
        [JsonProperty("priority")]
        public RedmineBase Priority { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("assigned_to")]
        public RedmineBase AssignTo { get; set; }
    }
}
