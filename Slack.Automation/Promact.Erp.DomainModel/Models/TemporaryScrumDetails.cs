using System;

namespace Promact.Erp.DomainModel.Models
{
    public class TemporaryScrumDetails : ModelBase
    {
        public int ProjectId { get; set; }
        public string SlackUserId { get; set; }
        public DateTime ScrumDate { get; set; }
    }
}
