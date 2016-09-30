using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ExternalLoginRepository
{
    public class OAuthLoginRepository : IOAuthLoginRepository
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        private readonly IRepository<SlackChannelDetails> _slackChannelDetails;
        public OAuthLoginRepository(ApplicationUserManager userManager, IHttpClientRepository httpClientRepository, IRepository<SlackUserDetails> slackUserDetails, IRepository<SlackChannelDetails> slackChannelDetails)
        {
            _userManager = userManager;
            _httpClientRepository = httpClientRepository;
            _slackUserDetails = slackUserDetails;
            _slackChannelDetails = slackChannelDetails;
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
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, accessToken);
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
            var clientId =GlobalClass.PromactOAuthClientId;
            var clientSecret = GlobalClass.PromactOAuthClientSecret;
            OAuthApplication oAuth = new OAuthApplication();
            oAuth.ClientId = clientId;
            oAuth.ClientSecret = clientSecret;
            oAuth.RefreshToken = refreshToken;
            oAuth.ReturnUrl = StringConstant.ClientReturnUrl;
            return oAuth;
        }

        /// <summary>
        /// Method to add Slack Users,channels and bots information 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task AddSlackUserInformation(string code)
        {           
            //var slackOAuthRequest = string.Format("?client_id={0}&client_secret={1}&code={2}&pretty=1", Environment.GetEnvironmentVariable(StringConstant.SlackOAuthClientId, EnvironmentVariableTarget.Process), Environment.GetEnvironmentVariable(StringConstant.SlackOAuthClientSecret, EnvironmentVariableTarget.Process), code);
            var slackOAuthRequest = string.Format("?client_id={0}&client_secret={1}&code={2}&pretty=1", GlobalClass.SlackOAuthClientId,GlobalClass.SlackOAuthClientSecret, code);
            var slackOAuthResponse = await _httpClientRepository.GetAsync(StringConstant.OAuthAcessUrl, slackOAuthRequest, null);
            var slackOAuth = JsonConvert.DeserializeObject<SlackOAuthResponse>(slackOAuthResponse);
            var userDetailsRequest = string.Format("?token={0}&pretty=1", slackOAuth.AccessToken);
            var userDetailsResponse = await _httpClientRepository.GetAsync(StringConstant.SlackUserListUrl, userDetailsRequest, null);
            var slackUsers = JsonConvert.DeserializeObject<SlackUserResponse>(userDetailsResponse);
            foreach (var user in slackUsers.Members)
            {
                if (user.Name != StringConstant.SlackBotStringName)
                {
                    user.CreatedOn = DateTime.UtcNow;
                    _slackUserDetails.Insert(user);
                    _slackUserDetails.Save();
                }
            }
            var channelDetailsResponse = await _httpClientRepository.GetAsync(StringConstant.SlackChannelListUrl, userDetailsRequest, null);
            var channels = JsonConvert.DeserializeObject<SlackChannelResponse>(channelDetailsResponse);
            foreach (var channel in channels.Channels)
            {
                channel.CreatedOn = DateTime.UtcNow;
                _slackChannelDetails.Insert(channel);
            }

            var groupDetailsResponse = await _httpClientRepository.GetAsync(StringConstant.SlackGroupListUrl, userDetailsRequest, null);
            var groups = JsonConvert.DeserializeObject<SlackGroupDetails>(groupDetailsResponse);
            foreach (var channel in groups.Groups)
            {
                channel.CreatedOn = DateTime.UtcNow;
                _slackChannelDetails.Insert(channel);
            }
        }

        /// <summary>
        /// Method to update slack user table when there is any changes in slack
        /// </summary>
        /// <param name="slackEvent"></param>
        public void SlackEventUpdate(SlackEventApiAC slackEvent)
        {
            slackEvent.Event.User.TeamId = slackEvent.TeamId;
            _slackUserDetails.Insert(slackEvent.Event.User);
            _slackUserDetails.Save();
        }
    }
}
