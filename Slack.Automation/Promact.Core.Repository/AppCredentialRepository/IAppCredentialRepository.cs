using Promact.Erp.DomainModel.Models;
using System.Threading.Tasks;

namespace Promact.Core.Repository.AppCredentialRepository
{
    public interface IAppCredentialRepository
    {
        /// <summary>
        /// Get the app credentials of the given module - JJ
        /// </summary>
        /// <param name="module">Name of app</param>
        /// <returns>object of AppCredential</returns>
        Task<AppCredential> FetchAppCredentialByModuleAsync(string module);

        /// <summary>
        /// Adds the app's credentials to the database - JJ
        /// </summary>
        /// <param name="appCredential">object of app credential</param>
        /// <returns>status of operation</returns>
        Task<int> AddUpdateAppCredentialAsync(AppCredential appCredential);

        /// <summary>
        /// Updates the app's credentials to the database - JJ
        /// </summary>
        /// <param name="appCredntial">object of app credential</param>
        /// <returns>status of operation</returns>
        Task<int> UpdateBotTokenAsync(AppCredential appCredntial);

        /// <summary>
        /// Get the app credentials of the app which has been selected for integration - JJ
        /// </summary>
        /// <returns>object of AppCredential</returns>
        Task<AppCredential> FetchSelectedAppAsync();
    }
}
