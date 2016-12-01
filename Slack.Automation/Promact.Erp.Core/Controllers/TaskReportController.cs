using Promact.Core.Repository.TaskMailRepository;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.StringConstants;

namespace Promact.Erp.Core.Controllers
{

    [RoutePrefix("api/TaskReport")]
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


        /**
         * @api {get}  api/TaskReport/:UserId
         * @apiVersion 1.0.0
         * @apiName TaskMailDetailsReport
         * @apiGroup TaskReport
         * @apiParam {string} UserId  user Id
         * @apiParam {string} UserRole  user Role
         * @apiParam {string} UserName  user Name
         * @apiParamExample {json} Request-Example:
         *      
         *        {
         *             "UserId": "1",
         *             "UserRole": "Admin",
         *             "UserName" : "test",
         *        }      
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK 
         * {
         *      "UserId": "1",
         *      "UserRole": "Admin",
         *      "UserName" : "test",
         *      "CreatedOn": "01-01-0001",
         *      "TaskMails" : 
         *      {
         *          Description : "Worked on Issue #123".
         *          Comment : "Worked comment",
         *          Hours : 1.5, 
         *          Status : Completed  
         *      }
         * }
         
         * }
         */
        [HttpGet]
        [Route("{UserId}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string UserId,string UserRole, string UserName)
        {
           string LoginId = User.Identity.GetUserId();
            return await _taskMailReport.TaskMailDetailsReportAsync(UserId,UserRole,UserName, LoginId);
        }

        /**
         * @api {get} api/TaskReport/:UserId/details  
         * @apiVersion 1.0.0
         * @apiName TaskMailDetailsReportPreviousDate
         * @apiGroup TaskReport
         * @apiParam {string} UserId  user Id
         * @apiParam {string} UserRole  user Role
         * @apiParam {string} UserName  user Name
         * @apiParam {string} CreatedOn user Task Mail CreatedOn
         * @apiParamExample {json} Request-Example:
         *      
         *        {
         *             "UserId": "1",
         *             "UserRole": "Admin",
         *             "UserName" : "test",
         *             "CreatedOn": "01-01-0001"
         *        }      
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK 
         * {
         *      "UserId": "1",
         *      "UserRole": "Admin",
         *      "UserName" : "test",
         *      "CreatedOn": "01-01-0001",
         *      "TaskMails" : 
         *      {
         *          Description : "Worked on Issue #123".
         *          Comment : "Worked comment",
         *          Hours : 1.5, 
         *          Status : Completed  
         *      }
         * }
         */
        [HttpGet]
        [Route("{UserId}/details")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportPreviousDateAsync(string UserId,string UserRole, string UserName ,string CreatedOns)
        {
            string LoginId = User.Identity.GetUserId();
            string PreviousPage = _stringConstant.PriviousPage;
            return await _taskMailReport.TaskMailDetailsReportNextPreviousDateAsync(UserId, UserName, UserRole, CreatedOns, LoginId, PreviousPage);
        }

        /**
        * @api {get} api/TaskReport/details :UserId 
        * @apiVersion 1.0.0
        * @apiName TaskMailDetailsReportNextDate
        * @apiGroup TaskReport
        * @apiParam {string} UserId  user Id
        * @apiParam {string} UserRole  user Role
        * @apiParam {string} UserName  user Name
        * @apiParam {string} CreatedOn user Task Mail CreatedOn
        * @apiParamExample {json} Request-Example:
        *      
        *        {
        *             "UserId": "1",
        *             "UserRole": "Admin",
        *             "UserName" : "test",
        *             "CreatedOn": "01-01-0001"
        *        }      
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *      "UserId": "1",
        *      "UserRole": "Admin",
        *      "UserName" : "test",
        *      "CreatedOn": "01-01-0001",
        *      "SelectedDate":"01-01-0001",
        *      "TaskMails" : 
        *      {
        *          Description : "Worked on Issue #123".
        *          Comment : "Worked comment",
        *          Hours : 1.5, 
        *          Status : Completed  
        *      }
        * }
        */
        [HttpGet]
        [Route("details/{UserId}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportNextDateAsync(string UserId, string UserRole, string UserName, string CreatedOns)
        {
            string LoginId = User.Identity.GetUserId();
            string NextPage = _stringConstant.NextPage;
            return await _taskMailReport.TaskMailDetailsReportNextPreviousDateAsync(UserId, UserName, UserRole, CreatedOns, LoginId, NextPage);
        }

        /**
        * @api {get} api/TaskReport :UserId 
        * @apiVersion 1.0.0
        * @apiName TaskMailDetailsReportSelectedDateAsync
        * @apiGroup TaskReport
        * @apiParam {string} UserId  user Id
        * @apiParam {string} UserRole  user Role
        * @apiParam {string} UserName  user Name
        * @apiParam {string} CreatedOn user Task Mail CreatedOn
        * @apiParam {string} SelectedDate user Task Mail SelectedDate
        * @apiParamExample {json} Request-Example:
        *        {
        *             "UserId": "1",
        *             "UserRole": "Admin",
        *             "UserName" : "test",
        *             "CreatedOn": "01-01-0001",
        *             "SelectedDate":"01-01-0001"
        *        }      
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *      "UserId": "1",
        *      "UserRole": "Admin",
        *      "UserName" : "test",
        *      "CreatedOn": "01-01-0001",
        *      "SelectedDate":"01-01-0001",
        *      "TaskMails" : 
        *      {
        *          Description : "Worked on Issue #123".
        *          Comment : "Worked comment",
        *          Hours : 1.5, 
        *          Status : Completed  
        *      }
        * }
        */
        [HttpGet]
        [Route("{UserId}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string UserId, string UserRole, string UserName, string CreatedOns, string SelectedDate)
        {
            string LoginId = User.Identity.GetUserId();
            return await _taskMailReport.TaskMailDetailsReportSelectedDateAsync(UserId, UserName, UserRole, CreatedOns, LoginId, SelectedDate);
        }


        /**
       * @api {get} api/TaskReport 
       * @apiVersion 1.0.0
       * @apiName getAllEmployee
       * @apiGroup TaskReport
       * @apiParam {null} no parameter
       * @apiSuccessExample {json} Success-Response:
       * HTTP/1.1 200 OK 
       * {
       *     "UserId": "1",
       *     "UserRole": "Admin",
       *     "UserName" : "test",
       *     "CreatedOn": "01-01-0001",
       *     "UserEmail": "test@xyz.com",
       *     "TaskMails" : null
       * }
       */
        [HttpGet]
        [Route("")]
        public async Task<List<TaskMailReportAc>> getAllEmployeeAsync()
        {
            string UserId = User.Identity.GetUserId();
            return await _taskMailReport.GetAllEmployeeAsync(UserId);
        }

    }
}
