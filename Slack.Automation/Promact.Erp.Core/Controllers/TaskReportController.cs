using System.Threading.Tasks;
using System.Web.Http;
using System.Collections.Generic;
using Promact.Erp.DomainModel.ApplicationClass;
using System;
using Promact.Core.Repository.TaskMailReportRepository;
using NLog;
using Promact.Erp.Util.StringLiteral;

namespace Promact.Erp.Core.Controllers
{

    [RoutePrefix("api/taskreport")]
    [Authorize]
    public class TaskReportController : BaseController 
    {
        private readonly ITaskMailReportRepository _taskMailReport;
        private readonly ILogger _logger;
        public TaskReportController(ITaskMailReportRepository taskMailReport, ISingletonStringLiteral stringConstant)
            :base(stringConstant)
        {
            _taskMailReport = taskMailReport;
            _logger = LogManager.GetLogger("TaskReportModule");
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
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string userId, string role, string userName)
        {
            return await _taskMailReport.TaskMailDetailsReportAsync(userId, role, userName, GetUserId(User.Identity));
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
        *             "pageType":"Next"
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
            _logger.Debug("before convert -" + createdOn);
            DateTime createdDate = DateTime.ParseExact(createdOn, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            _logger.Debug("after convert dd-MM-yyyy -" + createdDate);
            _logger.Debug("CreatedOn string - " + createdOn);
            if (pageType == _stringConstantRepository.NextPage)
            {
                _logger.Debug("Before Date Add CreatedOn (For Next Day)  - " + createdOn);
                createdDate = createdDate.AddDays(1);
                _logger.Debug("After Date Added createdDate (For Next Day) - " + createdDate);
            }
            else
            {
                _logger.Debug("Before Date subtract CreatedOn (For Previous Day) - " + createdOn);
                createdDate = createdDate.AddDays(-1);
                _logger.Debug("After Date subtract createdDate (For Previous Day) - " + createdDate);
            }
            _logger.Debug("Task Controller CreatedOn  - " + createdOn);
            _logger.Debug("Task Controller createdDate  - " + createdDate);
            return await _taskMailReport.TaskMailDetailsReportSelectedDateAsync(userId, userName, role, createdOn, GetUserId(User.Identity), createdDate);
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
            return await _taskMailReport.TaskMailDetailsReportSelectedDateAsync(userId, userName, role, createdOn, GetUserId(User.Identity), Convert.ToDateTime(selectedDate));
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
        public async Task<List<TaskMailReportAc>> GetUserInformationAsync()
        {
            _logger.Info("Get User Information by Id "+ GetUserId(User.Identity));
            return await _taskMailReport.GetUserInformationAsync(GetUserId(User.Identity));
        }

    }
}