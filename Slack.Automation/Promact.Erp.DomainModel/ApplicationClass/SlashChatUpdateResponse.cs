using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashChatUpdateResponse
    {
        /// <summary>
        /// Action details of Slash ChatUpdateMethod
        /// </summary>
        [JsonProperty("actions")]
        public SlashChatUpdateResponseAction Actions { get; set; }

        /// <summary>
        /// Callback_id details of Slash ChatUpdateMethod
        /// </summary>
        [JsonProperty("callback_id")]
        public int CallbackId { get; set; }

        /// <summary>
        /// Team details of Slash ChatUpdateMethod
        /// </summary>
        [JsonProperty("team")]
        public SlashChatUpdateResponseTeam Team { get; set; }

        /// <summary>
        /// Channel details of Slash ChatUpdateMethod
        /// </summary>
        [JsonProperty("channel")]
        public SlashChatUpdateResponseChannelUser Channel { get; set; }

        /// <summary>
        /// User details of Slash ChatUpdateMethod
        /// </summary>
        [JsonProperty("user")]
        public SlashChatUpdateResponseChannelUser User { get; set; }

        /// <summary>
        /// action_ts of Slash ChatUpdateMethod
        /// </summary>
        [JsonProperty("action_ts")]
        public string ActionTs { get; set; }

        /// <summary>
        /// message_ts of Slash ChatUpdateMethod
        /// </summary>
        [JsonProperty("message_ts")]
        public string MessageTs { get; set; }

        /// <summary>
        /// token of Slash ChatUpdateMethod
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
