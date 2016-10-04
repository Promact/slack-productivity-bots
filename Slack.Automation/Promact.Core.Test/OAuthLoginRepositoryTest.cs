using Autofac;
using Moq;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util;
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
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly EnvironmentVariableStore _envVariableStore;
        public OAuthLoginRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _oAuthLoginRepository = _componentContext.Resolve<IOAuthLoginRepository>();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _envVariableStore = _componentContext.Resolve<EnvironmentVariableStore>();
        }

        /// <summary>
        /// Test case to check method AddNewUserFromExternalLogin of OAuth Login Repository with true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AddNewUserFromExternalLogin()
        {
            var user = _oAuthLoginRepository.AddNewUserFromExternalLogin(StringConstant.EmailForTest, StringConstant.AccessTokenForTest, StringConstant.FirstNameForTest).Result;
            var accessToken = _attachmentRepository.AccessToken(user.UserName).Result;
            Assert.Equal(user.UserName, StringConstant.EmailForTest);
            Assert.Equal(accessToken, StringConstant.AccessTokenForTest);
        }

        /// <summary>
        /// Test case to check ExternalLoginInformation of OAuth Login Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void ExternalLoginInformation()
        {
            var oAuth = _oAuthLoginRepository.ExternalLoginInformation(StringConstant.AccessTokenForTest);
            Assert.Equal(oAuth.ReturnUrl, StringConstant.ClientReturnUrl);
        }

        ///// <summary>
        ///// Test case to check AddSlackUserInformation of OAuth Login Repository
        ///// </summary>
        [Fact, Trait("Category", "Required")]
        public void AddSlackUserInformation()
        {
            var slackOAuthResponse = Task.FromResult(StringConstant.SlackOAuthResponseText);
            var slackOAuthRequest = string.Format("?client_id={0}&client_secret={1}&code={2}&pretty=1", _envVariableStore.FetchEnvironmentVariableValues(StringConstant.SlackOAuthClientId), _envVariableStore.FetchEnvironmentVariableValues(StringConstant.SlackOAuthClientSecret), StringConstant.MessageTsForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.OAuthAcessUrl, slackOAuthRequest, null)).Returns(slackOAuthResponse);
            var userDetailsResponse = Task.FromResult(StringConstant.UserDetailsResponseText);
            var userDetailsRequest = string.Format("?token={0}&pretty=1", StringConstant.AccessTokenSlack);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.SlackUserListUrl, userDetailsRequest, null)).Returns(userDetailsResponse);
            var channelDetailsResponse = Task.FromResult(StringConstant.ChannelDetailsResponseText);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.SlackChannelListUrl, userDetailsRequest, null)).Returns(channelDetailsResponse);
            var groupDetailsResponse = Task.FromResult(StringConstant.GroupDetailsResponseText);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.SlackGroupListUrl, userDetailsRequest, null)).Returns(groupDetailsResponse);
            _oAuthLoginRepository.AddSlackUserInformation(StringConstant.MessageTsForTest);
            _mockHttpClient.Verify(x => x.GetAsync(StringConstant.OAuthAcessUrl, slackOAuthRequest, null), Times.Once);
            _mockHttpClient.Verify(x => x.GetAsync(StringConstant.SlackUserListUrl, userDetailsRequest, null), Times.Once);
            _mockHttpClient.Verify(x => x.GetAsync(StringConstant.SlackChannelListUrl, userDetailsRequest, null), Times.Once);
            _mockHttpClient.Verify(x => x.GetAsync(StringConstant.SlackGroupListUrl, userDetailsRequest, null), Times.Once);
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

        private SlackEventApiAC slackEvent = new SlackEventApiAC()
        {
            ApiAppId = StringConstant.StringIdForTest,
            Challenge = StringConstant.SlackHelpMessage,
            EventTs = StringConstant.MessageTsForTest,
            TeamId = StringConstant.ChannelIdForTest,
            Token = StringConstant.AccessTokenForTest,
            Type = StringConstant.TeamJoin,
            Event = new SlackEventDetailAC()
            {
                Type = StringConstant.TeamJoin,
                User = new SlackUserDetails()
                {
                    Deleted = false,
                    Name = StringConstant.FirstNameForTest,
                    TeamId = StringConstant.ChannelIdForTest,
                    UserId = StringConstant.StringIdForTest
                }
            }
        };
    }
}
