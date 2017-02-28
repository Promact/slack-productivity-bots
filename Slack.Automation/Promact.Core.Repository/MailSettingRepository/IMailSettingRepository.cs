using Promact.Erp.DomainModel.ApplicationClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.MailSettingRepository
{
    public interface IMailSettingRepository
    {
        /// <summary>
        /// Method to get list of project from oauth server
        /// </summary>
        /// <returns>list of project</returns>
        Task<List<ProjectAc>> GetAllProjectAsync();

        /// <summary>
        /// Method to get mail setting configuration from projectId
        /// </summary>
        /// <param name="projectId">project Id</param>
        /// <param name="module">name of module{leave, task or scrum}</param>
        /// <returns>mail setting configuration details</returns>
        Task<MailSettingAC> GetMailSettingDetailsByProjectIdAsync(int projectId, string module);

        /// <summary>
        /// Method to add mail setting configuration
        /// </summary>
        /// <param name="mailSettingAC">mail setting details</param>
        Task AddMailSettingAsync(MailSettingAC mailSettingAC);

        /// <summary>
        /// Method to get list of groups from group table
        /// </summary>
        /// <returns>list of group</returns>
        Task<List<string>> GetListOfGroupsNameAsync();

        /// <summary>
        /// Method to update mail setting configuration
        /// </summary>
        /// <param name="mailSettingAC">mail setting details</param>
        Task UpdateMailSettingAsync(MailSettingAC mailSettingAC);
    }
}
