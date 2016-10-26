using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.StringConstants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.LeaveReportRepository
{
    public class LeaveReportRepository : ILeaveReportRepository
    {
        private readonly IRepository<LeaveRequest> _leaveRequest;
        private readonly IProjectUserCallRepository _projectUserCall;
        private readonly IStringConstantRepository _stringConstant;
        public LeaveReportRepository(IRepository<LeaveRequest> leaveRequest, IStringConstantRepository stringConstant, IProjectUserCallRepository projectUserCall)
        {
            _leaveRequest = leaveRequest;
            _stringConstant = stringConstant;
            _projectUserCall = projectUserCall;
        }

        /// <summary>
        /// Method that returns the list of employees with their leave status based on their roles
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="userName"></param>
        /// <returns>List of employees with leave status based on roles</returns>       
        public async Task<IEnumerable<LeaveReport>> LeaveReport(string accessToken,string userName )
        {
            List<LeaveRequest> leaveRequests = _leaveRequest.GetAll().ToList();
                      
            User loginUser = await _projectUserCall.GetUserByUserName(userName, accessToken);

            if (loginUser.Role.Equals(_stringConstant.Admin))
            {
                List<LeaveRequest> distinctLeaveRequests = leaveRequests.GroupBy(x => x.EmployeeId).Select(x => x.FirstOrDefault()).ToList();
                List<LeaveReport> leaveReports = await GetLeaveReportList(distinctLeaveRequests, accessToken);
                return leaveReports;
            }
            else if (loginUser.Role.Equals(_stringConstant.Employee))
            {
                List<LeaveRequest> distinctLeaveRequests = leaveRequests.FindAll(x => x.EmployeeId == loginUser.Id);
                List<LeaveReport> leaveReports = await GetLeaveReportList(distinctLeaveRequests, accessToken);
                return leaveReports;
            }
            if(loginUser.Role.Equals(_stringConstant.TeamLeader))
            {
                List<User> projectUsers = await _projectUserCall.GetProjectUsersByTeamLeaderId(loginUser.Id, accessToken);
                List<LeaveReport> leaveReports = new List<LeaveReport>();
                foreach (var projectUser in projectUsers)
                {
                    List<LeaveRequest> distinctLeaveRequests = leaveRequests.Where(x => x.EmployeeId.Contains(projectUser.Id)).GroupBy(x => x.EmployeeId).Select(x => x.FirstOrDefault()).ToList();
                    List <LeaveReport> leaveReport = await GetLeaveReportList(distinctLeaveRequests, accessToken);
                    leaveReports.AddRange(leaveReport);
                }
                return leaveReports;
            }
            return null;
        }

        /// <summary>
        /// Method to returns the list of leave reports
        /// </summary>
        /// <param name="distinctLeaveRequests"></param>
        /// <param name="accessToken"></param>
        /// <returns>List of leave reports</returns>
        private async Task<List<LeaveReport>> GetLeaveReportList(List<LeaveRequest> distinctLeaveRequests, string accessToken)
        {
            List<LeaveReport> leaveReports = new List<LeaveReport>();
            foreach (var leaveRequest in distinctLeaveRequests)
            {
                User user = await GetEmployeeById(leaveRequest.EmployeeId, accessToken);
                if (user != null)
                {
                    LeaveReport leaveReport = new LeaveReport
                    {
                        Role = user.Role,
                        EmployeeId = user.Id,
                        EmployeeUserName = user.Email,
                        EmployeeName = string.Format("{0} {1}", user.FirstName, user.LastName),
                        TotalCasualLeave = user.NumberOfCasualLeave,
                        TotalSickLeave = user.NumberOfSickLeave,
                        UtilisedCasualLeave = GetUtilisedCasualLeavesByEmployee(leaveRequest.EmployeeId),
                        BalanceCasualLeave = user.NumberOfCasualLeave - GetUtilisedCasualLeavesByEmployee(leaveRequest.EmployeeId),
                        //UtilisedSickLeave = null,
                        //BalanceSickLeave = null
                    };
                    leaveReports.Add(leaveReport);
                }
            }
            return leaveReports;
        }

        /// <summary>
        /// Method to calculate number of casual leaves used by each employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Number of casual leaves utilised by that particular employee</returns>
        private int GetUtilisedCasualLeavesByEmployee(string employeeId)
        {
            var utilisedCasualLeave = 0;
            var leaves = _leaveRequest.Fetch(x => x.EmployeeId == employeeId).ToList();
            foreach (var leave in leaves)
            { 
             if (leave.Status == Condition.Approved)
                 {
                    utilisedCasualLeave = utilisedCasualLeave + leave.EndDate.Value.Date.Subtract(leave.FromDate).Days + 1;
                 }
            }
            return utilisedCasualLeave;
        }

        /// <summary>
        /// Method that returns the details of leave for an employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="accessToken"></param>
        /// <returns>Details of leave for an employee</returns>
        public async Task <IEnumerable<LeaveReportDetails>> LeaveReportDetails(string employeeId,string accessToken)
        {
            User user = await GetEmployeeById(employeeId,accessToken);
            if(user != null)
            {
                var leaves = _leaveRequest.Fetch(x => x.EmployeeId == employeeId).ToList();
                List<LeaveReportDetails> leaveReportDetails = new List<LeaveReportDetails>();

                foreach (var leave in leaves)
                {
                    LeaveReportDetails leaveReportDetail = new LeaveReportDetails();
                    if (leave.Status == Condition.Approved)
                    {
                        leaveReportDetail.EmployeeUserName = user.Email;
                        leaveReportDetail.EmployeeName = string.Format("{0} {1}", user.FirstName, user.LastName);
                        leaveReportDetail.LeaveFrom = leave.FromDate.ToShortDateString();
                        leaveReportDetail.StartDay = leave.FromDate.DayOfWeek.ToString();
                        leaveReportDetail.LeaveUpto = leave.EndDate.Value.ToShortDateString();
                        leaveReportDetail.EndDay = leave.EndDate.Value.DayOfWeek.ToString();
                        leaveReportDetail.Reason = leave.Reason;
                    }
                    leaveReportDetails.Add(leaveReportDetail);
                }
                return leaveReportDetails;
            }
            return null;
        }

        /// <summary>
        /// Method to get user details from the Oauth server using id and access token
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task<User> GetEmployeeById(string employeeId,string accessToken)
        {
            User user = await _projectUserCall.GetUserByEmployeeId(employeeId,accessToken);
            return user;
        }
    }
}
