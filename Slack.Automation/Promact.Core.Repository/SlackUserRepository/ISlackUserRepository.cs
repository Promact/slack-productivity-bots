using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// Method to get slack user information by their slack user id
        /// </summary>
        /// <param name="slackId"></param>
        /// <returns>user</returns>
        SlackUserDetails GetById(string slackId);
    }
}
