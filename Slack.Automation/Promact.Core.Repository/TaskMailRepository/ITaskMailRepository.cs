using Promact.Erp.DomainModel.ApplicationClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.TaskMailRepository
{
    public interface ITaskMailRepository
    {
        /// <summary>
        /// Method to start task mail
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>questionText in string format containing question statement</returns>
        Task<string> StartTaskMailAsync(string userId);

        /// <summary>
        /// Method to conduct task mail after started
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="userId"></param>
        /// <returns>questionText in string format containing question statement</returns>
        Task<string> QuestionAndAnswerAsync(string answer,string userId);

        /// <summary>
        /// This method use to fetch the task mail details.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="userName"></param>
        /// <param name="loginId"></param>
        /// <returns></returns>
        Task<List<TaskMailReportAc>> TaskMailDetailsReportAsync(string userId,string role,string userName, string loginId);
        
        /// <summary>
        /// This method getting list of Employees
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<TaskMailReportAc>> GetUserInformationAsync(string userId);

        /// <summary>
        /// This method use to fetch the selected date task mail details.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        Task<List<TaskMailReportAc>> TaskMailDetailsReportSelectedDateAsync(string userId, string userName, string role, string createdOn, string loginId, string selectedDate);

        /// <summary>
        /// This method use to fetch the next and previous date task mail details.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="createdOn"></param>
        /// <param name="loginId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<TaskMailReportAc>> TaskMailDetailsReportNextPreviousDateAsync(string userId, string userName, string role, string createdOn, string loginId,string type);

    }
}
