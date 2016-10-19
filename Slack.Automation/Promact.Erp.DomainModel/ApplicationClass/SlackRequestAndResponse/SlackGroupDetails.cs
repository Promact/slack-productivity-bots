using Newtonsoft.Json;
using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackGroupDetails
    {
        /// <summary>
        /// Bool value to get status of request to slack for getting group details
        /// </summary>
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        /// <summary>
        /// List of Slack Groups (Private Channels)
        /// </summary>
        [JsonProperty("groups")]
        public List<SlackChannelDetails> Groups { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        [JsonProperty("error")]
        public string ErrorMessage { get; set; }
    }
}
