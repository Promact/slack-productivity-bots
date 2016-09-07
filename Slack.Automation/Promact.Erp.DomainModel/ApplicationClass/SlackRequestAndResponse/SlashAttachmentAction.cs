using Newtonsoft.Json;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlashAttachmentAction
    {
        /// <summary>
        /// Name of Slash Button message Attachment
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Text of Slash Button message Attachment
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Type of Slash Button message Attachment
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Value of Slash Button message Attachment
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}

