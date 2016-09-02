using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.SlackChannelRepository
{
    public interface ISlackChannelRepository
    {
        /// <summary>
        /// Method to add slack channel 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        void AddSlackChannel(SlackChannelDetails slackChannelDetails);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slackId"></param>
        /// <returns></returns>
        SlackChannelDetails GetById(string slackId);
    }
}
