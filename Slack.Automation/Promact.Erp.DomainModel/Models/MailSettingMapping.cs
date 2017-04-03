using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Promact.Erp.DomainModel.Models
{
    public class MailSettingMapping : ModelBase
    {
        /// <summary>
        /// Email of MailSettingMapping
        /// </summary>
        [StringLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// IsTo of MailSettingMapping, true if is TO else false for CC
        /// </summary>
        public bool IsTo { get; set; }
        /// <summary>
        /// Foreign key "MailSettingId" of MailSettingMapping from MailSetting
        /// </summary>
        public int MailSettingId { get; set; }
        [ForeignKey("MailSettingId")]
        public virtual MailSetting MailSetting { get; set; }
        /// <summary>
        /// Foreign key "GroupId" of MailSettingMapping from Group
        /// </summary>
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
    }
}
