using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackEventDetailAC
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("user")]
        public SlackUserDetails User { get; set; }
        [JsonProperty("channel")]
        public SlackChannelDetails Channel { get; set; }
    }
}
