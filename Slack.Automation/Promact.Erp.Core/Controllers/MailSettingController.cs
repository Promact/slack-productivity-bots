using Promact.Core.Repository.MailSettingRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.StringLiteral;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    [Authorize]
    [RoutePrefix("api/mailsetting")]
    public class MailSettingController : BaseController
    {
        #region Private Variable
        private IMailSettingRepository _mailSettingRepository;
        #endregion

        #region Constructor
        public MailSettingController(ISingletonStringLiteral stringConstant, IMailSettingRepository mailSettingRepository)
            : base(stringConstant)
        {
            _mailSettingRepository = mailSettingRepository;
        }
        #endregion

        #region Public Methods
        /**
        * @api {get} api/mailsetting/project
        * @apiVersion 1.0.0
        * @apiName GetAllProjectAsync
        * @apiGroup MailSetting
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     [
        *     {
        *         Id : "1"
        *         Name : "something",
        *         SlackChannelName : "something",
        *         IsActive : true,
        *         TeamLeaderId : "asdfsfdsmnskdsdvlsd",
        *         CreatedBy : "fsdfsagsdfgsDFBDfbdbdf"
        *         CreatedDate : 23-02-2017 08:45:40
        *         UpdatedBy : "sdkfmnskdmfssdgfsdfg"
        *         UpdatedDate : 23-02-2017 08:45:40
        *     },
        *     {
        *         Id : ""
        *         Name : "something"
        *         SlackChannelName : "something"
        *         IsActive : true
        *         TeamLeaderId : "sadfvsadgvegbdfdsbds"
        *         CreatedBy : "SDGSGSDFgvsadg"
        *         CreatedDate : 23-02-2017 08:45:40
        *         UpdatedBy : "SDGDHhbdfbdxfbdsfHG"
        *         UpdatedDate : 23-02-2017 08:45:40
        *     }
        *     ]
        * }
        */
        [HttpGet]
        [Route("project")]
        public async Task<IHttpActionResult> GetAllProjectAsync()
        {
            return Ok(await _mailSettingRepository.GetAllProjectAsync());
        }

        /**
        * @api {get} api/mailsetting/{projectId}
        * @apiVersion 1.0.0
        * @apiName GetMailSettingDetailsByProjectIdAsync
        * @apiGroup MailSetting
        * @apiParam {int} projectId  Project Id
        * @apiParam {string} module  name of module
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     {
        *         Id : "1"
        *         Module : "task"
        *         ProjectId : "1"
        *         SendMail : true
        *         To : {"Team Leader","TL","Other TL"}
        *         CC : {"Management","Admin","Super-Admin"}
        *     }
        * }
        */
        [HttpGet]
        [Route("{projectId}")]
        public async Task<IHttpActionResult> GetMailSettingDetailsByProjectIdAsync(int projectId, string module)
        {
            return Ok(await _mailSettingRepository.GetMailSettingDetailsByProjectIdAsync(projectId, module));
        }

        /**
        * @api {post} api/mailsetting
        * @apiVersion 1.0.0
        * @apiName AddMailSettingAsync
        * @apiGroup MailSetting
        * @apiParam {MailSettingAC} mailSettingAC  mail setting object
        * @apiSuccessExample {json} Success-Response:
        *     {
        *         Id : "1"
        *         Module : "task"
        *         ProjectId : "1"
        *         SendMail : true
        *         To : {"Team Leader","TL","Other TL"}
        *         CC : {"Management","Admin","Super-Admin"}
        *     }
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        */
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddMailSettingAsync(MailSettingAC mailSettingAC)
        {
            await _mailSettingRepository.AddMailSettingAsync(mailSettingAC);
            return Ok();
        }

        /**
        * @api {get} api/mailsetting/group
        * @apiVersion 1.0.0
        * @apiName GetListOfGroupsNameAsync
        * @apiGroup MailSetting
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     {
        *         "Team Leader",
        *         "Admin",
        *         "Super-Admin"
        *     }
        * }
        */
        [HttpGet]
        [Route("group")]
        public async Task<IHttpActionResult> GetListOfGroupsNameAsync()
        {
            return Ok(await _mailSettingRepository.GetListOfGroupsNameAsync());
        }

        /**
        * @api {put} api/mailsetting
        * @apiVersion 1.0.0
        * @apiName UpdateMailSettingAsync
        * @apiGroup MailSetting
        * @apiParam {MailSettingAC} mailSettingAC  mail setting object
        * @apiSuccessExample {json} Success-Response:
        *     {
        *         Id : "1"
        *         Module : "task"
        *         ProjectId : "1"
        *         SendMail : true
        *         To : {"Team Leader","TL","Other TL"}
        *         CC : {"Management","Admin","Super-Admin"}
        *     }
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        */
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> UpdateMailSettingAsync(MailSettingAC mailSettingAC)
        {
            await _mailSettingRepository.UpdateMailSettingAsync(mailSettingAC);
            return Ok();
        }
        #endregion
    }
}
