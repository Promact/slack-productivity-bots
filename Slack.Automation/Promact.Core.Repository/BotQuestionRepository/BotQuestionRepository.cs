using Promact.Erp.DomainModel.Models;
using Promact.Erp.DomainModel.DataRepository;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass;
using System.Collections.Generic;
using System.Linq;

namespace Promact.Core.Repository.BotQuestionRepository
{
    public class BotQuestionRepository : IBotQuestionRepository
    {

        #region Private Variable

        private readonly IRepository<Question> _questionRepository;

        #endregion


        #region Constructor

        public BotQuestionRepository(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        #endregion


        #region Public Methods


        /// <summary>
        /// Method to add Question
        /// </summary>
        /// <param name="question">Question object</param>
        public async Task AddQuestionAsync(Question question)
        {
            _questionRepository.Insert(question);
            await _questionRepository.SaveChangesAsync();
        }


        /// <summary>
        /// Method to find question by it's id
        /// </summary>
        /// <param name="questionId">question Id</param>
        /// <returns>question</returns>
        public async Task<Question> FindByIdAsync(int questionId)
        {
            Question question = await _questionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
            return question;
        }


        /// <summary>
        /// Method to find question by it's type
        /// </summary>
        /// <param name="type">question's type</param>
        /// <returns>question</returns>
        public async Task<Question> FindFirstQuestionByTypeAsync(BotQuestionType type)
        {
            Question question = await _questionRepository.FirstOrDefaultAsync(x => x.Type == type);
            return question;
        }


        /// <summary>
        /// Method to find question by it's type and order number
        /// </summary>
        /// <param name="orderNumber">question's order number</param>
        /// <param name="type">question's type</param>
        /// <returns>question</returns>
        public async Task<Question> FindByTypeAndOrderNumberAsync(int orderNumber, int type)
        {
            BotQuestionType typeValue = (BotQuestionType)type;
            QuestionOrder orderNumberValue = (QuestionOrder)orderNumber;
            Question question = await _questionRepository.FirstOrDefaultAsync(x => x.OrderNumber == orderNumberValue &&
            x.Type == typeValue);
            return question;
        }


        /// <summary>
        /// Fetches the Questions based on type of question - JJ
        /// </summary>
        /// <param name="botQuestionType">type of question asked by bot</param>
        /// <returns>list of object of Question</returns>
        public async Task<List<Question>> GetQuestionsByTypeAsync(BotQuestionType botQuestionType)
        {
            IEnumerable<Question> questionList = await _questionRepository.FetchAsync(x => x.Type == botQuestionType);
            return questionList.OrderBy(x => x.OrderNumber).ToList();
        }


        #endregion
    }
}
