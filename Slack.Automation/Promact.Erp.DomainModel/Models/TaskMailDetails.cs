using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.Models
{
    public class TaskMailDetails
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Hours { get; set; }
        public string Comment { get; set; }

        public int TaskId { get; set; }
        [ForeignKey("TaskId")]
        public virtual TaskMail TaskMail { get; set; }

        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
