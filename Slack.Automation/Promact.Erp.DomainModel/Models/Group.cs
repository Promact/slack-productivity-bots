using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.Models
{
    public class Group : ModelBase
    {
        /// <summary>
        /// Name of Group
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// Type of Group
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// Updated Date of Group
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<GroupEmailMapping> GroupEmailMapping { get; set; }
    }
}
