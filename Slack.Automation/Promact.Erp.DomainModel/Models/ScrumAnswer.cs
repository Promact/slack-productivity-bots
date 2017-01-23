using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Promact.Erp.DomainModel.ApplicationClass;

namespace Promact.Erp.DomainModel.Models
{
    public class ScrumAnswer : ModelBase
    {
        /// <summary>
        /// Answer statement
        /// </summary>
        [Required]
        public string Answer { get; set; }

        /// <summary>
        /// Id of scrum
        /// </summary>
        [Required]
        public int ScrumId { get; set; }

        /// <summary>
        /// Id of question which is answered
        /// </summary>
        [Required]
        public int QuestionId { get; set; }

        /// <summary>
        /// Id of Employee who answered
        /// </summary>
        [Required]
        public string EmployeeId { get; set; }

        /// <summary>
        /// Status of answer given
        /// </summary>
        [Required]
        public ScrumAnswerStatus ScrumAnswerStatus { get; set; }

        /// <summary>
        /// Date on which Answer is given
        /// </summary>
        [Required]
        public DateTime AnswerDate { get; set; }
    
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
        [ForeignKey("ScrumId")]
        public virtual Scrum Scrum { get; set; }
    }
}
