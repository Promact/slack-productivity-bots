using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{ 
    public class SlashChatUpdateResponseAction
    {
        /// <summary>
        /// Name field in Slash ChatUpdate Action
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Value field in Slash ChatUpdate Action
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
