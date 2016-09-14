using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class ScrumProjectDetails
    {
        /// <summary>
        /// The date on which scrum was conducted
        /// </summary>
        public string ScrumDate { get; set; }

        /// <summary>
        /// List of object of EmployeeScrumDetails that stores the details of scrum answers for a particular employee
        /// </summary>
        public IList<EmployeeScrumDetails> EmployeeScrumAnswers { get; set; }
    }
}
