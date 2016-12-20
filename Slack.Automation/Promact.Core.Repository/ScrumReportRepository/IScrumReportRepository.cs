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
        /// <param name="userId"></param>
        /// <returns>List of projects</returns>
        Task<IEnumerable<ProjectAc>> GetProjectsAsync(string userId);

        /// <summary>
        /// Method to return the details of scrum for a particular project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="scrumDate"></param>
        /// <param name="userId"></param>
        /// <returns>Details of the scrum</returns>
        Task<ScrumProjectDetails> ScrumReportDetailsAsync(int projectId, DateTime scrumDate, string userId);
    }
}
