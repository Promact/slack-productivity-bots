using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
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
