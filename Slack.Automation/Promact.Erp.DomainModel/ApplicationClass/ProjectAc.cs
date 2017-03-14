
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class ProjectAc
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SlackChannelName { get; set; }

        public bool IsActive { get; set; }
        public string TeamLeaderId { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public User TeamLeader { get; set; }

        [JsonProperty("ApplicationUsers")]
        public List<User> Users { get; set; }
    }
}
