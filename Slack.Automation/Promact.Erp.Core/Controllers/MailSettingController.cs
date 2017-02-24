using Promact.Core.Repository.MailSettingRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.StringConstants;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    [RoutePrefix("api/mailsetting")]
    public class MailSettingController : BaseController
    {
        private IMailSettingRepository _mailSettingRepository;
        public MailSettingController(IStringConstantRepository stringConstant, IMailSettingRepository mailSettingRepository)
            : base(stringConstant)
        {
            _mailSettingRepository = mailSettingRepository;
        }

        [HttpGet]
        [Route("project")]
        public async Task<IHttpActionResult> GetAllProject()
        {
            return Ok(await _mailSettingRepository.GetAllProjectAsync());
        }

        [HttpGet]
        [Route("project/{projectId}/{module}")]
        public async Task<IHttpActionResult> GetMailSettingDetailsByProjectId(int projectId, string module)
        {
            return Ok(await _mailSettingRepository.GetMailSettingDetailsByProjectIdAsync(projectId, module));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddMailSetting(MailSettingAC mailSettingAC)
        {
            await _mailSettingRepository.AddMailSettingAsync(mailSettingAC);
            return Ok();
        }

        [HttpGet]
        [Route("group")]
        public async Task<IHttpActionResult> GetListOfGroups()
        {
            return Ok(await _mailSettingRepository.GetListOfGroupsAsync());
        }

        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> UpdateMailSetting(MailSettingAC mailSettingAC)
        {
            await _mailSettingRepository.UpdateMailSettingAsync(mailSettingAC);
            return Ok();
        }
    }
}
