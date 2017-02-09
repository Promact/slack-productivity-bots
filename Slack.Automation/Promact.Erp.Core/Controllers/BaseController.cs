using System.Security.Claims;
using System.Security.Principal;
using Promact.Erp.Util.StringConstants;
using System.Web.Http;
using System.Linq;

namespace Promact.Erp.Core.Controllers
{
    public class BaseController : ApiController
    {
        public readonly IStringConstantRepository _stringConstantRepository;
        public BaseController(IStringConstantRepository stringConstantRepository)
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
