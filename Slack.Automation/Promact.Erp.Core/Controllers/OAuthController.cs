using Autofac.Extras.NLog;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.Util.StringConstants;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class OAuthController : WebApiBaseController
    {
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly IRepository<SlackChannelDetails> _slackChannelDetails;
        private readonly ILogger _logger;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IOAuthLoginRepository _oAuthLoginRepository;
        public OAuthController(IHttpClientRepository httpClientRepository, IStringConstantRepository stringConstant, ILogger logger, IRepository<SlackChannelDetails> slackChannelDetails, IOAuthLoginRepository oAuthLoginRepository)
        {
            _httpClientRepository = httpClientRepository;
            _logger = logger;
            _stringConstant = stringConstant;
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
                var errorMessage = string.Format("{0}. Error -> {1}", _stringConstant.LoggerErrorMessageOAuthControllerRefreshToken, ex.ToString());
                _logger.Error(errorMessage, ex);
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
            string message = string.Empty;
            var errorMessage = string.Empty;
            try
            {
                await _oAuthLoginRepository.AddSlackUserInformation(code);
                message = StringConstant.SlackAppAdded;
            }
            catch (SlackAuthorizeException authEx)
            {
                errorMessage = string.Format("{0}. Error -> {1}", StringConstant.LoggerErrorMessageOAuthControllerSlackDetailsAdd, authEx.ToString());
                message = StringConstant.SlackAppError + authEx.Message;
            }
            catch (Exception ex)
            {
                errorMessage = string.Format("{0}. Error -> {1}", StringConstant.LoggerErrorMessageOAuthControllerSlackOAuth, ex.ToString());
                message = StringConstant.SlackAppError + ex.Message;
            }
            _logger.Error(errorMessage);
            var newUrl = this.Url.Link("Default", new
            {
                Controller = "Home",
                Action = "SlackAuthorize",
                message = message
            });
            return Redirect(newUrl);
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
                if (slackEvent.Type == _stringConstant.VerificationUrl)
                {
                    return Ok(slackEvent.Challenge);
                }
                if (slackEvent.Event.Type == _stringConstant.TeamJoin)
                {
                    _oAuthLoginRepository.SlackEventUpdate(slackEvent);
                    return Ok();
                }
                else if(slackEvent.Event.Type == "channel_created" || slackEvent.Event.Type =="group_open")
                {
                   _oAuthLoginRepository.SlackChannelAdd(slackEvent);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("{0}. Error -> {1}", _stringConstant.LoggerErrorMessageOAuthControllerSlackEvent, ex.ToString());
                _logger.Error(errorMessage, ex);
                throw ex;
            }
        }
    }
}
