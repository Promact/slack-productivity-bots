using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashResponse
    {
        public string response_type { get; set; }
        public string text { get; set; }
        public List<SlashAttachment> attachments { get; set; }
    }
}
