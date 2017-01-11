using System;

namespace Promact.Erp.DomainModel.Models
{
    public class TemporaryScrumDetails : ModelBase
    {
        public int ScrumId { get; set; }
        public string SlackUserId { get; set; }
        public int? QuestionId { get; set; }
        public int AnswerCount { get; set; }

        //always fetching based on SlackUserId and fetching EmployeeId from user list(which comes from OAuth)
        //gives a double surety
        //public string EmployeeId { get; set; }
    }
}
