using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class LeaveReportDetails
    {
        /// <summary>
        /// Name of the employee
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// User Name of the employee
        /// </summary>
        public string EmployeeUserName { get; set; }

        /// <summary>
        /// Leave taken from which date
        /// </summary>
        public string LeaveFrom { get; set; }

        /// <summary>
        /// Day on which leave was taken
        /// </summary>
        public string StartDay { get; set; }

        /// <summary>
        /// Upto which date leave was taken
        /// </summary>
        public string LeaveUpto { get; set; }

        /// <summary>
        /// Day on which leave ended
        /// </summary>
        public string EndDay { get; set; }

        /// <summary>
        /// Reason of leave
        /// </summary>
        public string Reason { get; set; }

    }
}
