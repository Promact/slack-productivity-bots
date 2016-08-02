using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.Models
{
    public class LeaveRequest : ModelBase
    {
        public string Type { get; set; }
        public string Reason { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RejoinDate { get; set; }
        public Condition Status { get; set; }
        public string EmployeeId { get; set; }
    }
}
