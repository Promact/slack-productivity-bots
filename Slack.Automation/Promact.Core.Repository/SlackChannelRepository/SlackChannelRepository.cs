using Promact.Core.Repository.DataRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.SlackChannelRepository
{
   public class SlackChannelRepository: ISlackChannelRepository
    {
        private readonly IRepository<SlackChannelDetails> _slackChannelDetailsContext;
        public SlackChannelRepository(IRepository<SlackChannelDetails> slackChannelDetailsContext)
        {
            _slackChannelDetailsContext = slackChannelDetailsContext;
        }

        /// <summary>
        /// Method to add slack channel 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        public void AddSlackChannel(SlackChannelDetails slackChannelDetails)
        {
            _slackChannelDetailsContext.Insert(slackChannelDetails);
            _slackChannelDetailsContext.Save();
        }

        /// <summary>
        /// Method to get slack channel information by their slack channel id
        /// </summary>
        /// <param name="slackId"></param>
        /// <returns></returns>
        public SlackChannelDetails GetById(string slackId)
        {
            var channel = _slackChannelDetailsContext.FirstOrDefault(x => x.Id == slackId);
            return channel;
        }
    }
}
