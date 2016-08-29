using Autofac;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.DomainModel.Models;
using Xunit;

namespace Promact.Core.Test
{
    public class TaskMailRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ITaskMailRepository _taskMailRepository;
        public TaskMailRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _taskMailRepository = _componentContext.Resolve<ITaskMailRepository>();
        }

        /// <summary>
        /// Test case for task mail start and ask first question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void StartTaskMail()
        {
            var response = await _taskMailRepository.StartTaskMail(slackUserName);
            Assert.NotEqual(response, null);
        }

        /// <summary>
        /// Test case for conduct task mail after started
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswer()
        {
            var firstResponse = await _taskMailRepository.StartTaskMail(slackUserName);
            var secongResponse = await _taskMailRepository.QuestionAndAnswer(slackUserName,answer);
            Assert.NotEqual(secongResponse, null);
        }

        private static string slackUserName = "siddhartha";
        private static string answer = "task mail";
    }
}
