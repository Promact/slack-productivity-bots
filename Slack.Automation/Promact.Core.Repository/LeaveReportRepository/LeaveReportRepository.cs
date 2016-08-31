using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.LeaveReportRepository
{
    public class LeaveReportRepository : ILeaveReportRepository
    {
        private readonly IRepository<LeaveRequest> _leaveRequest;
        private readonly IProjectUserCallRepository _projectUserCall;

        public LeaveReportRepository(IRepository<LeaveRequest> leaveRequest, IProjectUserCallRepository projectUserCall)
        {
            _leaveRequest = leaveRequest;
            _projectUserCall = projectUserCall;
        }

        /// <summary>
        /// Method that returns the list of employees with their leave status
        /// </summary>
        /// <returns>List of employees with leave status</returns>       
        public async Task<IEnumerable<LeaveReport>> LeaveReport(string accessToken,string userName )
        {
            List<LeaveRequest> leaveRequests = _leaveRequest.GetAll().ToList();
            List<LeaveReport> leaveReports = new List<LeaveReport>();
            User user = new User();
            User loginUser = await _projectUserCall.GetUserByUserName(userName, accessToken);

            if (loginUser.Role == "Admin")
            {
               
                List<LeaveRequest> distinctLeaveRequests = leaveRequests.GroupBy(x => x.EmployeeId).Select(x => x.FirstOrDefault()).ToList();

                foreach (var leaveRequest in distinctLeaveRequests)
                {
                    user = await getEmployeeById(leaveRequest.EmployeeId, accessToken);
                    if (user != null)
                    {
                        LeaveReport leaveReport = new LeaveReport
                        {
                            Role = user.Role,
                            EmployeeId = user.Id,
                            EmployeeUserName = user.Email,
                            EmployeeName = (user.FirstName + " " + user.LastName),
                            TotalCasualLeave = user.NumberOfCasualLeave,
                            TotalSickLeave = user.NumberOfSickLeave,
                            UtilisedCasualLeave = getUtilisedCasualLeavesByEmployee(leaveRequest.EmployeeId),
                            BalanceCasualLeave = user.NumberOfCasualLeave - getUtilisedCasualLeavesByEmployee(leaveRequest.EmployeeId),
                            UtilisedSickLeave = null,
                            BalanceSickLeave = null
                        };
                        leaveReports.Add(leaveReport);
                    }

                }
                return leaveReports;
            }
            //else if (loginUser.Role == "Employee")
            //{
            //    var userId = loginUser.Id; 
            //    List<LeaveRequest> distinctLeaveRequests = leaveRequests.GroupBy(x => x.EmployeeId).Select(x => x.FirstOrDefault()).ToList();
            //    List<LeaveRequest> distinctLeaveRequest = leaveRequests.
            //} 
            return null;
        }


        /// <summary>
        /// Method to calculate number of casual leaves used by each employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Number of casual leaves utilised by that particular employee</returns>
        private int getUtilisedCasualLeavesByEmployee(string employeeId)
        {
            var utilisedCasualLeave = 0;
            var leaves = _leaveRequest.Fetch(x => x.EmployeeId == employeeId).ToList();
            foreach (var leave in leaves)
            { 
             if (leave.Status == Condition.Approved)
                 {
                    utilisedCasualLeave = utilisedCasualLeave + leave.EndDate.Date.Subtract(leave.FromDate).Days + 1;
                 }
            }
            return utilisedCasualLeave;
        }

        /// <summary>
        /// Method that returns the details of leave for an employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Details of leave for an employee</returns>
        public async Task <IEnumerable<LeaveReportDetails>> LeaveReportDetails(string employeeId,string accessToken)
        {
            User user = new User();
            user = await getEmployeeById(employeeId,accessToken);
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
                        leaveReportDetail.EmployeeName = (user.FirstName + " " + user.LastName);
                        leaveReportDetail.LeaveFrom = leave.FromDate.ToShortDateString();
                        leaveReportDetail.StartDay = leave.FromDate.DayOfWeek.ToString();
                        leaveReportDetail.LeaveUpto = leave.EndDate.ToShortDateString();
                        leaveReportDetail.EndDay = leave.EndDate.DayOfWeek.ToString();
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
        private async Task<User> getEmployeeById(string employeeId,string accessToken)
        {
            User user = new User();
            user = await _projectUserCall.GetUserByEmployeeId(employeeId,accessToken);
            return user;
        }
    }
}
