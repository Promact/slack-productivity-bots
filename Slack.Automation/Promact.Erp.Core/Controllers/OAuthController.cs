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

        /**
        * @api {get} oAuth/RefreshToken
        * @apiVersion 1.0.0
        * @apiName RefreshToken
        * @apiGroup RefreshToken  
        * @apiParam {string} Name    refreshToken
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *   {
        *       "ClientId":"dastvgs3rt2031srtgr54dgrf",
        *       "ClientSecret":"frwhklsjelkjsktjlk656f5dyhddvsfdgv",
        *       "RefreshToken":"acjshrkjajjsdfxo",
        *       "ReturnUrl":"http://localhost:28182/Home/ExtrenalLoginCallBack"
        *   }
        * }
        */
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

        /**
        * @api {get} oAuth/SlackRequest
        * @apiVersion 1.0.0
        * @apiName SlackOAuth
        * @apiGroup SlackOAuth  
        * @apiParam {string} Name    code
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *       "Description":"This will add slack user, channel and group in application"
        * }
        */
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
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, StringConstant.LoggerErrorMessageOAuthControllerSlackOAuth);
                throw ex;
            }
        }

        /**
        * @api {post} slack/eventAlert
        * @apiVersion 1.0.0
        * @apiName SlackEvent
        * @apiGroup SlackEvent  
        * @apiParam {SlackEventApiAC} Name    slackEvent
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *       "Description":"This method will be hitted when there is any changes in slack user list
        * }
        */
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
