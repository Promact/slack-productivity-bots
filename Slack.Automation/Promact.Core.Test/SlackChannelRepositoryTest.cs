using Autofac;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackChannelRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ISlackChannelRepository _slackChannelRepository;
        public SlackChannelRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackChannelRepository = _componentContext.Resolve<ISlackChannelRepository>();
        }

        /// <summary>
        /// Method to check the functionality of Slack Channel add method
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachment()
        {
            SlackChannelDetails slackChannelDetails = new SlackChannelDetails();
            slackChannelDetails.Id = "g467djkjs";
            slackChannelDetails.Name = "julie";
            slackChannelDetails.Deleted = false;
            _slackChannelRepository.AddSlackChannel(slackChannelDetails);
            Assert.Equal(slackChannelDetails.Id, "g467djkjs");
        }

        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack User Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetById()
        {
            var slackUser = _slackChannelRepository.GetById("g467djkjs");
            Assert.Equal(null, slackUser);
        }
    }
}
