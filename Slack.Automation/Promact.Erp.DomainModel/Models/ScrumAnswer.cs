using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Promact.Erp.DomainModel.Models
{
    public class ScrumAnswer : ModelBase
    {
        [Required]
        public string Answer { get; set; }
        [Required]
        public int ScrumId { get; set; }
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public DateTime AnswerDate { get; set; }
        public ScrumAnswerStatus ScrumAnswerStatus { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
        [ForeignKey("ScrumId")]
        public virtual Scrum Scrum { get; set; }
    }
}
