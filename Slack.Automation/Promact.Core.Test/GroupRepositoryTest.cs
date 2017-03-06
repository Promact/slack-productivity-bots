using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.GroupRepository;
using Promact.Core.Repository.ServiceRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Promact.Core.Test
{



    public class GroupRepositoryTest
    {
        #region Private Variables

        private readonly IComponentContext _componentContext;
        private readonly IGroupRepository _groupRepository;
        private readonly IStringConstantRepository _stringConstant;
        private readonly Mock<IHttpClientService> _mockHttpClient;
        private readonly ApplicationUserManager _userManager;
        private readonly Mock<HttpContextBase> _mockHttpContextBase;
        private readonly Mock<IServiceRepository> _mockServiceRepository;
        #endregion

        #region Constructor

        public GroupRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _groupRepository = _componentContext.Resolve<IGroupRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _mockHttpContextBase = _componentContext.Resolve<Mock<HttpContextBase>>();
            _mockServiceRepository = _componentContext.Resolve<Mock<IServiceRepository>>();
        }

        #endregion

        #region Test Cases

        /// <summary>
        /// This test case for add group. 
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task AddGroupAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            int id = await CreateGroupAsync();
            var group = await _groupRepository.GetGroupByIdAsync(id);
            Assert.NotNull(group);
        }


        /// <summary>
        /// This test case for update group.
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateGroupAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            int id = await CreateGroupAsync();
            var group = await _groupRepository.GetGroupByIdAsync(id);
            GroupAC newGroupAC = new GroupAC();
            newGroupAC.Name = _stringConstant.NameForTest;
            newGroupAC.Id = group.Id;
            newGroupAC.Type = group.Type;
            List<string> listOfString = new List<string>();
            listOfString.Add(_stringConstant.TestEmail);
            listOfString.Add(_stringConstant.SecondTestEmail);
            newGroupAC.Emails = listOfString;
            int newId = await _groupRepository.UpdateGroupAsync(newGroupAC);
            Assert.Equal(id, newId);
        }

        /// <summary>
        /// This test case for  get group by id.
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task GetGroupByIdAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            int id = await CreateGroupAsync();
            var group = await _groupRepository.GetGroupByIdAsync(id);
            Assert.NotNull(group);
        }

        /// <summary>
        /// This test case used to check exception condition 
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task GetGroupByIdExceptionAsync()
        {
            int id = await CreateGroupAsync();
            Assert.Throws<AggregateException>(() => _groupRepository.GetGroupByIdAsync(2).Result);
        }

        /// <summary>
        /// This test case used for check group name is exists when user add new group 
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task CheckGroupNameIsExistsForAddAsync()
        {
            int id = await CreateGroupAsync();
            bool isExists = await _groupRepository.CheckGroupNameIsExistsAsync(_stringConstant.GroupName, 0);
            Assert.Equal(isExists, true);
        }

        /// <summary>
        /// This test case used for check group name is exists when user update group 
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task CheckGroupNameIsExistsForUpdateAsync()
        {
            int id = await CreateGroupAsync();
            GroupAC group = new GroupAC();
            List<string> listOfString = new List<string>();
            listOfString.Add(_stringConstant.TestEmail);
            listOfString.Add(_stringConstant.SecondTestEmail);
            group.Emails = listOfString;
            group.Name = _stringConstant.NameForTest;
            group.Type = 2;
            int newId = await _groupRepository.AddGroupAsync(group);
            bool isExists = await _groupRepository.CheckGroupNameIsExistsAsync(_stringConstant.GroupName, newId);
            Assert.Equal(isExists, true);
        }

        /// <summary>
        /// This test case used for get list of group async
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task GetListOfGroupACAsync()
        {
            int id = await CreateGroupAsync();
            List<GroupAC> listOfGroupAC = await _groupRepository.GetListOfGroupACAsync();
            Assert.NotEqual(listOfGroupAC.Count, 0);
        }

        /// <summary>
        /// This test case used for delete group by id.
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task DeleteGroupByIdAsync()
        {
            int id = await CreateGroupAsync();
            bool isDeleted = await _groupRepository.DeleteGroupByIdAsync(id);
            Assert.Equal(isDeleted, true);
        }


        /// <summary>
        /// This test case used for add Dynamic Group 
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task AddDynamicGroup()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            await _groupRepository.AddDynamicGroupAsync();
            List<GroupAC> listOfGroupAC = await _groupRepository.GetListOfGroupACAsync();
            Assert.NotEqual(listOfGroupAC.Count, 0);
        }


        /// <summary>
        /// This test case for update Dynamic Group
        /// </summary>
        /// <returns></returns>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateDynamicGroup()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            await _groupRepository.AddDynamicGroupAsync();
            await _groupRepository.AddDynamicGroupAsync();
            List<GroupAC> listOfGroupAC = await _groupRepository.GetListOfGroupACAsync();
            Assert.NotEqual(listOfGroupAC.Count, 0);
        }

        /// <summary>
        /// This test case for update Dynamic Group
        /// </summary>
        /// <returns></returns>
        public async Task GetActiveUserEmailList()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            List<string> listOfActiveEmail = await _groupRepository.GetActiveUserEmailListAsync();
            Assert.NotEqual(listOfActiveEmail.Count, 0);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// This method used for create Group
        /// </summary>
        /// <returns></returns>
        private async Task<int> CreateGroupAsync()
        {
            GroupAC group = new GroupAC();
            List<string> listOfString = new List<string>();
            listOfString.Add(_stringConstant.TestEmail);
            listOfString.Add(_stringConstant.SecondTestEmail);
            group.Emails = listOfString;
            group.Name = _stringConstant.GroupName;
            group.Type = 2;
            return await _groupRepository.AddGroupAsync(group);
        }


        /// </summary>
        /// <returns></returns>
        private async Task CreateUserAndMockingHttpContextToReturnAccessToken()
        {
            var user = new ApplicationUser()
            {
                Id = _stringConstant.StringIdForTest,
                UserName = _stringConstant.EmailForTest,
                Email = _stringConstant.EmailForTest
            };
            await _userManager.CreateAsync(user);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.AddLoginAsync(user.Id, info);
            Claim claim = new Claim(_stringConstant.Sub, _stringConstant.StringIdForTest);
            var mockClaims = new Mock<ClaimsIdentity>();
            IList<Claim> claims = new List<Claim>();
            claims.Add(claim);
            mockClaims.Setup(x => x.Claims).Returns(claims);
            _mockHttpContextBase.Setup(x => x.User.Identity).Returns(mockClaims.Object);
            var accessToken = Task.FromResult(_stringConstant.AccessTokenForTest);
            _mockServiceRepository.Setup(x => x.GerAccessTokenByRefreshToken(It.IsAny<string>())).Returns(accessToken);
            var emailGroupListResponse = Task.FromResult(_stringConstant.EmailListForGroup);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, _stringConstant.Email, _stringConstant.AccessTokenForTest)).Returns(emailGroupListResponse);

        }
        #endregion

    }

}
