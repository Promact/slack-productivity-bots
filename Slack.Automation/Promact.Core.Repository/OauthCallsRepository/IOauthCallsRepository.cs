using Promact.Erp.DomainModel.ApplicationClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.OauthCallsRepository
{
    public interface IOauthCallsRepository
    {
        /// <summary>
        /// Method to call an api of oAuth server and get Employee detail by their slack userId
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns>user Details</returns>
        Task<User> GetUserByUserId(string userName, string accessToken);

        /// <summary>
        /// Method to call an api of oAuth server and get List of TeamLeader's slack UserName from employee userName
        /// </summary>
        /// <param name="slackUserId"></param>
        /// <param name="accessToken"></param>
        /// <returns>teamLeader details</returns>
        Task<List<User>> GetTeamLeaderUserId(string slackUserId, string accessToken);

        /// <summary>
        /// Method to call an api of oAuth server and get List of Management People's Slack UserName
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>management details</returns>
        Task<List<User>> GetManagementUserName(string accessToken);

        Task<List<UserRoleAc>> GetUserRole(string userId, string accessToken);

        /// <summary>
        /// Method to call an api from project oAuth server and get Project details of the given group - JJ
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="accessToken"></param>
        /// <returns>object of ProjectAc</returns>
        Task<ProjectAc> GetProjectDetails(string groupName, string accessToken);


        Task<List<UserRoleAc>> GetListOfEmployee(string userId, string accessToken);

      

        /// <summary>
        /// This method is used to fetch list of users/employees of the given group name. - JJ
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="accessToken"></param>
        /// <returns>list of object of User</returns>
        Task<List<User>> GetUsersByGroupName(string groupName, string accessToken);


        /// <summary>
        /// Method to call an api of oAuth server and get Casual leave allowed to user by user slackName
        /// </summary>
        /// <param name="slackUserId"></param>
        /// <param name="accessToken"></param>
        /// <returns>Number of casual leave allowed</returns>
        Task<LeaveAllowed> CasualLeave(string slackUserId, string accessToken);

        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their Id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="accessToken"></param>
        /// <returns>User Details</returns>
        Task<User> GetUserByEmployeeIdAsync(string employeeId, string accessToken);

        /// <summary>
        /// Method to call an api from project oAuth server and get logged in user details by their username
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns>User Details</returns>
        Task<User> GetUserByUserNameAsync(string userName, string accessToken);

        /// <summary>
        /// Method to call an api from oauth server and get all the projects under a specific teamleader id along with users in it
        /// </summary>
        /// <param name="teamLeaderId"></param>
        /// <param name="accessToken"></param>
        /// <returns>list of users in a project</returns>
        Task<List<User>> GetProjectUsersByTeamLeaderIdAsync(string teamLeaderId, string accessToken);

        /// <summary>
        /// Method to call an api from oAuth server and get whether user is admin or not
        /// </summary>
        /// <param name="slackUserId"></param>
        /// <param name="accessToken"></param>

        /// <returns>true or false</returns>
        Task<bool> UserIsAdmin(string slackUserId, string accessToken);

        



        /// <summary>
        /// Method to call an api from oAuth server and get whether user is admin or not
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>

        /// <returns>true or false</returns>
        Task<bool> UserIsAdmin(string userName, string accessToken);

        /// <summary>
        /// Method to call an api from oauth server and get the list of all the projects
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>list of all the projects</returns>
        Task<List<ProjectAc>> GetAllProjectsAsync(string accessToken);

        /// <summary>
        /// Method to call an api from oauth server and get the details of a project using projecId
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="accessToken"></param>
        /// <returns>Details of a project</returns>
        Task<ProjectAc> GetProjectDetailsAsync(int projectId, string accessToken);
    }
}