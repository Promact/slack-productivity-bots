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
        /// Method to check the functionality of Slack User add method for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachment()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            Assert.Equal(slackUserDetails.Id, 1);
        }

        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack User Repository for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetById()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var slackUser = _slackUserRepository.GetById(StringConstant.StringIdForTest);
            Assert.Equal(slackUser.Name, StringConstant.FirstNameForTest);
        }

        /// <summary>
        /// Method to check the functionality of Slack User add method for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachmentFalse()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            Assert.NotEqual(slackUserDetails.Id, 3);
        }

        /// <summary>
        /// Test case to check the functionality of GetbyId method of Slack User Repository for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetByIdFalse()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            slackUserDetails.UserId = StringConstant.SlackChannelIdForTest;
            slackUserDetails.Name = StringConstant.FalseStringNameForTest;
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var slackUser = _slackUserRepository.GetById(StringConstant.SlackChannelIdForTest);
            Assert.NotEqual(slackUser.Name, StringConstant.FirstNameForTest);
        }

        private SlackUserDetails slackUserDetails = new SlackUserDetails()
        {
            UserId = StringConstant.StringIdForTest,
            Name = StringConstant.FirstNameForTest,
            TeamId = StringConstant.PromactStringName
        };
    }
}
