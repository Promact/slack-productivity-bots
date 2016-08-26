using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Core.Repository.DataRepository;

namespace Promact.Core.Repository.SlackUserRepository
{
    public class SlackUserRepository : ISlackUserRepository
    {
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        public SlackUserRepository(IRepository<SlackUserDetails> slackUserDetails)
        {
            _slackUserDetails = slackUserDetails;
        }
        public void AddSlackUser(SlackUserDetails slackUserDetails)
        {
            _slackUserDetails.Insert(slackUserDetails);
            _slackUserDetails.Save();
        }

        public SlackUserDetails GetById(string slackId)
        {
            var user = _slackUserDetails.FirstOrDefault(x => x.Id == slackId);
            return user;
        }
    }
}
