using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Erp.Util.StringConstants;
using System;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ExternalLoginRepository
{
    public class OAuthLoginRepository : IOAuthLoginRepository
    {      
        private readonly ApplicationUserManager _userManager;
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IRepository<SlackChannelDetails> _slackChannelDetails;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        public OAuthLoginRepository(ApplicationUserManager userManager,
            IHttpClientRepository httpClientRepository, IRepository<SlackUserDetails> slackUserDetails,
            IRepository<SlackChannelDetails> slackChannelDetails, IStringConstantRepository stringConstant,
            ISlackUserRepository slackUserRepository, IEnvironmentVariableRepository envVariableRepository)
        {
            _userManager = userManager;
            _httpClientRepository = httpClientRepository;
            _slackUserDetails = slackUserDetails;
            _stringConstant = stringConstant;
            _slackUserRepository = slackUserRepository;
            _slackChannelDetails = slackChannelDetails;
            _envVariableRepository = envVariableRepository;
        }

        /// <summary>
        /// Method to add a new user in Application user table and store user's external login information in UserLogin table
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accessToken"></param>
        /// <param name="slackUserName"></param>
        /// <returns>user information</returns>
        public async Task<ApplicationUser> AddNewUserFromExternalLogin(string email, string accessToken, string slackUserName)
        {
            ApplicationUser user = new ApplicationUser() { Email = email, UserName = email, SlackUserName = slackUserName };
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
        /// Method to add Slack Users,channels and groups information 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task AddSlackUserInformation(string code)
        {
            var slackOAuthRequest = string.Format("?client_id={0}&client_secret={1}&code={2}&pretty=1", _envVariableRepository.SlackOAuthClientId, _envVariableRepository.SlackOAuthClientSecret, code);
            var slackOAuthResponse = await _httpClientRepository.GetAsync(_stringConstant.OAuthAcessUrl, slackOAuthRequest, null);
            var slackOAuth = JsonConvert.DeserializeObject<SlackOAuthResponse>(slackOAuthResponse);
            var detailsRequest = string.Format("?token={0}&pretty=1", slackOAuth.AccessToken);
            var userDetailsResponse = await _httpClientRepository.GetAsync(_stringConstant.SlackUserListUrl, detailsRequest, null);
            var slackUsers = JsonConvert.DeserializeObject<SlackUserResponse>(userDetailsResponse);
            if (slackUsers.Ok)
            {
                foreach (var user in slackUsers.Members)
                {
                    if (!user.Deleted && !user.IsBot && user.Name != _stringConstant.SlackBotStringName)
                        _slackUserRepository.AddSlackUser(user);
                }
            }
            else
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + slackUsers.ErrorMessage);
            var channelDetailsResponse = await _httpClientRepository.GetAsync(_stringConstant.SlackChannelListUrl, detailsRequest, null);
            var channels = JsonConvert.DeserializeObject<SlackChannelResponse>(channelDetailsResponse);
            if (channels.Ok)
            {
                foreach (var channel in channels.Channels)
                {
                    if (!channel.Deleted)
                    {
                        channel.CreatedOn = DateTime.UtcNow;
                        _slackChannelDetails.Insert(channel);
                    }
                }
            }
            else
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + channels.ErrorMessage);

            var groupDetailsResponse = await _httpClientRepository.GetAsync(_stringConstant.SlackGroupListUrl, detailsRequest, null);
            var groups = JsonConvert.DeserializeObject<SlackGroupDetails>(groupDetailsResponse);
            if (groups.Ok)
            {
                foreach (var channel in groups.Groups)
                {
                    if (!channel.Deleted)
                    {
                        channel.CreatedOn = DateTime.UtcNow;
                        _slackChannelDetails.Insert(channel);
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
        public void SlackEventUpdate(SlackEventApiAC slackEvent)
        {
            var user = _slackUserDetails.FirstOrDefault(x => x.UserId == slackEvent.Event.User.UserId);
            if (user == null)
                _slackUserRepository.AddSlackUser(slackEvent.Event.User);
        }


        /// <summary>
        /// Method to update slack channel table when a channel is added or updated in team.
        /// </summary>
        /// <param name="slackEvent"></param>
        public void SlackChannelAdd(SlackEventApiAC slackEvent)
        {

            var channel = _slackChannelDetails.FirstOrDefault(x => x.ChannelId == slackEvent.Event.Channel.ChannelId);
            if (channel == null)
            {
                slackEvent.Event.Channel.CreatedOn = DateTime.UtcNow;
                _slackChannelDetails.Insert(slackEvent.Event.Channel);
            }
            else
            {
                channel.Name = slackEvent.Event.Channel.Name;
                _slackChannelDetails.Update(channel);
            }
        }
    }
}
