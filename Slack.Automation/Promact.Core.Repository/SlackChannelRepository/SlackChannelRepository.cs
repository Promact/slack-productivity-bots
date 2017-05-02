using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using System.Collections.Generic;
using NLog;

namespace Promact.Core.Repository.SlackChannelRepository
{
    public class SlackChannelRepository : ISlackChannelRepository
    {
        #region Private Variable
        private readonly IRepository<SlackChannelDetails> _slackChannelDetailsRepository;
        private readonly ILogger _loggerSlackEvent;
        #endregion

        #region Constructor
        public SlackChannelRepository(IRepository<SlackChannelDetails> slackChannelDetailsRepository)
        {
            _slackChannelDetailsRepository = slackChannelDetailsRepository;
            _loggerSlackEvent = LogManager.GetLogger("SlackEvent");
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
            _loggerSlackEvent.Debug("Channel : " + channel);
            if (channel != null)
            {
                _loggerSlackEvent.Debug("Deleting channel");
                _slackChannelDetailsRepository.Delete(channel.Id);
                await _slackChannelDetailsRepository.SaveChangesAsync();
                _loggerSlackEvent.Debug("Channel deleted");
            }
        }


        /// <summary>
        /// Method to fetch active slack channels - JJ
        /// </summary>
        ///<returns>list of object of SlackChannelDetails</returns>
        public async Task<IEnumerable<SlackChannelDetails>> FetchChannelAsync()
        {
            return await _slackChannelDetailsRepository.FetchAsync(x => !x.Deleted);
        }


        /// <summary>
        /// Method to fetch active slack channels - JJ
        /// </summary>
        /// <param name="projectId">Id of the OAuth Project</param>
        ///<returns>object of SlackChannelDetails</returns>
        public async Task<SlackChannelDetails> FetchChannelByProjectIdAsync(int projectId)
        {
            return await _slackChannelDetailsRepository.FirstOrDefaultAsync(x => x.ProjectId == projectId);
        }

            
        #endregion
    }
}
