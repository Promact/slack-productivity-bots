using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.EnvironmentVariableRepository;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Promact.Erp.Util.StringConstants;
using System.Net.Http;
using Promact.Erp.Util.HashingMd5;
using System;
using Promact.Erp.DomainModel.ApplicationClass;
using NLog;
using Promact.Core.Repository.GroupRepository;
using Promact.Core.Repository.ConfigurationRepository;

namespace Promact.Erp.Core.Controllers
{
    public class HomeController : MVCBaseController
    {
        #region Private Variables

        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IOAuthLoginRepository _oAuthLoginRepository;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ILogger _logger;
        private readonly IMd5Service _md5Service;
        private readonly IConfigurationRepository _configurationRepository;
        #endregion

        #region Constructor

        public HomeController(ApplicationUserManager userManager, IStringConstantRepository stringConstant,
            ApplicationSignInManager signInManager, IOAuthLoginRepository oAuthLoginRepository,
            IEnvironmentVariableRepository envVariableRepository, IMd5Service md5Service, IGroupRepository groupRepository,
            IConfigurationRepository configurationRepository) : base(stringConstant)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _oAuthLoginRepository = oAuthLoginRepository;
            _envVariableRepository = envVariableRepository;
            _logger = LogManager.GetLogger("AuthenticationModule");
            _md5Service = md5Service;
            _groupRepository = groupRepository;
            _configurationRepository = configurationRepository;
        }

        #endregion

        #region Public Methods
        /**
        * @api {get} Home/Index
        * @apiVersion 1.0.0
        * @apiName Index
        * @apiGroup Index    
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"Open the first/login page of the application"
        * }
        */
        public ActionResult Index()
        {
            _logger.Info("Index: Today " + DateTime.Today + "\n Today's Date :" + DateTime.Today.Date);
            _logger.Debug("User is login :" + User.Identity.IsAuthenticated);
            if (User.Identity.IsAuthenticated)
            {
                _logger.Info("User is Authenticated");
                return RedirectToAction(_stringConstantRepository.AfterLogIn, _stringConstantRepository.Home);
            }
            return View();
        }
        /**
        * @api {get} Home/AfterLogIn
        * @apiVersion 1.0.0
        * @apiName AfterLogIn
        * @apiGroup AfterLogIn    
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"After Login from OAuth server Page will be redirected to this page and will open a view of application"
        * }
        */
        [Authorize]
        public async Task<ActionResult> AfterLogIn()
        {
            _logger.Info("AfterLogIn: Today " + DateTime.Today + "\n Today's Date :" + DateTime.Today.Date);
            string userId = GetUserId(User.Identity);
            //this for get login user email address and encrypt hash code.
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                EmailHashCodeAC emailHaseCodeAC = new EmailHashCodeAC(_md5Service.GetMD5HashData(user.Email.ToLower()));
                ViewBag.emailHashCode = emailHaseCodeAC;
                await _groupRepository.AddDynamicGroupAsync();
                return View();
            }
            else
            {
                Request.GetOwinContext().Authentication.SignOut("Cookies");
                return RedirectToAction(_stringConstantRepository.SetUpError, _stringConstantRepository.Home);
            }
        }


        /**
       * @api {get} Home/SlackAuthorize
       * @apiVersion 1.0.0
       * @apiName SlackAuthorize
       * @apiGroup SlackAuthorize   
       * @apiParam {string} Name  message 
       * @apiSuccessExample {json} Success-Response:
       * HTTP/1.1 200 OK 
       * {
       *     "Description":"After Slack OAuth Authorization, user is redirected here with the status of Authorization message."
       * }
       */
        public ActionResult SlackAuthorize(string message)
        {
            _logger.Info("SlackAuthorize: Today " + DateTime.Today + "\n Today's Date :" + DateTime.Today.Date);
            ViewBag.Message = message;
            return View();
        }

        /**
        * @api {get} Home/LogOff
        * @apiVersion 1.0.0
        * @apiName LogOff
        * @apiGroup LogOff    
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"SignOut from our application"
        * }
        */
        public ActionResult LogOff()
        {
            //Set the cookie to expire
            Request.GetOwinContext().Authentication.SignOut("Cookies");
            return RedirectToAction(_stringConstantRepository.Index, _stringConstantRepository.Home);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        /**
        * @api {get} Home/SlackOAuthAuthorization?configurationId=7
        * @apiVersion 1.0.0
        * @apiName SlackOAuthAuthorization
        * @apiGroup SlackOAuthAuthorization   
        * @apiParam {int} configurationId  configurationId 
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"Add to slack button will redirect here and it will open a Slack OAuth Authorization Page for our app"
        * }
        */
        [Authorize]
        public async Task<ActionResult> SlackOAuthAuthorization(int configurationId)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var appCredential = await _configurationRepository.GetAppCredentialsByConfigurationIdAsync(configurationId);
                    if (!string.IsNullOrEmpty(appCredential?.ClientId))
                    {
                        if (String.Compare(appCredential.Module, _stringConstantRepository.Scrum, true) == 0)
                            return Redirect(_stringConstantRepository.LeaveManagementAuthorizationUrl + _stringConstantRepository.ScrumBotScopeAndClientId + appCredential.ClientId);
                        else if (String.Compare(appCredential.Module, _stringConstantRepository.TaskModule, true) == 0)
                            return Redirect(_stringConstantRepository.LeaveManagementAuthorizationUrl + _stringConstantRepository.TaskBotScopeAndClientId + appCredential.ClientId);
                        else if (String.Compare(appCredential.Module, _stringConstantRepository.LeaveModule, true) == 0)
                            return Redirect(_stringConstantRepository.LeaveManagementAuthorizationUrl + _stringConstantRepository.LeaveSlashCommandScopeAndClientId + appCredential.ClientId);
                        else if (String.Compare(appCredential.Module, _stringConstantRepository.RedmineModule, true) == 0)
                            return Redirect(_stringConstantRepository.LeaveManagementAuthorizationUrl + _stringConstantRepository.RedmineSlashCommandScopeAndClientId + appCredential.ClientId);
                    }
                }
                await _configurationRepository.DisableAppByConfigurationIdAsync(configurationId);
                return RedirectToAction(_stringConstantRepository.Index, _stringConstantRepository.Home);
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = string.Format("{0}. Error -> {1}", _stringConstantRepository.LoggerErrorMessageHomeControllerSlackOAuthAuthorization, ex.ToString());
                _logger.Error(errorMessage, ex);
                throw;
            }
        }

        public ActionResult SetUpError()
        {
            ViewBag.Message = _stringConstantRepository.PleaseAskYouAdminToAddSlackAppAndDoSetUp;
            return View();
        }
        #endregion
    }
}
