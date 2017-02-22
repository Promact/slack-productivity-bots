using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Promact.Erp.DomainModel.Models
{
    public class GroupEmailMapping : ModelBase
    {
        /// <summary>
        /// Email of GroupEmailMapping
        /// </summary>
        [Required]
        [StringLength(25)]
        public string Email { get; set; }
        /// <summary>
        /// Foreign key "GroupId" of GroupEmailMapping from Group
        /// </summary>
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
    }
}
