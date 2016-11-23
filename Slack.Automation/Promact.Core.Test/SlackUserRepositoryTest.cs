using Autofac;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util.StringConstants;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackUserRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IStringConstantRepository _stringConstant;
        private SlackProfile profile = new SlackProfile();
        private SlackUserDetails slackUserDetails = new SlackUserDetails();
        public SlackUserRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            Initialize();
        }

        /// <summary>
        /// Method to check the functionality of Slack User add method for true value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AddSlackUser()
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
            var slackUser = _slackUserRepository.GetById(_stringConstant.StringIdForTest);
            Assert.Equal(slackUser.Name, _stringConstant.FirstNameForTest);
        }

        /// <summary>
        /// Method to check the functionality of Slack User add method for false value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void AddSlackUserFalse()
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
            slackUserDetails.UserId = _stringConstant.SlackChannelIdForTest;
            slackUserDetails.Name = _stringConstant.FalseStringNameForTest;
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var slackUser = _slackUserRepository.GetById(_stringConstant.SlackChannelIdForTest);
            Assert.NotEqual(slackUser.Name, _stringConstant.FirstNameForTest);
        }

        /// <summary>
        /// Test case to check the functionality of GetAllSlackUsers method of Slack User Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetAllSlackUser()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var slackUsers = _slackUserRepository.GetAllSlackUsers();
            Assert.Equal(slackUsers.Count, 1);
        }

        /// <summary>
        /// Test case to check the functionality of GetBySlackName method of Slack User Repository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void GetBySlackName()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var slackUser = _slackUserRepository.GetBySlackName(_stringConstant.FirstNameForTest);
            Assert.Equal(slackUser.UserId, _stringConstant.StringIdForTest);
        }

        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialize()
        {
            profile.Skype = _stringConstant.TestUserId;
            profile.Email = _stringConstant.EmailForTest;
            profile.FirstName = _stringConstant.UserNameForTest;
            profile.LastName = _stringConstant.TestUser;
            profile.Phone = _stringConstant.PhoneForTest;
            profile.Title = _stringConstant.UserNameForTest;

            slackUserDetails.UserId = _stringConstant.StringIdForTest;
            slackUserDetails.Name = _stringConstant.FirstNameForTest;
            slackUserDetails.TeamId = _stringConstant.PromactStringName;
            slackUserDetails.IsBot = false;
            slackUserDetails.Profile = profile;
        }
    }
}
