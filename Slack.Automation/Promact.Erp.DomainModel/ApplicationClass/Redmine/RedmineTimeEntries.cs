using Newtonsoft.Json;
using System;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class RedmineTimeEntries
    {
        [JsonProperty("issue_id")]
        public int IssueId { get; set; }
        [JsonProperty("spent_on")]
        public string Date { get; set; }
        [JsonProperty("hours")]
        public double Hours { get; set; }
        [JsonProperty("activity_id")]
        public TimeEntryActivity ActivityId { get; set; }
    }
}
