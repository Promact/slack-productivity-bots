using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class GroupAC
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Type { get; set; }

        public List<string> Emails { get; set; }
    }
}
