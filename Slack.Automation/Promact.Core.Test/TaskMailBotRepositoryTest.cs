using Autofac;
using Moq;
using Promact.Core.Repository.BotRepository;
using Promact.Erp.Util.StringConstants;
using SlackAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class TaskMailBotRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly ITaskMailBotRepository _taskMailBotRepository;
        private readonly Mock<ISocketClientWrapper> _mockSocketWrapper;
        private readonly IStringConstantRepository _stringConstant;
        #endregion

        #region Constructor
        public TaskMailBotRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _taskMailBotRepository = _componentContext.Resolve<ITaskMailBotRepository>();
            _mockSocketWrapper = _componentContext.Resolve<Mock<ISocketClientWrapper>>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
        }
        #endregion

        #region Test Cases
        [Fact, Trait("Category", "Required")]
        public void TaskMail()
        {
            SlackSocketClient taskMailClient = new SlackSocketClient(_stringConstant.AccessTokenForTest);
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialization
        /// </summary>
        public void Initialize()
        {
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
