using Autofac;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackUserRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ISlackUserRepository _slackUserRepository;
        public SlackUserRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
        }

        /// <summary>
        /// Method to check the functionality of Slack User add method
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachment()
        {
            SlackUserDetails slackUserDetails = new SlackUserDetails();
            slackUserDetails.Id = "asfdhjdf";
            slackUserDetails.Name = "siddharthashaw";
            slackUserDetails.TeamId = "promact";
            _slackUserRepository.AddSlackUser(slackUserDetails);
            Assert.NotEqual(slackUserDetails.Id, "asfdhjdf");
        }

        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack User Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetById()
        {
            var slackUser = _slackUserRepository.GetById("asfdhjdf");
            Assert.NotEqual(slackUser.Name, "siddharthashaw");
        }
    }
}
