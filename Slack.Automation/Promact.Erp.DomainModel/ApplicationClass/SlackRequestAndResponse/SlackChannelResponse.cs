using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackChannelResponse
    {
        /// <summary>
        /// Bool value to get status of request to slack for getting channel details
        /// </summary>
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        /// <summary>
        /// List of Slack Channels
        /// </summary>
        [JsonProperty("channels")]
        public List<SlackChannelDetails> Channels { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        [JsonProperty("error")]
        public string ErrorMessage { get; set; }
    }
}
