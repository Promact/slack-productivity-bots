using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.Models
{
    public class IncomingWebHook
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string IncomingWebHookUrl { get; set; }
    }
}
