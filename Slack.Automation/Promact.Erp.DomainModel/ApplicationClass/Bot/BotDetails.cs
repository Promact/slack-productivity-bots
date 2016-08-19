using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class BotDetails
    {
        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
