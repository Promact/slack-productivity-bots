using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashResponse
    {
        /// <summary>
        /// Response Type : empheral or in_channel on slack
        /// </summary>
        public string response_type { get; set; }

        /// <summary>
        /// Text to be send on response of slash Command 
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Attachment to be send on response of slash command
        /// </summary>
        public List<SlashAttachment> attachments { get; set; }
    }
}
