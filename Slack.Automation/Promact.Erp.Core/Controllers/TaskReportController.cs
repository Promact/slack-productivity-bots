using Promact.Core.Repository.TaskMailRepository;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.StringConstants;

namespace Promact.Erp.Core.Controllers
{

    [RoutePrefix("api/taskreport")]
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
         * @api {get}  api/TaskReport/user/:id
         * @apiVersion 1.0.0
         * @apiName TaskMailDetailsReportAsync
         * @apiGroup TaskReport
         * @apiParam {string} id  user Id
         * @apiParam {string} role  user Role
         * @apiParam {string} name  user Name
         * @apiParamExample {json} Request-Example:
         *      
         *        {
         *             "id": "1",
         *             "role": "Admin",
         *             "name" : "test",
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
        [Route("user/{id}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string id,string role, string name)
        {
           return await _taskMailReport.TaskMailDetailsReportAsync(id, role, name, User.Identity.GetUserId());
        }

        /**
        * @api {get} api/TaskReport/user/:id 
        * @apiVersion 1.0.0
        * @apiName TaskMailDetailsReportNextPreviousDateAsync
        * @apiGroup TaskReport
        * @apiParam {string} id  user Id
        * @apiParam {string} role  user Role
        * @apiParam {string} name  user Name
        * @apiParam {string} createdOns user Task Mail CreatedOn
        * @apiParam {string} pageType user Task Mail CreatedOn
        * @apiParamExample {json} Request-Example:
        *      
        *        {
        *             "id": "1",
        *             "role": "Admin",
        *             "name" : "test",
        *             "createdOns": "01-01-2016",
        *             "pageType"
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
        [Route("user/{id}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportNextPreviousDateAsync(string id, string role, string name, string createdOns, string pageType)
        {
            if (pageType == _stringConstant.NextPage)
            {
                return await _taskMailReport.TaskMailDetailsReportNextPreviousDateAsync(id, name, role, createdOn, User.Identity.GetUserId(), _stringConstant.NextPage);
            }
            else 
            {
                return await _taskMailReport.TaskMailDetailsReportNextPreviousDateAsync(id, name, role, createdOn, User.Identity.GetUserId(), _stringConstant.Previouspage);
            }
        }

        /**
        * @api {get} api/TaskReport/user/:id 
        * @apiVersion 1.0.0
        * @apiName TaskMailDetailsReportSelectedDateAsync
        * @apiGroup TaskReport
        * @apiParam {string} id  user Id
        * @apiParam {string} role  user Role
        * @apiParam {string} name  user Name
        * @apiParam {string} createdOns user Task Mail CreatedOn
        * @apiParam {string} selectedDate user Task Mail SelectedDate
        * @apiParamExample {json} Request-Example:
        *        {
        *             "id": "1",
        *             "role": "Admin",
        *             "name" : "test",
        *             "createdOns": "01-01-0001",
        *             "selectedDate":"01-01-0001"
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
        [Route("user/{id}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string id, string role, string name, string createdOn, string selectedDate)
        {
            return await _taskMailReport.TaskMailDetailsReportSelectedDateAsync(id, name, role, createdOn, User.Identity.GetUserId(), selectedDate);
        }


        /**
       * @api {get} api/TaskReport 
       * @apiVersion 1.0.0
       * @apiName GetAllEmployeeAsync
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
        public async Task<List<TaskMailReportAc>> GetAllEmployeeAsync()
        {
            return await _taskMailReport.GetAllEmployeeAsync(User.Identity.GetUserId());
        }

    }
}
