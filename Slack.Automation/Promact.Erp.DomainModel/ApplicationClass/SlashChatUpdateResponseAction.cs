using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
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
