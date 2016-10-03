using Newtonsoft.Json;
using Promact.Erp.DomainModel.Models;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackUserDetails : ModelBase
    {
        /// <summary>
        /// User Id of Slack for user
        /// </summary>
        [JsonProperty("id")]
        public string UserId { get; set; }

        /// <summary>
        /// Team Id of Slack user
        /// </summary>
        [JsonProperty("team_id")]
        public string TeamId { get; set; }

        /// <summary>
        /// Name of the slack user
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// True if it is Bot
        /// </summary>
        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        /// <summary>
        /// Bool true for they exist in team or not
        /// </summary>
        [JsonProperty("deleted")]
        public bool Deleted { get; set; }
    }
}
