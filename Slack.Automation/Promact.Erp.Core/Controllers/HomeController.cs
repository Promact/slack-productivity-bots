using Autofac.Extras.NLog;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Promact.Erp.Core.Controllers
{
    public class HomeController : MVCBaseController
    {
        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationUserManager _userManager;
        private readonly ILogger _logger;
        private readonly IOAuthLoginRepository _oAuthLoginRepository;
        private readonly IEnvironmentVariableRepository _envVariableRepository;

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ILogger logger, IOAuthLoginRepository oAuthLoginRepository, IEnvironmentVariableRepository envVariableRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _oAuthLoginRepository = oAuthLoginRepository;
            _envVariableRepository = envVariableRepository;
        }

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
        public ActionResult AfterLogIn()
        {
            return View();
        }

        /**
        * @api {get} Home/ExtrenalLogin
        * @apiVersion 1.0.0
        * @apiName ExtrenalLogin
        * @apiGroup ExtrenalLogin    
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"Will redirect to OAuth server for external login"
        * }
        */
        public ActionResult ExtrenalLogin()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction(StringConstant.AfterLogIn, StringConstant.Home);
                }
                //BaseUrl of OAuth and clientId of App to be set 
                var url = string.Format("{0}?clientId={1}", StringConstant.OAuthUrl, _envVariableRepository.PromactOAuthClientId);
                //make call to the OAuth Server
                return Redirect(url);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("{0}. Error -> {1}", StringConstant.LoggerErrorMessageHomeControllerExtrenalLogin, ex.ToString());
                _logger.Error(errorMessage, ex);
                throw ex;
            }
        }

        /**
        * @api {get} Home/ExtrenalLoginCallBack
        * @apiVersion 1.0.0
        * @apiName ExtrenalLoginCallBack
        * @apiGroup ExtrenalLoginCallBack 
        * @apiParam {string} Name  accessToken
        * @apiParam {string} Name  email
        * @apiParam {string} Name  slackUserName
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"Redirect to a view page of application and user will be added from external OAuth to our application"
        * }
        */
        public async Task<ActionResult> ExtrenalLoginCallBack(string accessToken, string email, string slackUserName)
        {
            try
            {
                var user = _userManager.FindByEmail(email);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, false, false);
                    return RedirectToAction(StringConstant.AfterLogIn, StringConstant.Home);
                }
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction(StringConstant.AfterLogIn, StringConstant.Home);
                }
                if (user == null)
                {
                    user = await _oAuthLoginRepository.AddNewUserFromExternalLogin(email, accessToken, slackUserName);
                    //Signing user with username or email only
                    await _signInManager.SignInAsync(user, false, false);
                    return RedirectToAction(StringConstant.AfterLogIn, StringConstant.Home);
                }
                return View();
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("{0}. Error -> {1}", StringConstant.LoggerErrorMessageHomeControllerExtrenalLoginCallBack, ex.ToString());
                _logger.Error(errorMessage, ex);
                throw ex;
            }
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
            try
            {
                AuthenticationManager.SignOut();
                return RedirectToAction(StringConstant.Index, StringConstant.Home);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("{0}. Error -> {1}", StringConstant.LoggerErrorMessageHomeControllerLogoff, ex.ToString());
                _logger.Error(errorMessage, ex);
                throw ex;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        /**
        * @api {get} Home/SlackOAuthAuthorization
        * @apiVersion 1.0.0
        * @apiName SlackOAuthAuthorization
        * @apiGroup SlackOAuthAuthorization    
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"Add to slack button will redirect here and it will open a Slack OAuth Authorization Page for our app"
        * }
        */
        public ActionResult SlackOAuthAuthorization()
        {
            return Redirect(StringConstant.LeaveManagementAuthorizationUrl + StringConstant.OAuthAuthorizationScopeAndClientId + _envVariableRepository.SlackOAuthClientId);
        }
    }
}
