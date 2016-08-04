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
        public string channel { get; set; }

        /// <summary>
        /// UserName of Slack
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// Attachment of message to be send on slack using Incoming WebHook
        /// </summary>
        public List<SlashAttachment> attachments { get; set; }
    }
}
