using Autofac.Extras.NLog;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.EnvironmentVariableRepository;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Promact.Erp.Util.StringConstants;
using System.Net.Http;

namespace Promact.Erp.Core.Controllers
{
    public class HomeController : MVCBaseController
    {
        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationUserManager _userManager;
        private readonly ILogger _logger;
        private readonly IOAuthLoginRepository _oAuthLoginRepository;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IStringConstantRepository _stringConstant;

        public HomeController(ApplicationUserManager userManager, IStringConstantRepository stringConstant, ApplicationSignInManager signInManager, ILogger logger, IOAuthLoginRepository oAuthLoginRepository, IEnvironmentVariableRepository envVariableRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _oAuthLoginRepository = oAuthLoginRepository;
            _envVariableRepository = envVariableRepository;
            _stringConstant = stringConstant;
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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(_stringConstant.AfterLogIn, _stringConstant.Home);
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
        public ActionResult AfterLogIn()
        {
            return View();
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
            try
            {
                ViewBag.Message = message;
                return View();
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageHomeControllerAuthorizeStatusPage, ex.ToString());
                _logger.Error(errorMessage, ex);
                throw ex;
            }
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
                    return RedirectToAction(_stringConstant.AfterLogIn, _stringConstant.Home);
                }
                //BaseUrl of OAuth and clientId of App to be set 
                var url = string.Format(_stringConstant.ExternalLoginUrl, _stringConstant.OAuthUrl, _envVariableRepository.PromactOAuthClientId);
                //make call to the OAuth Server
                return Redirect(url);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageHomeControllerExtrenalLogin, ex.ToString());
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
        public async Task<ActionResult> ExtrenalLoginCallBack(string accessToken, string email, string slackUserId, string userId)
        {
            try
            {
                ApplicationUser user = _userManager.FindByEmail(email);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, false, false);
                    return RedirectToAction(_stringConstant.AfterLogIn, _stringConstant.Home);
                }
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction(_stringConstant.AfterLogIn, _stringConstant.Home);
                }
                if (user == null)
                {
                    user = await _oAuthLoginRepository.AddNewUserFromExternalLoginAsync(email, accessToken, slackUserId, userId);
                    if (user != null)
                    {
                        //Signing user with username or email only
                        await _signInManager.SignInAsync(user, false, false);
                        return RedirectToAction(_stringConstant.AfterLogIn, _stringConstant.Home);
                    }
                    return RedirectToAction(_stringConstant.SlackAuthorize, _stringConstant.Home, new { message = _stringConstant.UserCouldNotBeAdded });
                }
                return View();
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageHomeControllerExtrenalLoginCallBack, ex.ToString());
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
                return RedirectToAction(_stringConstant.Index, _stringConstant.Home);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(_stringConstant.ControllerErrorMessageStringFormat, _stringConstant.LoggerErrorMessageHomeControllerLogoff, ex.ToString());
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
            try
            {
                return Redirect(_stringConstant.LeaveManagementAuthorizationUrl + _stringConstant.OAuthAuthorizationScopeAndClientId + _envVariableRepository.SlackOAuthClientId);
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = string.Format("{0}. Error -> {1}", _stringConstant.LoggerErrorMessageHomeControllerSlackOAuthAuthorization, ex.ToString());
                _logger.Error(errorMessage, ex);
                throw;
            }
        }
    }
}
