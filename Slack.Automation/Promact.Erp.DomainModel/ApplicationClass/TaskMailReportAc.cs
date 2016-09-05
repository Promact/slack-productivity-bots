using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class TaskMailReportAc
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedOn { get; set; }
        //public string Name { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public decimal Hours { get; set; }
        public TaskMailStatus Status { get; set; }

    }
}
