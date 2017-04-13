using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using System.Linq;
using Promact.Erp.Util.StringLiteral;

namespace Promact.Erp.Core.Controllers
{
    public abstract class MVCBaseController : Controller
    {
        public readonly AppStringLiteral _stringConstantRepository;
        public MVCBaseController(ISingletonStringLiteral stringConstantRepository)
        {
            _stringConstantRepository = stringConstantRepository.StringConstant;
        }
        /// <summary>
        /// get login user id.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>user id</returns>
        public string GetUserId(IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity.Claims.ToList().Single(x => x.Type == _stringConstantRepository.Sub).Value;
        }
    }
}
