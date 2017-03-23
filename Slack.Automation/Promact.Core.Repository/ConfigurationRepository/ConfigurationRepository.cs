using Promact.Core.Repository.AppCredentialRepository;
using Promact.Core.Repository.BotRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ConfigurationRepository
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        #region Private Variables
        private readonly IRepository<Configuration> _configurationDataRepository;
        private readonly IAppCredentialRepository _appCredentialRepository;
        private readonly IStringConstantRepository _stringConstant;
        private readonly ITaskMailBotRepository _taskMailBotRepository;
        private readonly IScrumRepository _scrumRepository;
        #endregion

        #region Constructor
        public ConfigurationRepository(IRepository<Configuration> configurationDataRepository,
            IStringConstantRepository stringConstant, IAppCredentialRepository appCredentialRepository,
            ITaskMailBotRepository taskMailBotRepository, IScrumRepository scrumRepository)
        {
            _configurationDataRepository = configurationDataRepository;
            _stringConstant = stringConstant;
            _appCredentialRepository = appCredentialRepository;
            _taskMailBotRepository = taskMailBotRepository;
            _scrumRepository = scrumRepository;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to update configuration
        /// </summary>
        /// <param name="configuration">configuration</param>
        public async Task UpdateConfigurationAsync(Configuration configuration)
        {
            _configurationDataRepository.Update(configuration);
            await _configurationDataRepository.SaveChangesAsync();
            if (!configuration.Status)
                await StopBotByModuleAsync(configuration.Module);
        }

        /// <summary>
        /// Method to get list of configuration
        /// </summary>
        /// <returns>list of configuration</returns>
        public IEnumerable<Configuration> GetAllConfiguration()
        {
            return _configurationDataRepository.GetAll();
        }

        /// <summary>
        /// Method to get list status of all module
        /// </summary>
        /// <returns>list of status</returns>
        public async Task<ConfigurationStatusAC> GetAllConfigurationStatusAsync()
        {
            ConfigurationStatusAC configStatus = new ConfigurationStatusAC();
            configStatus.LeaveOn = await StatusOfModule(_stringConstant.LeaveModule);
            configStatus.ScrumOn = await StatusOfModule(_stringConstant.Scrum);
            configStatus.TaskOn = await StatusOfModule(_stringConstant.TaskModule);
            return configStatus;
        }

        /// <summary>
        /// Method to get app credentials by configuration Id and update the IsSelected bit to be true
        /// </summary>
        /// <param name="configurationId">setting configuration Id</param>
        /// <returns>app credentials</returns>
        public async Task<AppCredential> GetAppCredentialsByConfigurationIdAsync(int configurationId)
        {
            AppCredential appCredential = new AppCredential();
            Configuration configuration = await _configurationDataRepository.FirstOrDefaultAsync(x => x.Id == configurationId);
            if (configuration != null)
            {
                appCredential = await _appCredentialRepository.FetchAppCredentialByModule(configuration.Module);
                appCredential.IsSelected = true;
                await _appCredentialRepository.AddUpdateAppCredentialAsync(appCredential);
            }
            return appCredential;
        }

        /// <summary>
        /// Method to disable configuration by configuration Id
        /// </summary>
        /// <param name="configurationId">setting configuration Id</param>
        public async Task DisableAppByConfigurationIdAsync(int configurationId)
        {
            var configuration = await _configurationDataRepository.FirstAsync(x => x.Id == configurationId);
            configuration.Status = false;
            await UpdateConfigurationAsync(configuration);
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Method to get status of a module
        /// </summary>
        /// <param name="module">module name</param>
        /// <returns>status</returns>
        private async Task<bool> StatusOfModule(string module)
        {
            return (await _configurationDataRepository.FirstOrDefaultAsync(x => x.Module == module)).Status;
        }

        /// <summary>
        /// Method to Stop bot by module name
        /// </summary>
        /// <param name="module">name of module</param>
        /// <returns></returns>
        private async Task StopBotByModuleAsync(string module)
        {
            if (module == _stringConstant.TaskModule)
            {
                _taskMailBotRepository.TurnOffTaskMailBot();
            }
            if (module == _stringConstant.Scrum)
            {
                _scrumRepository.TurnOffScrumBot();
            }
            await _appCredentialRepository.ClearBotTokenByModule(module);
        }
        #endregion
    }
}
