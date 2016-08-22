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

        private int totalCasualLeaves = 14;
        private int totalSickLeaves = 7;

        public LeaveReportRepository(IRepository<LeaveRequest> leaveRequest, IProjectUserCallRepository projectUserCall)
        {
            _leaveRequest = leaveRequest;
            _projectUserCall = projectUserCall;
        }

        /// <summary>
        /// Method that returns the list of employees with their leave status
        /// </summary>
        /// <returns>List of employees with leave status</returns>
        //public IEnumerable<LeaveReport> LeaveReport()
        public async Task<IEnumerable<LeaveReport>> LeaveReport()
        {
            List<LeaveRequest> leaveRequests = _leaveRequest.GetAll().ToList();
           // List<LeaveRequest> distinctLeaveRequests = leaveRequests.GroupBy(x => x.EmployeeId).Select(x => x.Fetch()).ToList();
            List<LeaveRequest> distinctLeaveRequests = leaveRequests.GroupBy(x => x.EmployeeId).Select(x => x.FirstOrDefault()).ToList();
            List<LeaveReport> leaveReports = new List<LeaveReport>();
            User user = new User();
           
                            
           foreach (var leaveRequest in distinctLeaveRequests)
            {
                user = await _projectUserCall.GetUserByEmployeeId(leaveRequest.EmployeeId);
                LeaveReport leaveReport = new LeaveReport
                    {
                     //EmployeeName = leaveRequest.EmployeeId,
                        EmployeeName = user.FirstName,
                        TotalCasualLeave = totalCasualLeaves,
                        TotalSickLeave = totalSickLeaves,
                        UtilisedCasualLeave = getUtilisedCasualLeavesByEmployee(leaveRequest.EmployeeId),
                        BalanceCasualLeave = getBalanceCasualLeave(leaveRequest.EmployeeId),
                        UtilisedSickLeave = null,
                        BalanceSickLeave = null
                    };
                    leaveReports.Add(leaveReport);              
            }
            return leaveReports;
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
                  //utilisedCasualLeave = (leaveRequest.EndDate - leaveRequest.FromDate).TotalDays;
                    utilisedCasualLeave = utilisedCasualLeave + leave.EndDate.Date.Subtract(leave.FromDate).Days + 1;
                 }
            }
            return utilisedCasualLeave;
        }

        /// <summary>
        /// Method to calculate balance casual leaves for eac employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Casual leaves left for that particular employee</returns>
        private int getBalanceCasualLeave(string employeeId)
        {
            var balanceCasualLeaves = totalCasualLeaves - getUtilisedCasualLeavesByEmployee(employeeId);
            return balanceCasualLeaves;
        }

        /// <summary>
        /// Method that returns the details of leave for an employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Details of leave for an employee</returns>
        public IEnumerable<LeaveReportDetails> LeaveReportDetails(string employeeId)
        {
           // var leaveRequest = _leaveRequest.GetById(employeeId);

            var leaves = _leaveRequest.Fetch(x => x.EmployeeId == employeeId).ToList();
            List<LeaveReportDetails> leaveReportDetails = new List<LeaveReportDetails>();
           
            foreach (var leave in leaves)
            {
                LeaveReportDetails leaveReportDetail = new LeaveReportDetails();
                if (leave.Status == Condition.Approved)
                {
                    leaveReportDetail.EmployeeName = leave.EmployeeId;
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

    }
}
