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
        #region Private Variables
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
        #endregion

        #region Constructor
        public OAuthController(IHttpClientService httpClientService, IStringConstantRepository stringConstant, ISlackUserRepository slackUserRepository, ILogger logger, IRepository<SlackChannelDetails> slackChannelDetails, IOAuthLoginRepository oAuthLoginRepository)
        {
            _httpClientService = httpClientService;
            _logger = logger;
            _stringConstant = stringConstant;
            _slackChannelDetails = slackChannelDetails;
            _oAuthLoginRepository = oAuthLoginRepository;
            _slackUserRepository = slackUserRepository;
        }
        #endregion

        #region Private Methods
        /**
        * @api {get} oauth/refreshtoken
        * @apiVersion 1.0.0
        * @apiName RefreshTokenAsync
        * @apiGroup OAuth  
        * @apiParam {string} Name    refreshToken
        * @apiParam {string} Name    slackUserName
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *   {
        *       "ClientId":"dastvgs3rt2031srtgr54dgrf",
        *       "ClientSecret":"frwhklsjelkjsktjlk656f5dyhddvsfdgv",
        *       "RefreshToken":"acjshrkjajjsdfxo",
        *       "ReturnUrl":"http://localhost:28182/Home/ExtrenalLoginCallBack",
        *       "UserId":"JFF414GSDF"
        *   }
        * }
        */
        [HttpGet]
        [Route("oauth/refreshtoken")]
        public async Task<IHttpActionResult> RefreshTokenAsync(string refreshToken, string slackUserName)
        {
            var oAuth = _oAuthLoginRepository.ExternalLoginInformation(refreshToken);
            SlackUserDetailAc user = await _slackUserRepository.GetBySlackNameAsync(slackUserName);
            if (user != null)
                oAuth.UserId = user.UserId;
            return Ok(oAuth);
        }

        /**
        * @api {get} oauth/slackoauthrequest
        * @apiVersion 1.0.0
        * @apiName SlackOAuthAsync
        * @apiGroup SlackOAuth  
        * @apiParam {string} Name    code
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *       "Description":"This will add slack user, channel and group in application and redirect to appropriate page and display proper message"
        * }
        * @apiErrorExample {json} Error-Response:
        * HTTP/1.1 200 OK 
        * {
        *       "error":"This will redirect to appropriate page and display proper error message"
        * }
        */
        [HttpGet]
        [Route("oauth/slackoauthrequest")]
        public async Task<IHttpActionResult> SlackOAuthAsync(string code)
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
                errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageOAuthControllerSlackDetailsAdd, authEx.ToString());
                message = _stringConstant.SlackAppError + authEx.Message;
            }
            catch (Exception ex)
            {
                errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageOAuthControllerSlackOAuth, ex.ToString());
                message = _stringConstant.SlackAppError + ex.Message;
            }
            _logger.Error(errorMessage);
            var newUrl = this.Url.Link(_stringConstant.Default, new
            {
                Controller = _stringConstant.Home,
                Action = _stringConstant.SlackAuthorizeAction,
                message = message
            });
            return Redirect(newUrl);
        }

        /**
        * @api {post} slack/eventalert
        * @apiVersion 1.0.0
        * @apiName SlackEventAsync
        * @apiGroup SlackOAuth  
        * @apiParam {SlackEventApiAC} Name    slackEvent
        * @apiParamExample {json} Request-Example:
        * {
        *       "token":"Jhj5dZrVaK7ZwHHjRyZWjbDl",
        *       "challenge":"3eZbrw1aBm2rZgRNFdxV2595E9CY3gmdALWMmHkvFXO7tYXAYM8P",
        *       "type":"url_verification",
        *       "team_id":"T061EG9RZ",
        *       "api_app_id":"A0FFV41KK",
        *       "event_ts":"1465244570.336841",
        *       "authed_users":"
                    ["U061F7AUR"]",
        *       "event":"
        *       {
                "type": "reaction_added",
                "user": "U061F1EUR",
                "item": 
                    {
                        "type": "message",
                        "channel": "C061EG9SL",
                        "ts": "1464196127.000002"
                    },
                    "reaction": "slightly_smiling_face"
                },"
        * }  
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *       "Description":"This method will be hitted when there is any changes in slack user list or channel list
        * }
        * @apiErrorExample {json} Error-Response:
        * HTTP/1.1 200 OK 
        * {
        *       "Description":"This method will be hitted when there is any changes in slack user list or channel list
        * }
        */
        [HttpPost]
        [Route("slack/eventalert")]
        public async Task<IHttpActionResult> SlackEventAsync(SlackEventApiAC slackEvent)
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
                        await _oAuthLoginRepository.SlackEventUpdateAsync(events);
                        eventQueue.Dequeue();
                        return Ok();
                    }
                    else if (eventType == _stringConstant.UserChange)
                    {
                        //if (!slackEvent.Event.User.IsBot)
                        await _slackUserRepository.UpdateSlackUserAsync(events.Event.User);
                        eventQueue.Dequeue();
                        return Ok();
                    }
                    else if (eventType == _stringConstant.ChannelCreated || eventType == _stringConstant.ChannelRename || eventType == _stringConstant.GroupRename)
                    {
                        await _oAuthLoginRepository.SlackChannelAddAsync(events);
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
                var errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageOAuthControllerSlackEvent, userEx.ToString());
                _logger.Error(errorMessage, userEx);
                throw userEx;
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageOAuthControllerSlackEvent, ex.ToString());
                _logger.Error(errorMessage, ex);
                throw ex;
            }
        }
        #endregion
    }
}
