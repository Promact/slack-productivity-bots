using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class EmailApplication
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> To { get; set; }
    }
}
