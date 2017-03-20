using Promact.Erp.DomainModel.Models;
using System.Threading.Tasks;

namespace Promact.Core.Repository.AppCredentialRepository
{
    public interface IAppCredentialRepository
    {
        /// <summary>
        /// Get the app credentials of the given module - JJ
        /// </summary>
        /// <param name="module"></param>
        /// <returns>object of AppCredential</returns>
        Task<AppCredential> FetchAppCredentialByModule(string module);

        /// <summary>
        /// Adds the app's credentials to the database - JJ
        /// </summary>
        /// <param name="appCredential"></param>
        /// <returns></returns>
        Task<int> AddAppCredentialAsync(AppCredential appCredential);
    }
}
