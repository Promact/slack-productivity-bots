using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class RedmineTimeEntryApplicationClass
    {
        [JsonProperty("time_entry")]
        public RedmineTimeEntries TimeEntry { get; set; }
    }
}
