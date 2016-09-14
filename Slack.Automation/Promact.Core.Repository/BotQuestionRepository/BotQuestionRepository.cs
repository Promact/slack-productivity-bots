using Promact.Erp.DomainModel.Models;
using Promact.Erp.DomainModel.DataRepository;

namespace Promact.Core.Repository.BotQuestionRepository
{
    public class BotQuestionRepository : IBotQuestionRepository
    {
        private IRepository<Question> _questionRepository;
        public BotQuestionRepository(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        /// <summary>
        /// Method to add Question
        /// </summary>
        /// <param name="question"></param>
        public void AddQuestion(Question question)
        {
            _questionRepository.Insert(question);
            _questionRepository.Save();
        }

        /// <summary>
        /// Method to find question by it's id
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns>question</returns>
        public Question FindById(int questionId)
        {
            var question = _questionRepository.FirstOrDefault(x => x.Id == questionId);
            return question;
        }

        /// <summary>
        /// Method to find question by it's type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>question</returns>
        public Question FindByQuestionType(int type)
        {
            var question = _questionRepository.FirstOrDefault(x => x.Type == type);
            return question;
        }

        /// <summary>
        /// Method to find question by it's type and order number
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="type"></param>
        /// <returns>question</returns>
        public Question FindByTypeAndOrderNumber(int orderNumber, int type)
        {
            var question = _questionRepository.FirstOrDefault(x => x.OrderNumber == (orderNumber) && x.Type == type);
            return question;
        }
    }
}
