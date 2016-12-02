using Autofac;
using Moq;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Erp.DomainModel.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using Promact.Erp.DomainModel.DataRepository;
using Microsoft.AspNet.Identity;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util.StringConstants;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.HttpClient;

namespace Promact.Core.Test
{
    public class ScrumBotRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly Mock<IHttpClientService> _mockHttpClient;
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<Scrum> _scrumDataRepository;
        private readonly IRepository<ScrumAnswer> _scrumAnswerDataRepository;
        private readonly IScrumBotRepository _scrumBotRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly ISlackChannelRepository _slackChannelReposiroty;
        private readonly IStringConstantRepository _stringConstant;

        private Question question = new Question();
        private ApplicationUser testUser = new ApplicationUser();
        private SlackProfile profile = new SlackProfile();
        private Scrum scrum = new Scrum();
        private SlackUserDetails slackUserDetails = new SlackUserDetails();
        private SlackUserDetails testSlackUserDetails = new SlackUserDetails();
        private SlackChannelDetails slackChannelDetails = new SlackChannelDetails();
        private ApplicationUser user = new ApplicationUser();
        private Question question1 = new Question();
        private ScrumAnswer scrumAnswer = new ScrumAnswer();
        #endregion

        #region Constructor
        public ScrumBotRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumBotRepository = _componentContext.Resolve<IScrumBotRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _scrumDataRepository = _componentContext.Resolve<IRepository<Scrum>>();
            _scrumAnswerDataRepository = _componentContext.Resolve<IRepository<ScrumAnswer>>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _slackChannelReposiroty = _componentContext.Resolve<ISlackChannelRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();

            Initialization();
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialization()
        {
            scrumAnswer.Answer = _stringConstant.NoQuestion;
            scrumAnswer.CreatedOn = DateTime.UtcNow;
            scrumAnswer.AnswerDate = DateTime.UtcNow;
            scrumAnswer.EmployeeId = _stringConstant.UserIdForTest;
            scrumAnswer.Id = 1;
            scrumAnswer.QuestionId = 1;
            scrumAnswer.ScrumId = 1;
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.Answered;

            question1.CreatedOn = DateTime.UtcNow;
            question1.OrderNumber = 2;
            question1.QuestionStatement = _stringConstant.ScrumQuestionForTest;
            question1.Type = 1;

            user.Email = _stringConstant.EmailForTest;
            user.UserName = _stringConstant.EmailForTest;
            user.SlackUserId = _stringConstant.StringIdForTest;

            question.CreatedOn = DateTime.UtcNow;
            question.OrderNumber = 1;
            question.QuestionStatement = _stringConstant.ScrumQuestionForTest;
            question.Type = 1;

            testUser.Email = _stringConstant.EmailForTest;
            testUser.UserName = _stringConstant.EmailForTest;
            testUser.SlackUserId = _stringConstant.IdForTest;

            profile.Skype = _stringConstant.TestUserId;
            profile.Email = _stringConstant.EmailForTest;
            profile.FirstName = _stringConstant.UserNameForTest;
            profile.LastName = _stringConstant.TestUser;
            profile.Phone = _stringConstant.PhoneForTest;
            profile.Title = _stringConstant.UserNameForTest;

            scrum.CreatedOn = DateTime.UtcNow;
            scrum.ProjectId = _stringConstant.ProjectIdForTest;
            scrum.GroupName = _stringConstant.GroupName;
            scrum.ScrumDate = DateTime.UtcNow;
            scrum.TeamLeaderId = _stringConstant.TeamLeaderIdForTest;
            scrum.IsHalted = false;
            scrum.IsOngoing = true;


            slackUserDetails.UserId = _stringConstant.StringIdForTest;
            slackUserDetails.Name = _stringConstant.UserNameForTest;
            slackUserDetails.TeamId = _stringConstant.PromactStringName;
            slackUserDetails.CreatedOn = DateTime.UtcNow;
            slackUserDetails.Deleted = false;
            slackUserDetails.IsAdmin = false;
            slackUserDetails.IsBot = false;
            slackUserDetails.IsPrimaryOwner = false;
            slackUserDetails.IsOwner = false;
            slackUserDetails.IsRestrictedUser = false;
            slackUserDetails.IsUltraRestrictedUser = false;
            slackUserDetails.Profile = profile;
            slackUserDetails.RealName = _stringConstant.UserNameForTest + _stringConstant.TestUser;

            testSlackUserDetails.UserId = _stringConstant.IdForTest;
            testSlackUserDetails.Name = _stringConstant.TestUser;
            testSlackUserDetails.TeamId = _stringConstant.PromactStringName;
            testSlackUserDetails.Profile = profile;

            slackChannelDetails.ChannelId = _stringConstant.SlackChannelIdForTest;
            slackChannelDetails.Deleted = false;
            slackChannelDetails.CreatedOn = DateTime.UtcNow;
            slackChannelDetails.Name = _stringConstant.GroupName;

        }
        #endregion


        #region Test Cases


        #region Scrum Test Cases 


        #region Start Scrum


        /// <summary>
        /// Method StartScrum Testing with existing scrum but no scrum answer given yet
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateHasScrumNoAnswer()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _botQuestionRepository.AddQuestionAsync(question);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.GoodDay + "<@" + _stringConstant.UserNameForTest + ">!\n" + _stringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// No slack user found testing
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateNoSlackUser()
        {
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.SlackUserNotFound);
        }


        /// <summary>
        /// No slack channel found testing
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateNoSlackChannel()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.ChannelAddInstruction);
        }


        /// <summary>
        /// Method StartScrum Testing without registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumNoUser()
        {
            await AddChannelUserAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.YouAreNotInExistInOAuthServer);
        }
        

        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateNoQuestion()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.NoQuestion);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateNoProject()
        {
            slackChannelDetails.Name = _stringConstant.UserNameForTest;
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.NoProjectFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with No Employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateNoEmployee()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
            var msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.NoEmployeeFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiate()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.GoodDay + "<@" + _stringConstant.UserNameForTest + ">!\n" + _stringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with first employee inactive
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateFirstEmployeeInActive()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(_stringConstant.InValidOAuthUsers);
            string userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.NoEmployeeFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateHasScrum()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrum.ScrumDate = DateTime.UtcNow.Date.AddDays(-1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            scrumAnswer.AnswerDate = DateTime.UtcNow.Date.AddDays(-1);
            scrumAnswer.ScrumId = 2;
            scrumAnswer.EmployeeId = _stringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            string compareString = string.Format(_stringConstant.TestQuestion, DateTime.UtcNow.Date.AddDays(-1).ToShortDateString());
            Assert.Equal(compareString, msg);
        }



        /// <summary>
        /// Method StartScrum Testing with existing scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateHasScrumQuestionRemaining()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            Question question1 = new Question
            {
                CreatedOn = DateTime.UtcNow,
                OrderNumber = 2,
                QuestionStatement = _stringConstant.ScrumQuestionForTest,
                Type = 1
            };
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal("<@" + _stringConstant.UserNameForTest + "> " + _stringConstant.ScrumQuestionForTest, msg);
        }


        #endregion


        #region Halt Scrum


        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHalt()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumHalted, msg);
        }


        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHaltScrumComplete()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);
            scrum.IsOngoing = false;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumAlreadyConducted + _stringConstant.ScrumCannotBeHalted, msg);
        }


        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHaltNoScrumToday()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);
            scrum.ScrumDate = DateTime.Today.AddDays(-2);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumNotStarted + _stringConstant.ScrumCannotBeHalted, msg);
        }




        /// <summary>
        /// Method Scrum Testing for already halted
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAlreadyHalted()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);
            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumAlreadyHalted, msg);
        }



        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHaltNoScrum()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumNotStarted + _stringConstant.ScrumCannotBeHalted, msg);
        }


        #endregion


        #region Resume Scrum


        /// <summary>
        /// Method Scrum Testing for resume (getquestion)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumResume()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume);
            string expectedString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.UserNameForTest);
            Assert.Equal(_stringConstant.ScrumResumed + expectedString, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume without a scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumResumeNoScrum()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume);
            Assert.Equal(_stringConstant.NoProjectFound + _stringConstant.ScrumCannotBeResumed, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume for unhalted scrum (getquestion)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumResumeNotHalted()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);

            scrum.IsHalted = false;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string compareString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.UserNameForTest);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume);
            Assert.Equal(_stringConstant.ScrumNotHalted + compareString, msg);
        }


        #endregion


        #endregion


        #region Leave Test Cases 


        /// <summary>
        /// Method Leave Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveScrumHaltedProcessMessage()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.ScrumIsHalted, msg);
        }


        /// <summary>
        /// Method Leave Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task NotUserProcessMessage()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.Scrum + ">");
            Assert.Equal(_stringConstant.NotAUser, msg);
        }


        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task Leave()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            string usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);


            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(Environment.NewLine + _stringConstant.GoodDay + "<@" + _stringConstant.TestUser + ">!\n" + _stringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveNoProject()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            string usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.NoProjectFound, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant and applying employee are same
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveWithSameApplicant()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.LeaveError, msg);
        }


        /// <summary>
        /// Method Leave Testing with no scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveNoScrum()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.ScrumNotStarted, msg);
        }


        /// <summary>
        /// Method Leave Testing (where leave is applied on a group where scrum is already complete)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveScrumAlreadyComplete()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            scrum.IsOngoing = false;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.ScrumAlreadyConducted, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant has already answered a question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveScrumWithAnswer()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            testSlackUserDetails.UserId = _stringConstant.IdForTest;
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);
            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.EmployeeId = _stringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.IdForTest + ">");
            string compareString = string.Format(_stringConstant.AlreadyAnswered, _stringConstant.TestUser);
            Assert.Equal(compareString + Environment.NewLine + _stringConstant.NextQuestion, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant has already answered a question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveScrumWithInActiveApplicant()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            testSlackUserDetails.UserId = _stringConstant.IdForTest;
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(_stringConstant.EmployeesListFromOauthInValid);
            string userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.EmployeeId = _stringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.IdForTest + ">");
            string compareString = string.Format(_stringConstant.InActiveInOAuth, _stringConstant.TestUser) + Environment.NewLine + Environment.NewLine + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.UserNameForTest);
            Assert.Equal(compareString, msg);
        }



        /// <summary>
        /// Method Leave Testing where applicant has already answered a question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplicantNotExpected()
        {
            await AddChannelUserAsync();

            var userResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.EmployeeId = _stringConstant.StringIdForTest;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            string compareString = string.Format(_stringConstant.NotExpected, _stringConstant.UserNameForTest);
            Assert.Equal(compareString, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant is not in OAuth
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveNotInOAuth()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.YouAreNotInExistInOAuthServer, msg);
        }


        #endregion


        #region AddScrumAnswer Test Cases 



        /// <summary>
        /// Method AddScrumAnswer Testing without registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerNoUser()
        {
            await AddChannelUserAsync();
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with first employee's first answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerFirstEmployeeFirstAnswer()
        {
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();

            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, "<@" + _stringConstant.UserNameForTest + "> " + _stringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with first employee's first answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerNoQuestion()
        {
            await UserProjectSetup();
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.NoQuestion);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next answer of an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerNextAnswer()
        {
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question1);

            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            string compareString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing (update answer) with only One Answer marked to be asked later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateScrumAnswer()
        {
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            string expectedMsg = Environment.NewLine + string.Format(_stringConstant.PleaseAnswer, _stringConstant.TestUser);
            Assert.Equal(msg, expectedMsg);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next employee's first answer and scrum complete
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerScrumComplete()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            string usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.ScrumComplete);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next question which is to be answered now.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerNow()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            string usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            string compareString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next question which is to be answered now.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerNowQuestions()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            string usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            string compareString = "<@" + _stringConstant.UserNameForTest + "> " + _stringConstant.ScrumQuestionForTest;
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next question which is to be answered now.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerNowInValid()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauthInValid);
            string usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.UnExpectedInActiveUser);
        }


        /// <summary>
        /// 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInValidUser()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauthInValid);
            string usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.Answered;
            scrumAnswer.EmployeeId = _stringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.UnExpectedInActiveUser);
        }


        /// <summary>
        /// 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerInValidUser()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauthInValid);
            string usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            var oauthUser = Task.FromResult(_stringConstant.OAuthUserDetails);
            string requestUrl = string.Format("{0}{1}", _stringConstant.UserDetailUrl, _stringConstant.TestUserId);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(oauthUser);

            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question);

            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();

            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            scrumAnswer.EmployeeId = _stringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, string.Format(_stringConstant.InActiveInOAuth, _stringConstant.TestUser) + Environment.NewLine + string.Format(_stringConstant.MarkedInActive, _stringConstant.TestUser) + _stringConstant.ScrumQuestionForTest);
        }



        /// <summary>
        /// Method AddScrumAnswer Testing with next question which is to be answered now.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumWithInActiveUser()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauthInValid);
            string usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.ScrumComplete);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with unexpected employee's answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumUnexpectedEmployeeAnswer()
        {
            await UserProjectSetup();
            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            string expectedString = Environment.NewLine + string.Format(_stringConstant.PleaseAnswer, _stringConstant.TestUser);
            Assert.Equal(msg, expectedString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with unexpected employee's answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumExpectedEmployeeAnswer()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(_stringConstant.EmployeesListInValid);
            string userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            string expectedString = string.Format(_stringConstant.InActiveInOAuth, _stringConstant.UserNameForTest) + Environment.NewLine + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser);
            Assert.Equal(msg, expectedString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with normal conversation on slack channel
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task NormalConversation()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await AddChannelUserAsync();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, string.Empty);
        }


        #endregion


        #region ProcessMessagesAsync Test Cases


        /// <summary>
        /// Method ProcessMessagesAsync Testing with scrum help
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerProcess()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.GroupName, _stringConstant.ScrumHelp);
            Assert.Equal(msg, _stringConstant.ScrumHelpMessage);
        }


        /// <summary>
        /// Method Scrum Testing for in active project
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ProjectInActive()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            string userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.InActiveProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);

            scrum.IsHalted = false;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string compareString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.UserNameForTest);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume);
            Assert.Equal(_stringConstant.ProjectInActive + _stringConstant.ScrumCannotBeResumed, msg);
        }


        /// <summary>
        /// Method ProcessMessagesAsync Testing with not registered channel
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerProcessNoChannel()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.GroupName, _stringConstant.GroupDetailsResponseText);
            Assert.Equal(msg, string.Empty);
        }


        #endregion


        #region AddChannel Test Cases


        /// <summary>
        /// Method AddChannelManually Testing
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddChannel()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            await UserProjectSetup();

            string msg = await _scrumBotRepository.ProcessMessagesAsync(slackUserDetails.UserId, _stringConstant.GroupNameStartsWith + slackChannelDetails.ChannelId, _stringConstant.Add + " " + _stringConstant.Channel + " " + slackChannelDetails.Name);
            Assert.Equal(msg, _stringConstant.ChannelAddSuccess);
        }


        /// <summary>
        /// Method AddChannelManually but project inactive
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddChannelInActiveProject()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            string userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.InActiveProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(slackUserDetails.UserId, _stringConstant.GroupNameStartsWith + slackChannelDetails.ChannelId, _stringConstant.Add + " " + _stringConstant.Channel + " " + slackChannelDetails.Name);
            Assert.Equal(msg, _stringConstant.ProjectInActive);
        }


        /// <summary>
        /// Method AddChannelManually Testing with Not Privaet Group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddChannelNotPrivateGroup()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(slackUserDetails.UserId, _stringConstant.GroupName, _stringConstant.Add + " " + _stringConstant.Channel + " " + _stringConstant.GroupName);
            Assert.Equal(msg, _stringConstant.OnlyPrivateChannel);
        }


        /// <summary>
        /// Method AddChannelManually Testing with No User
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddChannelNoUser()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(slackUserDetails.UserId, _stringConstant.GroupNameStartsWith + slackChannelDetails.ChannelId, _stringConstant.Add + " " + _stringConstant.Channel + " " + _stringConstant.GroupName);
            Assert.Equal(msg, _stringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// Method AddChannelManually Testing with no project added
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddChannelNoProject()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            string msg = await _scrumBotRepository.ProcessMessagesAsync(slackUserDetails.UserId, _stringConstant.GroupNameStartsWith + slackChannelDetails.ChannelId, _stringConstant.Add + " " + _stringConstant.Channel + " " + _stringConstant.GroupName);
            Assert.Equal(msg, _stringConstant.ProjectNotInOAuth);
        }


        #endregion


        #endregion


        private async Task AddChannelUserAsync()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);
        }


        private async Task UserProjectSetup()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            string userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format("{0}", _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

        }
        #endregion
    }
}