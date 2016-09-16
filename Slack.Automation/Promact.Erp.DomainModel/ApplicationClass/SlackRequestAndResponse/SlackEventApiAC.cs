using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackEventApiAC
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("challenge")]
        public string Challenge { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("team_id")]
        public string TeamId { get; set; }
        [JsonProperty("api_app_id")]
        public string ApiAppId { get; set; }
        [JsonProperty("event_ts")]
        public string EventTs { get; set; }
        [JsonProperty("authed_users")]
        public List<string> AuthedUser { get; set; }
        [JsonProperty("event")]
        public SlackEventDetailAC Event { get; set; }
    }
}
