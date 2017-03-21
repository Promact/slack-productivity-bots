using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackOAuthResponse
    {
        /// <summary>
        /// Bool value of response true or false
        /// </summary>
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        /// <summary>
        /// Access Token provided from slack after OAuth Login
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Scope for app which is added to slack
        /// </summary>
        [JsonProperty("scope")]
        public string scope { get; set; }

        /// <summary>
        /// User Id of User as per slack
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Team name for which app is authorize
        /// </summary>
        [JsonProperty("team_name")]
        public string TeamName { get; set; }

        /// <summary>
        /// Team Id for which app is authorize
        /// </summary>
        [JsonProperty("team_id")]
        public string TeamId { get; set; }

        /// <summary>
        /// App's bot details
        /// </summary>
        [JsonProperty("bot")]
        public BotDetail Bot { get; set; }
        
        /// <summary>
        /// Incoming Webhook information for app
        /// </summary>
        [JsonProperty("incoming_webhook")]
        public IncomingWebHookOAuthResponse IncomingWebhook { get; set; }
    }
}
