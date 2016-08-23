using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class Question : ModelBase
    {
        [Required]
        public string QuestionStatement { get; set; }
        [Required]
        public int Type { get; set; }
    }
}
