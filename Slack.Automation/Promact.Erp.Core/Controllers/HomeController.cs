using Autofac;
using Microsoft.AspNetCore.Identity;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Promact.Erp.Core.Controllers
{
    public  class HomeController :  Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public HomeController()
        {

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
            //BaseUrl of OAuth and clientId of App to be set 
            var clientId = "dhadf15sgdth4td54hfg";
            var basePath = "http://localhost:35716/OAuth/ExternalLogin";
            var Url = basePath + "?clientId=" + clientId;
            //make call to the OAuth Server
            return Redirect(Url);
        }

        /// <summary>
        /// Method will recieve access token and email and user will register here
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public ActionResult ExtrenalLoginCallBack(string accessToken, string email)
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            //Creating a user with email only. Password not required
            var user = new ApplicationUser { UserName = email, Email = email };
            var result = _userManager.CreateAsync(user).Result;
            if(result.Succeeded)
            {
                //Adding external Oauth details
                UserLoginInfo info = new UserLoginInfo("Promact","akjska6565s4fs", "Promact");
                result = _userManager.AddLoginAsync(user,info).Result;
                if (result.Succeeded)
                {
                    //Signing user with username or email only
                    _signInManager.SignInAsync(user, false).Wait();
                    return RedirectToAction("AfterLogIn", "Home");
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
            _signInManager.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }
    }
}
