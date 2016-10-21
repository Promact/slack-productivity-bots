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
        /// Method to update slack user 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        public void UpdateSlackUser(SlackUserDetails slackUserDetails)
        {
            var user = _slackUserDetails.FirstOrDefault(x => x.UserId == slackUserDetails.UserId);
            if (user != null)
            {
                user.Deleted = slackUserDetails.Deleted;
                user.IsAdmin = slackUserDetails.IsAdmin;
                user.IsBot = slackUserDetails.IsBot;
                user.IsOwner = slackUserDetails.IsOwner;
                user.IsPrimaryOwner = slackUserDetails.IsPrimaryOwner;
                user.IsRestrictedUser = slackUserDetails.IsRestrictedUser;
                user.IsUltraRestrictedUser = slackUserDetails.IsUltraRestrictedUser;
                user.Name = slackUserDetails.Name;
                user.RealName = slackUserDetails.RealName;
                user.Status = slackUserDetails.Status;
                user.TeamId = slackUserDetails.TeamId;
                user.TimeZoneLabel = slackUserDetails.TimeZoneLabel;
                user.TimeZoneOffset = slackUserDetails.TimeZoneOffset;

                user.Title = slackUserDetails.Profile.Title;
                user.Email = slackUserDetails.Profile.Email;
                user.Skype = slackUserDetails.Profile.Skype;
                user.LastName = slackUserDetails.Profile.LastName;
                user.FirstName = slackUserDetails.Profile.FirstName;
                user.Phone = slackUserDetails.Profile.Phone;

                _slackUserDetails.Update(user);
            }
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
