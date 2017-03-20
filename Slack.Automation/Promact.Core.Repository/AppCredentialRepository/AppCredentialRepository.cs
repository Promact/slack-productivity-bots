using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using System;
using System.Threading.Tasks;

namespace Promact.Core.Repository.AppCredentialRepository
{
    public class AppCredentialRepository : IAppCredentialRepository
    {

        private readonly IRepository<AppCredential> _appCredentialDataRepository;

        public AppCredentialRepository(IRepository<AppCredential> appCredentialDataRepository)
        {
            _appCredentialDataRepository = appCredentialDataRepository;
        }

        /// <summary>
        /// Get the app credentials of the given module - JJ
        /// </summary>
        /// <param name="module"></param>
        /// <returns>object of AppCredential</returns>
        public async Task<AppCredential> FetchAppCredentialByModule(string module)
        {
            return await _appCredentialDataRepository.FirstOrDefaultAsync(x => x.Module == module);
        }

        /// <summary>
        /// Adds the app's credentials to the database - JJ
        /// </summary>
        /// <param name="appCredential"></param>
        /// <returns></returns>
        public async Task<int> AddAppCredentialAsync(AppCredential appCredential)
        {
            AppCredential credential = await FetchAppCredentialByModule(appCredential?.Module);
            if (credential == null)
            {
                appCredential.CreatedOn = DateTime.UtcNow.Date;
                _appCredentialDataRepository.Insert(appCredential);
            }
            else
            {
                credential.ClientId = appCredential.ClientId;
                credential.ClientSecret = appCredential.ClientSecret;
                _appCredentialDataRepository.Update(credential);
            }
          return  await _appCredentialDataRepository.SaveChangesAsync();
        }

    }
}
