using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.BotQuestionRepository
{
    public interface IBotQuestionRepository
    {
        /// <summary>
        /// Method to add Question
        /// </summary>
        /// <param name="question"></param>
        void AddQuestion(Question question);

        /// <summary>
        /// Method to find question by it's id
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns>question</returns>
        Task<Question> FindByIdAsync(int questionId);

        /// <summary>
        /// Method to find question by it's type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>question</returns>
        Task<Question> FindByQuestionTypeAsync(int type);

        /// <summary>
        /// Method to find question by it's type and order number
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="type"></param>
        /// <returns>question</returns>
        Task<Question> FindByTypeAndOrderNumberAsync(int orderNumber, int type);
    }
}
