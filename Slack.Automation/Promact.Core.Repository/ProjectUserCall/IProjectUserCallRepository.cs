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
        Task<ProjectAc> GetProjectDetails(string groupName);
        Task<User> GetUsersByGroupName(string groupName,string userName);
        Task<ProjectAc> GetProjectDetailsByUserName(string userName, string accessToken);
        Task<List<User>> GetUsersByGroupName(string groupName);
        Task<User> GetUserById(string EmployeeId);
        Task<User> GetUserByEmployeeId(string employeeId, string accessToken);
        Task<User> GetUserByUserName(string userName, string accessToken);
    }
}
