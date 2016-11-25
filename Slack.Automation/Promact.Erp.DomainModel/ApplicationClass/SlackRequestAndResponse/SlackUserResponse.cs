using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackUserResponse
    {
        /// <summary>
        /// Bool value to get status of request to slack for getting user details
        /// </summary>
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        /// <summary>
        /// List of Slack User member
        /// </summary>
        [JsonProperty("members")]
        public List<SlackUserDetails> Members { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        [JsonProperty("error")]
        public string ErrorMessage { get; set; }
    }
}
