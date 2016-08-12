using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class OAuthController: OAuthControllerBase
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
            var clientId = AppSettingsUtil.ClientId;
            var clientSecret = AppSettingsUtil.ClientSecret;
            OAuthApplication oAuth = new OAuthApplication();
            oAuth.ClientId = clientId;
            oAuth.ClientSecret = clientSecret;
            oAuth.RefreshToken = refreshToken;
            oAuth.ReturnUrl = AppSettingsUtil.ClientReturnUrl;
            return Ok(oAuth);
        }
    }
}
