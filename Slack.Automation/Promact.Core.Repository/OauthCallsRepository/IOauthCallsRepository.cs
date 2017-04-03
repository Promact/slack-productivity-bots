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
        /// <param name="userId">userId of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>user Details.Object of User</returns>
        Task<User> GetUserByUserIdAsync(string userId, string accessToken);


        /// <summary>
        /// Method to call an api from project oAuth server and get List of TeamLeader's slack UserName from employee userName. - SS
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>teamLeader details.List of object of User</returns>
        Task<List<User>> GetTeamLeaderUserIdAsync(string userId, string accessToken);


        /// <summary>
        /// Method to call an api from project oAuth server and get List of Management People's Slack UserName. - SS
        /// </summary>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>management details.List of object of User</returns>
        Task<List<User>> GetManagementUserNameAsync(string accessToken);


        /// <summary>
        /// Method to call an api from project oAuth server and get Project details of the project id - JJ
        /// </summary>
        /// <param name="projectId">Id of OAuth Project</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>object of ProjectAc</returns>
        Task<ProjectAc> GetProjectDetailsAsync(int projectId, string accessToken);


        /// <summary>
        /// Method to call an api of oAuth server and get Casual leave allowed to user by user slackName. - SS
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>Number of casual leave allowed. Object of LeaveAllowed</returns>
        Task<LeaveAllowed> AllowedLeave(string userId, string accessToken);


        /// <summary>
        /// Method to call an api from oAuth server and get whether user is admin or not. - SS
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>true if user has admin role else false</returns>
        Task<bool> UserIsAdminAsync(string userId, string accessToken);


        /// <summary>
        /// Method to get list of projects from oauth-server for an user
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>list of project</returns>
        Task<List<ProjectAc>> GetListOfProjectsEnrollmentOfUserByUserIdAsync(string userId, string accessToken);


        /// <summary>
        /// Method to get list of team member by project Id
        /// </summary>
        /// <param name="projectId">project Id</param>
        /// <param name="accessToken">access token</param>
        /// <returns>list of team members</returns>
        Task<List<User>> GetAllTeamMemberByProjectIdAsync(int projectId, string accessToken);

    }
}