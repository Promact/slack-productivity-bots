using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;

namespace Promact.Core.Repository.SlackChannelRepository
{
    public class SlackChannelRepository : ISlackChannelRepository
    {

        #region Private Variable

        private readonly IRepository<SlackChannelDetails> _slackChannelDetailsReposirtory;
        
        #endregion


        #region Constructor

        public SlackChannelRepository(IRepository<SlackChannelDetails> slackChannelDetailsReposirtory)
        {
            _slackChannelDetailsReposirtory = slackChannelDetailsReposirtory;
        }

        #endregion


        #region Public Methods


        /// <summary>
        /// Method to add slack channel. - JJ
        /// </summary>
        /// <param name="slackChannelDetails">object of SlackChannelDetails</param>
        public async Task AddSlackChannelAsync(SlackChannelDetails slackChannelDetails)
        {
            _slackChannelDetailsReposirtory.Insert(slackChannelDetails);
            await _slackChannelDetailsReposirtory.SaveChangesAsync();
        }


        /// <summary>
        /// Method to get slack channel information by their slack channel id - JJ
        /// </summary>
        /// <param name="slackChannelId">Id of slack channel</param>
        /// <returns>object of SlackChannelDetails</returns>
        public async Task<SlackChannelDetails> GetByIdAsync(string slackChannelId)
        {
            SlackChannelDetails channel = await _slackChannelDetailsReposirtory.FirstOrDefaultAsync(x => x.ChannelId == slackChannelId);
            return channel;
        }


        /// <summary>
        /// Method to update slack channel details - JJ
        /// </summary>
        /// <param name="slackChannelDetails">object of SlackChannelDetails</param>
        public async Task UpdateSlackChannelAsync(SlackChannelDetails slackChannelDetails)
        {
            _slackChannelDetailsReposirtory.Update(slackChannelDetails);
            await _slackChannelDetailsReposirtory.SaveChangesAsync();
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
                _slackChannelDetailsReposirtory.Delete(channel.Id);
                await _slackChannelDetailsReposirtory.SaveChangesAsync();
            }
        }


        #endregion
    }
}
