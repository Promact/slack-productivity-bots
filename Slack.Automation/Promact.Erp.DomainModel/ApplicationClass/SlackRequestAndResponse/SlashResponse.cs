using Newtonsoft.Json;
using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlashResponse
    {
        /// <summary>
        /// Response Type : empheral or in_channel on slack
        /// </summary>
        [JsonProperty("response_type")]
        public string ResponseType { get; set; }

        /// <summary>
        /// Text to be send on response of slash Command 
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Attachment to be send on response of slash command
        /// </summary>
        [JsonProperty("attachments")]
        public List<SlashAttachment> Attachments { get; set; }
    }
}
