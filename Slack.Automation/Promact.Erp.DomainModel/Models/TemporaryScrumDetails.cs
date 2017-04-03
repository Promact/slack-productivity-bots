using System;

namespace Promact.Erp.DomainModel.Models
{
    public class TemporaryScrumDetails : ModelBase
    {
        public int ScrumId { get; set; }
        public string SlackUserId { get; set; }
        public int? QuestionId { get; set; }
        public int AnswerCount { get; set; }
    }
}
