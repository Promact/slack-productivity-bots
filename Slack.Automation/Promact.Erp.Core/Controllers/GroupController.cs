using Promact.Core.Repository.GroupRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.ExceptionHandler;
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
        * @apiError BadRequest
        * @apiErrorExample {json} Error-Response:
        * HTTP/1.1 400 Bad Request 
        * {
        *    "error": "Problems parsing JSON object"
        * }     
        */
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> InsertGroupAsync(GroupAC groupAC)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _groupRepository.AddGroupAsync(groupAC));
            }
            return BadRequest();
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
        * @apiError GroupNotFound
        * @apiErrorExample {json} Error-Response:
        * HTTP/1.1 404 Not Found 
        * {
        *   "error": "GroupNotFound"
        * }  
        */
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetGroupByIdAsync(int id)
        {
            try
            {
                return Ok(await _groupRepository.GetGroupByIdAsync(id));
            }
            catch (GroupNotFound)
            {
                return NotFound();
            }
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
        * @apiError BadRequest
        * @apiErrorExample {json} Error-Response:
        * HTTP/1.1 400 Bad Request 
        * {
        *    "error": "Problems parsing JSON object"
        * } 
        * @apiError GroupNotFound
        * @apiErrorExample {json} Error-Response:
        * HTTP/1.1 404 Not Found 
        * {
        *   "error": "GroupNotFound"
        * }     
        */
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> UpdateGroupAsync(int id, GroupAC groupAC)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    groupAC.Id = id;
                    return Ok(await _groupRepository.UpdateGroupAsync(groupAC));
                }
                return BadRequest();
            }
            catch (GroupNotFound)
            {
                return NotFound();
            }
        }

        /**
       * @api {put} api/group/available/:id
       * @apiVersion 1.0.0
       * @apiName CheckGroupNameIsExistsAsync
       * @apiGroup Group   
       * @apiParam {int} id   
       * @apiParamExample
       *       {
       *         "name":"Group1"
       *         "id":"1"
       *       }
       * @apiSuccessExample {json} Success-Response:
       * HTTP/1.1 200 OK 
       * {
       *     "Id" : "1"
       * }   
       */
        [HttpGet]
        [Route("available/{id:int}")]
        public async Task<IHttpActionResult> CheckGroupNameIsExistsAsync(string name, int id)
        {
            return Ok(await _groupRepository.CheckGroupNameIsExistsAsync(name, id));
        }


        /**
       * @api {delete} api/group/delete/:id
       * @apiVersion 1.0.0
       * @apiName DeleteGroupByIdAsync
       * @apiGroup Group   
       * @apiParam {int} id   
       * @apiParamExample
       *       {
       *         "id":"1"
       *       }
       * @apiSuccessExample {json} Success-Response:
       * HTTP/1.1 200 OK 
       * {
       *     "result" : "true"
       * } 
       * @apiError GroupNotFound
       * @apiErrorExample {json} Error-Response:
       * HTTP/1.1 404 Not Found 
       * {
       *   "error": "GroupNotFound"
       * }    
       */
        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteGroupByIdAsync(int id)
        {
            try
            {
                return Ok(await _groupRepository.DeleteGroupByIdAsync(id));
            }
            catch (GroupNotFound)
            {
                return NotFound();
            }
        }


        /**
        * @api {get} api/group/email
        * @apiVersion 1.0.0
        * @apiName GetActiveUserEmailList
        * @apiGroup Group 
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        *   {
        *       "result" : ["abc@promactinfo.com","abc1@promactinfo.com"]
        *   }
        */
        [HttpGet]
        [Route("email")]
        public async Task<IHttpActionResult> GetActiveUserEmailList()
        {
            return Ok(await _groupRepository.GetActiveUserEmailListAsync());
        }

        #endregion
    }
}
