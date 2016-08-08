using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
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
