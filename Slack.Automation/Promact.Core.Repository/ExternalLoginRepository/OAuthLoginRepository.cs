using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ExternalLoginRepository
{
    public class OAuthLoginRepository : IOAuthLoginRepository
    {
        #region Private Variables
        private readonly ApplicationUserManager _userManager;
        private readonly IHttpClientService _httpClientService;
        private readonly IRepository<SlackUserDetails> _slackUserDetailsRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IRepository<SlackChannelDetails> _slackChannelDetailsRepository;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IRepository<IncomingWebHook> _incomingWebHookRepository;
        #endregion

        #region Constructor
        public OAuthLoginRepository(ApplicationUserManager userManager,
            IHttpClientService httpClientService, IRepository<SlackUserDetails> slackUserDetailsRepository,
            IRepository<SlackChannelDetails> slackChannelDetailsRepository, IStringConstantRepository stringConstant,
            ISlackUserRepository slackUserRepository, IEnvironmentVariableRepository envVariableRepository,
            IRepository<IncomingWebHook> incomingWebHookRepository)
        {
            _userManager = userManager;
            _httpClientService = httpClientService;
            _slackUserDetailsRepository = slackUserDetailsRepository;
            _stringConstant = stringConstant;
            _slackUserRepository = slackUserRepository;
            _slackChannelDetailsRepository = slackChannelDetailsRepository;
            _envVariableRepository = envVariableRepository;
            _incomingWebHookRepository = incomingWebHookRepository;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to add a new user in Application user table and store user's external login information in UserLogin table
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accessToken"></param>
        /// <param name="slackUserId"></param>
        /// <param name="uerId"></param>
        /// <returns>user information</returns>
        public async Task<ApplicationUser> AddNewUserFromExternalLoginAsync(string email, string accessToken, string slackUserId, string uerId)
        {
            ApplicationUser user = new ApplicationUser() { Email = email, UserName = email, SlackUserId = slackUserId, Id=uerId };
            //Creating a user with email only. Password not required
            var result = await _userManager.CreateAsync(user);
            //Adding external Oauth details
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, accessToken);
            result = await _userManager.AddLoginAsync(user.Id, info);
            return user;
        }

        /// <summary>
        /// Method to get OAuth Server's app information
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>Oauth</returns>
        public OAuthApplication ExternalLoginInformation(string refreshToken)
        {
            var clientId = _envVariableRepository.PromactOAuthClientId;
            var clientSecret = _envVariableRepository.PromactOAuthClientSecret;
            OAuthApplication oAuth = new OAuthApplication();
            oAuth.ClientId = clientId;
            oAuth.ClientSecret = clientSecret;
            oAuth.RefreshToken = refreshToken;
            oAuth.ReturnUrl = _stringConstant.ClientReturnUrl;
            return oAuth;
        }

        /// <summary>
        /// Method to add/update Slack Users,channels and groups information 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task AddSlackUserInformationAsync(string code)
        {
            var slackOAuthRequest = string.Format(_stringConstant.SlackOauthRequestUrl, _envVariableRepository.SlackOAuthClientId, _envVariableRepository.SlackOAuthClientSecret, code);
            var slackOAuthResponse = await _httpClientService.GetAsync(_stringConstant.OAuthAcessUrl, slackOAuthRequest, null);
            var slackOAuth = JsonConvert.DeserializeObject<SlackOAuthResponse>(slackOAuthResponse);
            var checkUserIncomingWebHookExist = _incomingWebHookRepository.Any(x => x.UserId == slackOAuth.UserId);
            if (!checkUserIncomingWebHookExist)
            {
                IncomingWebHook slackItem = new IncomingWebHook()
                {
                    UserId = slackOAuth.UserId,
                    IncomingWebHookUrl = slackOAuth.IncomingWebhook.Url
                };
                _incomingWebHookRepository.Insert(slackItem);
                _incomingWebHookRepository.Save();
            }
            var detailsRequest = string.Format(_stringConstant.SlackUserDetailsUrl, slackOAuth.AccessToken);
            var userDetailsResponse = await _httpClientService.GetAsync(_stringConstant.SlackUserListUrl, detailsRequest, null);
            var slackUsers = JsonConvert.DeserializeObject<SlackUserResponse>(userDetailsResponse);
            if (slackUsers.Ok)
            {
                foreach (var user in slackUsers.Members)
                {
                    var checkUserExist = _slackUserDetailsRepository.Any(x => x.UserId == user.UserId);
                    if (!user.Deleted && !checkUserExist)
                        await _slackUserRepository.AddSlackUserAsync(user);
                }
            }
            else
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + slackUsers.ErrorMessage);
            var channelDetailsResponse = await _httpClientService.GetAsync(_stringConstant.SlackChannelListUrl, detailsRequest, null);
            var channels = JsonConvert.DeserializeObject<SlackChannelResponse>(channelDetailsResponse);
            if (channels.Ok)
            {
                foreach (var channel in channels.Channels)
                {
                    SlackChannelDetails slackChannel = await _slackChannelDetailsRepository.FirstOrDefaultAsync(x => x.ChannelId == channel.ChannelId);
                    if (slackChannel == null)
                    {
                        if (!channel.Deleted)
                        {
                            channel.CreatedOn = DateTime.UtcNow;
                            _slackChannelDetailsRepository.Insert(channel);
                        }
                    }
                    else
                    {
                        slackChannel.Name = channel.Name;
                        _slackChannelDetailsRepository.Update(slackChannel);
                    }
                }
            }
            else
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + channels.ErrorMessage);

            var groupDetailsResponse = await _httpClientService.GetAsync(_stringConstant.SlackGroupListUrl, detailsRequest, null);
            var groups = JsonConvert.DeserializeObject<SlackGroupDetails>(groupDetailsResponse);
            if (groups.Ok)
            {
                foreach (var channel in groups.Groups)
                {
                    SlackChannelDetails slackChannel = await _slackChannelDetailsRepository.FirstOrDefaultAsync(x => x.ChannelId == channel.ChannelId);
                    if (slackChannel == null)
                    {
                        if (!channel.Deleted)
                        {
                            channel.CreatedOn = DateTime.UtcNow;
                            _slackChannelDetailsRepository.Insert(channel);
                        }
                    }
                    else
                    {
                        slackChannel.Name = channel.Name;
                        _slackChannelDetailsRepository.Update(slackChannel);
                    }
                }
            }
            else
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + groups.ErrorMessage);
        }

        /// <summary>
        /// Method to update slack user table when there is any changes in slack
        /// </summary>
        /// <param name="slackEvent"></param>
        public async Task SlackEventUpdateAsync(SlackEventApiAC slackEvent)
        {
            var user = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackEvent.Event.User.UserId);
            if (user == null)
                await _slackUserRepository.AddSlackUserAsync(slackEvent.Event.User);
        }


        /// <summary>
        /// Method to update slack channel table when a channel is added or updated in team.
        /// </summary>
        /// <param name="slackEvent"></param>
        public async Task SlackChannelAddAsync(SlackEventApiAC slackEvent)
        {

            var channel = await _slackChannelDetailsRepository.FirstOrDefaultAsync(x => x.ChannelId == slackEvent.Event.Channel.ChannelId);
            if (channel == null)
            {
                slackEvent.Event.Channel.CreatedOn = DateTime.UtcNow;
                _slackChannelDetailsRepository.Insert(slackEvent.Event.Channel);
            }
            else
            {
                channel.Name = slackEvent.Event.Channel.Name;
                _slackChannelDetailsRepository.Update(channel);
            }
        }
        #endregion
    }
}
