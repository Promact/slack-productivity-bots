using IdentityModel.Client;
using NLog;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ServiceRepository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly IEnvironmentVariableRepository _environmentVariable;
        private readonly ILogger _logger;

        public ServiceRepository(IEnvironmentVariableRepository environmentVariable)
        {
            _environmentVariable = environmentVariable;
            _logger = LogManager.GetLogger("AuthenticationModule");
        }


        /// <summary>
        /// This method used for get access token by refresh token.
        /// </summary>
        /// <param name="refreshToken">passed refresh token</param>
        /// <returns>access token</returns>
        public async Task<string> GerAccessTokenByRefreshToken(string refreshToken)
        {
            _logger.Debug("Start GerAccessTokenByRefreshToken Method" + refreshToken);
            _logger.Debug("OAuthUrl :" + AppSettingUtil.OAuthUrl);
            //feching access token using refresh token. 
            var discovery = new DiscoveryClient(AppSettingUtil.OAuthUrl);
            discovery.Policy.RequireHttps = false;
            var doc = await discovery.GetAsync();
            var tokenClient = new TokenClient(doc.TokenEndpoint, _environmentVariable.PromactOAuthClientId, _environmentVariable.PromactOAuthClientSecret);
            var requestRefreshToken = tokenClient.RequestRefreshTokenAsync(refreshToken).Result;
            _logger.Debug("Request RequestRefreshTokenAsync response : " + requestRefreshToken.IsError);
            _logger.Debug("Request RequestRefreshTokenAsync Error : " + requestRefreshToken.Error);
            _logger.Debug("Request RequestRefreshTokenAsync Error Description : " + requestRefreshToken.ErrorDescription);
            _logger.Debug("Request RequestRefreshTokenAsync Error Type: " + requestRefreshToken.ErrorType);
            _logger.Debug("Access Token : " + requestRefreshToken.AccessToken);
            return requestRefreshToken.AccessToken;
        }

    }
}
