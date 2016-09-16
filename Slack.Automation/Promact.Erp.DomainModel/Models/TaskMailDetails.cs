using Promact.Erp.DomainModel.ApplicationClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Promact.Erp.DomainModel.Models
{
    public class TaskMailDetails
    {
        /// <summary>
        /// Primary key Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Description of task mail
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Number of hours send in a single task
        /// </summary>
        [Range(0,8.0)]
        public decimal Hours { get; set; }

        /// <summary>
        /// Comment or remark or roadblock in task
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Status of task completed or inprogress or roadblock
        /// </summary>
        public TaskMailStatus Status { get; set; }

        /// <summary>
        /// Status of mail send for task mail
        /// </summary>
        public SendEmailConfirmation SendEmailConfirmation { get; set; }

        /// <summary>
        /// Foreign Key TaskId from TaskMail table
        /// </summary>
        public int TaskId { get; set; }
        [ForeignKey("TaskId")]
        public virtual TaskMail TaskMail { get; set; }

        /// <summary>
        /// Foreign Key QuestionId from Questiion table
        /// </summary>
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
