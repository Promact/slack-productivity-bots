using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.Models
{
    public class TaskMail : ModelBase
    {
        public string EmployeeId { get; set; }
        public ICollection<TaskMailDetails> TaskMailDetails { get; set; }
    }
}
