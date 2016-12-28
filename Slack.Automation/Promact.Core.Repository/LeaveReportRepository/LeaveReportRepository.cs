using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.LeaveReportRepository
{
    public class LeaveReportRepository : ILeaveReportRepository
    {

        #region Private Variables
        private readonly IRepository<LeaveRequest> _leaveRequest;
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly IStringConstantRepository _stringConstant;
        #endregion

        #region Constructor
        public LeaveReportRepository(IRepository<LeaveRequest> leaveRequest, IStringConstantRepository stringConstant, IOauthCallsRepository oauthCallsRepository)
        {
            _leaveRequest = leaveRequest;
            _stringConstant = stringConstant;
            _oauthCallsRepository = oauthCallsRepository;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Method to return the list of leave reports
        /// </summary>
        /// <param name="distinctLeaveRequests"></param>
        /// <param name="accessToken"></param>
        /// <returns>List of leave reports</returns>
        private async Task<List<LeaveReport>> GetLeaveReportListAsync(List<LeaveRequest> distinctLeaveRequests, string accessToken)
        {
            List<LeaveReport> leaveReports = new List<LeaveReport>();
            foreach (var leaveRequest in distinctLeaveRequests)
            {
                //Get details of the employee from oauth server
                User user = await GetEmployeeByIdAsync(leaveRequest.EmployeeId, accessToken);
                if (user != null)
                {
                    LeaveReport leaveReport = new LeaveReport
                    {
                        Role = user.Role,
                        EmployeeId = user.Id,
                        EmployeeUserName = user.Email,
                        EmployeeName = string.Format(_stringConstant.EmployeeFirstLastNameFormat, user.FirstName, user.LastName),
                        TotalCasualLeave = user.NumberOfCasualLeave,
                        TotalSickLeave = user.NumberOfSickLeave,
                        UtilisedCasualLeave = await GetUtilisedCasualLeavesByEmployeeAsync(leaveRequest.EmployeeId),
                        BalanceCasualLeave = user.NumberOfCasualLeave - (await GetUtilisedCasualLeavesByEmployeeAsync(leaveRequest.EmployeeId)),
                        UtilisedSickLeave = await GetUtilisedSickLeavesByEmployeeAsync(leaveRequest.EmployeeId),
                        BalanceSickLeave = user.NumberOfSickLeave - ( await GetUtilisedSickLeavesByEmployeeAsync(leaveRequest.EmployeeId)),
                    };
                    leaveReports.Add(leaveReport);
                }
            }
            return leaveReports;
        }

        /// <summary>
        /// Method to calculate number of casual leaves used by a specific employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Number of casual leaves utilised</returns>
        private async Task<double> GetUtilisedCasualLeavesByEmployeeAsync(string employeeId)
        {
            var utilisedCasualLeave = 0;
            //Get all leaves applied by the specific employee 
            List<LeaveRequest> leaves = (await _leaveRequest.FetchAsync(x => x.EmployeeId == employeeId)).ToList();
            foreach (var leave in leaves)
            {
                //Calculate utilised casual leaves
                if (leave.Status == Condition.Approved && leave.Type == LeaveType.cl)
                {
                    utilisedCasualLeave = utilisedCasualLeave + leave.EndDate.Value.Date.Subtract(leave.FromDate).Days + 1;
                }
            }
            return utilisedCasualLeave;
        }

        /// <summary>
        /// Method to calculate number of sick leaves used by a specific employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Number of sick leaves utilised</returns>
        private async Task<double> GetUtilisedSickLeavesByEmployeeAsync(string employeeId)
        {
            var utilisedSickLeave = 0;
            //Get all leaves applied by the specific employee
            List<LeaveRequest> leaves = (await _leaveRequest.FetchAsync(x => x.EmployeeId == employeeId)).ToList();
            foreach (var leave in leaves)
            {
                //Calculate utilised sick leaves
                if (leave.Status == Condition.Approved && leave.Type == LeaveType.sl)
                {
                    utilisedSickLeave = utilisedSickLeave + leave.EndDate.Value.Date.Subtract(leave.FromDate).Days + 1;
                }
            }
            return utilisedSickLeave;
        }

        /// <summary>
        /// Method to get user details from the Oauth server using their id and access token
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="accessToken"></param>
        /// <returns>User details</returns>
        private async Task<User> GetEmployeeByIdAsync(string employeeId, string accessToken)
        {
            User user = await _oauthCallsRepository.GetUserByEmployeeIdAsync(employeeId, accessToken);
            return user;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Method that returns leave report based on the role of logged in user
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="userId"></param>
        /// <returns>Leave report</returns>       
        public async Task<IEnumerable<LeaveReport>> LeaveReportAsync(string accessToken,string userId )
        {
            //Get all approved leave requests
            List<LeaveRequest> leaveRequests = _leaveRequest.GetAll().ToList().FindAll(x => x.Status.Equals(Condition.Approved));
            List<LeaveReport> leaveReports = new List<LeaveReport>();
           
            //Get details of logged in user
            User loginUser = await GetEmployeeByIdAsync(userId, accessToken);

            //Check if there exists any approved leave request 
            if(leaveRequests.Any())
            {
                //Return leave report as per the role of logged in user
                //For admin, leave report of all the employees 
                if (loginUser.Role.Equals(_stringConstant.Admin))
                {
                    List<LeaveRequest> distinctLeaveRequests = leaveRequests.GroupBy(x => x.EmployeeId).Select(x => x.FirstOrDefault()).ToList();
                    leaveReports = await GetLeaveReportListAsync(distinctLeaveRequests, accessToken);
                    return leaveReports;
                }
                //For employee, only his leave report
                else if (loginUser.Role.Equals(_stringConstant.Employee))
                {
                    List<LeaveRequest> distinctLeaveRequests = leaveRequests.FindAll(x => x.EmployeeId == loginUser.Id);
                    leaveReports = await GetLeaveReportListAsync(distinctLeaveRequests, accessToken);
                    return leaveReports;
                }
                //For teamleader, leave report of all the team member(s) 
                if (loginUser.Role.Equals(_stringConstant.TeamLeader))
                {
                    List<User> projectUsers = await _oauthCallsRepository.GetProjectUsersByTeamLeaderIdAsync(loginUser.Id, accessToken);
                    foreach (var projectUser in projectUsers)
                    {
                        List<LeaveRequest> distinctLeaveRequests = leaveRequests.Where(x => x.EmployeeId.Contains(projectUser.Id)).GroupBy(x => x.EmployeeId).Select(x => x.FirstOrDefault()).ToList();
                        List<LeaveReport> leaveReport = await GetLeaveReportListAsync(distinctLeaveRequests, accessToken);
                        leaveReports.AddRange(leaveReport);
                    }
                    return leaveReports;
                }
            }            
            return leaveReports;
        }

        
        /// <summary>
        /// Method that returns the details of leave for an employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="accessToken"></param>
        /// <returns>Details of leave for an employee</returns>
        public async Task <IEnumerable<LeaveReportDetails>> LeaveReportDetailsAsync(string employeeId,string accessToken)
        {
            //Get user details
            User user = await GetEmployeeByIdAsync(employeeId,accessToken);
            List<LeaveReportDetails> leaveReportDetails = new List<LeaveReportDetails>();

            if (user != null)
            {
                //Get all leaves applied by the employee
                List<LeaveRequest> leaves = (await _leaveRequest.FetchAsync(x => x.EmployeeId == employeeId)).ToList();

                //Assign details of approved leaves
                foreach (var leave in leaves)
                {
                    LeaveReportDetails leaveReportDetail = new LeaveReportDetails();
                    if (leave.Status == Condition.Approved)
                    {
                        leaveReportDetail.EmployeeUserName = user.Email;
                        leaveReportDetail.EmployeeName = string.Format(_stringConstant.EmployeeFirstLastNameFormat, user.FirstName, user.LastName);
                        leaveReportDetail.LeaveFrom = leave.FromDate.ToShortDateString();
                        leaveReportDetail.StartDay = leave.FromDate.DayOfWeek.ToString();
                        leaveReportDetail.LeaveUpto = leave.EndDate.Value.ToShortDateString();
                        leaveReportDetail.EndDay = leave.EndDate.Value.DayOfWeek.ToString();
                        leaveReportDetail.Reason = leave.Reason;
                        leaveReportDetail.Type = leave.Type.ToString();
                    }
                    leaveReportDetails.Add(leaveReportDetail);
                }
                return leaveReportDetails;
            }
            return leaveReportDetails;
        }
        #endregion
    }
}
