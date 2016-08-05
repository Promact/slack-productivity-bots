using Autofac;
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
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// External Login Method. It will call and external OAuth for Login
        /// <returns></returns>
        public ActionResult ExtrenalLogin()
        {
            var clientId = "dhadf15sgdth4td54hfg";
            var basePath = "http://localhost:35716/OAuth/ExternalLogin";
            var Url = basePath + "?clientId=" + clientId;
            return Redirect(Url);
        }

        /// <summary>
        /// Method will recieve access token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public ActionResult AccessToken(string accessToken, string email)
        {
            var user = new ApplicationUser { UserName = email, Email = email };
            // yet to work
            return View();
        }
    }
}
