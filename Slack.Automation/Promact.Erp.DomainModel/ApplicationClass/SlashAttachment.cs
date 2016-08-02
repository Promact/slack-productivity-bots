using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashAttachment
    {
        public string fallback { get; set; }
        public string title { get; set; }
        public string callback_id { get; set; }
        public string color { get; set; }
        public string attachment_type { get; set; }
        public List<SlashAttachmentAction> actions { get; set; }
    }
}
