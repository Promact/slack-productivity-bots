using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class StringConstantJsonAC
    {
        public StringConstantJsonAC()
        {
            User = new Dictionary<string, string>();
            Leave = new Dictionary<string, string>();
            TaskMail = new Dictionary<string, string>();
            Scrum = new Dictionary<string, string>();
            CommonString = new Dictionary<string, string>();
            Redmine = new Dictionary<string, string>();
        }
        public Dictionary<string, string> User { get; set; }
        public Dictionary<string, string> Leave { get; set; }
        public Dictionary<string, string> TaskMail { get; set; }
        public Dictionary<string, string> Scrum { get; set; }
        public Dictionary<string, string> Redmine { get; set; }
        public Dictionary<string, string> CommonString { get; set; }
    }
}
