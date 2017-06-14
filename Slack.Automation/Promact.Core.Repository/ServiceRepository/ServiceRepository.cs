using IdentityModel.Client;
using NLog;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ServiceRepository
{
    public class ServiceRepository : IServiceRepository
    {
        #region Private Variables
        private readonly IEnvironmentVariableRepository _environmentVariable;
        private readonly ILogger _logger;
        private readonly ApplicationUserManager _userManager;
        #endregion

        #region Constructor
        public ServiceRepository(IEnvironmentVariableRepository environmentVariable, ApplicationUserManager userManager)
        {
            _environmentVariable = environmentVariable;
            _logger = LogManager.GetLogger("AuthenticationModule");
            _userManager = userManager;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// This method used for get access token by refresh token.
        /// </summary>
        /// <param name="refreshToken">passed refresh token</param>
        /// <param name="userId">userId of user</param>
        /// <returns>access token</returns>
        public async Task<string> GerAccessTokenByRefreshToken(string refreshToken, string userId)
        {
            _logger.Debug("Start GerAccessTokenByRefreshToken Method" + refreshToken);
            _logger.Debug("OAuthUrl :" + AppSettingUtil.OAuthUrl);
            //feching access token using refresh token. 
            var discovery = new DiscoveryClient(AppSettingUtil.OAuthUrl);
            discovery.Policy.RequireHttps = false;
            var doc = await discovery.GetAsync();
            var tokenClient = new TokenClient(doc.TokenEndpoint, _environmentVariable.PromactOAuthClientId, _environmentVariable.PromactOAuthClientSecret);
            var requestRefreshToken = await tokenClient.RequestRefreshTokenAsync(refreshToken);
            _logger.Debug("Request RequestRefreshTokenAsync response : " + requestRefreshToken.IsError);
            _logger.Debug("Request RequestRefreshTokenAsync Error : " + requestRefreshToken.Error);
            if(requestRefreshToken.IsError)
            {
                var userLoginInfo = (await _userManager.GetLoginsAsync(userId)).Single(x=>x.ProviderKey == refreshToken);
                await _userManager.RemoveLoginAsync(userId, userLoginInfo);
                userLoginInfo.ProviderKey = requestRefreshToken.RefreshToken;
                await _userManager.AddLoginAsync(userId, userLoginInfo);
                requestRefreshToken = await tokenClient.RequestRefreshTokenAsync(refreshToken);
            }
            _logger.Debug("Access Token : " + requestRefreshToken.AccessToken);
            return requestRefreshToken.AccessToken; 
        }
        #endregion
    }
}
