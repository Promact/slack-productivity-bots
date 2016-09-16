using System;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class Scrum : ModelBase
    {
        [Required]
        public string GroupName { get; set; }
        [Required]
        public string TeamLeaderId { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public DateTime ScrumDate { get; set; }
        public bool IsHalted { get; set; }
    }
}

