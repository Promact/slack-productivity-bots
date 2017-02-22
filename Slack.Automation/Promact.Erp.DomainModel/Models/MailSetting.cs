using System;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class MailSetting : ModelBase
    {
        /// <summary>
        /// Module name of MailSetting
        /// </summary>
        [Required]
        [StringLength(25)]
        public string Module { get; set; }
        /// <summary>
        /// Send mail status of MailSetting
        /// </summary>
        public bool SendMail { get; set; }
        /// <summary>
        /// Updated Date of MailSetting
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
        /// <summary>
        /// OAuth server's project's Id
        /// </summary>
        [Required]
        public string ProjectId { get; set; }
    }
}
