using Promact.Core.Repository.GroupRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.StringConstants;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    [RoutePrefix("api/[controller]")]
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
        * @apiName GetListOfGroup
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
        public async Task<IHttpActionResult> GetListOfGroup()
        {
            return Ok(await _groupRepository.GetListOfGroupAC());
        }

        /**
        * @api {post} api/group
        * @apiVersion 1.0.0
        * @apiName InsertGroup
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
        public async Task<IHttpActionResult> InsertGroup(GroupAC groupAC)
        {
            return Ok(await _groupRepository.AddGroup(groupAC));
        }

        /**
        * @api {get} api/group/:id
        * @apiVersion 1.0.0
        * @apiName GetGroupById
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
        [Route("{id}")]
        public async Task<IHttpActionResult> GetGroupById(int id)
        {
            return Ok(await _groupRepository.GetGroupById(id));
        }

        /**
        * @api {put} api/group/:id
        * @apiVersion 1.0.0
        * @apiName UpdateGroup
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
        public async Task<IHttpActionResult> UpdateGroup(int id, GroupAC groupAC)
        {
            groupAC.Id = id;
            return Ok(await _groupRepository.UpdateGroup(groupAC));
        }

        /**
       * @api {put} api/group/available/:groupName/:groupId
       * @apiVersion 1.0.0
       * @apiName CheckCheckGroupNameIsExists
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
        [Route("/available/{id}")]
        public async Task<IHttpActionResult> CheckCheckGroupNameIsExists(string groupName, int groupId)
        {
            return Ok(await _groupRepository.CheckGroupNameIsExists(groupName, groupId));
        }

        #endregion
    }
}
