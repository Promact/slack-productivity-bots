using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.TaskMailReportRepository
{
    public interface ITaskMailReportRepository
    {
        /// <summary>
        ///Method geting Employee or list of Employees 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of task mail report</returns>
        Task<List<TaskMailReportAc>> GetUserInformationAsync(string userId);

        /// <summary>
        /// This Method use to fetch the task mail details.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="loginId"></param>
        /// <returns>list of task mail report with task mail details</returns>
        Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string userId, string role, string userName, string loginId);

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
        Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string userId, string userName, string role, string createdOn, string loginId, DateTime selectedDate);
    }
}
