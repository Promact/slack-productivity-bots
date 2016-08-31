using Promact.Erp.DomainModel.ApplicationClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ProjectUserCall
{
    public interface IProjectUserCallRepository
    {
        /// <summary>
        /// Method to call an api of oAuth server and get Employee detail by their slack userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>user Details</returns>
        Task<User> GetUserByUsername(string userName, string accessToken);

        /// <summary>
        /// Method to call an api of oAuth server and get List of TeamLeader's slack UserName from employee userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>teamLeader details</returns>
        Task<List<User>> GetTeamLeaderUserName(string userName, string accessToken);

        /// <summary>
        /// Method to call an api of oAuth server and get List of Management People's Slack UserName
        /// </summary>
        /// <returns>management details</returns>
        Task<List<User>> GetManagementUserName(string accessToken);
        Task<ProjectAc> GetProjectDetails(string groupName);
     
        Task<ProjectAc> GetProjectDetailsByUserName(string userName, string accessToken);
        Task<List<User>> GetUsersByGroupName(string groupName);
        Task<User> GetUserById(string EmployeeId);
        Task<User> GetUserByEmployeeId(string employeeId);

        /// <summary>
        /// Method to call an api of oAuth server and get Casual leave allowed to user by user slackName
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns>Number of casual leave allowed</returns>
        Task<double> CasualLeave(string slackUserName, string accessToken);
        Task<List<User>> GetUsersByGroupName(string groupName);
        Task<User> GetUserById(string EmployeeId);
        Task<User> GetUserByEmployeeId(string employeeId, string accessToken);
        Task<User> GetUserByUserName(string userName, string accessToken);
    }
}
