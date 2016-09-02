using Autofac;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util;
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

        private SlackChannelDetails slackChannelDetails = new SlackChannelDetails
        {
            Deleted = false,
            Id = StringConstant.ChannelIdForTest,
            Name = StringConstant.ChannelNameForTest
        };

        /// <summary>
        /// Method to check the functionality of Slack Channel add method
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachment()
        {
            _slackChannelRepository.AddSlackChannel(slackChannelDetails);
            Assert.Equal(slackChannelDetails.Id, StringConstant.ChannelIdForTest);
        }

        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack Channel Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetById()
        {
            _slackChannelRepository.AddSlackChannel(slackChannelDetails);
            var slackUser = _slackChannelRepository.GetById(StringConstant.ChannelIdForTest);
            Assert.Equal(slackUser.Id,StringConstant.ChannelIdForTest);
        }
    }
}
