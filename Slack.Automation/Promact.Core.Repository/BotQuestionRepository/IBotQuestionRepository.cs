using Promact.Erp.DomainModel.ApplicationClass;
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
        /// <param name="question">Question object</param>
        void AddQuestion(Question question);

        /// <summary>
        /// Method to find question by it's id
        /// </summary>
        /// <param name="questionId">question Id</param>
        /// <returns>question</returns>
        Task<Question> FindByIdAsync(int questionId);

        /// <summary>
        /// Method to find question by it's type
        /// </summary>
        /// <param name="type">question's type</param>
        /// <returns>question</returns>
        Task<Question> FindFirstQuestionByTypeAsync(BotQuestionType type);

        /// <summary>
        /// Method to find question by it's type and order number
        /// </summary>
        /// <param name="orderNumber">question's order number</param>
        /// <param name="type">question's type</param>
        /// <returns>question</returns>
        Task<Question> FindByTypeAndOrderNumberAsync(int orderNumber, int type);
    }
}
