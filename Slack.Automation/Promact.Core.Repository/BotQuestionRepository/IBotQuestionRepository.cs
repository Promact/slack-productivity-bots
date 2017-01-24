using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;


namespace Promact.Core.Repository.BotQuestionRepository
{
    public interface IBotQuestionRepository
    {
        /// <summary>
        /// Method to add Question
        /// </summary>
        /// <param name="question">Question object</param>
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
        Task<Question> FindFirstQuestionByTypeAsync(BotQuestionType type);

        /// <summary>
        /// Method to find question by it's type and order number
        /// </summary>
        /// <param name="orderNumber">question's order number</param>
        /// <param name="type">question's type</param>
        /// <returns>question</returns>
        Task<Question> FindByTypeAndOrderNumberAsync(int orderNumber, int type);


        /// <summary>
        /// Fetches the Questions based on type of question - JJ
        /// </summary>
        /// <param name="botQuestionType">type of question asked by bot</param>
        /// <returns>list of object of Question</returns>
        Task<List<Question>> GetQuestionsByTypeAsync(BotQuestionType botQuestionType);
    }
}
