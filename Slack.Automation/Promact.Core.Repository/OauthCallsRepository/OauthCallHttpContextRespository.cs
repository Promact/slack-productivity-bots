using Autofac.Extras.NLog;
using Newtonsoft.Json;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringLiteral;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Promact.Core.Repository.OauthCallsRepository
{
    public class OauthCallHttpContextRespository : IOauthCallHttpContextRespository
    {
        #region Private 
        private readonly AppStringLiteral _stringConstant;
        private readonly IHttpClientService _httpClientService;
        private readonly HttpContextBase _httpContextBase;
        private readonly ApplicationUserManager _userManager;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public OauthCallHttpContextRespository(ISingletonStringLiteral stringConstant, IHttpClientService httpClientService,
            HttpContextBase httpContextBase, ApplicationUserManager userManager, IAttachmentRepository attachmentRepository, ILogger logger)
        {
            _stringConstant = stringConstant.StringConstant;
            _httpClientService = httpClientService;
            _httpContextBase = httpContextBase;
            _userManager = userManager;
            _attachmentRepository = attachmentRepository;
            _logger = logger;
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
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken, _stringConstant.Bearer);
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
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken, _stringConstant.Bearer);
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
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken, _stringConstant.Bearer);
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
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken, _stringConstant.Bearer);
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
            _logger.Info("basedUrl :" + _stringConstant.UserUrl);
            _logger.Info("requestUrl :" + requestUrl);
            _logger.Info("accessToken :" + accessToken);
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken, _stringConstant.Bearer);
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
            var response = await _httpClientService.GetAsync(_stringConstant.UserUrl, requestUrl, accessToken, _stringConstant.Bearer);
            var userRoleListAc = JsonConvert.DeserializeObject<List<UserRoleAc>>(response);
            return userRoleListAc;
        }

        /// <summary>
        /// Method to call an api from oAuth server and get whether user is admin or not. - SS
        /// </summary>
        /// <returns>true if user has admin role else false</returns>
        public async Task<bool> CurrentUserIsAdminAsync()
        {
            var accessToken = await GetCurrentUserAcceesToken();
            bool result = false;
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserIsAdmin, (await GetCurrentUserDetails()).Id);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUserUrl, requestUrl, accessToken, _stringConstant.Bearer);
            if (response != null)
            {
                result = JsonConvert.DeserializeObject<bool>(response);
            }
            return result;
        }

        /// <summary>
        /// This method used for get list of user emails based on role. -an
        /// </summary>
        /// <returns>list of teamleader ,managment and employee email</returns>
        public async Task<UserEmailListAc> GetUserEmailListBasedOnRoleAsync()
        {
            var accessToken = await GetCurrentUserAcceesToken();
            UserEmailListAc userEmailListAc = new UserEmailListAc(); 
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUserUrl, _stringConstant.Email, accessToken, _stringConstant.Bearer);
            if (response != null)
                userEmailListAc = JsonConvert.DeserializeObject<UserEmailListAc>(response);
            return userEmailListAc;
        }

        /// <summary>
        /// Method to get list of projects from oauth-server for an user
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>list of project</returns>
        public async Task<List<ProjectAc>> GetListOfProjectsEnrollmentOfUserByUserIdAsync(string userId)
        {
            var accessToken = await GetCurrentUserAcceesToken();
            List<ProjectAc> projects = new List<ProjectAc>();
            var requestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, userId);
            var response = await _httpClientService.GetAsync(_stringConstant.ProjectUrl, requestUrl, accessToken, _stringConstant.Bearer);
            if (response != null)
            {
                projects = JsonConvert.DeserializeObject<List<ProjectAc>>(response);
            }
            return projects;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Method to get current user access token
        /// </summary>
        /// <returns>access token</returns>
        private async Task<string> GetCurrentUserAcceesToken()
        {
            var userName = (await GetCurrentUserDetails()).UserName;
            return (await _attachmentRepository.UserAccessTokenAsync(userName));
        }

        /// <summary>
        /// Method used to get current user details
        /// </summary>
        /// <returns>user details</returns>
        private async Task<ApplicationUser> GetCurrentUserDetails()
        {
            return (await _userManager.FindByIdAsync(GetCurrentClaims().Claims.ToList().Single(x => x.Type == _stringConstant.Sub).Value));
        }

        /// <summary>
        /// Method used to get current list of claims
        /// </summary>
        /// <returns>list of claims</returns>
        private ClaimsIdentity GetCurrentClaims()
        {
            return _httpContextBase.User.Identity as ClaimsIdentity;
        }
        #endregion
    }
}
