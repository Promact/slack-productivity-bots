using Promact.Core.Repository.TaskMailRepository;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.StringConstants;

namespace Promact.Erp.Core.Controllers
{
    [Authorize]
    public class TaskReportController: ApiController
    {
        private readonly ITaskMailRepository _taskMailReport;
        private readonly IStringConstantRepository _stringConstant;
        public TaskReportController(ITaskMailRepository taskMailReport, IStringConstantRepository stringConstant)
        {
            this._taskMailReport = taskMailReport;
            _stringConstant = stringConstant;
        }

        //[HttpGet]
        //[Route("taskMailReport")]
        //public async Task<IHttpActionResult> taskMailReport()
        //{
        //    string UserId = User.Identity.GetUserId();
        //    IEnumerable<TaskMailReportAc> taskMailReportAc = await _taskMailReport.TaskMailReport(UserId);
        //    return Ok(taskMailReportAc);
        //}

        //[HttpGet]
        //[Route("taskMailReport/{currentPage}/{itemsPerPage}")]
        //public async Task<IHttpActionResult> taskMailReport(int currentPage,int itemsPerPage)
        //{
        //    string UserId = User.Identity.GetUserId();
        //    IEnumerable<TaskMailReportAc> taskMailReportAc = await _taskMailReport.TaskMailReport(UserId,currentPage, itemsPerPage);
        //    return Ok(taskMailReportAc);
        //}

        /**
         * @api {get} taskMailDetailsReport:{UserId}/{UserRole}/{UserName}
         * @apiVersion 1.0.0
         * @apiName TaskReport
         * @apiGroup TaskReport
         * @apiParam {string} UserId  user Id
         * @apiParam {string} UserRole  user Role
         * @apiParam {string} UserName  user Name
         * @apiParamExample {json} Request-Example:
         *      
         *        {
         *             "UserId": "1"
         *             "UserRole": "Admin"
         *             "UserName" : "test"
         *             "description":"get the TaskMailUser Object"
         *        }      
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK 
         * {
         *      "UserId": "1"
         *      "UserRole": "Admin"
         *      "UserName" : "test"
         *      "description":"get the TaskMailUser Object"
         * }
         */

        [HttpGet]
        [Route("taskMailDetailsReport/{UserId}/{UserRole}/{UserName}")]
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReport(string UserId,string UserRole,string UserName)
        {
            string LoginId = User.Identity.GetUserId();
            return await _taskMailReport.TaskMailDetailsReport(UserId,UserRole,UserName, LoginId);
        }

        /**
         * @api {get} taskMailDetailsReportPreviousDate:{UserRole}/{CreatedOn}/{UserId}/{UserName}
         * @apiVersion 1.0.0
         * @apiName TaskReport
         * @apiGroup TaskReport
         * @apiParam {string} UserId  user Id
         * @apiParam {string} UserRole  user Role
         * @apiParam {string} UserName  user Name
         * @apiParam {string} CreatedOn user Task Mail CreatedOn
         * @apiParamExample {json} Request-Example:
         *      
         *        {
         *             "UserId": "1"
         *             "UserRole": "Admin"
         *             "UserName" : "test"
         *             "CreatedOn": "01-01-0001"
         *             "description":"get the TaskMailUser Object For Previous Date"
         *        }      
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK 
         * {
         *      "UserId": "1"
         *      "UserRole": "Admin"
         *      "UserName" : "test"
         *      "CreatedOn": "01-01-0001"
         *      "description":"get the TaskMailUser Object For Previous Date"
         * }
         */

        [HttpGet]
        [Route("taskMailDetailsReportPreviousDate/{UserRole}/{CreatedOn}/{UserId}/{UserName}")]
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportPreviousDate(string UserRole, string CreatedOn,string UserId,string UserName)
        {
            string LoginId = User.Identity.GetUserId();
            string PreviousPage = _stringConstant.PriviousPage;
            return await _taskMailReport.TaskMailDetailsReportNextPreviousDate(UserId, UserName, UserRole,CreatedOn, LoginId, PreviousPage);
            //return await _taskMailReport.TaskMailDetailsReport(UserId, UserRole, UserName, LoginId);
        }

        /**
         * @api {get} taskMailDetailsReportNextDate:{UserRole}/{CreatedOn}/{UserId}/{UserName}
         * @apiVersion 1.0.0
         * @apiName TaskReport
         * @apiGroup TaskReport
         * @apiParam {string} UserId  user Id
         * @apiParam {string} UserRole  user Role
         * @apiParam {string} UserName  user Name
         * @apiParam {string} CreatedOn user Task Mail CreatedOn
         * @apiParamExample {json} Request-Example:
         *      
         *        {
         *             "UserId": "1"
         *             "UserRole": "Admin"
         *             "UserName" : "test"
         *             "CreatedOn": "01-01-0001"
         *             "description":"get the TaskMailUser Object For Next Day"
         *        }      
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK 
         * {
         *      "UserId": "1"
         *      "UserRole": "Admin"
         *      "UserName" : "test"
         *      "CreatedOn": "01-01-0001"
         *      "description":"get the TaskMailUser Object For Next Day"
         * }
         */
        [HttpGet]
        [Route("taskMailDetailsReportNextDate/{UserRole}/{CreatedOn}/{UserId}/{UserName}")]
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportNextDate(string UserRole, string CreatedOn, string UserId, string UserName)
        {
            string LoginId = User.Identity.GetUserId();
            string NextPage = _stringConstant.NextPage;
            return await _taskMailReport.TaskMailDetailsReportNextPreviousDate(UserId, UserName, UserRole, CreatedOn, LoginId, NextPage);
            //return await _taskMailReport.TaskMailDetailsReport(UserId, UserRole, UserName, LoginId);
        }

        /**
        * @api {get} taskMailDetailsReportSelectedDate:{UserRole}/{CreatedOn}/{UserId}/{UserName}/{SelectedDate}
        * @apiVersion 1.0.0
        * @apiName TaskReport
        * @apiGroup TaskReport
        * @apiParam {string} UserId  user Id
        * @apiParam {string} UserRole  user Role
        * @apiParam {string} UserName  user Name
        * @apiParam {string} CreatedOn user Task Mail CreatedOn
        * @apiParam {string} SelectedDate user Task Mail SelectedDate
        * @apiParamExample {json} Request-Example:
        *      
        *        {
        *             "UserId": "1"
        *             "UserRole": "Admin"
        *             "UserName" : "test"
        *             "CreatedOn": "01-01-0001"
        *             "SelectedDate":"01-01-0001"
        *             "description":"get the TaskMailUser Object For Selected Date"
        *        }      
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *      "UserId": "1"
        *      "UserRole": "Admin"
        *      "UserName" : "test"
        *      "CreatedOn": "01-01-0001"
        *      "SelectedDate":"01-01-0001"
        *      "description":"get the TaskMailUser Object For Selected Date"
        * }
        */
        [HttpGet]
        [Route("taskMailDetailsReportSelectedDate/{UserRole}/{CreatedOn}/{UserId}/{UserName}/{SelectedDate}")]
        public async Task<List<TaskMailUserAc>> TaskMailDetailsReportSelectedDate(string UserRole, string CreatedOn, string UserId, string UserName,string SelectedDate)
        {
            string LoginId = User.Identity.GetUserId();
            return await _taskMailReport.TaskMailDetailsReportSelectedDate(UserId, UserName, UserRole, CreatedOn, LoginId, SelectedDate);
            //return await _taskMailReport.TaskMailDetailsReport(UserId, UserRole, UserName, LoginId);
        }


        /**
       * @api {get} getAllEmployee
       * @apiVersion 1.0.0
       * @apiName TaskReport
       * @apiGroup TaskReport
       * @apiParamExample {json} Request-Example:
       *      
       *        {
       *             "description":"get the TaskMailUser Object"
       *        }      
       * @apiSuccessExample {json} Success-Response:
       * HTTP/1.1 200 OK 
       * {
       *     "description":"get the TaskMailUser Object"
       * }
       */
        [HttpGet]
        [Route("getAllEmployee")]
        public async Task<List<TaskMailUserAc>> getAllEmployee()
        {
            string UserId = User.Identity.GetUserId();
            return await _taskMailReport.GetAllEmployee(UserId);
        }

    }
}
