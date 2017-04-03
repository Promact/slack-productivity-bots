using System.Collections.Generic;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class EmailApplication
    {
        public EmailApplication()
        {
            To = new List<string>();
            CC = new List<string>();
        }
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
        public List<string> To { get; set; }

        /// <summary>
        /// Email cc
        /// </summary>
        public List<string> CC { get; set; }
    }
}
