using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class IncomingWebHookOAuthResponse
    {
        /// <summary>
        /// Channel name of Incoming Webhook
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Channel Id of Incoming Webhook
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// Team Url
        /// </summary>
        [JsonProperty("configuration_url")]
        public string ConfigurationUrl { get; set; }

        /// <summary>
        /// Incoming Webhook Url
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
