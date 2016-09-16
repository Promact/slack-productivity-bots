using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class EmployeeScrumDetails
    {
        /// <summary>
        /// Name of the employee
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// Answer to question what did you do yesterday
        /// </summary>
        public string[] Answer1 { get; set; }

        /// <summary>
        /// Answer to question what will you do today
        /// </summary>
        public string[] Answer2 { get; set; }

        /// <summary>
        /// Answer to question any roadblock
        /// </summary>
        public string[] Answer3 { get; set; }

        public string Status { get; set; }

    }
}
