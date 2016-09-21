using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class TaskMailUserAc
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime IsMax { get; set; }
        public DateTime IsMin { get; set; }
        [JsonProperty("TaskMails")]
        public List<TaskMailReportAc> TaskMails { get; set; }
    }
}
