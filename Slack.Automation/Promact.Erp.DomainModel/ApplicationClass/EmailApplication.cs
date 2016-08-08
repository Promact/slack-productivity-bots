using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class EmailApplication
    {
        /// <summary>
        /// Email address from
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Email address Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email Body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Email to
        /// </summary>
        public string To { get; set; }
    }
}
