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
        /// <param name="questionId"></param>
        /// <returns>question</returns>
        Question FindById(int questionId);

        /// <summary>
        /// Method to find question by it's type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>question</returns>
        Question FindByQuestionType(int type);

        /// <summary>
        /// Method to find question by it's type and order number
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="type"></param>
        /// <returns>question</returns>
        Question FindByTypeAndOrderNumber(int orderNumber, int type);
    }
}
