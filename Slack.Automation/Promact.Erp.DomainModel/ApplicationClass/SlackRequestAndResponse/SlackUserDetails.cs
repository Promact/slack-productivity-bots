using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackUserDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("team_id")]
        public string TeamId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("deleted")]
        public bool Deleted { get; set; }
    }
}
