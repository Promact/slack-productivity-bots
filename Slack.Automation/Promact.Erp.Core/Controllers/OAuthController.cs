using Newtonsoft.Json;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class OAuthController : WebApiBaseController
    {
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        public OAuthController(IHttpClientRepository httpClientRepository, IRepository<SlackUserDetails> slackUserDetails)
        {
            _httpClientRepository = httpClientRepository;
            _slackUserDetails = slackUserDetails;
        }
        /// <summary>
        /// Method to get refresh Token from OAuth and send app clientSecretId
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("oAuth/RefreshToken")]
        public IHttpActionResult RefreshToken(string refreshToken)
        {
            var clientId = AppSettingsUtil.ClientId;
            var clientSecret = AppSettingsUtil.ClientSecret;
            OAuthApplication oAuth = new OAuthApplication();
            oAuth.ClientId = clientId;
            oAuth.ClientSecret = clientSecret;
            oAuth.RefreshToken = refreshToken;
            oAuth.ReturnUrl = AppSettingsUtil.ClientReturnUrl;
            return Ok(oAuth);
        }

        /// <summary>
        /// Method to Authorize user/team from slack OAuth and get access token and basic information corresponding to user for the app
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("oAuth/SlackRequest")]
        public async Task<IHttpActionResult> OAuth(string code)
        {
            var slackOAuthRequest = string.Format("?client_id={0}&client_secret={1}&code={2}&pretty=1", AppSettingsUtil.OAuthClientId, AppSettingsUtil.OAuthClientSecret, code);
            var slackOAuthResponse = await _httpClientRepository.GetAsync(StringConstant.OAuthAcessUrl, slackOAuthRequest, null);
            var slackOAuth = JsonConvert.DeserializeObject<SlackOAuthResponse>(slackOAuthResponse);
            var userDetailsRequest = string.Format("?token={0}&pretty=1", slackOAuth.AccessToken);
            var userDetailsResponse = await _httpClientRepository.GetAsync(StringConstant.SlackUserListUrl,userDetailsRequest,null);
            var slackUsers = JsonConvert.DeserializeObject<SlackUserResponse>(userDetailsResponse);
            foreach (var user in slackUsers.Members)
            {
                _slackUserDetails.Insert(user);
                _slackUserDetails.Save();
            }
            return Ok(slackOAuth);
        }
    }
}
