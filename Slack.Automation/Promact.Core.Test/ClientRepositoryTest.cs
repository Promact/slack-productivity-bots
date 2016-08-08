using Autofac;
using Newtonsoft.Json;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class ClientRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IClient _clientRepository;
        private readonly ISlackRepository _slackRepository;
        public ClientRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _clientRepository = _componentContext.Resolve<IClient>();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
        }

        /// <summary>
        /// Testing with True Value
        /// </summary>
        [Fact]
        public void WebRequestMethod()
        {
            try
            {
                var attachment = _slackRepository.SlackResponseAttachment(Convert.ToString(30), "Hello");
                var text = new SlashIncomingWebhook() { Channel = "@siddhartha", Username = "LeaveBot", Attachments = attachment };
                var textJson = JsonConvert.SerializeObject(text);
                _clientRepository.WebRequestMethod(textJson, "https://hooks.slack.com/services/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");
            }
            catch(Exception ex)
            {
                throw ex;
            }
            //Assert Required
        }

        /// <summary>
        /// Testing with False Value
        /// </summary>
        [Fact]
        public void WebRequestMethodFalse()
        {
            try
            {
                var attachment = _slackRepository.SlackResponseAttachment(Convert.ToString(30), "Hello");
                var text = "";
                var textJson = JsonConvert.SerializeObject(text);
                _clientRepository.WebRequestMethod(textJson, "https://hooks.slack.com/services/T04K6NL66/B1X804551/FlC6INs0AplQI8At");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Assert Required
        }
    }
}
