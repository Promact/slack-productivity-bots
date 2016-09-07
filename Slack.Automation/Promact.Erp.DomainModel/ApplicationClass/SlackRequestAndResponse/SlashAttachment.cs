using Newtonsoft.Json;
using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlashAttachment
    {
        /// <summary>
        /// FallBack of Slash message Attachment
        /// </summary>
        [JsonProperty("fallback")]
        public string Fallback { get; set; }

        /// <summary>
        /// Title of Slash message Attachment
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Callback_id of Slash message Attachment
        /// </summary>
        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        /// <summary>
        /// Color of Slash message Attachment
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; }

        /// <summary>
        /// Attachment_type  of Slash message Attachment
        /// </summary>
        [JsonProperty("attachment_type")]
        public string AttachmentType { get; set; }

        /// <summary>
        /// Action of Slash message Attachment Whether it may have text or text along with button
        /// </summary>
        [JsonProperty("actions")]
        public List<SlashAttachmentAction> Actions { get; set; }
    }
}
