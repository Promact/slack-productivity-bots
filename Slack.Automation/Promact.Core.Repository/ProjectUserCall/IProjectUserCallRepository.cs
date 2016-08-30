using Promact.Erp.DomainModel.ApplicationClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ProjectUserCall
{
    public interface IProjectUserCallRepository
    {
        Task<User> GetUserByUsername(string userName, string accessToken);
        Task<List<User>> GetTeamLeaderUserName(string userName, string accessToken);
        Task<List<User>> GetManagementUserName(string accessToken);
        Task<ProjectAc> GetProjectDetails(string groupName,string accessToken);

        Task<ProjectAc> GetProjectDetailsByUserName(string userName, string accessToken);
        Task<List<User>> GetUsersByGroupName(string groupName, string accessToken);
        Task<User> GetUserById(string EmployeeId,string accessToken);
        Task<User> GetUserByEmployeeId(string employeeId);
        Task<User> GetUserBySlackUserName(string userName,string accessToken);
    }
}
