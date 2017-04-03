using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.RedmineRepository;
using Promact.Erp.Util.StringConstants;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    public class RedmineController : BaseController
    {
        #region Private Variables
        private readonly IRedmineRepository _redmineRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        #endregion

        #region Constructor
        public RedmineController(IStringConstantRepository stringConstant, IRedmineRepository redmineRepository,
            IAttachmentRepository attachmentRepository)
            : base(stringConstant)
        {
            _redmineRepository = redmineRepository;
            _attachmentRepository = attachmentRepository;
        }
        #endregion

        #region Public Method
        /**
        * @api {post} redmineslashcommand
        * @apiVersion 1.0.0
        * @apiName SlackRequestAsync
        * @apiGroup Redmine
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"A message of redmine will be send user using incoming webhook"
        * }
        * @apiErrorExample {json} Error-Response:
        * HTTP/1.1 400 BadRequeest 
        * {
        *     "error":"A message of error will be send user using incoming webhook"
        * }
        */
        [HttpPost]
        [Route("redmineslashcommand")]
        public async Task<IHttpActionResult> SlackRequestAsync()
        {
            var request = HttpContext.Current.Request.Form;
            var redmine = _attachmentRepository.SlashCommandTransfrom(request);
            await _redmineRepository.SlackRequestAsync(redmine);
            return Ok();
        }
        #endregion
    }
}
