using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.Email_Templates;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Promact.Core.Repository.EmailServiceTemplateRepository
{
    public class EmailServiceTemplateRepository : IEmailServiceTemplateRepository
    {
        #region Private Variable
        private readonly IStringConstantRepository _stringConstant;
        #endregion

        #region Constructor
        public EmailServiceTemplateRepository(IStringConstantRepository stringConstant)
        {
            _stringConstant = stringConstant;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest template object</param>
        /// <returns>template emailBody as string</returns>
        public string EmailServiceTemplate(LeaveRequest leaveRequest)
        {

            var dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            LeaveApplication leaveTemplate = new LeaveApplication();
            // Assigning Value in template page
            leaveTemplate.Session = new Dictionary<string, object>
            {
                {_stringConstant.FromDate,leaveRequest.FromDate.ToString(dateFormat) },
                {_stringConstant.EndDate,leaveRequest.EndDate.Value.ToString(dateFormat) },
                {_stringConstant.Reason,leaveRequest.Reason },
                {_stringConstant.Type,leaveRequest.Type.ToString() },
                {_stringConstant.Status,leaveRequest.Status.ToString() },
                {_stringConstant.ReJoinDate,leaveRequest.RejoinDate.Value.ToString(dateFormat) },
                {_stringConstant.CreatedOn,leaveRequest.CreatedOn.ToString(dateFormat) },
            };
            leaveTemplate.Initialize();
            var emailBody = leaveTemplate.TransformText();
            return emailBody;
        }

        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest template object</param>
        /// <returns>template emailBody as string</returns>
        public string EmailServiceTemplateSickLeave(LeaveRequest leaveRequest)
        {
            var dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            SickLeaveApplication leaveTemplate = new SickLeaveApplication();
            // Assigning Value in template page
            leaveTemplate.Session = new Dictionary<string, object>
            {
                {_stringConstant.FromDate,leaveRequest.FromDate.ToString(dateFormat) },
                {_stringConstant.Reason,leaveRequest.Reason },
                {_stringConstant.Type,leaveRequest.Type.ToString() },
                {_stringConstant.Status,leaveRequest.Status.ToString() },
                {_stringConstant.CreatedOn,leaveRequest.CreatedOn.ToString(dateFormat) },
            };
            leaveTemplate.Initialize();
            var emailBody = leaveTemplate.TransformText();
            return emailBody;
        }

        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">TaskMail template object</param>
        /// <returns>template emailBody as string</returns>
        public string EmailServiceTemplateTaskMail(List<TaskMailDetails> taskMail)
        {
            Erp.Util.Email_Templates.TaskMail leaveTemplate = new Erp.Util.Email_Templates.TaskMail();
            // Assigning Value in template page
            leaveTemplate.Session = new Dictionary<string, object>
            {
                {_stringConstant.TaskMailDescription, taskMail},
            };
            leaveTemplate.Initialize();
            var emailBody = leaveTemplate.TransformText();
            return emailBody;
        }

        public string EmailServiceTemplateLeaveUpdate(LeaveRequest leave)
        {
            var dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            LeaveApproveAndRejected leaveTemplate = new LeaveApproveAndRejected();
            leaveTemplate.Session = new Dictionary<string, object>
            {
                {_stringConstant.FromDate,leave.FromDate.ToString(dateFormat) },
                {_stringConstant.EndDate,leave.EndDate.Value.ToString(dateFormat) },
                {_stringConstant.Reason,leave.Reason },
                {_stringConstant.Type,leave.Type.ToString() },
                {_stringConstant.Status,leave.Status.ToString() },
                {_stringConstant.ReJoinDate,leave.RejoinDate.Value.ToString(dateFormat) },
                {_stringConstant.CreatedOn,leave.CreatedOn.ToString(dateFormat) },
            };
            leaveTemplate.Initialize();
            var emailBody = leaveTemplate.TransformText();
            return emailBody;
        }
        #endregion
    }
}
