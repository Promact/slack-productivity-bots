using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class OAuthController:ApiController
    {
        /// <summary>
        /// Method to get refresh Token from OAuth and send app clientSecretId
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("oAuth/RefreshToken")]
        public IHttpActionResult RefreshToken(string refreshToken)
        {
            var clientId = "dhadf15sgdth4td54hfg";
            var clientSecret = "dhadf15sgdth4td54hfg5d4f8";
            OAuthApplication oAuth = new OAuthApplication();
            oAuth.ClientId = clientId;
            oAuth.ClientSecret = clientSecret;
            oAuth.RefreshToken = refreshToken;
            oAuth.ReturnUrl = "http://localhost:28182/Home/AccessToken";
            return Ok(oAuth);
        }
    }
}
