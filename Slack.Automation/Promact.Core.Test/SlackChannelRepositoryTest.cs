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


        #region Test Cases


        /// <summary>
        /// Method to check the functionality of Slack Channel add method
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackChannelAdd()
        {
            _slackChannelRepository.AddSlackChannel(slackChannelDetails);
            Assert.Equal(slackChannelDetails.Id, StringConstant.ChannelIdForTest);
        }

        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack Channel Repository - true case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetById()
        {
            _slackChannelRepository.AddSlackChannel(slackChannelDetails);
            var slackChannel = _slackChannelRepository.GetById(StringConstant.ChannelIdForTest);
            Assert.Equal(slackChannel.Id, StringConstant.ChannelIdForTest);
        }


        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack Channel Repository - false case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetByIdFalse()
        {
            _slackChannelRepository.AddSlackChannel(slackChannelDetails);
            var slackUser = _slackChannelRepository.GetById(StringConstant.ChannelIdForTest);
            Assert.NotEqual(slackUser.Id, StringConstant.TeamLeaderIdForTest);
        }


        #endregion


    }
}
