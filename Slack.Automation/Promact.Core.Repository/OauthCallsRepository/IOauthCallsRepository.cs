using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass;


namespace Promact.Core.Repository.OauthCallsRepository
{
    public interface IOauthCallsRepository
    {

        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their slack userId. - SS
        /// </summary>
        /// <param name="slackUserId">userId of slack user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>user Details.Object of User</returns>
        Task<User> GetUserByUserIdAsync(string slackUserId, string accessToken);


        /// <summary>
        /// Method to call an api from project oAuth server and get List of TeamLeader's slack UserName from employee userName. - SS
        /// </summary>
        /// <param name="slackUserId">userId of slack user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>teamLeader details.List of object of User</returns>
        Task<List<User>> GetTeamLeaderUserIdAsync(string slackUserId, string accessToken);


        /// <summary>
        /// Method to call an api from project oAuth server and get List of Management People's Slack UserName. - SS
        /// </summary>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>management details.List of object of User</returns>
        Task<List<User>> GetManagementUserNameAsync(string accessToken);


        /// <summary>
        /// Used to get user role. - RS
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>user details. List of object of UserRoleAc</returns>
        Task<List<UserRoleAc>> GetUserRoleAsync(string userId, string accessToken);


        /// <summary>
        /// Method to call an api from project oAuth server and get Project details of the given channel. - JJ 
        /// </summary>
        /// <param name="channelName">slack channel name</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>object of ProjectAc</returns>
        Task<ProjectAc> GetProjectDetailsAsync(string channelName, string accessToken);


        /// <summary>
        /// List of employee under this employee. - RS
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>List of user. List of object of UserRoleAc</returns>
        Task<List<UserRoleAc>> GetListOfEmployeeAsync(string userId, string accessToken);


        /// <summary>
        /// This method is used to fetch list of users/employees of the given channel name from OAuth server. - JJ
        /// </summary>
        /// <param name="channelName">slack channel name</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>list of object of User</returns>
        Task<List<User>> GetUsersByChannelNameAsync(string channelName, string accessToken);


        /// <summary>
        /// Method to call an api of oAuth server and get Casual leave allowed to user by user slackName. - SS
        /// </summary>
        /// <param name="slackUserId">userId of slack user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>Number of casual leave allowed. Object of LeaveAllowed</returns>
        Task<LeaveAllowed> CasualLeaveAsync(string slackUserId, string accessToken);


        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their Id. - GA
        /// </summary>
        /// <param name="employeeId">id of employee</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>User Details. Object of User</returns>
        Task<User> GetUserByEmployeeIdAsync(string employeeId, string accessToken);


        /// <summary>
        /// Method to call an api from oauth server and get all the projects under a specific teamleader id along with users in it. - GA
        /// </summary>
        /// <param name="teamLeaderId">id of the team leader</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>list of users in a project.List of object of User</returns>
        Task<List<User>> GetProjectUsersByTeamLeaderIdAsync(string teamLeaderId, string accessToken);


        /// <summary>
        /// Method to call an api from oAuth server and get whether user is admin or not. - SS
        /// </summary>
        /// <param name="slackUserId">userId of slack user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>true if user has admin role else false</returns>
        Task<bool> UserIsAdminAsync(string slackUserId, string accessToken);


        /// <summary>
        /// Method is used to call an api from oauth server and return list of all the projects. - GA
        /// </summary>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>list of all the projects</returns>
        Task<List<ProjectAc>> GetAllProjectsAsync(string accessToken);


        /// <summary>
        /// Method to call an api from oauth server and get the details of a project using projecId. - GA
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>Details of a project</returns>
        Task<ProjectAc> GetProjectDetailsAsync(int projectId, string accessToken);

    }
}