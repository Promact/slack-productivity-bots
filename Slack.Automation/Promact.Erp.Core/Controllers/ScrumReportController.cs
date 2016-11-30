using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.ScrumReportRepository;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Promact.Erp.Core.Controllers
{
    [RoutePrefix("api/project")]
    [Authorize]
    public class ScrumReportController : WebApiBaseController
    {
        #region Private Variables
        private readonly IScrumReportRepository _scrumReportRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private ApplicationUserManager _userManager;
        #endregion

        #region Constructor
        public ScrumReportController(IScrumReportRepository scrumRepository, IAttachmentRepository attachmentRepository, ApplicationUserManager userManager)
        {
            _scrumReportRepository = scrumRepository;
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
        }
        #endregion

        #region Public methods

        /**
        * @api {get} api/project
        * @apiVersion 1.0.0
        * @apiName GetProject
        * @apiGroup ScrumReport    
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"A report will be generated to display the projects based on the logged in users role"
        *    
        *     [
        *       {
        *         "id": 8,
        *         "name": "abc",
        *         "slackChannelName": "abc",
        *         "isActive": true,
        *         "teamLeaderId": "74df3a1d-d755-4260-801a-bb964fb83293",
        *         "createdBy": "7b722d05-a448-4c08-b768-2b0ff98f92e2",
        *         "createdDate": "2016-11-11",
        *         "teamLeader": {
        *           "Id": "74df3a1d-d755-4260-801a-bb964fb83293",
        *           "FirstName": "raj",
        *           "LastName": "raj",
        *           "IsActive": true,
        *           "Role": "TeamLeader",
        *           "NumberOfCasualLeave": 14, 
        *           "NumberOfSickLeave": 7,
        *           "JoiningDate": "2016-07-20T18:30:00",
        *           "SlackUserName": "raj",   
        *           "Email": "raj@promactinfo.com", 
        *           "UserName": "raj@promactinfo.com", 
        *           "UniqueName": "raj-raj@promactinfo.com",
        *         },
        *         "applicationUsers": [
        *           {
        *             "Id": "f2e3a733-96cb-4164-ba04-599718ff0ae1",
        *             "FirstName": "ram",
        *             "LastName": "ram",
        *             "IsActive": true,
        *             "Role": "Employee",
        *             "NumberOfCasualLeave": 14,
        *             "NumberOfSickLeave": 7,
        *             "JoiningDate": "2016-01-13T18:30:00",
        *             "SlackUserName": "ram",
        *             "Email": "ram@promactinfo.com",
        *             "UserName": "ram@promactinfo.com",
        *             "UniqueName": "ram-ram@promactinfo.com",
        *           }
        *         ] 
        *       },
        *     ]      
        * }   
        */
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ScrumProjectListAsync()
        {
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(User.Identity.Name);
            var loginUser = await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok(await _scrumReportRepository.GetProjectsAsync(loginUser.Id, accessToken));
        }

        /**
        * @api {get} api/project/{id}/detail?date={date}
        * @apiVersion 1.0 
        * @apiName GetProjectDetail
        * @apiGroup ScrumReport
        * @apiParam {int} id   
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK 
        * {
        *     "Description":"A report will be generated displaying the details of a scrum for a particular project "
        *     
        *     [
        *      {
        *       "ScrumDate": "2016-11-15",
        *       "ProjectCreationDate": "Nov 21,2016",
        *       "EmployeeScrumAnswers" : [
        *         {   
        *           "EmployeeName": "gourav agarwal",
        *           "Answer1": null,
        *           "Answer2": null,
        *           "Answer3": null,
        *           "Status" : "Person not available"
        *         },
        *        ]
        *      },
        *     ]       
        * }
        */
        [HttpGet]
        [Route("{id}/detail")]
        public async Task<IHttpActionResult> ScrumDetailsAsync(int id)
        {
            string queryString = Request.RequestUri.Query.Substring(1,24);
            DateTime date = Convert.ToDateTime(queryString); 
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(User.Identity.Name);
            var loginUser = await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok(await _scrumReportRepository.ScrumReportDetailsAsync(id, date,loginUser.Id, accessToken));
        }
        #endregion
    }
}
