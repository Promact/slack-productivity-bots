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
        /// <returns>questionText in string format containing question statement</returns>
        Task<string> StartTaskMail(string userName);

        /// <summary>
        /// Method to conduct task mail after started
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="answer"></param>
        /// <returns>questionText in string format containing question statement</returns>
        Task<string> QuestionAndAnswer(string userName, string answer);
        Task<List<TaskMailReportAc>> TaskMailReport(string userName,int currentPage,int itemsPerPage);
        Task<List<TaskMailReportAc>> TaskMailDetailsReport(int id);
    }
}
