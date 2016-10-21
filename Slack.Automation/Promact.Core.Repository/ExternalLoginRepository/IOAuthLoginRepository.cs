using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ExternalLoginRepository
{
    public interface IOAuthLoginRepository
    {
        Task<ApplicationUser> AddNewUserFromExternalLogin(string email, string accessToken, string slackUserName);
        OAuthApplication ExternalLoginInformation(string refreshToken);
        Task AddSlackUserInformation(string code);
        void SlackEventUpdate(SlackEventApiAC slackEvent);
        void SlackChannelAdd(SlackEventApiAC slackEvent);
    }
}
