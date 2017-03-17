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
        private readonly IStringConstantRepository _stringConstant;
        #endregion

        #region Constructor
        public ConfigurationRepository(IRepository<Configuration> configurationDataRepository, IStringConstantRepository stringConstant)
        {
            _configurationDataRepository = configurationDataRepository;
            _stringConstant = stringConstant;
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
            configStatus.ScrumOn = await StatusOfModule(_stringConstant.ScrumModule);
            configStatus.TaskOn = await StatusOfModule(_stringConstant.TaskModule);
            return configStatus;
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
        #endregion
    }
}
