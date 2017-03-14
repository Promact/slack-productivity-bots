using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class PostRedminIssue
    {
        [JsonProperty("project_id")]
        public int ProjectId { get; set; }
        [JsonProperty("tracker_id")]
        public Tracker TrackerId { get; set; }
        [JsonProperty("status_id")]
        public Status StatusId { get; set; }
        [JsonProperty("priority_id")]
        public Priority PriorityId { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("assigned_to_id")]
        public int AssignTo { get; set; }
    }
}
