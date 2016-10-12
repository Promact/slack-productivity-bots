using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackProfile
    {
        /// <summary>
        /// First Name of the slack user
        /// </summary>
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name of the slack user
        /// </summary>
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Title of the slack user
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Contact No. of the slack user
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Skype of the slack user
        /// </summary>
        [JsonProperty("skype")]
        public string Skype { get; set; }

        /// <summary>
        /// Email of the slack user
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
