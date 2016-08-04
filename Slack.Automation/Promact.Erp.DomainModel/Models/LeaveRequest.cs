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
        /// <summary>
        /// Leave Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Reason for Leave
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Leave Start Date
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Leave End Date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Rejoin date to back office after Leave
        /// </summary>
        public DateTime RejoinDate { get; set; }

        /// <summary>
        /// Status of Leave : Approved, Pending, Rejected and Cancel
        /// </summary>
        public Condition Status { get; set; }

        /// <summary>
        /// EmployeeId as per OAuth Server
        /// </summary>
        public string EmployeeId { get; set; }
    }
}
