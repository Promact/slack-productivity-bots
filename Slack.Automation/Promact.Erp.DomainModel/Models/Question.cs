using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class Question : ModelBase
    {
        /// <summary>
        /// Question to asked in task mail and scrum meeting
        /// </summary>
        [Required]
        public string QuestionStatement { get; set; }

        /// <summary>
        /// Type of Question. 1 for scrum meeting and 2 for task mail question 
        /// </summary>
        [Required]
        public int Type { get; set; }

        /// <summary>
        /// Order of asking question in task mail and scrum meeting
        /// </summary>
        public int OrderNumber { get; set; }
        public ICollection<TaskMailDetails> TaskMailDetails { get; set; }
    }
}
