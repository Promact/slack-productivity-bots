using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.TaskMailRepository
{
    public interface ITaskMailRepository
    {
        Task<string> StartTaskMail(string userName, string accessToken);
        Task<string> QuestionAndAnswer(string userName, string accessToken, string answer);

    }
}
