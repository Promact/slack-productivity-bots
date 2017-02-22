using System;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class MailSetting
    {
        /// <summary>
        /// Primary Key of MailSetting table
        /// </summary>
        public int Id { get; set; }
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
        /// Created Date of MailSetting
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Updated Date of MailSetting
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
    }
}
