using Promact.Erp.DomainModel.Models;
using System.Threading.Tasks;

namespace Promact.Core.Repository.BotQuestionRepository
{
    public interface IBotQuestionRepository
    {
        /// <summary>
        /// Method to add Question
        /// </summary>
        /// <param name="question"></param>
        Task AddQuestionAsync(Question question);

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
        Task<Question> FindByQuestionTypeAsync(int type);

        /// <summary>
        /// Method to find question by it's type and order number
        /// </summary>
        /// <param name="orderNumber">question's order number</param>
        /// <param name="type">question's type</param>
        /// <returns>question</returns>
        Task<Question> FindByTypeAndOrderNumberAsync(int orderNumber, int type);
    }
}
