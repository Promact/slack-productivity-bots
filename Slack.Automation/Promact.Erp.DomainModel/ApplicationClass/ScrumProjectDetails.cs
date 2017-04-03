using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class ScrumProjectDetails
    {
        /// <summary>
        /// The date on which scrum was conducted
        /// </summary>
        public string ScrumDate { get; set; }

        /// <summary>
        /// Date on which project was created
        /// </summary>
        public string ProjectCreationDate { get; set; }

        /// <summary>
        /// List of object of EmployeeScrumDetails that stores the details of scrum answers for a particular employee
        /// </summary>
        public IList<EmployeeScrumDetails> EmployeeScrumAnswers { get; set; }
    }
}
