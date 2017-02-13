using Promact.Erp.DomainModel.ApplicationClass;
using System;
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
    }
}
