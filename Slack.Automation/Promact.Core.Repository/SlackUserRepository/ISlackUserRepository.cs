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
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        Task AddSlackUserAsync(SlackUserDetails slackUserDetails);


        /// <summary>
        /// Method to update slack user. - JJ
        /// </summary>
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        Task UpdateSlackUserAsync(SlackUserDetails slackUserDetails);


        /// <summary>
        /// Method to get slack user information by their slack user id
        /// </summary>
        /// <param name="slackId">slack user id</param>
        /// <returns>object of SlackUserDetailAc</returns>
        Task<SlackUserDetailAc> GetByIdAsync(string slackId);


        /// <summary>
        /// Method to get slack user information by their slack user name. - JJ
        /// </summary>
        /// <param name="slackName">slack user name</param>
        /// <returns>object of SlackUserDetailAc</returns>
        Task<SlackUserDetailAc> GetBySlackNameAsync(string slackName);


    }
}
