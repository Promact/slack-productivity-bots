using IdentityModel.Client;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ServiceRepository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly IEnvironmentVariableRepository _environmentVariable;

        public ServiceRepository(IEnvironmentVariableRepository environmentVariable)
        {
            _environmentVariable = environmentVariable;
        }


        /// <summary>
        /// This method used for get access token by refresh token.
        /// </summary>
        /// <param name="refreshToken">passed refresh token</param>
        /// <returns>access token</returns>
        public async Task<string> GerAccessTokenByRefreshToken(string refreshToken)
        {
            //feching access token using refresh token. 
            var discovery = new DiscoveryClient(AppSettingUtil.OAuthUrl);
            discovery.Policy.RequireHttps = false;
            var doc = await discovery.GetAsync();
            var tokenClient = new TokenClient(doc.TokenEndpoint, "KN0KJCV4KJA9HE3", "ALbj9aak26wRHUmSh8xj8GFYzRiVBC");
            var requestRefreshToken = tokenClient.RequestRefreshTokenAsync(refreshToken).Result;
            return requestRefreshToken.AccessToken;
        }

    }
}
