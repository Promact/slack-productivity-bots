using Newtonsoft.Json;
using NLog;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.ExternalLoginRepository;
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
        private readonly IOAuthLoginRepository _oAuthLoginRepository;
        public OAuthController(IHttpClientRepository httpClientRepository, IRepository<SlackUserDetails> slackUserDetails, ILogger logger, IRepository<SlackChannelDetails> slackChannelDetails, IOAuthLoginRepository oAuthLoginRepository)
        {
            _httpClientRepository = httpClientRepository;
            _slackUserDetails = slackUserDetails;
            _logger = logger;
            _slackChannelDetails = slackChannelDetails;
            _oAuthLoginRepository = oAuthLoginRepository;
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
                var oAuth = _oAuthLoginRepository.ExternalLoginInformation(refreshToken);
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
                await _oAuthLoginRepository.AddSlackUserInformation(code);
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
                if (slackEvent.Type == StringConstant.VerificationUrl)
                {
                    return Ok(slackEvent.Challenge);
                }
                if (slackEvent.Type == StringConstant.TeamJoin)
                {
                    _oAuthLoginRepository.SlackEventUpdate(slackEvent);
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
