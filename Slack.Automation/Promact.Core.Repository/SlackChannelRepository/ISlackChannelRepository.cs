using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.SlackChannelRepository
{
    public interface ISlackChannelRepository
    {
        /// <summary>
        /// Method to add slack channel 
        /// </summary>
        /// <param name="slackChannelDetails"></param>
        void AddSlackChannel(SlackChannelDetails slackChannelDetails);

        /// <summary>
        /// Method to get slack channel information by their slack channel id
        /// </summary>
        /// <param name="slackId"></param>
        /// <returns>object of SlackChannelDetails</returns>
        SlackChannelDetails GetById(string slackId);
    }
}
