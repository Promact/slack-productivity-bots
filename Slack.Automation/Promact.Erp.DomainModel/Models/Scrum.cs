using System;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class Scrum : ModelBase
    {
        /// <summary>
        /// Channel Name
        /// </summary>
        [Required]
        public string GroupName { get; set; }

        /// <summary>
        /// Team Leader's Id from Oauth
        /// </summary>
        [Required]
        public string TeamLeaderId { get; set; }

        /// <summary>
        /// Project Id from Oauth
        /// </summary>
        [Required]
        public int ProjectId { get; set; }

        /// <summary>
        /// Date on which Scrum is conducted
        /// </summary>
        [Required]
        public DateTime ScrumDate { get; set; }

        /// <summary>
        /// True if scrum is halted
        /// </summary>
        public bool IsHalted { get; set; }
    }
}

