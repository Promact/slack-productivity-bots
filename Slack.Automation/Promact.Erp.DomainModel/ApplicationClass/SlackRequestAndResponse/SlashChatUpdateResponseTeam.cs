using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlashChatUpdateResponseTeam
    {
        /// <summary>
        /// Id field in Slash ChatUpdate Team
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }

        /// <summary>
        /// domain field in Slash ChatUpdate Team
        /// </summary>
        [JsonProperty("domain")]
        public string Domain { get; set; }
    }
}
