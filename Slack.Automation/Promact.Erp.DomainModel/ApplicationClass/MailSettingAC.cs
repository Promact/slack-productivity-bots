using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class MailSettingAC
    {
        public int Id { get; set; }
        public string Module { get; set; }
        public int ProjectId { get; set; }
        public bool SendMail { get; set; }
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
    }
}
