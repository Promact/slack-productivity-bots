using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.Redmine
{
    public class PostRedmineResponse
    {
        [JsonProperty("issue")]
        public PostRedminIssue Issue { get; set; }
    }
}
