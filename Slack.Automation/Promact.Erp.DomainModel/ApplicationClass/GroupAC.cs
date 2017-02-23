using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class GroupAC
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public List<string> Emails { get; set; }
    }
}
