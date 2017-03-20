using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class AppCredential : ModelBase
    {
        [Required]
        public string Module { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }
    }
}
