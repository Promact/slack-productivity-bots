using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.SlackChannelRepository
{
    public interface ISlackChannelRepository
    {
        void AddSlackChannel(SlackChannelDetails slackChannelDetails);
        SlackChannelDetails GetById(string slackId);
    }
}
