using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashIncomingWebhook
    {
        /// <summary>
        /// Channel Name or Id for Incoming WebHook
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// UserName of Slack
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Attachment of message to be send on slack using Incoming WebHook
        /// </summary>
        [JsonProperty("attachments")]
        public List<SlashAttachment> Attachments { get; set; }
    }
}
