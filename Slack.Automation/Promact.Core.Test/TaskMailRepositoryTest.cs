using Autofac;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using Xunit;

namespace Promact.Core.Test
{
    public class TaskMailRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ITaskMailRepository _taskMailRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;

        public TaskMailRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _taskMailRepository = _componentContext.Resolve<ITaskMailRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
        }

        /// <summary>
        /// Test case for task mail start and ask first question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void StartTaskMail()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(question);
            var response = await _taskMailRepository.StartTaskMail(slackUserName);
            Assert.NotEqual(response, question.QuestionStatement);
        }

        /// <summary>
        /// Test case for conduct task mail after started
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswer()
        {
            var firstResponse = await _taskMailRepository.StartTaskMail(slackUserName);
            var secondResponse = await _taskMailRepository.QuestionAndAnswer(slackUserName, answer);
            Assert.NotEqual(secondResponse, StringConstant.RequestToStartTaskMail);
        }

        private static string slackUserName = "siddhartha";
        private static string answer = null;
        private SlackUserDetails slackUserDetails = new SlackUserDetails()
        {
            Id = "asfdhjdf",
            Name = "siddharthashaw",
            TeamId = "promact"
        };
        private Question question = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 1,
            QuestionStatement = "On which task you worked on Today?",
            Type = 2
        };
    }
}
