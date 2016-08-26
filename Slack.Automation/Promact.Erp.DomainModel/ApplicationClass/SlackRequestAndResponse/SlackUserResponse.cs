using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackUserResponse
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }
        [JsonProperty("members")]
        public List<SlackUserDetails> Members { get; set; }
    }
}
