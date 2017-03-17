using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class Configuration : ModelBase
    {
        [Required]
        [MaxLength(25)]
        public string Module { get; set; }
        public bool Status { get; set; }
    }
}
