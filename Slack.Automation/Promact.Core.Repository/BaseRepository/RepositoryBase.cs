using System.Threading.Tasks;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;

namespace Promact.Core.Repository.BaseRepository
{
    public abstract class RepositoryBase
    {

        #region Private Variable


        private readonly IRepository<ApplicationUser> _applicationUser;
        private readonly IAttachmentRepository _attachmentRepository;


        #endregion


        #region Constructor


        public RepositoryBase(IRepository<ApplicationUser> applicationUser,
            IAttachmentRepository attachmentRepository)
        {
            _applicationUser = applicationUser;
            _attachmentRepository = attachmentRepository;
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Fetches access token of the user - JJ
        /// </summary>
        /// <param name="slackUserId">id of slack user</param>
        /// <returns>access token</returns>
        public async Task<string> GetAccessToken(string slackUserId)
        {
            // getting user from user's slack id
            ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
            // getting access token for that user
            if (applicationUser != null)
            {
                // get access token of user for promact oauth server
                string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);
                return accessToken;
            }
            return null;
        }


        #endregion
    }
}
