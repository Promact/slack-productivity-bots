using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashIncomingWebhook
    {
        public string channel { get; set; }
        public string username { get; set; }
        public List<SlashAttachment> attachments { get; set; }
    }
}
