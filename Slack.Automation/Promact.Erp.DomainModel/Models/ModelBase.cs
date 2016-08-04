using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.Models
{
    /// <summary>
    /// Class used for inherited on other class to used the below listed parameters
    /// </summary>
    public class ModelBase
    {
        /// <summary>
        /// Primary Key Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
