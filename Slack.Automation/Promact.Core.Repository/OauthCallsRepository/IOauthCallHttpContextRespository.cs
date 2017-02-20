using Promact.Erp.DomainModel.ApplicationClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.OauthCallsRepository
{
    public interface IOauthCallHttpContextRespository
    {
        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their Id. - GA
        /// </summary>
        /// <param name="employeeId">id of employee</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>User Details. Object of User</returns>
        Task<User> GetUserByEmployeeIdAsync(string employeeId);

        /// <summary>
        /// Method to call an api from oauth server and get all the projects under a specific teamleader id along with users in it. - GA
        /// </summary>
        /// <param name="teamLeaderId">id of the team leader</param>
        /// <returns>list of users in a project.List of object of User</returns>
        Task<List<User>> GetProjectUsersByTeamLeaderIdAsync(string teamLeaderId);

        /// <summary>
        /// Method is used to call an api from oauth server and return list of all the projects. - GA
        /// </summary>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>list of all the projects</returns>
        Task<List<ProjectAc>> GetAllProjectsAsync();

        /// <summary>
        /// Method to call an api from oauth server and get the details of a project using projecId. - GA
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <returns>Details of a project</returns>
        Task<ProjectAc> GetProjectDetailsAsync(int projectId);

        /// <summary>
        /// Used to get user role. - RS
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <returns>user details. List of object of UserRoleAc</returns>
        Task<List<UserRoleAc>> GetUserRoleAsync(string userId);

        /// <summary>
        /// List of employee under this employee. - RS
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <returns>List of user. List of object of UserRoleAc</returns>
        Task<List<UserRoleAc>> GetListOfEmployeeAsync(string userId);

        /// <summary>
        /// Method to call an api from oAuth server and get whether user is admin or not. - SS
        /// </summary>
        /// <returns>true if current user has admin role else false</returns>
        Task<bool> CurrentUserIsAdminAsync();
    }
}
