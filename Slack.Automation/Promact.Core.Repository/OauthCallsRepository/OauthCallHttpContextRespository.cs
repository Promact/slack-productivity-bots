using Newtonsoft.Json;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Promact.Core.Repository.OauthCallsRepository
{
    public class OauthCallHttpContextRespository : IOauthCallHttpContextRespository
    {
        #region Private 
        private readonly IStringConstantRepository _stringConstant;
        private readonly IHttpClientService _httpClientService;
        private readonly HttpContextBase _httpContextBase;
        private readonly ApplicationUserManager _userManager;
        private readonly IAttachmentRepository _attachmentRepository;
        #endregion

        #region Constructor
        public OauthCallHttpContextRespository(IStringConstantRepository stringConstant, IHttpClientService httpClientService,
            HttpContextBase httpContextBase, ApplicationUserManager userManager, IAttachmentRepository attachmentRepository)
        {
            _stringConstant = stringConstant;
            _httpClientService = httpClientService;
            _httpContextBase = httpContextBase;
            _userManager = userManager;
            _attachmentRepository = attachmentRepository;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to call an api from project oAuth server and get Employee detail by their Id. - GA
        /// </summary>
        /// <param name="employeeId">id of employee</param>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>User Details. Object of User</returns>
        public async Task<User> GetUserByEmployeeIdAsync(string employeeId)
        {
            var accessToken = await GetCurrentUserAcceesToken();
            User userDetails = new User();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, employeeId, _stringConstant.UserDetailUrl);
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            if (response != null)
            {
                userDetails = JsonConvert.DeserializeObject<User>(response);
            }
            return userDetails;
        }

        /// <summary>
        /// Method to call an api from oauth server and get all the projects under a specific teamleader id along with users in it. - GA
        /// </summary>
        /// <param name="teamLeaderId">id of the team leader</param>
        /// <returns>list of users in a project.List of object of User</returns>
        public async Task<List<User>> GetProjectUsersByTeamLeaderIdAsync(string teamLeaderId)
        {
            var accessToken = await GetCurrentUserAcceesToken();
            List<User> projectUsers = new List<User>();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, teamLeaderId, _stringConstant.ProjectUsersByTeamLeaderId);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            if (response != null)
            {
                projectUsers = JsonConvert.DeserializeObject<List<User>>(response);
            }
            return projectUsers;
        }

        /// <summary>
        /// Method is used to call an api from oauth server and return list of all the projects. - GA
        /// </summary>
        /// <param name="accessToken">user's access token from Promact OAuth Server</param>
        /// <returns>list of all the projects</returns>
        public async Task<List<ProjectAc>> GetAllProjectsAsync()
        {
            var accessToken = await GetCurrentUserAcceesToken();
            List<ProjectAc> projects = new List<ProjectAc>();
            var requestUrl = _stringConstant.AllProjectUrl;
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            if (response != null)
            {
                projects = JsonConvert.DeserializeObject<List<ProjectAc>>(response);
            }
            return projects;
        }


        /// <summary>
        /// Method to call an api from oauth server and get the details of a project using projecId. - GA
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <returns>Details of a project</returns>
        public async Task<ProjectAc> GetProjectDetailsAsync(int projectId)
        {
            var accessToken = await GetCurrentUserAcceesToken();
            ProjectAc project = new ProjectAc();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, projectId, _stringConstant.GetProjectDetails);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken);
            if (response != null)
            {
                project = JsonConvert.DeserializeObject<ProjectAc>(response);
            }
            return project;
        }



        /// <summary>
        /// Used to get user role. - RS
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <returns>user details. List of object of UserRoleAc</returns>
        public async Task<List<UserRoleAc>> GetUserRoleAsync(string userId)
        {
            var accessToken = await GetCurrentUserAcceesToken();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, userId, _stringConstant.UserRoleUrl);
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            var userRoleListAc = JsonConvert.DeserializeObject<List<UserRoleAc>>(response);
            return userRoleListAc;
        }


        /// <summary>
        /// List of employee under this employee. - RS
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <returns>List of user. List of object of UserRoleAc</returns>
        public async Task<List<UserRoleAc>> GetListOfEmployeeAsync(string userId)
        {
            var accessToken = await GetCurrentUserAcceesToken();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, userId, _stringConstant.TeamMembersUrl);
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken);
            var userRoleListAc = JsonConvert.DeserializeObject<List<UserRoleAc>>(response);
            return userRoleListAc;
        }
        #endregion

        #region Private Method
        private async Task<string> GetCurrentUserAcceesToken()
        {
            var claimIdentity = _httpContextBase.User.Identity as ClaimsIdentity;
            var userName = (await _userManager.FindByIdAsync(claimIdentity.Claims.ToList().Single(x => x.Type == _stringConstant.Sub).Value)).UserName;
            return (await _attachmentRepository.UserAccessTokenAsync(userName));
        }
        #endregion
    }
}
