using Promact.Erp.DomainModel.ApplicationClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.MailSettingDetailsByProjectAndModule
{
    public interface IMailSettingDetailsByProjectAndModuleRepository
    {
        /// <summary>
        /// Method used to get mail setting for a module by projectId
        /// </summary>
        /// <param name="projectId">project Id</param>
        /// <param name="module">mail setting module</param>
        /// <param name="userId">user's user Id</param>
        /// <returns>mail setting details</returns> 
        Task<MailSettingAC> GetMailSettingAsync(int projectId, string module, string userId);
    }
}
