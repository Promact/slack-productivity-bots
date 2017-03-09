using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;

namespace Promact.Core.Repository.SlackChannelRepository
{
    public class SlackChannelRepository : ISlackChannelRepository
    {

        #region Private Variable

        private readonly IRepository<SlackChannelDetails> _slackChannelDetailsRepository;

        #endregion


        #region Constructor

        public SlackChannelRepository(IRepository<SlackChannelDetails> slackChannelDetailsRepository)
        {
            _slackChannelDetailsRepository = slackChannelDetailsRepository;
        }

        #endregion


        #region Public Methods


        /// <summary>
        /// Method to add slack channel. - JJ
        /// </summary>
        /// <param name="slackChannelDetails">object of SlackChannelDetails</param>
        public async Task AddSlackChannelAsync(SlackChannelDetails slackChannelDetails)
        {
            _slackChannelDetailsRepository.Insert(slackChannelDetails);
            await _slackChannelDetailsRepository.SaveChangesAsync();
        }


        /// <summary>
        /// Method to get slack channel information by their slack channel id - JJ
        /// </summary>
        /// <param name="slackChannelId">Id of slack channel</param>
        /// <returns>object of SlackChannelDetails</returns>
        public async Task<SlackChannelDetails> GetByIdAsync(string slackChannelId)
        {
            SlackChannelDetails channel = await _slackChannelDetailsRepository.FirstOrDefaultAsync(x => x.ChannelId == slackChannelId);
            return channel;
        }


        /// <summary>
        /// Method to update slack channel details - JJ
        /// </summary>
        /// <param name="slackChannelDetails">object of SlackChannelDetails</param>
        public async Task UpdateSlackChannelAsync(SlackChannelDetails slackChannelDetails)
        {
            _slackChannelDetailsRepository.Update(slackChannelDetails);
            await _slackChannelDetailsRepository.SaveChangesAsync();
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
                _slackChannelDetailsRepository.Delete(channel.Id);
                await _slackChannelDetailsRepository.SaveChangesAsync();
            }
        }


        #endregion
    }
}
