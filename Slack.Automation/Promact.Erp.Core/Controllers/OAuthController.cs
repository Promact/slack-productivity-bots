using NLog;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringLiteral;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class OAuthController : BaseController
    {
        #region Private Variables

        private readonly IHttpClientService _httpClientService;
        private readonly IRepository<SlackChannelDetails> _slackChannelDetails;
        private readonly ILogger _loggerSlackEvent;
        private readonly IOAuthLoginRepository _oAuthLoginRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly ApplicationUserManager _userManager;
        private readonly IOauthCallHttpContextRespository _oauthCallRepository;

        #endregion

        #region Constructor
        public OAuthController(IHttpClientService httpClientService, ISingletonStringLiteral stringConstantRepository,
            ISlackUserRepository slackUserRepository, IRepository<SlackChannelDetails> slackChannelDetails, IOAuthLoginRepository oAuthLoginRepository,
            ApplicationUserManager userManager, ISlackChannelRepository slackChannelRepository,
            IOauthCallHttpContextRespository oauthCallRepository) : base(stringConstantRepository)
        {
            _httpClientService = httpClientService;
            _loggerSlackEvent = LogManager.GetLogger("SlackEvent");
            _slackChannelDetails = slackChannelDetails;
            _oAuthLoginRepository = oAuthLoginRepository;
            _userManager = userManager;
            _slackUserRepository = slackUserRepository;
            _slackChannelRepository = slackChannelRepository;
            _oauthCallRepository = oauthCallRepository;
        }
        #endregion

        #region Public Methods
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
                message = _stringConstantRepository.SlackAppAdded;
            }
            catch (SlackAuthorizeException authEx)
            {
                errorMessage = string.Format(_stringConstantRepository.ControllerErrorMessageStringFormat, _stringConstantRepository.LoggerErrorMessageOAuthControllerSlackDetailsAdd, authEx.ToString());
                message = _stringConstantRepository.SlackAppError + authEx.Message;
            }
            var newUrl = this.Url.Link(_stringConstantRepository.Default, new
            {
                Controller = _stringConstantRepository.Home,
                Action = _stringConstantRepository.SlackAuthorizeAction,
                message = message
            });
            return Redirect(newUrl);
        }

        /**
        * @api {get} oauth/userIsAdmin
        * @apiVersion 1.0.0
        * @apiName CurrentUserIsAdminOrNot
        * @apiGroup OAuth  
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *       "FirstName : "Siddhartha",
        *       "IsAdmin" : true
        * }
        */
        [HttpGet]
        [Route("oauth/user/admin")]
        public async Task<IHttpActionResult> CurrentUserIsAdminOrNot()
        {
            UserAdminBasicDetailsAC userDetails = new UserAdminBasicDetailsAC();
            userDetails.FirstName = (await _oauthCallRepository.GetUserByEmployeeIdAsync(GetUserId(User.Identity))).FirstName;
            userDetails.IsAdmin = await _oauthCallRepository.CurrentUserIsAdminAsync();
            return Ok(userDetails);
        }
        #endregion
    }
}