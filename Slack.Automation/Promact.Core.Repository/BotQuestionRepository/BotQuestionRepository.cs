using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.Models;
using Promact.Core.Repository.DataRepository;

namespace Promact.Core.Repository.BotQuestionRepository
{
    public class BotQuestionRepository : IBotQuestionRepository
    {
        private IRepository<Question> _questionRepository;
        public BotQuestionRepository(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public void AddQuestion(Question question)
        {
            _questionRepository.Insert(question);
            _questionRepository.Save();
        }

        public Question FindById(int questionId)
        {
            var question = _questionRepository.FirstOrDefault(x => x.Id == questionId);
            return question;
        }

        public Question FindByQuestionType(int type)
        {
            var question = _questionRepository.FirstOrDefault(x => x.Type == type);
            return question;
        }

        public Question FindByTypeAndOrderNumber(int orderNumber, int type)
        {
            var question = _questionRepository.FirstOrDefault(x => x.OrderNumber == (orderNumber) && x.Type == type);
            return question;
        }
    }
}
