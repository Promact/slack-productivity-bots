using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass;

namespace Promact.Core.Repository.LeaveReportRepository
{
    public interface ILeaveReportRepository
    {
        /// <summary>
        /// Method that returns the list of employees with their leave status based on their roles
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="userId"></param>
        /// <returns>List of employees with leave status based on roles</returns> 
        Task<IEnumerable<LeaveReport>> LeaveReportAsync(string accessToken,string userId);

        /// <summary>
        /// Method that returns the details of leave for an employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="accessToken"></param>
        /// <returns>Details of leave for an employee</returns>
        Task<IEnumerable<LeaveReportDetails>> LeaveReportDetailsAsync(string employeeId, string accessToken);
        
    }
}
