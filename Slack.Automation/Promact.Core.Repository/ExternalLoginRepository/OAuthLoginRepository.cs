using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Promact.Core.Repository.SlackChannelRepository;
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
using System.Collections.Generic;
using System.Threading.Tasks;



namespace Promact.Core.Repository.ExternalLoginRepository
{
    public class OAuthLoginRepository : IOAuthLoginRepository
    {
        #region Private Variables
        private readonly ApplicationUserManager _userManager;
        private readonly IHttpClientService _httpClientService;
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly IRepository<SlackChannelDetails> _slackChannelDetails;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IRepository<IncomingWebHook> _incomingWebHook;


        #endregion

        #region Constructor
        public OAuthLoginRepository(ApplicationUserManager userManager,
            IHttpClientService httpClientService, IRepository<SlackUserDetails> slackUserDetails,
            IRepository<SlackChannelDetails> slackChannelDetails, IStringConstantRepository stringConstant,
            ISlackUserRepository slackUserRepository, IEnvironmentVariableRepository envVariableRepository,
            IRepository<IncomingWebHook> incomingWebHook, ISlackChannelRepository slackChannelRepository)
        {
            _userManager = userManager;
            _httpClientService = httpClientService;
            _slackUserDetails = slackUserDetails;
            _stringConstant = stringConstant;
            _slackUserRepository = slackUserRepository;
            _slackChannelDetails = slackChannelDetails;
            _envVariableRepository = envVariableRepository;
            _incomingWebHook = incomingWebHook;
            _slackChannelRepository = slackChannelRepository;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to add a new user in Application user table and store user's external login information in UserLogin table
        /// </summary>
        /// <param name="email"></param>
        /// <param name="refreshToken"></param>
        /// <param name="slackUserId"></param>
        /// <param name="userId"></param>
        /// <returns>user information</returns>
        public async Task<ApplicationUser> AddNewUserFromExternalLoginAsync(string email, string refreshToken, string userId)
        {
            ApplicationUser userInfo = _userManager.FindById(userId);
            if (userInfo == null)// check user is already added or not
            {
                userInfo = new ApplicationUser() { Email = email, UserName = email, Id = userId };
                //Creating a user with email only. Password not required
                IdentityResult result = await _userManager.CreateAsync(userInfo);
            }
            IList<UserLoginInfo> userLoginInformation = await _userManager.GetLoginsAsync(userId);
            if (userLoginInformation.Count > 0)//check already added external oauth detials if it exists so remove it. 
                await _userManager.RemoveLoginAsync(userId, userLoginInformation[0]);

            //Adding external Oauth details
            UserLoginInfo userLoginInfo = new UserLoginInfo(_stringConstant.PromactStringName, refreshToken);
            await _userManager.AddLoginAsync(userInfo.Id, userLoginInfo);
            return userInfo;
        }

        /// <summary>
        /// Method to get OAuth Server's app information
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>Oauth</returns>
        public OAuthApplication ExternalLoginInformation(string refreshToken)
        {
            string clientId = _envVariableRepository.PromactOAuthClientId;
            string clientSecret = _envVariableRepository.PromactOAuthClientSecret;
            OAuthApplication oAuth = new OAuthApplication();
            oAuth.ClientId = clientId;
            oAuth.ClientSecret = clientSecret;
            oAuth.RefreshToken = refreshToken;
            oAuth.ReturnUrl = _stringConstant.ClientReturnUrl;
            return oAuth;
        }

        /// <summary>
        /// Method to add/update Slack User,channels and groups information 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task AddSlackUserInformationAsync(string code, string email)
        {
            string slackOAuthRequest = string.Format(_stringConstant.SlackOauthRequestUrl, _envVariableRepository.SlackOAuthClientId, _envVariableRepository.SlackOAuthClientSecret, code);
            string slackOAuthResponse = await _httpClientService.GetAsync(_stringConstant.OAuthAcessUrl, slackOAuthRequest, null);
            SlackOAuthResponse slackOAuth = JsonConvert.DeserializeObject<SlackOAuthResponse>(slackOAuthResponse);
            bool checkUserIncomingWebHookExist = _incomingWebHook.Any(x => x.UserId == slackOAuth.UserId);
            if (!checkUserIncomingWebHookExist)
            {
                IncomingWebHook slackItem = new IncomingWebHook()
                {
                    UserId = slackOAuth.UserId,
                    IncomingWebHookUrl = slackOAuth.IncomingWebhook.Url
                };
                _incomingWebHook.Insert(slackItem);
                await _incomingWebHook.SaveChangesAsync();
            }

            string detailsRequest = string.Format(_stringConstant.SlackUserDetailsUrl, slackOAuth.AccessToken);
            //get the basic user identity of the logged in slack user
            string basicUserDetailsResponse = await _httpClientService.GetAsync(_stringConstant.BasicSlackUserUrl, detailsRequest, null);
            SlackBasicUserDetailsAc slackUser = JsonConvert.DeserializeObject<SlackBasicUserDetailsAc>(basicUserDetailsResponse);
            if (slackUser.Ok)
            {
                //get all the slack users of the team
                string userDetailsResponse = await _httpClientService.GetAsync(_stringConstant.SlackUserListUrl, detailsRequest, null);
                SlackUserResponse slackUsers = JsonConvert.DeserializeObject<SlackUserResponse>(userDetailsResponse);
                if (slackUsers.Ok)
                {
                    SlackUserDetails slackUserDetails = slackUsers.Members?.Find(x => x.UserId == slackUser?.User?.UserId);
                    ApplicationUser applicationUser = await _userManager.FindByEmailAsync(email);
                    if (applicationUser != null && !string.IsNullOrEmpty(slackUserDetails?.Profile?.Email) && String.Compare(slackUserDetails.Profile.Email, applicationUser.Email, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        //if the currently logged in slack user's email and the logged in OAuth user's email match
                        applicationUser.SlackUserId = slackUserDetails.UserId;
                        await _userManager.UpdateAsync(applicationUser);
                        await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
                    }
                    else
                        throw new SlackAuthorizeException(_stringConstant.NotInSlackOrNotExpectedUser);
                }
                else
                    throw new SlackAuthorizeException(_stringConstant.SlackAuthError + slackUsers.ErrorMessage);
            }
            else
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + slackUser.ErrorMessage);

            //the public channels' details
            string channelDetailsResponse = await _httpClientService.GetAsync(_stringConstant.SlackChannelListUrl, detailsRequest, null);
            SlackChannelResponse channels = JsonConvert.DeserializeObject<SlackChannelResponse>(channelDetailsResponse);
            if (channels.Ok)
            {
                foreach (var channel in channels.Channels)
                {
                    await AddChannelGroupAsync(channel);
                }
            }
            else
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + channels.ErrorMessage);

            //the public groups' details
            string groupDetailsResponse = await _httpClientService.GetAsync(_stringConstant.SlackGroupListUrl, detailsRequest, null);
            SlackGroupDetails groups = JsonConvert.DeserializeObject<SlackGroupDetails>(groupDetailsResponse);
            if (groups.Ok)
            {
                foreach (var channel in groups.Groups)
                {
                    await AddChannelGroupAsync(channel);
                }
            }
            else
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + groups.ErrorMessage);
        }


        /// <summary>
        /// Add slack channel details to the database - JJ
        /// </summary>
        /// <param name="slackChannelDetails">Slack channel details obtained from slack</param>
        /// <returns></returns>
        private async Task AddChannelGroupAsync(SlackChannelDetails slackChannelDetails)
        {
            SlackChannelDetails slackChannel = await _slackChannelDetails.FirstOrDefaultAsync(x => x.ChannelId == slackChannelDetails.ChannelId);
            if (slackChannel == null)
            {
                if (!slackChannelDetails.Deleted)
                {
                    slackChannelDetails.CreatedOn = DateTime.UtcNow;
                    await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
                }
            }
            else
            {
                slackChannel.Name = slackChannelDetails.Name;
                await _slackChannelRepository.UpdateSlackChannelAsync(slackChannel);
            }
        }


        /// <summary>
        /// Method to update slack user table when there is any changes in slack
        /// </summary>
        /// <param name="slackEvent"></param>
        public async Task SlackEventUpdateAsync(SlackEventApiAC slackEvent)
        {
            SlackUserDetails user = await _slackUserDetails.FirstOrDefaultAsync(x => x.UserId == slackEvent.Event.User.UserId);
            if (user == null)
                await _slackUserRepository.AddSlackUserAsync(slackEvent.Event.User);
        }


        /// <summary>
        /// Method to update slack channel table when a channel is added or updated in team.
        /// </summary>
        /// <param name="slackEvent"></param>
        public async Task SlackChannelAddAsync(SlackEventApiAC slackEvent)
        {
            SlackChannelDetails channel = await _slackChannelDetails.FirstOrDefaultAsync(x => x.ChannelId == slackEvent.Event.Channel.ChannelId);
            if (channel == null)
            {
                slackEvent.Event.Channel.CreatedOn = DateTime.UtcNow;
                _slackChannelDetails.Insert(slackEvent.Event.Channel);
                await _slackChannelDetails.SaveChangesAsync();
            }
            else
            {
                channel.Name = slackEvent.Event.Channel.Name;
                _slackChannelDetails.Update(channel);
                await _slackChannelDetails.SaveChangesAsync();
            }
        }


        /// <summary>
        /// Method check user slackid is exists or ot 
        /// </summary>
        /// <param name="userId">login user id</param>
        /// <returns>boolean true or false</returns>
        public async Task<bool> CheckUserSlackInformation(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (!string.IsNullOrEmpty(user.SlackUserId))
            {
                if (_slackUserDetails.Any(x => x.UserId == user.SlackUserId))
                    return true;
            }
            return false;
        }


        #endregion
    }
}
