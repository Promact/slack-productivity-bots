using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashAttachmentAction
    {
        /// <summary>
        /// Name of Slash Button message Attachment
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Text of Slash Button message Attachment
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Type of Slash Button message Attachment
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Value of Slash Button message Attachment
        /// </summary>
        public string value { get; set; }
    }
}

