using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumReportRepository
{
    public interface IScrumReportRepository
    {
        /// <summary>
        /// Method to return the list of projects depending on the role of the logged in user
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>List of projects</returns>
        Task<IEnumerable<ProjectAc>> GetProjectsAsync(string userId);

        /// <summary>
        /// Method to return the details of scrum for a particular project
        /// </summary>
        /// <param name="projectId">project Id</param>
        /// <param name="scrumDate">Date of scrum</param>
        /// <param name="userId">userId of User</param>
        /// <returns>Details of the scrum</returns>
        Task<ScrumProjectDetails> ScrumReportDetailsAsync(int projectId, DateTime scrumDate, string userId); 
    }
}
