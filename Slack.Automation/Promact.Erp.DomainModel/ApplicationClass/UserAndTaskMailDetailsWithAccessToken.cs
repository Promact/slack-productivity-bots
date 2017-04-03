using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class UserAndTaskMailDetailsWithAccessToken
    {
        public UserAndTaskMailDetailsWithAccessToken()
        {
            IsTaskMailContinue = false;
        }
        public User User { get; set; }
        public string AccessToken { get; set; }
        public TaskMail TaskMail { get; set; }
        public string QuestionText { get; set; }
        public IEnumerable<TaskMailDetails> TaskList { get; set; }
        public bool IsTaskMailContinue { get; set; }
    }
}
