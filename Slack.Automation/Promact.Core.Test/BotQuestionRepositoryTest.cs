using Autofac;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class BotQuestionRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IBotQuestionRepository _botQuestionRepository;
        public BotQuestionRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
        }

        /// <summary>
        /// Test cases to check add method of bot question repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AddQuestion()
        {
            _botQuestionRepository.AddQuestion(question);
            Assert.Equal(1, question.Id);
        }

        /// <summary>
        /// Test case to check FindById method of bot question repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void FindById()
        {
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = _botQuestionRepository.FindById(1);
            Assert.NotEqual(1, responseQuestion.Type);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void FindByQuestionType()
        {
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = _botQuestionRepository.FindByQuestionType(2);
            Assert.Equal(DateTime.UtcNow.Date, responseQuestion.CreatedOn.Date);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void FindByTypeAndOrderNumber()
        {
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = _botQuestionRepository.FindByTypeAndOrderNumber(1,2);
            Assert.Equal(responseQuestion.QuestionStatement, question.QuestionStatement);
        }

        /// <summary>
        /// Private variable
        /// </summary>
        private Question question = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 1,
            QuestionStatement = StringConstant.QuestionForTest,
            Type = 2
        };
    }
}
