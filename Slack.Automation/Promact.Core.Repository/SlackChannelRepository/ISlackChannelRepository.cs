using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System.Threading.Tasks;

namespace Promact.Core.Repository.SlackChannelRepository
{
    public interface ISlackChannelRepository
    {

        /// <summary>
        /// Method to add slack channel - JJ
        /// </summary>
        /// <param name="slackChannelDetails">object of SlackChannelDetails</param>
        Task AddSlackChannelAsync(SlackChannelDetails slackChannelDetails);


        /// <summary>
        /// Method to get slack channel information by their slack channel id - JJ
        /// </summary>
        /// <param name="slackChannelId">Id of slack channel</param>
        /// <returns>object of SlackChannelDetails</returns>
        Task<SlackChannelDetails> GetByIdAsync(string slackChannelId);


        /// <summary>
        /// Method to delete slack channel by their slack channel id - JJ
        /// </summary>
        /// <param name="slackChannelId">Id of slack channel</param>
        Task DeleteChannelAsync(string slackChannelId);


        /// <summary>
        /// Method to update slack channel - JJ
        /// </summary>
        /// <param name="slackChannelDetails">object of SlackChannelDetails</param>
        Task UpdateSlackChannelAsync(SlackChannelDetails slackChannelDetails);


    }
}
