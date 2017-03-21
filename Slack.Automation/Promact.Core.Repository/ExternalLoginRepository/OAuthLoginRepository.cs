using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NLog;
using Promact.Core.Repository.AppCredentialRepository;
using Promact.Core.Repository.BotRepository;
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
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly IRepository<SlackChannelDetails> _slackChannelDetails;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IEnvironmentVariableRepository _envVariableRepository;
        private readonly IRepository<IncomingWebHook> _incomingWebHookRepository;
        private readonly IAppCredentialRepository _appCredentialRepository;
        private readonly ILogger _logger;

        #endregion

        #region Constructor
        public OAuthLoginRepository(ApplicationUserManager userManager,
            IHttpClientService httpClientService, IRepository<SlackUserDetails> slackUserDetailsRepository,
            IRepository<SlackChannelDetails> slackChannelDetailsRepository, IStringConstantRepository stringConstant,
            ISlackUserRepository slackUserRepository, IEnvironmentVariableRepository envVariableRepository,
            IRepository<IncomingWebHook> incomingWebHook, ISlackChannelRepository slackChannelRepository,
             IAppCredentialRepository appCredentialRepository)
        {
            _userManager = userManager;
            _httpClientService = httpClientService;
            _slackUserDetailsRepository = slackUserDetailsRepository;
            _stringConstant = stringConstant;
            _slackUserRepository = slackUserRepository;
            _slackChannelDetails = slackChannelDetailsRepository;
            _envVariableRepository = envVariableRepository;
            _incomingWebHookRepository = incomingWebHook;
            _slackChannelRepository = slackChannelRepository;
            _appCredentialRepository = appCredentialRepository;
            _logger = LogManager.GetLogger("AuthenticationModule");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to add a new user in Application user table and store user's external login information in UserLogin table
        /// </summary>
        /// <param name="email"></param>
        /// <param name="refreshToken"></param>
        /// <param name="userId"></param>
        /// <returns>user information</returns>
        public async Task<ApplicationUser> AddNewUserFromExternalLoginAsync(string email, string refreshToken, string userId)
        {
            _logger.Debug("Start AddNewUserFromExternalLoginAsync:" + email + "RefreshToken: " + refreshToken + " UserID: " + userId);
            ApplicationUser userInfo = _userManager.FindById(userId);
            if (userInfo == null)// check user is already added or not
            {
                userInfo = new ApplicationUser() { Email = email, UserName = email, Id = userId };
                //Creating a user with email only. Password not required
                IdentityResult result = await _userManager.CreateAsync(userInfo);

                //Adding external Oauth details
                UserLoginInfo userLoginInfo = new UserLoginInfo(_stringConstant.PromactStringName, refreshToken);
                var success = await _userManager.AddLoginAsync(userInfo.Id, userLoginInfo);
            }
            await UpdateApplicationUserAsync(email);
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
        /// <returns></returns>
        public async Task AddSlackUserInformationAsync(string code)
        {
            string slackOAuthRequest = string.Format(_stringConstant.SlackOauthRequestUrl, _envVariableRepository.SlackOAuthClientId, _envVariableRepository.SlackOAuthClientSecret, code);
            string slackOAuthResponse = await _httpClientService.GetAsync(_stringConstant.OAuthAcessUrl, slackOAuthRequest, null, _stringConstant.Bearer);
            SlackOAuthResponse slackOAuth = JsonConvert.DeserializeObject<SlackOAuthResponse>(slackOAuthResponse);
            _logger.Info("slackOAuth UserID" + slackOAuth.UserId);
            bool checkUserIncomingWebHookExist = _incomingWebHookRepository.Any(x => x.UserId == slackOAuth.UserId);
            if (!checkUserIncomingWebHookExist)
            {
                IncomingWebHook slackItem = new IncomingWebHook()
                {
                    UserId = slackOAuth.UserId,
                    IncomingWebHookUrl = slackOAuth.IncomingWebhook.Url
                };
                _incomingWebHookRepository.Insert(slackItem);
                await _incomingWebHookRepository.SaveChangesAsync();
            }

            string detailsRequest = string.Format(_stringConstant.SlackUserDetailsUrl, slackOAuth.AccessToken);
            //get all the slack users of the team
            string userDetailsResponse = await _httpClientService.GetAsync(_stringConstant.SlackUserListUrl, detailsRequest, null, _stringConstant.Bearer);
            SlackUserResponse slackUsers = JsonConvert.DeserializeObject<SlackUserResponse>(userDetailsResponse);
            if (slackUsers.Ok)
            {
                SlackUserDetails slackUserDetails = slackUsers.Members?.Find(x => x.UserId == slackOAuth.UserId);
                _logger.Debug("slackUserDetails" + slackUserDetails.UserId);
                if (!string.IsNullOrEmpty(slackUserDetails?.Profile?.Email))
                {
                    //fetch the details of the user who is authenticated with Promact OAuth server with the given slack user's email
                    _logger.Debug("Profile Email" + slackUserDetails.Profile.Email);
                    ApplicationUser applicationUser = await _userManager.FindByEmailAsync(slackUserDetails.Profile.Email);
                    if (applicationUser != null)
                    {
                        _logger.Debug("slackUserDetails UserID" + slackUserDetails.UserId);
                        applicationUser.SlackUserId = slackUserDetails.UserId;
                        _logger.Debug("applicationUser SlackUserId" + applicationUser.SlackUserId);
                        var succeeded = await _userManager.UpdateAsync(applicationUser);
                        _logger.Debug("applicationUser Object:" + JsonConvert.SerializeObject(applicationUser));
                        _logger.Debug("Update Application User succeeded" + succeeded.Succeeded);
                        _logger.Debug("Update Application User Errors" + succeeded.Errors);
                        ApplicationUser testApllicationUser = await _userManager.FindByEmailAsync(applicationUser.Email);
                        _logger.Debug("Test Application Slcak UserId" + testApllicationUser.SlackUserId);
                        _logger.Debug("Test ApplicationUser Object:" + JsonConvert.SerializeObject(testApllicationUser));
                        await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
                        _logger.Debug("Add Slack User Id Done");
                        //the public channels' details
                        string channelDetailsResponse = await _httpClientService.GetAsync(_stringConstant.SlackChannelListUrl, detailsRequest, null, _stringConstant.Bearer);
                        SlackChannelResponse channels = JsonConvert.DeserializeObject<SlackChannelResponse>(channelDetailsResponse);
                        if (channels.Ok)
                        {
                            _logger.Info("Channels error:" + channels.ErrorMessage);
                            foreach (var channel in channels.Channels)
                            {
                                _logger.Info("Channel:" + channel);
                                await AddChannelGroupAsync(channel);
                            }
                        }
                        else
                            throw new SlackAuthorizeException(_stringConstant.SlackAuthError + channels.ErrorMessage);
                        _logger.Info("Slack User Id  : " + (await _userManager.FindByEmailAsync(applicationUser.Email)).SlackUserId);
                        //the public groups' details
                        string groupDetailsResponse = await _httpClientService.GetAsync(_stringConstant.SlackGroupListUrl, detailsRequest, null, _stringConstant.Bearer);
                        SlackGroupDetails groups = JsonConvert.DeserializeObject<SlackGroupDetails>(groupDetailsResponse);
                        _logger.Info("Groups:" + groups.ErrorMessage);
                        _logger.Debug("Slack User Id  : " + (await _userManager.FindByEmailAsync(applicationUser.Email)).SlackUserId);
                        if (groups.Ok)
                        {
                            foreach (var channel in groups.Groups)
                            {
                                _logger.Info("Group:" + channel);
                                await AddChannelGroupAsync(channel);
                                _logger.Debug("Slack User Id  : " + (await _userManager.FindByEmailAsync(applicationUser.Email)).SlackUserId);
                            }
                        }
                        else
                            throw new SlackAuthorizeException(_stringConstant.SlackAuthError + groups.ErrorMessage);
                    }
                    else
                        throw new SlackAuthorizeException(string.Format(_stringConstant.NotInSlackOrNotExpectedUser, slackUserDetails.Profile.Email));
                }
                else
                    throw new SlackAuthorizeException(_stringConstant.UserNotInSlack);
            }
            else
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + slackUsers.ErrorMessage);

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
                throw new SlackAuthorizeException(_stringConstant.SlackAuthError + "No app has been selected for integration with slack");
        }


        /// <summary>
        /// Method to update slack user table when there is any changes in slack
        /// </summary>
        /// <param name="slackEvent"></param>
        public async Task SlackEventUpdateAsync(SlackEventApiAC slackEvent)
        {
            SlackUserDetails user = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackEvent.Event.User.UserId);
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
        /// <returns>empty string</returns>
        public async Task<string> CheckUserSlackInformation(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            _logger.Debug("Slack User Id  : " + (await _userManager.FindByEmailAsync(user.Email)).SlackUserId);
            if (!string.IsNullOrEmpty(user.SlackUserId))
            {
                _logger.Debug("Slack User Id  : " + (await _userManager.FindByEmailAsync(user.Email)).SlackUserId);
                if (_slackUserDetailsRepository.Any(x => x.UserId == user.SlackUserId))
                    return string.Empty;
            }
            return user.Email;
        }


        #endregion

        #region Private Region
        /// <summary>
        /// Used to update application user details with slack id - JJ
        /// </summary>
        /// <param name="email">Email id of the logged in user</param>
        /// <returns>true if successfully updated</returns>
        private async Task<bool> UpdateApplicationUserAsync(string email)
        {
            ApplicationUser userInfo = await _userManager.FindByEmailAsync(email);
            if (userInfo != null)
            {
                SlackUserDetails slackUser = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.Email == email);
                if (slackUser != null)
                {
                    userInfo.SlackUserId = slackUser.UserId;
                    await _userManager.UpdateAsync(userInfo);
                    return true;
                }
            }
            return false;
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
        /// Method to start bot as per module name
        /// </summary>
        /// <param name="module">name of module</param>
        private async Task StartBotByModuleAsync(string module)
        {
            var appCredential = await _appCredentialRepository.FetchAppCredentialByModule(module);
            if(!string.IsNullOrEmpty(appCredential?.BotToken))
            {
                if (module == _stringConstant.TaskModule)
                    _taskMailBotRepository.StartAndConnectTaskMailBot(appCredential.BotToken);
                if (module == _stringConstant.Scrum)
                    _scrumRepository.StartAndConnectScrumBot(appCredential.BotToken);
            }
        }
        #endregion
    }
}