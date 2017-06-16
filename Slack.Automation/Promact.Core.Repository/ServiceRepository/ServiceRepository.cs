using IdentityModel.Client;
using Microsoft.AspNet.Identity;
using NLog;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.StringLiteral;
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
        private readonly AppStringLiteral _stringConstant;
        #endregion

        #region Constructor
        public ServiceRepository(IEnvironmentVariableRepository environmentVariable, ApplicationUserManager userManager,
            ISingletonStringLiteral stringLiteral)
        {
            _environmentVariable = environmentVariable;
            _logger = LogManager.GetLogger("AuthenticationModule");
            _userManager = userManager;
            _stringConstant = stringLiteral.StringConstant;
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
            await UpdateExistingRefreshToken(refreshToken, requestRefreshToken.RefreshToken, userId);
            _logger.Debug("Access Token : " + requestRefreshToken.AccessToken);
            return requestRefreshToken.AccessToken;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Method to update old refresh token with new token of user
        /// </summary>
        /// <param name="oldRefreshToken">previous refresh token</param>
        /// <param name="newRefreshToken">new refresh token</param>
        /// <param name="userId">user's Id</param>
        private async Task UpdateExistingRefreshToken(string oldRefreshToken, string newRefreshToken, string userId)
        {
            var userLoginInfo = (await _userManager.GetLoginsAsync(userId)).Single(x => x.ProviderKey == oldRefreshToken);
            await _userManager.RemoveLoginAsync(userId, userLoginInfo);
            userLoginInfo = new UserLoginInfo(_stringConstant.PromactStringName, newRefreshToken);
            await _userManager.AddLoginAsync(userId, userLoginInfo);
        }
        #endregion
    }
}
