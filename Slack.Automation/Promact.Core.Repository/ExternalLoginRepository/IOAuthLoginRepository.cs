using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ExternalLoginRepository
{
    public interface IOAuthLoginRepository
    {
        /// <summary>
        /// Method to add a new user in Application user table and store user's external login information in UserLogin table
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accessToken"></param>
        /// <param name="slackUserName"></param>
        /// <returns>user information</returns>
        Task<ApplicationUser> AddNewUserFromExternalLogin(string email, string accessToken, string slackUserName,string userId);

        /// <summary>
        /// Method to get OAuth Server's app information
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>Oauth</returns>
        OAuthApplication ExternalLoginInformation(string refreshToken);

        /// <summary>
        /// Method to add Slack Users,channels and groups information 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task AddSlackUserInformation(string code);

        /// <summary>
        /// Method to update slack user table when there is any changes in slack
        /// </summary>
        /// <param name="slackEvent"></param>
        void SlackEventUpdate(SlackEventApiAC slackEvent);

        /// <summary>
        /// Method to update slack channel table when a channel is added or updated in team.
        /// </summary>
        /// <param name="slackEvent"></param>
        void SlackChannelAdd(SlackEventApiAC slackEvent);
    }
}
