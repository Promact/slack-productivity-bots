using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.TaskMailRepository
{
    public interface ITaskMailRepository
    {
        Task StartTaskMail(string userName, string accessToken);
        Task SendFirstQuestion(string userName, string accessToken);
    }
}
