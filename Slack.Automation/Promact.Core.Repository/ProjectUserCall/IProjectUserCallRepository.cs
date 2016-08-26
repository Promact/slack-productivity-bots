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
        //Task<User> GetUserByUsername(string userName);
        //Task<List<ProjectUserDetailsApplicationClass>> GetTeamLeaderUserName(string userName);
        //Task<List<ProjectUserDetailsApplicationClass>> GetManagementUserName();
        Task<User> GetUserByEmployeeId(string employeeId, string accessToken);
    }
}
