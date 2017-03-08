using NLog;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.EmailServiceTemplateRepository;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.Email;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.TaskMailReportRepository
{
    public class TaskMailReportRepository : ITaskMailReportRepository
    {
        #region Private Variables
        private readonly IRepository<TaskMail> _taskMailRepository;
        private readonly IRepository<TaskMailDetails> _taskMailDetailRepository;
        private readonly IOauthCallHttpContextRespository _oauthCallsRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IEmailService _emailService;
        private readonly ApplicationUserManager _userManager;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEmailServiceTemplateRepository _emailServiceTemplate;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public TaskMailReportRepository(IRepository<TaskMail> taskMailRepository, IStringConstantRepository stringConstant,
            IOauthCallHttpContextRespository oauthCallsRepository, IRepository<TaskMailDetails> taskMailDetailRepository,
            IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> userRepository, IEmailService emailService,
            IBotQuestionRepository botQuestionRepository, ApplicationUserManager userManager,
            IEmailServiceTemplateRepository emailServiceTemplate)
        {
            _taskMailRepository = taskMailRepository;
            _stringConstant = stringConstant;
            _oauthCallsRepository = oauthCallsRepository;
            _taskMailDetailRepository = taskMailDetailRepository;
            _attachmentRepository = attachmentRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _botQuestionRepository = botQuestionRepository;
            _userManager = userManager;
            _emailServiceTemplate = emailServiceTemplate;
            _logger = LogManager.GetLogger("TaskReportModule");
        }
        #endregion

        #region Public Method
        /// <summary>
        ///Method geting Employee or list of Employees 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of task mail report</returns>
        public async Task<List<TaskMailReportAc>> GetUserInformationAsync(string userId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            var user = await _userRepository.FirstAsync(x => x.Id == userId);

            //getting user information from Promact Oauth Server.
            _logger.Info("Getting user information from oauth server");
            List<UserRoleAc> userRoleAcList = await _oauthCallsRepository.GetUserRoleAsync(user.Id);
            var userInformation = userRoleAcList.First(x => x.UserName == user.UserName);
            if (userInformation.Role == _stringConstant.RoleAdmin)
            {
                //if user is admin then remove from user list. because admin dose not have any taks mail.
                userRoleAcList.Remove(userInformation);
            }
            foreach (var userRole in userRoleAcList)
            {
                TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userRole.UserId, userRole.Role, userRole.Name, userEmail: userRole.UserName);
                taskMailReportAcList.Add(taskMailReportAc);
            }
            _logger.Debug("Task Report List");
            return taskMailReportAcList;
        }

        /// <summary>
        /// This Method use to fetch the task mail details.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="loginId"></param>
        /// <returns>list of task mail report with task mail details</returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string userId, string role, string userName, string loginId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            if (role == _stringConstant.RoleAdmin || role == _stringConstant.RoleEmployee)
            {
                //getting the employee task mail reports   
                taskMailReportAcList = await GetTaskMailDetailsInformationAsync(userId, role, userName, loginId);
            }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                //getting the team members information.
                List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
                //getting maximum and minimum date from the team members task mails
                var maxMinTaskMailDate = await GetMaxMinDateAsync(userRoleAcList);
                //first time there are no selected date that's why pass maxdate as a selected date.
                //getting the team members task mail reports
                taskMailReportAcList = await TaskMailDetailsAsync(role, loginId, maxMinTaskMailDate.Item1.Date, maxMinTaskMailDate.Item1.Date, maxMinTaskMailDate.Item2.Date);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// this Method use to fetch the task mail details for the selected date.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <returns>list of task mail report with task mail details</returns>
        public async Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string userId, string userName, string role, string createdOn, string loginId, DateTime selectedDate)
        {
            _logger.Debug("In Repo Created On" + createdOn);
            _logger.Debug("In Repo Selected Date " + selectedDate);
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            if (role == _stringConstant.RoleAdmin || role == _stringConstant.RoleEmployee)
            {
                //getting the employee task mail reports for selected date  
                taskMailReportAcList = await TaskMailsDetailAsync(userId, userName, role, loginId, createdOn, selectedDate);
            }
            else if (role == _stringConstant.RoleTeamLeader)
            {
                //getting the team members information 
                List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
                //find maximum and minimum date from the team members task mails
                var maxMinTaskMailDate = await GetMaxMinDateAsync(userRoleAcList);
                //getting the team members task mail reports for selected date
                taskMailReportAcList = await TaskMailDetailsAsync(role, loginId, selectedDate.Date, maxMinTaskMailDate.Item1.Date, maxMinTaskMailDate.Item2.Date);
            }
            return taskMailReportAcList;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Getting user information
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns>fetch users role</returns>
        private async Task<List<UserRoleAc>> GetUserRoleAsync(string loginId)
        {
            //getting user information from Promact Oauth Server
            return await _oauthCallsRepository.GetListOfEmployeeAsync(loginId);
        }


        /// <summary>
        /// Getting max and min date from users task mails
        /// </summary>
        /// <param name="userRoleAcList"></param>
        /// <returns>max and min Date</returns> 
        private async Task<Tuple<DateTime, DateTime>> GetMaxMinDateAsync(List<UserRoleAc> userRoleAcList)
        {
            //getting list of userId.
            var userIdList = userRoleAcList.Select(x => x.UserId);
            //getting list of task mails using userIdList.
            var taskMails = (await _taskMailRepository.FetchAsync(x => userIdList.Contains(x.EmployeeId))).ToList();
            //getting maximum and minimum date form the team members task mails
            DateTime maxDate = taskMails.Max(x => x.CreatedOn);
            DateTime minDate = taskMails.Min(x => x.CreatedOn);
            return new Tuple<DateTime, DateTime>(maxDate, minDate);
        }


        /// <summary>
        /// Task mail details for teamLeader
        /// </summary>
        /// <param name="role"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns>list of task mail reports</returns>
        private async Task<List<TaskMailReportAc>> TaskMailDetailsAsync(string role, string loginId, DateTime selectedDate, DateTime maxDate, DateTime minDate)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            List<UserRoleAc> userRoleAcList = await GetUserRoleAsync(loginId);
            //getting the team members task mails using users information.
            foreach (var userRole in userRoleAcList)
            {
                TaskMailReportAc taskMailReportAc = await GetTaskReportAsync(userRole.UserId, role, userRole.Name, selectedDate, maxDate, minDate);
                taskMailReportAcList.Add(taskMailReportAc);
            }
            return taskMailReportAcList;
        }

        /// <summary>
        /// Getting task mail reports
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="selectedDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns>task mail report</returns>
        private async Task<TaskMailReportAc> GetTaskReportAsync(string userId, string role, string userName, DateTime selectedDate, DateTime maxDate, DateTime minDate)
        {
            _logger.Debug("In Get Task Report Async selected Date" + selectedDate);
            _logger.Debug("In Get Task Report Async max Date" + maxDate);
            _logger.Debug("In Get Task Report Async min Date" + minDate);
            TaskMailReportAc taskMailReportAc;
            var taskMail = (await _taskMailRepository.FirstOrDefaultAsync(y => y.EmployeeId == userId && DbFunctions.TruncateTime(y.CreatedOn) == DbFunctions.TruncateTime(selectedDate)));
            if (taskMail != null)
            {
                //getting the team members task mails details.
                _logger.Debug("if taskmail not null then" + taskMail.CreatedOn.Date);
                taskMailReportAc = await GetTaskMailReportAsync(userId, role, userName, taskMail.Id, taskMail.CreatedOn.Date, maxDate, minDate);
            }
            else
            {
                _logger.Debug("if taskmail not then selected date" + selectedDate.Date);
                _logger.Debug("if taskmail not then max Date" + maxDate.Date);
                _logger.Debug("if taskmail not then min Date" + minDate.Date);
                //if team member does not have any task mail than show default task mail to the end users. 
                taskMailReportAc = GetTaskMailReport(userId, role, userName, selectedDate.Date, maxDate.Date, minDate.Date);
            }
            return taskMailReportAc;
        }

        /// <summary>
        /// Task mail fetails for admin or employee 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <returns>list of task mail reports</returns>
        private async Task<List<TaskMailReportAc>> TaskMailsDetailAsync(string userId, string userName, string role, string createdOn, string loginId, DateTime selectedDate)
        {
            _logger.Debug("Task Mail Detail Async created on" + createdOn);
            _logger.Debug("Task Mail Detail Async selected date" + selectedDate);
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            //find maximum and minimum date from the employee task mails
            IEnumerable<TaskMail> taskMails = (await _taskMailRepository.FetchAsync(x => x.EmployeeId == userId)).ToList();
            DateTime maxDate = taskMails.Max(x => x.CreatedOn);
            DateTime minDate = taskMails.Min(x => x.CreatedOn);
            _logger.Debug("Task Mail Detail Async maxDate" + maxDate);
            _logger.Debug("Task Mail Detail Async minDate" + minDate);
            //getting task mail information.
            TaskMailReportAc taskMailReportAc = await GetTaskReportAsync(userId, role, userName, selectedDate.Date, maxDate.Date, minDate.Date);
            taskMailReportAcList.Add(taskMailReportAc);
            return taskMailReportAcList;
        }

        /// <summary>
        /// Get default task mail 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="createdOn"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns>Task mail report</returns>
        private TaskMailReportAc GetTaskMailReport(string userId, string role, string userName, DateTime createdOn, DateTime maxDate, DateTime minDate)
        {
            List<TaskMailDetailReportAc> taskMailDetailReportList = new List<TaskMailDetailReportAc>();
            var taskMailDetailReportAc = new TaskMailDetailReportAc(description: _stringConstant.NotAvailable, comment: _stringConstant.NotAvailable, status: TaskMailStatus.none);
            taskMailDetailReportList.Add(taskMailDetailReportAc);
            TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userId, role, userName, taskMailDetailReportList, createdOn: createdOn, maxDate: maxDate, minDate: minDate);
            return taskMailReportAc;
        }

        /// <summary>
        /// Getting taskmail details infromation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="taskId"></param>
        /// <param name="createdOn"></param>
        /// <param name="maxDate"></param>
        /// <param name="minDate"></param>
        /// <returns>List of task mail report</returns>
        private async Task<TaskMailReportAc> GetTaskMailReportAsync(string userId, string role, string userName, int taskId, DateTime createdOn, DateTime maxDate, DateTime minDate)
        {
            List<TaskMailDetailReportAc> taskMailDetailReportAcList = new List<TaskMailDetailReportAc>();
            // getting taskmail details infromation using taskId. 
            var taskMailDetailList = (await _taskMailDetailRepository.FetchAsync(x => x.TaskId == taskId));
            foreach (var taskMailDetail in taskMailDetailList)
            {
                TaskMailDetailReportAc taskmailReportAc = new TaskMailDetailReportAc(taskMailDetail.Description, taskMailDetail.Comment, id: taskMailDetail.Id, hours: taskMailDetail.Hours, status: taskMailDetail.Status);
                taskMailDetailReportAcList.Add(taskmailReportAc);
            }
            TaskMailReportAc taskMailReportAc = new TaskMailReportAc(userId, role, userName, taskMailDetailReportAcList, createdOn: createdOn, maxDate: maxDate, minDate: minDate);
            return taskMailReportAc;
        }

        /// <summary>
        /// Task mail details report information for the user role admin and employee
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="loginId"></param>
        /// <returns>List task mail Report</returns>
        private async Task<List<TaskMailReportAc>> GetTaskMailDetailsInformationAsync(string userId, string role, string userName, string loginId)
        {
            List<TaskMailReportAc> taskMailReportAcList = new List<TaskMailReportAc>();
            var taskMail = (await _taskMailRepository.FetchAsync(y => y.EmployeeId == userId)).ToList();
            TaskMailReportAc taskMailReportAc;
            if (taskMail.Any())
            {
                //first time there are no selected date that's why pass maxdate as a selected date.
                taskMailReportAc = await GetTaskMailReportAsync(userId, role, userName, taskMail.OrderByDescending(y => y.CreatedOn).First().Id, taskMail.Max(x => x.CreatedOn).Date, taskMail.Max(x => x.CreatedOn).Date, taskMail.Min(x => x.CreatedOn).Date);
            }
            else
            {
                //if employee does not have any task mail than show default task mail to the end users. 
                taskMailReportAc = GetTaskMailReport(userId, role, userName, DateTime.UtcNow.Date, DateTime.UtcNow.Date, DateTime.UtcNow.Date);
            }
            taskMailReportAcList.Add(taskMailReportAc);
            return taskMailReportAcList;
        }

        #endregion
    }
}
