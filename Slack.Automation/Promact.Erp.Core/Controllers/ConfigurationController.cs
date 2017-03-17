using Promact.Core.Repository.ConfigurationRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    [RoutePrefix(BaseUrl)]
    public class ConfigurationController : BaseController
    {
        #region Variables
        private readonly IConfigurationRepository _configurationRepository;
        public const string BaseUrl = "api/configuration";
        #endregion

        #region Constructor
        public ConfigurationController(IStringConstantRepository stringConstant, IConfigurationRepository configurationRepository)
            : base(stringConstant)
        {
            _configurationRepository = configurationRepository;
        }
        #endregion

        #region Public Methods
        /**
        * @api {get} api/configuration
        * @apiVersion 1.0.0
        * @apiName GetAllConfiguration
        * @apiGroup Configuration
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     [{
        *         Id : "1"
        *         Module : "task"
        *         Status : "1"
        *         CreatedOn : "15-03-2017 03:07:06"
        *     }]
        * }
        */
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllConfiguration()
        {
            return Ok(_configurationRepository.GetAllConfiguration());
        }

        /**
        * @api {put} api/configuration
        * @apiVersion 1.0.0
        * @apiName UpdateConfigurationAsync
        * @apiGroup Configuration
        * @apiParam {Configuration} configuration configuration object
        * @apiSuccessExample {json} Success-Response:
        *     {
        *         Id : "1"
        *         Module : "task"
        *         Status : "1"
        *         CreatedOn : "15-03-2017 03:07:06"
        *     }
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        */
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> UpdateConfigurationAsync(Configuration configuration)
        {
            await _configurationRepository.UpdateConfigurationAsync(configuration);
            return Ok();
        }

        /**
        * @api {get} api/configuration/status
        * @apiVersion 1.0.0
        * @apiName GetAllConfigurationStatusAsync
        * @apiGroup Configuration
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     {
        *         ScrumOn : "true"
        *         TaskOn : "true"
        *         LeaveOn : "false"
        *     }
        * }
        */
        [HttpGet]
        [Route("status")]
        public async Task<IHttpActionResult> GetAllConfigurationStatusAsync()
        {
            return Ok(await _configurationRepository.GetAllConfigurationStatusAsync());
        }
        #endregion
    }
}
