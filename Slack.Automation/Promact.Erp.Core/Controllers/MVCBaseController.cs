using Promact.Erp.Util.StringConstants;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;

namespace Promact.Erp.Core.Controllers
{
    public abstract class MVCBaseController : Controller
    {
        private readonly IStringConstantRepository _stringConstant;
        public MVCBaseController(IStringConstantRepository stringConstant)
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
