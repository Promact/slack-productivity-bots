using Autofac;
using Moq;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.StringConstants;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class OAuthLoginRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IOAuthLoginRepository _oAuthLoginRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IStringConstantRepository _stringConstant;
        private SlackEventApiAC slackEvent = new SlackEventApiAC();
        private SlackChannelDetails channel = new SlackChannelDetails();
        private SlackProfile profile = new SlackProfile();
        public OAuthLoginRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _oAuthLoginRepository = _componentContext.Resolve<IOAuthLoginRepository>();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _slackChannelRepository = _componentContext.Resolve<ISlackChannelRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _envVariableRepository = _componentContext.Resolve<IEnvironmentVariableRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            Initialize();
        }

        /// <summary>
        /// Test case to check method AddNewUserFromExternalLogin of OAuth Login Repository with true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AddNewUserFromExternalLogin()
        {
            var user = _oAuthLoginRepository.AddNewUserFromExternalLogin(_stringConstant.EmailForTest, _stringConstant.AccessTokenForTest, _stringConstant.FirstNameForTest,_stringConstant.UserIdForTest).Result;
            var accessToken = _attachmentRepository.AccessToken(user.UserName).Result;
            Assert.Equal(user.UserName, _stringConstant.EmailForTest);
            Assert.Equal(accessToken, _stringConstant.AccessTokenForTest);
        }

        /// <summary>
        /// Test case to check ExternalLoginInformation of OAuth Login Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void ExternalLoginInformation()
        {
            var oAuth = _oAuthLoginRepository.ExternalLoginInformation(_stringConstant.AccessTokenForTest);
            Assert.Equal(oAuth.ReturnUrl, _stringConstant.ClientReturnUrl);
        }

        ///// <summary>
        ///// Test case to check AddSlackUserInformation of OAuth Login Repository
        ///// </summary>
        [Fact, Trait("Category", "Required")]
        public void AddSlackUserInformation()
        {
            var slackOAuthResponse = Task.FromResult(_stringConstant.SlackOAuthResponseText);
            var slackOAuthRequest = string.Format("?client_id={0}&client_secret={1}&code={2}&pretty=1", _envVariableRepository.SlackOAuthClientId, _envVariableRepository.SlackOAuthClientSecret, _stringConstant.MessageTsForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.OAuthAcessUrl, slackOAuthRequest, null)).Returns(slackOAuthResponse);
            var userDetailsResponse = Task.FromResult(_stringConstant.UserDetailsResponseText);
            var userDetailsRequest = string.Format("?token={0}&pretty=1", _stringConstant.AccessTokenSlack);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.SlackUserListUrl, userDetailsRequest, null)).Returns(userDetailsResponse);
            var channelDetailsResponse = Task.FromResult(_stringConstant.ChannelDetailsResponseText);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.SlackChannelListUrl, userDetailsRequest, null)).Returns(channelDetailsResponse);
            var groupDetailsResponse = Task.FromResult(_stringConstant.GroupDetailsResponseText);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.SlackGroupListUrl, userDetailsRequest, null)).Returns(groupDetailsResponse);
            _oAuthLoginRepository.AddSlackUserInformation(_stringConstant.MessageTsForTest);
            _mockHttpClient.Verify(x => x.GetAsync(_stringConstant.OAuthAcessUrl, slackOAuthRequest, null), Times.Once);
            _mockHttpClient.Verify(x => x.GetAsync(_stringConstant.SlackUserListUrl, userDetailsRequest, null), Times.Once);
            _mockHttpClient.Verify(x => x.GetAsync(_stringConstant.SlackChannelListUrl, userDetailsRequest, null), Times.Once);
            _mockHttpClient.Verify(x => x.GetAsync(_stringConstant.SlackGroupListUrl, userDetailsRequest, null), Times.Once);
        }

        /// <summary>
        /// Test case to check SlackEventUpdate of OAuth Login Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackEventUpdate()
        {
            _oAuthLoginRepository.SlackEventUpdate(slackEvent);
            var user = _slackUserRepository.GetById(slackEvent.Event.User.UserId);
            Assert.Equal(user.Name, slackEvent.Event.User.Name);
        }

        /// <summary>
        /// Test case to check SlackEventUpdate of OAuth Login Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackAddChannel()
        {
            slackEvent.Event.Channel = channel;
            _oAuthLoginRepository.SlackChannelAdd(slackEvent);
            var channelAdded = _slackChannelRepository.GetById(slackEvent.Event.Channel.ChannelId);
            Assert.Equal(channelAdded.Name, slackEvent.Event.Channel.Name);
        }

        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialize()
        {
            profile.Skype = _stringConstant.TestUserId;
            profile.Email = _stringConstant.EmailForTest;
            profile.FirstName = _stringConstant.UserNameForTest;
            profile.LastName = _stringConstant.TestUser;
            profile.Phone = _stringConstant.PhoneForTest;
            profile.Title = _stringConstant.UserNameForTest;

            slackEvent.ApiAppId = _stringConstant.StringIdForTest;
            slackEvent.Challenge = _stringConstant.SlackHelpMessage;
            slackEvent.EventTs = _stringConstant.MessageTsForTest;
            slackEvent.TeamId = _stringConstant.ChannelIdForTest;
            slackEvent.Token = _stringConstant.AccessTokenForTest;
            slackEvent.Type = _stringConstant.TeamJoin;
            slackEvent.Event = new SlackEventDetailAC()
            {
                Type = _stringConstant.TeamJoin,
                User = new SlackUserDetails()
                {
                    Deleted = false,
                    Name = _stringConstant.FirstNameForTest,
                    TeamId = _stringConstant.ChannelIdForTest,
                    UserId = _stringConstant.StringIdForTest,
                    Profile = profile
                }
            };

            channel.ChannelId = "ChannelIdForTest";
            channel.CreatedOn = DateTime.UtcNow;
            channel.Deleted = false;
            channel.Name = _stringConstant.Employee;
                        
        }

    }
}
