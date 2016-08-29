using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.SlackUserRepository
{
    public interface ISlackUserRepository
    {
        void AddSlackUser(SlackUserDetails slackUserDetails);
        SlackUserDetails GetById(string slackId);
    }
}
