using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackChannelDetails
    {

        /// <summary>
        /// Id of channel in Slack
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }


        /// <summary>
        /// Name of the slack channel
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// False if the channel is not archived
        /// </summary>
        [JsonProperty("is_archived")]
        public bool Deleted { get; set; }
    }
}
