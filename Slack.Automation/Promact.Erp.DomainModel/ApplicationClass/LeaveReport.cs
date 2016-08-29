using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
   public class LeaveReport
    {
        /// <summary>
        /// Id of the employee
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Name of the employee
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// User Name of the employee
        /// </summary>
        public string EmployeeUserName { get; set; }

        /// <summary>
        /// Total number of sick leaves alloted
        /// </summary>
        public int TotalSickLeave { get; set; }

        /// <summary>
        /// Total number of casual leaves alloted
        /// </summary>
        public int TotalCasualLeave { get; set; }

        /// <summary>
        /// Number of casusal leaves used
        /// </summary>
        public int UtilisedCasualLeave { get; set; }

        /// <summary>
        /// Number of casual leaves left
        /// </summary>
        public int BalanceCasualLeave { get; set; }


        /// <summary>
        /// Number of sick leaves used
        /// </summary>
        public int? UtilisedSickLeave { get; set; }

        /// <summary>
        /// Number of sick leaves left
        /// </summary>
        public int? BalanceSickLeave { get; set; }
       
    }
}
