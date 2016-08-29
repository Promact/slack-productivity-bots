using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Promact.Erp.Core.Controllers
{
    public class HomeController : MVCBaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AfterLogIn", "Home");
            }
            //BaseUrl of OAuth and clientId of App to be set 
            var url = string.Format("{0}?clientId={1}", AppSettingsUtil.OAuthUrl, AppSettingsUtil.ClientId);
            //make call to the OAuth Server
            return Redirect(url);
        }

        /// <summary>
        /// Method will recieve access token and email and user will register here
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExtrenalLoginCallBack(string accessToken, string email ,string slackUserName)
        {
            var user = UserManager.FindByEmail(email);
            if (user!=null)
            {
                UserLoginInfo info = new UserLoginInfo(AppSettingsUtil.ProviderName, accessToken);
                await UserManager.AddLoginAsync(user.Id, info);
                await SignInManager.SignInAsync(user, false, false);
                return RedirectToAction("AfterLogIn", "Home");
            }
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AfterLogIn", "Home");
            }
            if (user == null)
            {
                user = new ApplicationUser() { Email = email, UserName = email, SlackUserName = slackUserName };
                //Creating a user with email only. Password not required
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //Adding external Oauth details
                    UserLoginInfo info = new UserLoginInfo(AppSettingsUtil.ProviderName, accessToken);
                    result = await UserManager.AddLoginAsync(user.Id, info);
                    if (result.Succeeded)
                    {
                        //Signing user with username or email only
                        await SignInManager.SignInAsync(user, false, false);
                        return RedirectToAction("AfterLogIn", "Home");
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// Method to signOut from our application not from OAuth server
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
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
            return Redirect(AppSettingsUtil.LeaveManagementAuthorizationUrl+StringConstant.OAuthAuthorizationScopeAndClientId+AppSettingsUtil.OAuthClientId);
        }
    }
}
