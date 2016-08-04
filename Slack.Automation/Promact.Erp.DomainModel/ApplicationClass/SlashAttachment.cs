using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashAttachment
    {
        /// <summary>
        /// FallBack of Slash message Attachment
        /// </summary>
        public string fallback { get; set; }

        /// <summary>
        /// Title of Slash message Attachment
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Callback_id of Slash message Attachment
        /// </summary>
        public string callback_id { get; set; }

        /// <summary>
        /// Color of Slash message Attachment
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// Attachment_type  of Slash message Attachment
        /// </summary>
        public string attachment_type { get; set; }

        /// <summary>
        /// Action of Slash message Attachment Whether it may have text or text along with button
        /// </summary>
        public List<SlashAttachmentAction> actions { get; set; }
    }
}
