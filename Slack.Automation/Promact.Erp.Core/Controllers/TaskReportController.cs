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
            _taskMailReport = taskMailReport;
            _stringConstant = stringConstant;
        }


        /**
         * @api {get}  api/TaskReport/user/:userId
         * @apiVersion 1.0.0
         * @apiName TaskMailDetailsReportAsync
         * @apiGroup TaskReport
         * @apiParam {string} userId  user Id
         * @apiParam {string} role  user Role
         * @apiParam {string} userName  user Name
         * @apiParamExample {json} Request-Example:
         *      
         *        {
         *             "userId": "1",
         *             "role": "Admin",
         *             "userName" : "test",
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
        [Route("user/{userId}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string userId,string role, string userName)
        {
           return await _taskMailReport.TaskMailDetailsReportAsync(userId, role, userName, User.Identity.GetUserId());
        }

        /**
        * @api {get} api/TaskReport/user/:userId 
        * @apiVersion 1.0.0
        * @apiName TaskMailDetailsReportNextPreviousDateAsync
        * @apiGroup TaskReport
        * @apiParam {string} userId  user Id
        * @apiParam {string} role  user Role
        * @apiParam {string} userName  user Name
        * @apiParam {string} createdOn user Task Mail CreatedOn
        * @apiParam {string} pageType user Task Mail CreatedOn
        * @apiParamExample {json} Request-Example:
        *      
        *        {
        *             "userId": "1",
        *             "role": "Admin",
        *             "userName" : "test",
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
        [Route("user/{userId}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportNextPreviousDateAsync(string userId, string role, string userName, string createdOn, string pageType)
        {
            if (pageType == _stringConstant.NextPage)
            {
                return await _taskMailReport.TaskMailDetailsReportNextPreviousDateAsync(userId, userName, role, createdOn, User.Identity.GetUserId(), _stringConstant.NextPage);
            }
            else 
            {
                return await _taskMailReport.TaskMailDetailsReportNextPreviousDateAsync(userId, userName, role, createdOn, User.Identity.GetUserId(), _stringConstant.Previouspage);
            }
        }

        /**
        * @api {get} api/TaskReport/user/:userId 
        * @apiVersion 1.0.0
        * @apiName TaskMailDetailsReportSelectedDateAsync
        * @apiGroup TaskReport
        * @apiParam {string} userId  user Id
        * @apiParam {string} role  user Role
        * @apiParam {string} userName  user Name
        * @apiParam {string} createdOn user Task Mail CreatedOn
        * @apiParam {string} selectedDate user Task Mail SelectedDate
        * @apiParamExample {json} Request-Example:
        *        {
        *             "userId": "1",
        *             "role": "Admin",
        *             "userName" : "test",
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
        [Route("user/{userId}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string userId, string role, string userName, string createdOn, string selectedDate)
        {
            return await _taskMailReport.TaskMailDetailsReportSelectedDateAsync(userId, userName, role, createdOn, User.Identity.GetUserId(), selectedDate);
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
