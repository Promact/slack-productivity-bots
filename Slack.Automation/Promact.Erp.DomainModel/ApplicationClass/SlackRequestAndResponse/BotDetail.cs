using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class BotDetail
    {
        /// <summary>
        /// Bot's user id
        /// </summary>
        [JsonProperty("bot_user_id")]
        public string BotUserId { get; set; }

        /// <summary>
        /// Bot's access token
        /// </summary>
        [JsonProperty("bot_access_token")]
        public string BotAccessToken { get; set; }

    }
}
