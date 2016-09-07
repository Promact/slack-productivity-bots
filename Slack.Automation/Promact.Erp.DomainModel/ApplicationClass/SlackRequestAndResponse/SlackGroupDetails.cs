using Newtonsoft.Json;
using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackGroupDetails
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }
        [JsonProperty("groups")]
        public List<SlackChannelDetails> Groups { get; set; }
    }
}
