using System.Threading.Tasks;
using Xunit;
using Autofac;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System.Collections.Generic;
using Promact.Erp.Util.StringLiteral;

namespace Promact.Core.Test
{
    public class SlackChannelRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly AppStringLiteral _stringConstant;
        private SlackChannelDetails slackChannelDetails = new SlackChannelDetails();
        public SlackChannelRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackChannelRepository = _componentContext.Resolve<ISlackChannelRepository>();
            _stringConstant = _componentContext.Resolve<ISingletonStringLiteral>().StringConstant;
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
        public async Task SlackChannelAdd()
        {
            await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
            Assert.Equal(slackChannelDetails.ChannelId, _stringConstant.ChannelIdForTest);
        }

        /// <summary>
        /// Test case to check the functionality of GetbyIdAsync method of Slack Channel Repository - true case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetByIdAsync()
        {
            await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
            SlackChannelDetails slackChannel = await _slackChannelRepository.GetByIdAsync(_stringConstant.ChannelIdForTest);
            Assert.Equal(slackChannel.ChannelId, _stringConstant.ChannelIdForTest);
        }


        /// <summary>
        /// Test case to check the functionality of GetbyIdAsync method of Slack Channel Repository - false case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetByIdFalseAsync()
        {
            await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
            SlackChannelDetails slackChannel = await _slackChannelRepository.GetByIdAsync(_stringConstant.ChannelIdForTest);
            Assert.NotEqual(slackChannel.ChannelId, _stringConstant.TeamLeaderIdForTest);
        }


        /// <summary>
        /// Test case to check the functionality of UpdateSlackChannelAsync method of Slack Channel Repository - true case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateSlackChannelAsync()
        {
            await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
            slackChannelDetails.Name = _stringConstant.ChannelName;
            await _slackChannelRepository.UpdateSlackChannelAsync(slackChannelDetails);
            SlackChannelDetails slackChannel = await _slackChannelRepository.GetByIdAsync(_stringConstant.ChannelIdForTest);
            Assert.Equal(slackChannel.Name, _stringConstant.ChannelName);
        }


        /// <summary>
        /// Test case to check the functionality of DeleteChannelAsync method of Slack Channel Repository - true case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task DeleteChannelAsync()
        {
            await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
            await _slackChannelRepository.DeleteChannelAsync(slackChannelDetails.ChannelId);
            SlackChannelDetails slackChannel = await _slackChannelRepository.GetByIdAsync(slackChannelDetails.ChannelId);
            Assert.Null(slackChannel);
        }


        /// <summary>
        /// Test case to check the functionality of FetchChannelByProjectIdAsync method of Slack Channel Repository - true case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task FetchChannelByProjectIdAsync()
        {
            slackChannelDetails.ProjectId = 1;
            await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
            SlackChannelDetails slackChannel = await _slackChannelRepository.FetchChannelByProjectIdAsync((int)slackChannelDetails.ProjectId);
            Assert.Equal(slackChannel.Name, slackChannelDetails.Name);
        }


        /// <summary>
        /// Test case to check the functionality of FetchChannelAsync method of Slack Channel Repository - true case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task FetchChannelAsync()
        {
            await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
            IEnumerable<SlackChannelDetails> slackChannels = await _slackChannelRepository.FetchChannelAsync();
            Assert.NotEmpty(slackChannels);
        }


        #endregion


    }
}
