using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using System;

namespace Promact.Core.Repository.SlackUserRepository
{
    public class SlackUserRepository : ISlackUserRepository
    {
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        public SlackUserRepository(IRepository<SlackUserDetails> slackUserDetails)
        {
            _slackUserDetails = slackUserDetails;
        }

        /// <summary>
        /// Method to add slack user 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        public void AddSlackUser(SlackUserDetails slackUserDetails)
        {
            slackUserDetails.Title = slackUserDetails.Profile.Title;
            slackUserDetails.Email = slackUserDetails.Profile.Email;
            slackUserDetails.Skype = slackUserDetails.Profile.Skype;
            slackUserDetails.LastName = slackUserDetails.Profile.LastName;
            slackUserDetails.FirstName = slackUserDetails.Profile.FirstName;
            slackUserDetails.Phone = slackUserDetails.Profile.Phone;
            slackUserDetails.CreatedOn = DateTime.UtcNow;
            _slackUserDetails.Insert(slackUserDetails);
        }

        /// <summary>
        /// Method to get slack user information by their slack user id
        /// </summary>
        /// <param name="slackId"></param>
        /// <returns>user</returns>
        public SlackUserDetails GetById(string slackId)
        {
            var user = _slackUserDetails.FirstOrDefault(x => x.UserId == slackId);
            return user;
        }
    }
}
