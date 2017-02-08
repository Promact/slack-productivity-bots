using Promact.Erp.Util.StringConstants;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using System.Linq;

namespace Promact.Erp.Core.Controllers
{
    public abstract class MVCBaseController : Controller
    {
        private readonly IStringConstantRepository _stringConstantRepository;
        public MVCBaseController(IStringConstantRepository stringConstantRepository)
        {
            _stringConstantRepository = stringConstantRepository;
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
