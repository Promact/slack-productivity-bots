using Newtonsoft.Json;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;


namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlackBasicUserDetailsAc
    {
        /// <summary>
        /// Bool value to get status of request to slack for getting user details
        /// </summary>
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        /// <summary>
        /// List of Slack User member
        /// </summary>
        [JsonProperty("user")]
        public SlackUserDetails User { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        [JsonProperty("team")]
        public SlackUserDetails Team { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        [JsonProperty("error")]
        public string ErrorMessage { get; set; }
    }
}
