using Newtonsoft.Json;
using Promact.Erp.DomainModel.Models;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackChannelDetails : ModelBase
    {
        /// <summary>
        /// Id of channel in Slack
        /// </summary>
        [JsonProperty("id")]
        public string ChannelId { get; set; }

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

        /// <summary>
        /// Id of corresponding OAuth Project
        /// </summary>
        public int? ProjectId { get; set; }
    }
}
