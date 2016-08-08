using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashCommand
    {
        /// <summary>
        /// Token issued from slack
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// Team_id of slack
        /// </summary>
        [JsonProperty("team_id")]
        public string TeamId { get; set; }

        /// <summary>
        /// Team Doamin of slack
        /// </summary>
        [JsonProperty("team_domain")]
        public string TeamDomain { get; set; }

        /// <summary>
        /// Channel Id of the slack
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// Channel Name of Slack
        /// </summary>
        [JsonProperty("channel_name")]
        public string ChannelName { get; set; }

        /// <summary>
        /// User Id according to slack
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// User Name according to Slack
        /// </summary>
        [JsonProperty("user_name")]
        public string Username { get; set; }

        /// <summary>
        /// Slash Command issued by user
        /// </summary>
        [JsonProperty("command")]
        public string Command { get; set; }

        /// <summary>
        /// Text send along with /command by user
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Url to response back for slash Command
        /// </summary>
        [JsonProperty("response_url")]
        public string ResponseUrl { get; set; }
    }
}
