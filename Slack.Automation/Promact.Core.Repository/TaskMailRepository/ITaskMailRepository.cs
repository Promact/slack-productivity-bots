using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.TaskMailRepository
{
    public interface ITaskMailRepository
    {
        /// <summary>
        /// Method to start task mail
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <returns>questionText in string format containing question statement</returns>
        Task<string> StartTaskMail(string userName,string userId);

        /// <summary>
        /// Method to conduct task mail after started
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="answer"></param>
        /// <param name="userId"></param>
        /// <returns>questionText in string format containing question statement</returns>
        Task<string> QuestionAndAnswer(string userName, string answer,string userId);

        /// <summary>
        /// This method use to fetch the task mail details.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <param name="name"></param>
        /// <param name="loginId"></param>
        /// <returns></returns>
        Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string id,string role,string name,string loginId);
        /// <summary>
        /// This method getting list of Employees
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<TaskMailReportAc>> GetAllEmployeeAsync(string id);

        /// <summary>
        /// This method use to fetch the selected date task mail details.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string id, string name, string role, string createdOn, string loginId, string selectedDate);
        
        /// <summary>
        /// This method use to fetch the next and previous date task mail details.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<TaskMailReportAc>> TaskMailDetailsReportNextPreviousDateAsync(string id, string name, string role, string createdOn, string loginId,string type);

    }
}
