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
        /// Test cases to check add method of bot question repository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AddQuestion()
        {
            _botQuestionRepository.AddQuestion(question);
            Assert.Equal(1, question.Id);
        }

        /// <summary>
        /// Test case to check FindById method of bot question repository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void FindById()
        {
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = _botQuestionRepository.FindById(1);
            Assert.NotEqual(1, responseQuestion.Type);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void FindByQuestionType()
        {
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = _botQuestionRepository.FindByQuestionType(2);
            Assert.Equal(DateTime.UtcNow.Date, responseQuestion.CreatedOn.Date);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void FindByTypeAndOrderNumber()
        {
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = _botQuestionRepository.FindByTypeAndOrderNumber(1,2);
            Assert.Equal(responseQuestion.QuestionStatement, question.QuestionStatement);
        }

        /// <summary>
        /// Test cases to check add method of bot question repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AddQuestionFalse()
        {
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            Assert.NotEqual(20, question.Id);
        }

        /// <summary>
        /// Test case to check FindById method of bot question repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void FindByIdFalse()
        {
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = _botQuestionRepository.FindById(3);
            Assert.NotEqual(StringConstant.TaskMailBotStatusErrorMessage, responseQuestion.QuestionStatement);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void FindByQuestionTypeFalse()
        {
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = _botQuestionRepository.FindByQuestionType(2);
            Assert.NotEqual(responseQuestion.OrderNumber, 5);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void FindByTypeAndOrderNumberFalse()
        {
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = _botQuestionRepository.FindByTypeAndOrderNumber(1, 2);
            Assert.NotEqual(responseQuestion.QuestionStatement, StringConstant.TaskMailBotStatusErrorMessage);
        }

        /// <summary>
        /// Private variable
        /// </summary>
        private Question question = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 1,
            QuestionStatement = StringConstant.FirstQuestionForTest,
            Type = 2
        };
    }
}
