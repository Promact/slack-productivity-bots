using Newtonsoft.Json;
using NLog;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class OAuthController : WebApiBaseController
    {
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        private readonly IRepository<SlackChannelDetails> _slackChannelDetails;
        private readonly ILogger _logger;
        public OAuthController(IHttpClientRepository httpClientRepository, IRepository<SlackUserDetails> slackUserDetails, ILogger logger, IRepository<SlackChannelDetails> slackChannelDetails)
        {
            _httpClientRepository = httpClientRepository;
            _slackUserDetails = slackUserDetails;
            _logger = logger;
            _slackChannelDetails = slackChannelDetails;
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
            try
            {
                var clientId = Environment.GetEnvironmentVariable(StringConstant.PromactOAuthClientId, EnvironmentVariableTarget.User);
                var clientSecret = Environment.GetEnvironmentVariable(StringConstant.PromactOAuthClientSecret, EnvironmentVariableTarget.User);
                OAuthApplication oAuth = new OAuthApplication();
                oAuth.ClientId = clientId;
                oAuth.ClientSecret = clientSecret;
                oAuth.RefreshToken = refreshToken;
                oAuth.ReturnUrl = StringConstant.ClientReturnUrl;
                return Ok(oAuth);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, StringConstant.LoggerErrorMessageOAuthControllerRefreshToken);
                throw ex;
            }
        }

        /// <summary>
        /// Method to Authorize user/team from slack OAuth and get access token and basic information corresponding to user for the app
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("oAuth/SlackRequest")]
        public async Task<IHttpActionResult> SlackOAuth(string code)
        {
            try
            {
                var slackOAuthRequest = string.Format("?client_id={0}&client_secret={1}&code={2}&pretty=1", Environment.GetEnvironmentVariable(StringConstant.SlackOAuthClientId, EnvironmentVariableTarget.User), Environment.GetEnvironmentVariable(StringConstant.SlackOAuthClientSecret, EnvironmentVariableTarget.User), code);
                var slackOAuthResponse = await _httpClientRepository.GetAsync(StringConstant.OAuthAcessUrl, slackOAuthRequest, null);
                var slackOAuth = JsonConvert.DeserializeObject<SlackOAuthResponse>(slackOAuthResponse);
                var userDetailsRequest = string.Format("?token={0}&pretty=1", slackOAuth.AccessToken);
                var userDetailsResponse = await _httpClientRepository.GetAsync(StringConstant.SlackUserListUrl, userDetailsRequest, null);
                var slackUsers = JsonConvert.DeserializeObject<SlackUserResponse>(userDetailsResponse);
                foreach (var user in slackUsers.Members)
                {
                    if (user.Name != StringConstant.SlackBotStringName)
                    {
                        _slackUserDetails.Insert(user);
                        _slackUserDetails.Save();
                    }
                }
                var channelDetailsResponse = await _httpClientRepository.GetAsync(StringConstant.SlackChannelListUrl, userDetailsRequest, null);
                var channels = JsonConvert.DeserializeObject<SlackChannelResponse>(channelDetailsResponse);
                foreach (var channel in channels.Channels)
                {
                    _slackChannelDetails.Insert(channel);
                    _slackChannelDetails.Save();
                }

                var groupDetailsResponse = await _httpClientRepository.GetAsync(StringConstant.SlackGroupListUrl, userDetailsRequest, null);
                var groups = JsonConvert.DeserializeObject<SlackGroupDetails>(groupDetailsResponse);
                foreach (var channel in groups.Groups)
                {
                    _slackChannelDetails.Insert(channel);
                    _slackChannelDetails.Save();
                }
                return Ok(slackOAuth);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, StringConstant.LoggerErrorMessageOAuthControllerSlackOAuth);
                throw ex;
            }
        }

        /// <summary>
        /// Method to handle the slack event. Event is when a new user will join the team
        /// </summary>
        /// <param name="slackEvent"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("slack/eventAlert")]
        public IHttpActionResult SlackEvent(SlackEventApiAC slackEvent)
        {
            try
            {
                if (slackEvent.Type == "url_verification")
                {
                    return Ok(slackEvent.Challenge);
                }
                if (slackEvent.Type == "team_join")
                {
                    slackEvent.Event.User.TeamId = slackEvent.TeamId;
                    _slackUserDetails.Insert(slackEvent.Event.User);
                    _slackUserDetails.Save();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, StringConstant.LoggerErrorMessageOAuthControllerSlackEvent);
                throw ex;
            }
        }
    }
}
