using Newtonsoft.Json;
using Promact.Erp.DomainModel.JSonConverterUtil;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackEventDetailAC
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("user")]
        public SlackUserDetails User { get; set; }
        [JsonProperty("channel")]
        [JsonConverter(typeof(SlackChannelDetailsConverter))]
        public SlackChannelDetails Channel { get; set; }
    }
}
