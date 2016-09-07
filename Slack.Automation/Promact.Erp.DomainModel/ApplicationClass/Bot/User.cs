using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.Bot
{
    public class Users
    {

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("presence")]
        public string Presence { get; set; }

        [JsonProperty("profile")]
        public Dictionary<string, string> Profile { get; set; }

        [JsonProperty("real_name")]
        public string RealName { get; set; }

        [JsonProperty("team_id")]
        public string TeamId { get; set; }

}
}
