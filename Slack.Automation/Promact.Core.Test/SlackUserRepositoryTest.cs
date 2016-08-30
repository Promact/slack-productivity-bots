using Autofac;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util;
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
            _slackUserRepository.AddSlackUser(slackUserDetails);
            Assert.Equal(slackUserDetails.Id, StringConstant.StringIdForTest);
        }

        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack User Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetById()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var slackUser = _slackUserRepository.GetById(StringConstant.StringIdForTest);
            Assert.Equal(slackUser.Name, StringConstant.FirstNameForTest);
        }

        private SlackUserDetails slackUserDetails = new SlackUserDetails()
        {
            Id = StringConstant.StringIdForTest,
            Name = StringConstant.FirstNameForTest,
            TeamId = StringConstant.PromactStringName
        };
    }
}
