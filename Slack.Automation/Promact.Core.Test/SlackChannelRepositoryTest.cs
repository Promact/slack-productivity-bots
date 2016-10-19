using Autofac;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util;
using Promact.Erp.Util.StringConstants;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackChannelRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly IStringConstantRepository _stringConstant;
        private SlackChannelDetails slackChannelDetails = new SlackChannelDetails();
        public SlackChannelRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackChannelRepository = _componentContext.Resolve<ISlackChannelRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            Initialize();
        }


        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialize()
        {
            slackChannelDetails.Deleted = false;
            slackChannelDetails.ChannelId = _stringConstant.ChannelIdForTest;
            slackChannelDetails.Name = _stringConstant.ChannelNameForTest;
        }

        #region Test Cases


        /// <summary>
        /// Method to check the functionality of Slack Channel add method
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackChannelAdd()
        {
            _slackChannelRepository.AddSlackChannel(slackChannelDetails);
            Assert.Equal(slackChannelDetails.ChannelId, _stringConstant.ChannelIdForTest);
        }

        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack Channel Repository - true case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetById()
        {
            _slackChannelRepository.AddSlackChannel(slackChannelDetails);
            var slackChannel = _slackChannelRepository.GetById(_stringConstant.ChannelIdForTest);
            Assert.Equal(slackChannel.ChannelId, _stringConstant.ChannelIdForTest);
        }


        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack Channel Repository - false case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetByIdFalse()
        {
            _slackChannelRepository.AddSlackChannel(slackChannelDetails);
            var slackUser = _slackChannelRepository.GetById(_stringConstant.ChannelIdForTest);
            Assert.NotEqual(slackUser.ChannelId, _stringConstant.TeamLeaderIdForTest);
        }


        #endregion


    }
}
