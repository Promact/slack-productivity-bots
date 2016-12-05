using Autofac.Extras.NLog;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class OAuthController : WebApiBaseController
    {
        private static Queue<SlackEventApiAC> eventQueue;
        private readonly IHttpClientService _httpClientService;
        private readonly IRepository<SlackChannelDetails> _slackChannelDetails;
        private readonly ILogger _logger;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IOAuthLoginRepository _oAuthLoginRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        static OAuthController()
        {
            eventQueue = new Queue<SlackEventApiAC>();
        }
        public OAuthController(IHttpClientService httpClientService, IStringConstantRepository stringConstant, ISlackUserRepository slackUserRepository, ILogger logger, IRepository<SlackChannelDetails> slackChannelDetails, IOAuthLoginRepository oAuthLoginRepository)
        {
            _httpClientService = httpClientService;
            _logger = logger;
            _stringConstant = stringConstant;
            _slackChannelDetails = slackChannelDetails;
            _oAuthLoginRepository = oAuthLoginRepository;
            _slackUserRepository = slackUserRepository;
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
        public IHttpActionResult RefreshToken(string refreshToken, string slackUserName)
        {
            var oAuth = _oAuthLoginRepository.ExternalLoginInformationAsync(refreshToken);
            SlackUserDetailAc user = _slackUserRepository.GetBySlackName(slackUserName);
            if (user != null)
                oAuth.UserId = user.UserId;
            return Ok(oAuth);
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
                await _oAuthLoginRepository.AddSlackUserInformationAsync(code);
                message = _stringConstant.SlackAppAdded;
            }
            catch (SlackAuthorizeException authEx)
            {
                errorMessage = string.Format("{0}. Error -> {1}", _stringConstant.LoggerErrorMessageOAuthControllerSlackDetailsAdd, authEx.ToString());
                message = _stringConstant.SlackAppError + authEx.Message;
            }
            catch (Exception ex)
            {
                errorMessage = string.Format("{0}. Error -> {1}", _stringConstant.LoggerErrorMessageOAuthControllerSlackOAuth, ex.ToString());
                message = _stringConstant.SlackAppError + ex.Message;
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
        *       "Description":"This method will be hitted when there is any changes in slack user list or channel list
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
                eventQueue.Enqueue(slackEvent);
                foreach (var events in eventQueue)
                {
                    string eventType = slackEvent.Event.Type;
                    if (eventType == _stringConstant.TeamJoin)
                    {
                        //if (!slackEvent.Event.User.IsBot)
                        _oAuthLoginRepository.SlackEventUpdate(events);
                        eventQueue.Dequeue();
                        return Ok();
                    }
                    else if (eventType == _stringConstant.UserChange)
                    {
                        //if (!slackEvent.Event.User.IsBot)
                        _slackUserRepository.UpdateSlackUser(events.Event.User);
                        eventQueue.Dequeue();
                        return Ok();
                    }
                    else if (eventType == _stringConstant.ChannelCreated || eventType == _stringConstant.ChannelRename || eventType == _stringConstant.GroupRename)
                    {
                        _oAuthLoginRepository.SlackChannelAdd(events);
                        eventQueue.Dequeue();
                        return Ok();
                    }
                    else
                    {
                        eventQueue.Dequeue();
                        return BadRequest();
                    }
                }
                return null;
            }
            catch (SlackUserNotFoundException userEx)
            {
                var errorMessage = string.Format("{0}. Error -> {1}", _stringConstant.LoggerErrorMessageOAuthControllerSlackEvent, userEx.ToString());
                _logger.Error(errorMessage, userEx);
                throw userEx;
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
