using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.Models
{
    public class IncomingWebHook
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string IncomingWebHookUrl { get; set; }
    }
}
