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
        /// Method to add slack channel. - JJ
        /// </summary>
        /// <param name="slackChannelDetails">object of SlackChannelDetails</param>
        public async Task AddSlackChannelAsync(SlackChannelDetails slackChannelDetails)
        {
            _slackChannelDetailsContext.Insert(slackChannelDetails);
            await _slackChannelDetailsContext.SaveChangesAsync();
        }


        /// <summary>
        /// Method to get slack channel information by their slack channel id - JJ
        /// </summary>
        /// <param name="slackChannelId">Id of slack channel</param>
        /// <returns>object of SlackChannelDetails</returns>
        public async Task<SlackChannelDetails> GetByIdAsync(string slackChannelId)
        {
            SlackChannelDetails channel = await _slackChannelDetailsContext.FirstOrDefaultAsync(x => x.ChannelId == slackChannelId);
            return channel;
        }


        /// <summary>
        /// Method to update slack channel details - JJ
        /// </summary>
        /// <param name="slackChannelDetails">object of SlackChannelDetails</param>
        public async Task UpdateSlackChannelAsync(SlackChannelDetails slackChannelDetails)
        {
            _slackChannelDetailsContext.Update(slackChannelDetails);
            await _slackChannelDetailsContext.SaveChangesAsync();
        }


        /// <summary>
        /// Method to delete slack channel by their slack channel id - JJ
        /// </summary>
        /// <param name="slackChannelId">Id of slack channel</param>
        public async Task DeleteChannelAsync(string slackChannelId)
        {
            SlackChannelDetails channel = await GetByIdAsync(slackChannelId);
            if (channel != null)
            {
                _slackChannelDetailsContext.Delete(channel.Id);
                await _slackChannelDetailsContext.SaveChangesAsync();
            }
        }


    }
}
