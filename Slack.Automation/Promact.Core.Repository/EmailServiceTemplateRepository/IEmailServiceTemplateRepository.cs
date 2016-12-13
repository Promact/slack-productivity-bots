using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.EmailServiceTemplateRepository
{
    public interface IEmailServiceTemplateRepository
    {
        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <returns>template emailBody as string</returns>
        string EmailServiceTemplate(LeaveRequest leaveRequest);

        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest object</param>
        /// <returns>template emailBody as string</returns>
        string EmailServiceTemplateSickLeave(LeaveRequest leaveRequest);

        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="taskMail">List of TaskMail</param>
        /// <returns>template emailBody as string</returns>
        string EmailServiceTemplateTaskMail(List<TaskMailDetails> taskMail);

        /// <summary>
        /// Method to generate template body 
        /// </summary>
        /// <param name="leave">LeaveRequest object</param>
        /// <returns>template emailBody as string</returns>
        string EmailServiceTemplateLeaveUpdate(LeaveRequest leave);
    }
}
