using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.SlackUserRepository
{
    public interface ISlackUserRepository
    {
        /// <summary>
        /// Method to add slack user 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        void AddSlackUser(SlackUserDetails slackUserDetails);

        /// <summary>
        /// Method to update slack user 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        void UpdateSlackUser(SlackUserDetails slackUserDetails);

        /// <summary>
        /// Method to get slack user information by their slack user id
        /// </summary>
        /// <param name="slackId"></param>
        /// <returns>user</returns>
        SlackUserDetails GetById(string slackId);
    }
}
