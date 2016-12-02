using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using System.Threading.Tasks;

namespace Promact.Core.Repository.SlackChannelRepository
{
    public class SlackChannelRepository : ISlackChannelRepository
    {
        private readonly IRepository<SlackChannelDetails> _slackChannelDetailsContext;

        public SlackChannelRepository(IRepository<SlackChannelDetails> slackChannelDetailsContext)
        {
            _slackChannelDetailsContext = slackChannelDetailsContext;
        }

        /// <summary>
        /// Method to add slack channel 
        /// </summary>
        /// <param name="slackChannelDetails"></param>
        public async Task AddSlackChannelAsync(SlackChannelDetails slackChannelDetails)
        {
            _slackChannelDetailsContext.Insert(slackChannelDetails);
            await _slackChannelDetailsContext.SaveChangesAsync();
        }

        /// <summary>
        /// Method to get slack channel information by their slack channel id
        /// </summary>
        /// <param name="slackId"></param>
        /// <returns>object of SlackChannelDetails</returns>
        public async Task<SlackChannelDetails> GetByIdAsync(string slackId)
        {
            SlackChannelDetails channel = await _slackChannelDetailsContext.FirstOrDefaultAsync(x => x.ChannelId == slackId);
            return channel;
        }
    }
}
