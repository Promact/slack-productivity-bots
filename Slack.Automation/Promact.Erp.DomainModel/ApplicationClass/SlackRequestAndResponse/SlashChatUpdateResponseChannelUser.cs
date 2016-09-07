using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlashChatUpdateResponseChannelUser
    {
        /// <summary>
        /// Id field in Slash ChatUpdate Channel and User
        /// </summary>
        [JsonProperty("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Name field in Slash ChatUpdate Channel and User
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
