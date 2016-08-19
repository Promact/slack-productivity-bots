using Promact.Erp.DomainModel.ApplicationClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ProjectUserCall
{
    public interface IProjectUserCallRepository
    {
        Task<User> GetUserByUsername(string userName);
        Task<List<User>> GetTeamLeaderUserName(string userName);
       Task<List<User>> GetManagementUserName();
                Task<ProjectAc> GetProjectDetails(string groupName);
    }
}
