using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlashChatUpdateResponseTeam
    {
        /// <summary>
        /// Id field in Slash ChatUpdate Team
        /// </summary>
        [JsonProperty("Id")]
        public string Id { get; set; }

        /// <summary>
        /// domain field in Slash ChatUpdate Team
        /// </summary>
        [JsonProperty("domain")]
        public string Domain { get; set; }
    }
}
