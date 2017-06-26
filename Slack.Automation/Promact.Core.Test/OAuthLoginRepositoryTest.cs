using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Core.Repository.ServiceRepository;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringLiteral;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class OAuthLoginRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IOAuthLoginRepository _oAuthLoginRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly Mock<IHttpClientService> _mockHttpClient;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly AppStringLiteral _stringConstant;
        private readonly Mock<IServiceRepository> _mockServiceRepository;
        private readonly ApplicationUserManager _userManager;
        private SlackEventApiAC slackEvent = new SlackEventApiAC();
        private SlackChannelDetails channel = new SlackChannelDetails();
        private SlackProfile profile = new SlackProfile();
        private ApplicationUser user = new ApplicationUser();
        #endregion

        #region Constructor
        public OAuthLoginRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _oAuthLoginRepository = _componentContext.Resolve<IOAuthLoginRepository>();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _slackChannelRepository = _componentContext.Resolve<ISlackChannelRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _envVariableRepository = _componentContext.Resolve<IEnvironmentVariableRepository>();
            _stringConstant = _componentContext.Resolve<ISingletonStringLiteral>().StringConstant;
            _mockServiceRepository = _componentContext.Resolve<Mock<IServiceRepository>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test case to check method AddNewUserFromExternalLogin of OAuth Login Repository with true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddNewUserFromExternalLoginAsync()
        {
            var user = await _oAuthLoginRepository.AddNewUserFromExternalLoginAsync(_stringConstant.EmailForTest, _stringConstant.AccessTokenForTest, _stringConstant.UserIdForTest);
            var accessTokenForTest = Task.FromResult(_stringConstant.AccessTokenForTest);
            _mockServiceRepository.Setup(x => x.GerAccessTokenByRefreshToken(_stringConstant.AccessTokenForTest, user.Id)).Returns(accessTokenForTest);
            var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
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

        /// <summary>
        /// Test case to check AddSlackUserInformation of OAuth Login Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddSlackUserInformation()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var slackOAuthResponse = Task.FromResult(_stringConstant.SlackOAuthResponseText);
            var slackOAuthRequest = string.Format(_stringConstant.SlackOauthRequestUrl, _envVariableRepository.SlackOAuthClientId, _envVariableRepository.SlackOAuthClientSecret, _stringConstant.MessageTsForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.OAuthAcessUrl, slackOAuthRequest, null, _stringConstant.Bearer)).Returns(slackOAuthResponse);
            var userDetailsResponse = Task.FromResult(_stringConstant.UserDetailsResponseText);
            var userDetailsRequest = string.Format(_stringConstant.SlackUserDetailsUrl, _stringConstant.AccessTokenSlack);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.SlackUserListUrl, userDetailsRequest, null, _stringConstant.Bearer)).Returns(userDetailsResponse);
            var channelDetailsResponse = Task.FromResult(_stringConstant.ChannelDetailsResponseText);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.SlackChannelListUrl, userDetailsRequest, null, _stringConstant.Bearer)).Returns(channelDetailsResponse);
            var groupDetailsResponse = Task.FromResult(_stringConstant.GroupDetailsResponseText);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.SlackGroupListUrl, userDetailsRequest, null, _stringConstant.Bearer)).Returns(groupDetailsResponse);

            await _oAuthLoginRepository.AddSlackUserInformationAsync(_stringConstant.MessageTsForTest);
            _mockHttpClient.Verify(x => x.GetAsync(_stringConstant.OAuthAcessUrl, slackOAuthRequest, null, _stringConstant.Bearer), Times.Once);
            _mockHttpClient.Verify(x => x.GetAsync(_stringConstant.SlackUserListUrl, userDetailsRequest, null, _stringConstant.Bearer), Times.Once);
            _mockHttpClient.Verify(x => x.GetAsync(_stringConstant.SlackChannelListUrl, userDetailsRequest, null, _stringConstant.Bearer), Times.Once);
            _mockHttpClient.Verify(x => x.GetAsync(_stringConstant.SlackGroupListUrl, userDetailsRequest, null, _stringConstant.Bearer), Times.Once);
        }


        #endregion

        #region Initialisation
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

            user.Email = _stringConstant.EmailForTest;
            user.UserName = _stringConstant.EmailForTest;
            user.SlackUserId = _stringConstant.StringIdForTest;

            channel.ChannelId = _stringConstant.ChannelIdForTest;
            channel.CreatedOn = DateTime.UtcNow;
            channel.Deleted = false;
            channel.Name = _stringConstant.Employee;

        }
        #endregion
    }
}
