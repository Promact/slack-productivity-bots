using System;

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
