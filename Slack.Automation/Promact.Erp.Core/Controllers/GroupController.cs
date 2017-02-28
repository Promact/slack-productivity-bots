using Promact.Core.Repository.GroupRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.StringConstants;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    [RoutePrefix("api/group")]
    [Authorize]
    public class GroupController : BaseController
    {
        #region Private Variables
        private readonly IGroupRepository _groupRepository;
        #endregion

        #region Constructor
        public GroupController(IStringConstantRepository stringConstant, IGroupRepository groupRepository) : base(stringConstant)
        {
            _groupRepository = groupRepository;
        }

        #endregion


        #region Public Method(s)

        /**
        * @api {get} api/group
        * @apiVersion 1.0.0
        * @apiName GetListOfGroupAsync
        * @apiGroup Group    
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     [
        *       {
        *         "Id":"1"
        *         "Name":"Group1"
        *         "Type":"StaticGroup"
        *         "Emails" : [{ankit@promactinfo.com},{bhanvadia@gmail.com}]
        *       },
        *       { "Id":"2"
        *         "Name":"Group2"
        *         "Type":"DynamicGroup"
        *         "Emails" : [{abc@promactinfo.com},{xyz@gmail.com}]
        *       },
        *     ]      
        * }   
        */
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetListOfGroupAsync()
        {
            return Ok(await _groupRepository.GetListOfGroupACAsync());
        }

        /**
        * @api {post} api/group
        * @apiVersion 1.0.0
        * @apiName InsertGroupAsync
        * @apiGroup Group   
        * @apiParam {object} groupAC   
        * @apiParamExample
        *       {
        *         "Id":null
        *         "Name":"Group1"
        *         "Type":"StaticGroup"
        *         "Emails" : [{ankit@promactinfo.com},{bhanvadia@gmail.com}]
        *       }
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *    "Id" : "1"
        * }   
        */
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> InsertGroupAsync(GroupAC groupAC)
        {
            return Ok(await _groupRepository.AddGroupAsync(groupAC));
        }

        /**
        * @api {get} api/group/:id
        * @apiVersion 1.0.0
        * @apiName GetGroupByIdAsync
        * @apiGroup Group   
        * @apiParam {int} id   
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *      {
        *         "Id":"1"
        *         "Name":"Group1"
        *         "Type":"StaticGroup"
        *         "Emails" : [{ankit@promactinfo.com},{bhanvadia@gmail.com}]
        *       }
        * }   
        */
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetGroupByIdAsync(int id)
        {
            return Ok(await _groupRepository.GetGroupByIdAsync(id));
        }

        /**
        * @api {put} api/group/:id
        * @apiVersion 1.0.0
        * @apiName UpdateGroupAsync
        * @apiGroup Group   
        * @apiParam {int,object} id,GroupAC   
        * @apiParamExample
        *       {
        *         "Id":"1"
        *         "Name":"Group1"
        *         "Type":"StaticGroup"
        *         "Emails" : [{ankit@promactinfo.com},{bhanvadia@gmail.com}]
        *       }
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Id" : "1"
        * }   
        */
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateGroupAsync(int id, GroupAC groupAC)
        {
            groupAC.Id = id;
            return Ok(await _groupRepository.UpdateGroupAsync(groupAC));
        }

        /**
       * @api {put} api/group/available/:groupName/:available/id
       * @apiVersion 1.0.0
       * @apiName CheckCheckGroupNameIsExistsAsync
       * @apiGroup Group   
       * @apiParam {int,int} groupName,groupId   
       * @apiParamExample
       *       {
       *         "groupName":"Group1"
       *         "groupId":"1"
       *       }
       * @apiSuccessExample {json} Success-Response:
       * HTTP/1.1 200 OK 
       * {
       *     "Id" : "1"
       * }   
       */
        [HttpGet]
        [Route("available/{name}/{id:int}")]
        public async Task<IHttpActionResult> CheckCheckGroupNameIsExistsAsync(string name, int id)
        {
            return Ok(await _groupRepository.CheckGroupNameIsExistsAsync(name, id));
        }

        #endregion
    }
}
