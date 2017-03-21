using Autofac;
using Promact.Core.Repository.ConfigurationRepository;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class ConfigurationRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IRepository<Configuration> _configurationDataRepository;
        private readonly IRepository<AppCredential> _appCredentialDataRepository;
        private Configuration taskConfiguration = new Configuration();
        private Configuration leaveConfiguration = new Configuration();
        private Configuration scrumConfiguration = new Configuration();
        #endregion

        #region Constructor
        public ConfigurationRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _configurationRepository = _componentContext.Resolve<IConfigurationRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _configurationDataRepository = _componentContext.Resolve<IRepository<Configuration>>();
            _appCredentialDataRepository = _componentContext.Resolve<IRepository<AppCredential>>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test case to test the method UpdateConfigurationAsync of ConfigurationRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateConfigurationAsync()
        {
            await AddConfigurationAsync();
            var updateConfiguration = await _configurationDataRepository.FirstOrDefaultAsync(x => x.Module == _stringConstant.TaskModule);
            Assert.False(updateConfiguration.Status);
            updateConfiguration.Status = true;
            await _configurationRepository.UpdateConfigurationAsync(updateConfiguration);
            var recentConfiguration = await _configurationDataRepository.FirstOrDefaultAsync(x => x.Module == _stringConstant.TaskModule);
            Assert.True(recentConfiguration.Status);
        }

        /// <summary>
        /// Test case to test the method GetAllConfiguration of ConfigurationRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetAllConfigurationAsync()
        {
            await AddConfigurationAsync();
            var configurations = _configurationRepository.GetAllConfiguration();
            Assert.Equal(configurations.Count(), 3);
        }

        /// <summary>
        /// Test case to test the method GetAllConfigurationStatusAsync of ConfigurationRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetAllConfigurationStatusAsync()
        {
            taskConfiguration.Status = true;
            await AddConfigurationAsync();
            var configurations = await _configurationRepository.GetAllConfigurationStatusAsync();
            Assert.True(configurations.TaskOn);
            Assert.False(configurations.ScrumOn);
            Assert.False(configurations.LeaveOn);
        }

        /// <summary>
        /// Test case to test the method GetAppCredentialsByConfigurationIdAsync of ConfigurationRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetAppCredentialsByConfigurationIdAsync()
        {
            await AddConfigurationAsync();
            AppCredential appCredential = new AppCredential() { ClientId = _stringConstant.StringIdForTest, ClientSecret = _stringConstant.StringIdForTest, CreatedOn = DateTime.UtcNow, Module = _stringConstant.TaskModule };
            _appCredentialDataRepository.Insert(appCredential);
            await _appCredentialDataRepository.SaveChangesAsync();
            var configurationId = (await _configurationDataRepository.FirstAsync(x => x.Module == _stringConstant.TaskModule)).Id;
            var configuration = await _configurationRepository.GetAppCredentialsByConfigurationIdAsync(configurationId);
            Assert.Equal(configuration.ClientId, _stringConstant.StringIdForTest);
        }

        /// <summary>
        /// Test case to test the method DisableAppByConfigurationIdAsync of ConfigurationRepository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task DisableAppByConfigurationIdAsync()
        {
            await AddConfigurationAsync();
            var configurationId = (await _configurationDataRepository.FirstAsync(x => x.Module == _stringConstant.TaskModule)).Id;
            await _configurationRepository.DisableAppByConfigurationIdAsync(configurationId);
            var configuration = await _configurationDataRepository.FirstAsync(x=>x.Id == configurationId);
            Assert.False(configuration.Status);
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialization
        /// </summary>
        public void Initialize()
        {
            taskConfiguration.CreatedOn = DateTime.UtcNow;
            taskConfiguration.Module = _stringConstant.TaskModule;
            taskConfiguration.Status = false;
            leaveConfiguration.CreatedOn = DateTime.UtcNow;
            leaveConfiguration.Module = _stringConstant.LeaveModule;
            leaveConfiguration.Status = false;
            scrumConfiguration.CreatedOn = DateTime.UtcNow;
            scrumConfiguration.Module = _stringConstant.Scrum;
            scrumConfiguration.Status = false;
        }
        #endregion

        #region Private Method
        private async Task AddConfigurationAsync()
        {
            _configurationDataRepository.Insert(taskConfiguration);
            _configurationDataRepository.Insert(scrumConfiguration);
            _configurationDataRepository.Insert(leaveConfiguration);
            await _configurationDataRepository.SaveChangesAsync();
        }
        #endregion
    }
}
