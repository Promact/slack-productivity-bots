using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
   public class SlackUserDetailAc
    {
        /// <summary>
        /// User Id of Slack for user
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Real Name of the slack user
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

    }
}
