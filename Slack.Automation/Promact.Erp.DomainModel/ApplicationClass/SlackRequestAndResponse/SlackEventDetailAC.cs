using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackEventDetailAC
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("user")]
        public SlackUserDetails User { get; set; }
    }
}
