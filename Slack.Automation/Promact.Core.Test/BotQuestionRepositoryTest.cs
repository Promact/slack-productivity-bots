using Autofac;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.StringConstants;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class BotQuestionRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly IStringConstantRepository _stringConstant;
        private Question question = new Question();
        #endregion

        #region Constructor
        public BotQuestionRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test cases to check add method of bot question repository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddQuestion()
        {
            await _botQuestionRepository.AddQuestionAsync(question);
            Assert.Equal(1, question.Id);
        }

        /// <summary>
        /// Test case to check FindById method of bot question repository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task FindByIdAsync()
        {
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = await _botQuestionRepository.FindByIdAsync(1);
            Assert.NotEqual(BotQuestionType.Scrum, responseQuestion.Type);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task FindByQuestionTypeAsync()
        {
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = await _botQuestionRepository.FindFirstQuestionByTypeAsync(BotQuestionType.TaskMail);
            Assert.Equal(DateTime.UtcNow.Date, responseQuestion.CreatedOn.Date);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task FindByTypeAndOrderNumberAsync()
        {
            await _botQuestionRepository.AddQuestionAsync(question);
            var responseQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(1, 2);
            Assert.Equal(responseQuestion.QuestionStatement, question.QuestionStatement);
        }

        /// <summary>
        /// Test cases to check add method of bot question repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddQuestionFalse()
        {
            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question);
            Assert.NotEqual(20, question.Id);
        }

        /// <summary>
        /// Test case to check FindById method of bot question repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task FindByIdFalseAsync()
        {
            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question);
            var responseQuestion = await _botQuestionRepository.FindByIdAsync(3);
            Assert.NotEqual(_stringConstant.TaskMailBotStatusErrorMessage, responseQuestion.QuestionStatement);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task FindByQuestionTypeFalseAsync()
        {
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            var responseQuestion = await _botQuestionRepository.FindFirstQuestionByTypeAsync(BotQuestionType.TaskMail);
            Assert.NotEqual(responseQuestion.OrderNumber,QuestionOrder.Comment);
        }

        /// <summary>
        /// Test case to check FindByQuestionType method of bot question repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task FindByTypeAndOrderNumberFalseAsync()
        {
            await _botQuestionRepository.AddQuestionAsync(question);
            var responseQuestion = await _botQuestionRepository.FindByTypeAndOrderNumberAsync(1, 2);
            Assert.NotEqual(responseQuestion.QuestionStatement, _stringConstant.TaskMailBotStatusErrorMessage);
        }

        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialize()
        {
            question.CreatedOn = DateTime.UtcNow;
            question.OrderNumber = QuestionOrder.YourTask;
            question.QuestionStatement = _stringConstant.FirstQuestionForTest;
            question.Type = BotQuestionType.TaskMail;
        }
        #endregion
    }
}
