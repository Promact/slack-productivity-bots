using Newtonsoft.Json;
using System;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class RedmineProject
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("identifier")]
        public string Indentifier { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("created_on")]
        public DateTime CreateOn { get; set; }
        [JsonProperty("updated_on")]
        public DateTime UpdatedOn { get; set; }
    }
}
