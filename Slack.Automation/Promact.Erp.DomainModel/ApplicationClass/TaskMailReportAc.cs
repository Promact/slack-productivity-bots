using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class TaskMailReportAc
    {
        public TaskMailReportAc(string userId, string userRole, string userName, List<TaskMailDetailReportAc> taskMailDetailReportAc=null ,DateTime? createdOn=null, DateTime? maxDate=null, DateTime? minDate=null, string userEmail = null)
        {
            UserId = userId;
            UserName = userName;
            UserRole = userRole;
            CreatedOn = createdOn;
            MaxDate = maxDate;
            MinDate = minDate;
            TaskMails = taskMailDetailReportAc;
            UserEmail = userEmail;
        }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string UserEmail { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? MinDate { get; set; }
        [JsonProperty("TaskMails")]
        public List<TaskMailDetailReportAc> TaskMails { get; set; }
    }
}
