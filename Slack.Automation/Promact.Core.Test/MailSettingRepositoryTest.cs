using Autofac;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.MailSettingRepository;
using Promact.Core.Repository.ServiceRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Promact.Core.Test
{
    public class MailSettingRepositoryTest
    {
        #region Private Variable
        private IComponentContext _componentContext;
        private readonly IMailSettingRepository _mailSettingRepository;
        private readonly IStringConstantRepository _stringConstant;
        private readonly ApplicationUserManager _userManager;
        private readonly Mock<HttpContextBase> _mockHttpContextBase;
        private readonly Mock<IServiceRepository> _mockServiceRepository;
        private readonly Mock<IHttpClientService> _mockHttpClientService;
        private MailSettingAC mailSetting = new MailSettingAC();
        private Group managementGroup = new Group();
        private Group teamLeaderGroup = new Group();
        private readonly IRepository<Group> _groupDataRepository;
        private readonly IRepository<MailSetting> _mailSettingDataRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public MailSettingRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _mailSettingRepository = _componentContext.Resolve<IMailSettingRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _mockHttpContextBase = _componentContext.Resolve<Mock<HttpContextBase>>();
            _mockServiceRepository = _componentContext.Resolve<Mock<IServiceRepository>>();
            _mockHttpClientService = _componentContext.Resolve<Mock<IHttpClientService>>();
            _groupDataRepository = _componentContext.Resolve<IRepository<Group>>();
            _mailSettingDataRepository = _componentContext.Resolve<IRepository<MailSetting>>();
            _mapper = _componentContext.Resolve<IMapper>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test case to test the method GetAllProjectAsync of MailSettingRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetAllProjectAsync()
        {
            await CreateUserAndMockingHttpContextToReturnAccessToken();
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForAdminFromOauth);
            var requestUrl = _stringConstant.AllProjectUrl;
            _mockHttpClientService.Setup(x=>x.GetAsync(_stringConstant.ProjectUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(responseProjects);
            var result = await _mailSettingRepository.GetAllProjectAsync();
            Assert.True(result.Any());
        }

        /// <summary>
        /// Test case to test the method GetMailSettingDetailsByProjectIdAsync of MailSettingRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingDetailsByProjectIdAsync()
        {
            _groupDataRepository.Insert(teamLeaderGroup);
            _groupDataRepository.Insert(managementGroup);
            await _groupDataRepository.SaveChangesAsync();
            await _mailSettingRepository.AddMailSettingAsync(mailSetting);
            var result = await _mailSettingRepository.GetMailSettingDetailsByProjectIdAsync(1,_stringConstant.LeaveModule);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test case to test the method AddMailSettingAsync of MailSettingRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddMailSettingAsync()
        {
            await _mailSettingRepository.AddMailSettingAsync(mailSetting);
            var result = await _mailSettingDataRepository.FirstOrDefaultAsync(x=>x.ProjectId == 1);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test case to test the method GetListOfGroupsAsync of MailSettingRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetListOfGroupsAsync()
        {
            _groupDataRepository.Insert(teamLeaderGroup);
            _groupDataRepository.Insert(managementGroup);
            await _groupDataRepository.SaveChangesAsync();
            var result = await _mailSettingRepository.GetListOfGroupsAsync();
            Assert.True(result.Any());
        }

        /// <summary>
        /// Test case to test the method UpdateMailSettingAsync of MailSettingRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateMailSettingAsync()
        {
            _groupDataRepository.Insert(teamLeaderGroup);
            _groupDataRepository.Insert(managementGroup);
            await _groupDataRepository.SaveChangesAsync();
            await _mailSettingRepository.AddMailSettingAsync(mailSetting);
            var previousMailSetting = await _mailSettingDataRepository.FirstOrDefaultAsync(x => x.ProjectId == 1);
            var mailSettingToUpdate = _mapper.Map<MailSetting, MailSettingAC>(previousMailSetting);
            mailSettingToUpdate.SendMail = false;
            mailSettingToUpdate.CC = new List<string>() { _stringConstant.EmailForTest, _stringConstant.Management };
            mailSettingToUpdate.To = new List<string>() { _stringConstant.TeamLeader, _stringConstant.TeamLeaderEmailForTest };
            await _mailSettingRepository.UpdateMailSettingAsync(mailSettingToUpdate);
            var result = await _mailSettingDataRepository.FirstOrDefaultAsync(x => x.ProjectId == 1);
            Assert.Equal(result.Id ,previousMailSetting.Id);
            Assert.False(result.SendMail);
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Private method to create a user add login info and mocking of Identity and return access token
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
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialization
        /// </summary>
        public void Initialize()
        {
            mailSetting.Module = _stringConstant.LeaveModule;
            mailSetting.SendMail = true;
            mailSetting.ProjectId = 1;
            mailSetting.CC = new List<string>() { _stringConstant.EmailForTest, _stringConstant.Management };
            mailSetting.To = new List<string>() { _stringConstant.TeamLeader, _stringConstant.TeamLeaderEmailForTest };
            managementGroup.CreatedOn = DateTime.UtcNow;
            managementGroup.Name = _stringConstant.Management;
            managementGroup.Type = 1;
            teamLeaderGroup.CreatedOn = DateTime.UtcNow;
            teamLeaderGroup.Name = _stringConstant.TeamLeader;
            teamLeaderGroup.Type = 1;
        }
        #endregion
    }
}
