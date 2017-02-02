using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using Promact.Erp.Util.StringConstants;

namespace Promact.Erp.Core.Controllers
{
   
    public class BaseController : ApiController
    {
        private readonly IStringConstantRepository _stringConstant;
        public BaseController(IStringConstantRepository stringConstant)
        {
            _stringConstant = stringConstant;
        }


        /// <summary>
        /// get login user id.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>user id</returns>
        public string GetUserId(IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                foreach (var claim in claimsIdentity.Claims)
                {
                    if (claim.Type == _stringConstant.Sub)
                    { return claim.Value; }
                }
            }
            return null;
        }
    }
}
