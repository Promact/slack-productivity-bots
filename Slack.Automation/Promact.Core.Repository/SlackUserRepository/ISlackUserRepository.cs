using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Fetch the list of Slack Users
        /// </summary>
        /// <returns>list of object of SlackUserDetails</returns>
        List<SlackUserDetailAc> GetAllSlackUsers();


        /// <summary>
        /// Method to get slack user information by their slack user name
        /// </summary>
        /// <param name="slackName"></param>
        /// <returns>user</returns>
        SlackUserDetails GetBySlackName(string slackName);


    }
}
