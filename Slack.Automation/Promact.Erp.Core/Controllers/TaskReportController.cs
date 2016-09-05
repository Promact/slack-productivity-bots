using Promact.Core.Repository.TaskMailRepository;
using System.Threading.Tasks;
using System.Web.Http;
using Promact.Erp.DomainModel.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Promact.Erp.DomainModel.ApplicationClass;

namespace Promact.Erp.Core.Controllers
{
   
   
    public class TaskReportController: ApiController
    {
        private readonly ITaskMailRepository _taskMailReport;

        public TaskReportController(ITaskMailRepository taskMailReport)
        {
            this._taskMailReport = taskMailReport;
        }

        [HttpGet]
        [Route("taskMailReport")]
        public async Task<IHttpActionResult> taskMailReport()
        {
            string UserId = User.Identity.GetUserId();
            IEnumerable<TaskMailReportAc> taskMailReportAc = await _taskMailReport.TaskMailReport(UserId);
            return Ok(taskMailReportAc);
        }


        [HttpGet]
        [Route("taskMailDetailsReport/{id}")]
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReport(int id)
        {
            return await _taskMailReport.TaskMailDetailsReport(id);
        }
        
    }
}
