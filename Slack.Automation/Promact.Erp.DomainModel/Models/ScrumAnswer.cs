using System;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class ScrumAnswer : ModelBase
    {
        [Required]
        public string Answer { get; set; }
        [Required]
        public int ScrumID { get; set; }
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public DateTime AnswerDate { get; set; }
    }
}
