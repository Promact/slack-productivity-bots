using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumReportRepository
{
    public interface IScrumReportRepository
    {
        /// <summary>
        /// Method to return the list of projects depending on the role of the logged in user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns>List of projects</returns>
        Task<IEnumerable<ProjectAc>> GetProjects(string userName, string accessToken);

        /// <summary>
        /// Method to return the details of scrum for a particular project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="scrumDate"></param>
        ///<param name = "accessToken" ></ param >
        /// <returns>Details of the scrum</returns>
        Task<ScrumProjectDetails> ScrumReportDetails(int projectId, DateTime scrumDate, string accessToken); 
    }
}
