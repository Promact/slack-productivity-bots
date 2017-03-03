using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.MailSettingDetailsByProjectAndModule;
using Promact.Core.Repository.ServiceRepository;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Promact.Core.Test
{
    public class MailSettingDetailsByProjectAndModuleTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IMailSettingDetailsByProjectAndModuleRepository _mailSettingDetailsRepository;
        private readonly IRepository<MailSetting> _mailSettingDataRepository;
        private readonly IRepository<MailSettingMapping> _mailSettingMappingDataRepository;
        private readonly IRepository<Group> _groupDataRepository;
        private readonly IRepository<GroupEmailMapping> _groupEmailMappingDataRepository;
        private readonly IStringConstantRepository _stringConstant;
        private readonly ApplicationUserManager _userManager;
        private readonly Mock<HttpContextBase> _mockHttpContextBase;
        private readonly Mock<IServiceRepository> _mockServiceRepository;
        MailSetting mailSetting = new MailSetting();
        MailSettingMapping mailSettingMapping = new MailSettingMapping();
        Group group = new Group();
        GroupEmailMapping groupEmailMapping = new GroupEmailMapping();
        #endregion

        #region Constructor
        public MailSettingDetailsByProjectAndModuleTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _mailSettingDataRepository = _componentContext.Resolve<IRepository<MailSetting>>();
            _mailSettingMappingDataRepository = _componentContext.Resolve<IRepository<MailSettingMapping>>();
            _groupDataRepository = _componentContext.Resolve<IRepository<Group>>();
            _groupEmailMappingDataRepository = _componentContext.Resolve<IRepository<GroupEmailMapping>>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _mailSettingDetailsRepository = _componentContext.Resolve<IMailSettingDetailsByProjectAndModuleRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _mockHttpContextBase = _componentContext.Resolve<Mock<HttpContextBase>>();
            _mockServiceRepository = _componentContext.Resolve<Mock<IServiceRepository>>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test for isTo true with static email Id
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingEmailIsToAsync()
        {
            await AddGroup();
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSettingAsync(1, _stringConstant.TaskModule, _stringConstant.StringIdForTest);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test for isTo true with group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingForGroupIsToAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessTokenAsync();
            await AddGroup();
            mailSettingMapping.Email = null;
            mailSettingMapping.GroupId = group.Id;
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSettingAsync(1, _stringConstant.TaskModule, _stringConstant.StringIdForTest);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test for isTo true with static email Id
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingEmailIsToFalseAsync()
        {
            await AddGroup();
            mailSettingMapping.IsTo = false;
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSettingAsync(1, _stringConstant.TaskModule, _stringConstant.StringIdForTest);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test for isTo true with group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingForGroupIsToFalseAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessTokenAsync();
            await AddGroup();
            mailSettingMapping.IsTo = false;
            mailSettingMapping.Email = null;
            mailSettingMapping.GroupId = group.Id;
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSettingAsync(1, _stringConstant.TaskModule, _stringConstant.StringIdForTest);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test for isTo true with group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingForGroupIsToManagementAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessTokenAsync();
            group.Name = _stringConstant.Management;
            await AddGroup();
            mailSettingMapping.Email = null;
            mailSettingMapping.GroupId = group.Id;
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSettingAsync(1, _stringConstant.TaskModule, _stringConstant.StringIdForTest);
            Assert.True(result.SendMail);
        }


        /// <summary>
        /// Test for isTo true with group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingForGroupIsToTeamMemberAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessTokenAsync();
            group.Name = "Team Members";
            await AddGroup();
            mailSettingMapping.Email = null;
            mailSettingMapping.GroupId = group.Id;
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSettingAsync(1, _stringConstant.TaskModule, _stringConstant.StringIdForTest);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test for isTo true with group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingForGroupIsToDefaultAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessTokenAsync();
            group.Name = _stringConstant.Email;
            await AddGroup();
            mailSettingMapping.Email = null;
            mailSettingMapping.GroupId = group.Id;
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSettingAsync(1, _stringConstant.TaskModule, _stringConstant.StringIdForTest);
            Assert.True(result.SendMail);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialization
        /// </summary>
        private void Initialize()
        {
            mailSetting.CreatedOn = DateTime.UtcNow;
            mailSetting.Module = _stringConstant.TaskModule;
            mailSetting.ProjectId = 1;
            mailSetting.SendMail = true;
            mailSettingMapping.CreatedOn = DateTime.UtcNow;
            mailSettingMapping.Email = _stringConstant.EmailForTest;
            mailSettingMapping.IsTo = true;
            group.CreatedOn = DateTime.UtcNow;
            group.Name = "Team Leader";
            group.Type = 1;
            groupEmailMapping.CreatedOn = DateTime.UtcNow;
            groupEmailMapping.Email = _stringConstant.Email;
        }

        /// <summary>
        /// Method to add mail setting
        /// </summary>
        private async Task AddMailSetting()
        {
            _mailSettingDataRepository.Insert(mailSetting);
            await _mailSettingDataRepository.SaveChangesAsync();
            mailSettingMapping.MailSettingId = mailSetting.Id;
            _mailSettingMappingDataRepository.Insert(mailSettingMapping);
            await _mailSettingMappingDataRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Method to add group
        /// </summary>
        private async Task AddGroup()
        {
            _groupDataRepository.Insert(group);
            await _groupDataRepository.SaveChangesAsync();
            groupEmailMapping.GroupId = group.Id;
            _groupEmailMappingDataRepository.Insert(groupEmailMapping);
            await _groupEmailMappingDataRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Private method to create a user add login info and mocking of Identity and return access token
        /// </summary>
        private async Task CreateUserAndMockingHttpContextToReturnAccessTokenAsync()
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
        }
        #endregion
    }
}
