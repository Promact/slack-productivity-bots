using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NLog;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Promact.Erp.Core.Controllers
{
    public class HomeController : MVCBaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ILogger _logger;

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ILogger logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// After Login from OAuth server Page will be redirected to this page
        /// </summary>
        /// <returns></returns>
        public ActionResult AfterLogIn()
        {
            return View();
        }

        /// <summary>
        /// External Login Method. It will call and external OAuth for Login
        /// <returns></returns>
        public ActionResult ExtrenalLogin()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction(StringConstant.AfterLogIn, StringConstant.Home);
                }
                //BaseUrl of OAuth and clientId of App to be set 
                var url = string.Format("{0}?clientId={1}", StringConstant.OAuthUrl, Environment.GetEnvironmentVariable(StringConstant.PromactOAuthClientId, EnvironmentVariableTarget.User));
                //make call to the OAuth Server
                return Redirect(url);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, StringConstant.LoggerErrorMessageHomeControllerExtrenalLogin);
                throw ex;
            }
        }

        /// <summary>
        /// Method will recieve access token and email and user will register here
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExtrenalLoginCallBack(string accessToken, string email, string slackUserName)
        {
            try
            {
                var user = _userManager.FindByEmail(email);
                if (user != null)
                {
                    UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, accessToken);
                    await _userManager.AddLoginAsync(user.Id, info);
                    await _signInManager.SignInAsync(user, false, false);
                    return RedirectToAction(StringConstant.AfterLogIn, StringConstant.Home);
                }
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction(StringConstant.AfterLogIn, StringConstant.Home);
                }
                if (user == null)
                {
                    user = new ApplicationUser() { Email = email, UserName = email, SlackUserName = slackUserName };
                    //Creating a user with email only. Password not required
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        //Adding external Oauth details
                        UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, accessToken);
                        result = await _userManager.AddLoginAsync(user.Id, info);
                        if (result.Succeeded)
                        {
                            //Signing user with username or email only
                            await _signInManager.SignInAsync(user, false, false);
                            return RedirectToAction(StringConstant.AfterLogIn, StringConstant.Home);
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, StringConstant.LoggerErrorMessageHomeControllerExtrenalLoginCallBack);
                throw ex;
            }
        }

        /// <summary>
        /// Method to signOut from our application not from OAuth server
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            try
            {
                AuthenticationManager.SignOut();
                return RedirectToAction(StringConstant.Index, StringConstant.Home);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, StringConstant.LoggerErrorMessageHomeControllerLogoff);
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

        /// <summary>
        /// Add to slack button will redirect here and it will open a Slack OAuth Authorization Page for our app
        /// </summary>
        /// <returns></returns>
        public ActionResult SlackOAuth()
        {
            return Redirect(StringConstant.LeaveManagementAuthorizationUrl + StringConstant.OAuthAuthorizationScopeAndClientId + Environment.GetEnvironmentVariable(StringConstant.SlackOAuthClientId, EnvironmentVariableTarget.User));
        }
    }
}
