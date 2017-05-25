using AutoMapper;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.Email_Templates;
using Promact.Erp.Util.StringLiteral;
using System.Collections.Generic;
using System.Threading;

namespace Promact.Core.Repository.EmailServiceTemplateRepository
{
    public class EmailServiceTemplateRepository : IEmailServiceTemplateRepository
    {
        #region Private Variable
        private readonly AppStringLiteral _stringConstant;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public EmailServiceTemplateRepository(ISingletonStringLiteral stringConstant, IMapper mapper)
        {
            _stringConstant = stringConstant.StringConstant;
            _mapper = mapper;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to generate template body
        /// </summary>
        /// <param name="leaveRequest">LeaveRequest object</param>
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
        /// <param name="leaveRequest">LeaveRequest object</param>
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
        /// <param name="taskMail">List of TaskMail</param>
        /// <returns>template emailBody as string</returns>
        public string EmailServiceTemplateTaskMail(IEnumerable<TaskMailDetails> taskMail)
        {

            var taskMailDetails = new List<TaskMailDetailsAC>();
            Erp.Util.Email_Templates.TaskMail leaveTemplate = new Erp.Util.Email_Templates.TaskMail();
            foreach (var task in taskMail)
            {
                TaskMailDetailsAC taskMailDetailAC = new TaskMailDetailsAC();
                taskMailDetailAC = _mapper.Map<TaskMailDetails, TaskMailDetailsAC>(task);
                if (task.Comment.ToLower() == SendEmailConfirmation.no.ToString())
                    taskMailDetailAC.Comment = _stringConstant.Hyphen;
                taskMailDetailAC.Status = GetNameFromTaskMailStatus(task.Status);
                taskMailDetails.Add(taskMailDetailAC);
            }
            // Assigning Value in template page
            leaveTemplate.Session = new Dictionary<string, object>
            {
                {_stringConstant.TaskMailDescription, taskMailDetails},
            };
            leaveTemplate.Initialize();
            var emailBody = leaveTemplate.TransformText();
            return emailBody;
        }

        /// <summary>
        /// Method to generate template body 
        /// </summary>
        /// <param name="leave">LeaveRequest object</param>
        /// <returns>template emailBody as string</returns>
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

        #region Private Method
        /// <summary>
        /// Method to get proper format of string for task mail status
        /// </summary>
        /// <param name="status">task mail status</param>
        /// <returns>string in proper casing</returns>
        private string GetNameFromTaskMailStatus(TaskMailStatus status)
        {
            string reply = string.Empty;
            switch (status)
            {
                case TaskMailStatus.completed:
                    reply = _stringConstant.Completed;
                    break;
                case TaskMailStatus.inprogress:
                    reply = _stringConstant.InProgress;
                    break;
                case TaskMailStatus.roadblock:
                    reply = _stringConstant.RoadBlock;
                    break;
            }
            return reply;
        }
        #endregion
    }
}
