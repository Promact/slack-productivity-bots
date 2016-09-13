using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;

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
            _slackUserDetails.Insert(slackUserDetails);
            _slackUserDetails.Save();
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
