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
using Promact.Core.Repository.ServiceRepository;
using Promact.Core.Repository.ScrumSetUpRepository;

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
        private readonly IRepository<TemporaryScrumDetails> _temporaryScrumDetailsRepository;
        private readonly IScrumBotRepository _scrumBotRepository;
        private readonly IScrumSetUpRepository _scrumSetUpRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly ISlackChannelRepository _slackChannelReposiroty;
        private readonly IStringConstantRepository _stringConstant;
        private readonly Mock<IServiceRepository> _mockServiceRepository;

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
        private TemporaryScrumDetails temporaryScrumDetails = new TemporaryScrumDetails();


        #endregion


        #region Constructor


        public ScrumBotRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumBotRepository = _componentContext.Resolve<IScrumBotRepository>();
            _scrumSetUpRepository = _componentContext.Resolve<IScrumSetUpRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _scrumDataRepository = _componentContext.Resolve<IRepository<Scrum>>();
            _scrumAnswerDataRepository = _componentContext.Resolve<IRepository<ScrumAnswer>>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _slackChannelReposiroty = _componentContext.Resolve<ISlackChannelRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _temporaryScrumDetailsRepository = _componentContext.Resolve<IRepository<TemporaryScrumDetails>>();
            _mockServiceRepository = _componentContext.Resolve<Mock<IServiceRepository>>();
            Initialization();

        }
        #endregion


        #region Initialization


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
            question1.OrderNumber = QuestionOrder.Today;
            question1.QuestionStatement = _stringConstant.ScrumQuestionForTest;
            question1.Type = BotQuestionType.Scrum;

            user.Id = _stringConstant.StringIdForTest;
            user.Email = _stringConstant.EmailForTest;
            user.UserName = _stringConstant.EmailForTest;
            user.SlackUserId = _stringConstant.StringIdForTest;

            question.CreatedOn = DateTime.UtcNow;
            question.OrderNumber = QuestionOrder.Yesterday;
            question.QuestionStatement = _stringConstant.ScrumQuestionForTest;
            question.Type = BotQuestionType.Scrum;

            testUser.Id = _stringConstant.IdForTest;
            testUser.Email = _stringConstant.Email;
            testUser.UserName = _stringConstant.Email;
            testUser.SlackUserId = _stringConstant.IdForTest;

            profile.Skype = _stringConstant.TestUserId;
            profile.Email = _stringConstant.EmailForTest;
            profile.FirstName = _stringConstant.UserNameForTest;
            profile.LastName = _stringConstant.TestUser;
            profile.Phone = _stringConstant.PhoneForTest;
            profile.Title = _stringConstant.UserNameForTest;

            scrum.CreatedOn = DateTime.UtcNow;
            scrum.ProjectId = 1;
            scrum.ScrumDate = DateTime.UtcNow;
            scrum.TeamLeaderId = _stringConstant.TeamLeaderIdForTest;
            scrum.IsHalted = false;
            scrum.IsOngoing = true;

            slackUserDetails.UserId = _stringConstant.StringIdForTest;
            slackUserDetails.Name = _stringConstant.TestUser;
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
            slackUserDetails.RealName = _stringConstant.TestUser + _stringConstant.TestUser;

            testSlackUserDetails.UserId = _stringConstant.IdForTest;
            testSlackUserDetails.Name = _stringConstant.UserNameForTest;
            testSlackUserDetails.TeamId = _stringConstant.PromactStringName;
            testSlackUserDetails.Profile = profile;

            slackChannelDetails.ChannelId = _stringConstant.SlackChannelIdForTest;
            slackChannelDetails.Deleted = false;
            slackChannelDetails.CreatedOn = DateTime.UtcNow;
            slackChannelDetails.Name = _stringConstant.GroupName;
            slackChannelDetails.ProjectId = 1;

            temporaryScrumDetails.ScrumId = 1;
            temporaryScrumDetails.CreatedOn = DateTime.UtcNow;

            var accessTokenForTest = Task.FromResult(_stringConstant.AccessTokenForTest);
            _mockServiceRepository.Setup(x => x.GerAccessTokenByRefreshToken(_stringConstant.AccessTokenForTest)).Returns(accessTokenForTest);
        }


        private async Task AddChannelUserAsync()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);
        }

        private async Task InActiveUserProjectSetup()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInValidUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        }

        private async Task UserProjectSetup()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        }


        #endregion


        #region Test Cases


        #region Scrum Test Cases 


        #region Start Scrum


        /// <summary>
        /// Method to test user whose slack credentials are not available
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateNotUser()
        {
            await AddChannelUserAsync();
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.SlackChannelIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.SlackUserNotFound, actualString);
        }


        /// <summary>
        /// Method to test user whose slack credentials are not available
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumLeaveNotUser()
        {
            await AddChannelUserAsync();
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.SlackChannelIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.LeaveCommand, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.SlackUserNotFound, actualString);
        }


        /// <summary>
        /// Method to test user whose slack credentials are not available
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumLeaveNotUserInValidCommand()
        {
            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.SlackChannelIdForTest, _stringConstant.SlackChannelIdForTest, "leave <@", _stringConstant.ScrumBotName);
            Assert.Equal(string.Empty, actualString);
        }


        /// <summary>
        /// No slack user found testing
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumNoUser()
        {
            await AddChannelUserAsync();
            var msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            Assert.Equal(msg, _stringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// No slack channel found testing
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateNoSlackChannel()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            Assert.Equal(msg, _stringConstant.ChannelAddInstruction);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateNoQuestion()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.NoQuestion, actual);
        }


        /// <summary>
        /// Method StartScrum Testing when scrum is completed
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateScrumComplete()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            scrum.IsOngoing = false;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ScrumAlreadyConducted, actual);
        }


        /// <summary>
        /// Method StartScrum Testing when scrum is completed
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateScrumHalted()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ScrumIsHalted, actual);
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

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            string expectedString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expectedString, actualString);
        }


        /// <summary>
        /// Method StartScrum Testing with In Valid Start Command
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumStartInValidStartCommand()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Start + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            string expectedString = string.Format(_stringConstant.InValidStartCommand, _stringConstant.ScrumBotName);
            Assert.Equal(expectedString, actualString);
        }


        /// <summary>
        /// Method StartScrum Testing with In Valid Start Command
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateInValidStartCommand()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Start + " " + _stringConstant.StringIdForTest, _stringConstant.ScrumBotName);
            Assert.Equal(string.Empty, actualString);
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
            scrumAnswer.EmployeeId = _stringConstant.StringIdForTest;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            string compareString = string.Format(_stringConstant.TestQuestion, DateTime.UtcNow.Date.AddDays(-1).ToShortDateString()) + Environment.NewLine;
            Assert.Equal(compareString, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum but inactive user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateHasScrumUserInactive()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            string compareString = string.Format(_stringConstant.InActiveInOAuth, _stringConstant.UserNameForTest) + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(compareString, actualString);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum but user not in slack
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumInitiateHasScrumNoUser()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            testSlackUserDetails.UserId = _stringConstant.ChannelIdForTest;
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.ChannelIdForTest, 0, question.Id);

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            string compareString = string.Format(_stringConstant.UserNotInProject, _stringConstant.UserNameForTest) + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(compareString, actualString);
        }
                  

        /// <summary>
        /// Method StartScrum Testing with existing scrum and in-active users
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task StartScrumCompleteHasInActiveUsers()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
            await _botQuestionRepository.AddQuestionAsync(question);

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.StartBot, _stringConstant.ScrumBotName);
            string expectedString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expectedString, actualString);
        }


        /// <summary>
        /// Method Scrum Resume Testing with in-active users
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumResumeHasInActiveUsers()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            slackUserDetails.UserId = _stringConstant.ChannelIdForTest;
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.ChannelIdForTest, 0, question.Id);
            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume, _stringConstant.ScrumBotName);
            string expectedString = _stringConstant.ScrumNotHalted + Environment.NewLine + string.Format(_stringConstant.UserNotInProject, _stringConstant.TestUser) + string.Format(_stringConstant.InActiveInOAuth, _stringConstant.UserNameForTest) + _stringConstant.ScrumComplete;
            Assert.Equal(expectedString, actualString);
        }


        #endregion


        #region Halt Scrum


        /// <summary>
        /// Method StartScrum Testing with on going scrum but inactive user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHaltUserHasOnGoingScrumInactive()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt, _stringConstant.ScrumBotName);
            string compareString = _stringConstant.ScrumCannotBeHalted + Environment.NewLine + string.Format(_stringConstant.InActiveInOAuth, _stringConstant.UserNameForTest) + _stringConstant.ScrumComplete;
            Assert.Equal(compareString, actualString);
        }


        /// <summary>
        /// Method StartScrum Testing with on going scrum, scrum not completed as in complete scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHaltScrumCompleteButInCompleteAnswer()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.EmployeeId = user.Id;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt, _stringConstant.ScrumBotName);
            string compareString = _stringConstant.ScrumCannotBeHalted + Environment.NewLine + string.Format(_stringConstant.InActiveInOAuth, _stringConstant.UserNameForTest) + string.Format(_stringConstant.MarkedInActive, _stringConstant.TestUser) + _stringConstant.ScrumQuestionForTest;
            Assert.Equal(compareString, actualString);
        }


        /// <summary>
        /// Scrum halt Testing with existing scrum but inactive user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHaltUserInactive()
        {
            await AddChannelUserAsync();

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt, _stringConstant.ScrumBotName);
            string compareString = string.Format(_stringConstant.InActiveInOAuth, _stringConstant.UserNameForTest);
            Assert.Equal(compareString, actualString);
        }


        /// <summary>
        /// Scrum halt Testing with no users
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHaltNoUsers()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            await _botQuestionRepository.AddQuestionAsync(question);
            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.NoEmployeeFound, actualString);
        }


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

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ScrumHalted, msg);
        }


        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHaltScrumComplete()
        {
            await AddChannelUserAsync();

            scrum.IsOngoing = false;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ScrumAlreadyConducted, actualString);
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

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ScrumNotStarted, actualString);
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
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ScrumAlreadyHalted, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with on going scrum but no question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumHaltNoQuestion()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.NoQuestion + _stringConstant.ScrumCannotBeHalted, actualString);
        }


        #endregion


        #region Resume Scrum


        /// <summary>
        /// Method StartScrum Testing with on going scrum but inactive user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumResumeUserHasOnGoingScrumInactive()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume, _stringConstant.ScrumBotName);
            string compareString = _stringConstant.ScrumNotHalted + string.Format(_stringConstant.InActiveInOAuth, _stringConstant.UserNameForTest) + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(compareString, actualString);
        }


        /// <summary>
        /// Method StartScrum Testing with on going scrum but inactive user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumResumeDeletedUserHasOnGoingScrum()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthOneEmployee);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume, _stringConstant.ScrumBotName);
            string compareString = _stringConstant.ScrumNotHalted + string.Format(_stringConstant.UserNotInProject, _stringConstant.UserNameForTest) + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(compareString, actualString);
        }


        /// <summary>
        /// Method StartScrum Testing with on going scrum but no question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumResumeUserHasOnGoingScrum()
        {
            await AddChannelUserAsync();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume, _stringConstant.ScrumBotName);
            string compareString = _stringConstant.NoQuestion + _stringConstant.ScrumCannotBeResumed;
            Assert.Equal(compareString, actualString);
        }


        /// <summary>
        /// Scrum Resume testing
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
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume, _stringConstant.ScrumBotName);
            string compareString = _stringConstant.ScrumResumed + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(compareString, actualString);
        }


        /// <summary>
        /// Scrum Resume testing scrum not halted
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumResumeScrumNotHalted()
        {
            await AddChannelUserAsync();
            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);

            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume, _stringConstant.ScrumBotName);
            string compareString = _stringConstant.ScrumNotHalted + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(compareString, actualString);
        }


        /// <summary>
        /// Scrum Resume Testing with in-active users and in-active project
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumResumeInActiveProject()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.InActiveProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);
            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume, _stringConstant.ScrumBotName);
            string expectedString = _stringConstant.ProjectInActive;
            Assert.Equal(expectedString, actualString);
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
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ScrumIsHalted, msg);
        }


        /// <summary>
        /// Method to test that slack channel is not linked to OAuth project
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ChannelNotLinked()
        {
            slackChannelDetails.ProjectId = null;
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ProjectChannelNotLinked, actual);
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
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.Scrum + ">", _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.NotAUser, msg);
        }


        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task Leave()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            slackUserDetails.UserId = _stringConstant.IdForTest;
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            testSlackUserDetails.UserId = _stringConstant.StringIdForTest;
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            string expected = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expected, actual);
        }


        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveInActiveApplicant()
        {
            await AddChannelUserAsync();
            await InActiveUserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.IdForTest + ">", _stringConstant.ScrumBotName);
            string expected = string.Format(_stringConstant.InActiveInOAuth, _stringConstant.TestUser) + _stringConstant.ScrumComplete;
            Assert.Equal(expected, actual);
        }


        /// <summary>
        /// Method StartScrum Testing with in-active users
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveHasInActiveUser()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.EmployeeId = _stringConstant.StringIdForTest;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();

            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 1, question.Id);
            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            string expectedString = string.Format(_stringConstant.AlreadyAnswered, slackUserDetails.Name) + _stringConstant.NextQuestion + Environment.NewLine;
            Assert.Equal(expectedString, actualString);
        }


        /// <summary>
        /// Method StartScrum Testing with no slack user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveHasNoUser()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            scrumAnswer.EmployeeId = _stringConstant.ChannelIdForTest;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();

            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.ChannelIdForTest, 1, question.Id);
            string actualString = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            string expectedString = string.Format(_stringConstant.PleaseAnswer, _stringConstant.UserNameForTest);
            Assert.Equal(expectedString, actualString);
        }


        /// <summary>
        /// Method Leave Testing with in active user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveInActiveUser()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.IdForTest + ">", _stringConstant.ScrumBotName);
            string expected = string.Format(_stringConstant.InActiveInOAuth, _stringConstant.UserNameForTest) + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expected, actual);
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

            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
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
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
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

            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
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
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ScrumAlreadyConducted, msg);
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
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.YouAreNotInExistInOAuthServer, msg);
        }


        /// <summary>
        /// Method Leave Testing with the user marking on leave not in project
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveUserNotFound()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthOneEmployee);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();

            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            string expected = string.Format(_stringConstant.UserNotInProject, _stringConstant.UserNameForTest) + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expected, actual);
        }


        /// <summary>
        /// Method Leave Testing with the leave applicant not in project
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplicantNotFound()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthOneEmployee);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            string expected = string.Format(_stringConstant.UserNotInProject, _stringConstant.UserNameForTest) + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expected, actual);
        }


        /// <summary>
        /// Method Leave Testing with the leave applicant in active
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveInCorrectApplicant()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthThreeEmployee);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.ThirdUserSlackUserId, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            string expected = _stringConstant.UserNotInSlack + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expected, actual);
        }


        /// <summary>
        /// Method Leave Testing with the expected user not in slack
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveExpectedUserNotInSlack()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            testSlackUserDetails.UserId = _stringConstant.ThirdUserSlackUserId;
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthThreeEmployeeInActive);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.ThirdUserSlackUserId, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">", _stringConstant.ScrumBotName);
            string expected = _stringConstant.UserNotInSlack + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expected, actual);
        }


        /// <summary>
        /// Method Leave Testing with the leave applicant not in project
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LeaveApplicantNotInProject()
        {
            await AddChannelUserAsync();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthOneEmployee);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.ThirdUserSlackUserId, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.IdForTest + ">", _stringConstant.ScrumBotName);
            string expected = string.Format(_stringConstant.UserNotInProject, _stringConstant.UserNameForTest) + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expected, actual);
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
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum, _stringConstant.ScrumBotName);
            Assert.Equal(msg, _stringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with first employee's first answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerFirstEmployeeFirstAnswer()
        {
            await UserProjectSetup();
            await AddChannelUserAsync();
            await _botQuestionRepository.AddQuestionAsync(question);
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum, _stringConstant.ScrumBotName);
            Assert.Equal("<@" + _stringConstant.TestUser + "> " + _stringConstant.ScrumQuestionForTest + Environment.NewLine, actual);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next question not found
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerNextQuestionNotFound()
        {
            await UserProjectSetup();
            await AddChannelUserAsync();
            await _botQuestionRepository.AddQuestionAsync(question);
            question1.OrderNumber = QuestionOrder.YourTask;
            await _botQuestionRepository.AddQuestionAsync(question1);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.NoQuestion, actual);
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
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum, _stringConstant.ScrumBotName);
            Assert.Equal(msg, _stringConstant.NoQuestion);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing (update answer) with only One Answer marked to be asked later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateScrumAnswer()
        {
            await UserProjectSetup();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);

            string actualMsg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum, _stringConstant.ScrumBotName);
            string expectedMsg = string.Format(_stringConstant.PleaseAnswer, _stringConstant.UserNameForTest);
            Assert.Equal(expectedMsg, actualMsg);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing (update answer) with only One Answer marked to be asked later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddScrumAnswerInActiveUser()
        {
            await UserProjectSetup();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);

            string actualMsg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum, _stringConstant.ScrumBotName);
            string expectedMsg = string.Format(_stringConstant.PleaseAnswer, _stringConstant.UserNameForTest);
            Assert.Equal(expectedMsg, actualMsg);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing (update answer) with only One Answer marked to be asked later but inactive user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task UpdateScrumAnswerInActiveUser()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInActiveUser);
            string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            UserLoginInfo infor = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, infor);

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            await _scrumAnswerDataRepository.SaveChangesAsync();
            await AddChannelUserAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.IdForTest, 0, question.Id);

            string actualMsg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum, _stringConstant.ScrumBotName);
            string expectedMsg = string.Format(_stringConstant.InActiveInOAuth, _stringConstant.UserNameForTest) + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(expectedMsg, actualMsg);
        }


        //not required
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
            string msg = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum, _stringConstant.ScrumBotName);
            Assert.Equal(msg, string.Empty);
        }


        #endregion


        #region ProcessMessagesAsync Test Cases


        //not required
        /// <summary>
        /// Method ProcessMessagesAsync Testing with scrum help
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerProcess()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.GroupName, _stringConstant.ScrumHelp, _stringConstant.ScrumBotName);
            string expected = string.Format(_stringConstant.ScrumHelpMessage, _stringConstant.ScrumBotName);
            Assert.Equal(expected, actual);
        }


        //not required
        /// <summary>
        /// Method ProcessMessagesAsync Testing with not registered channel
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ScrumAnswerProcessNoChannel()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.StringIdForTest, _stringConstant.GroupName, _stringConstant.GroupDetailsResponseText, _stringConstant.ScrumBotName);
            Assert.Equal(string.Empty, actual);
        }


        #endregion


        #region AddChannel Test Cases


        //not required
        /// <summary>
        /// Method AddChannelManually Testing
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddChannel()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            await UserProjectSetup();
            string actual = await _scrumBotRepository.ProcessMessagesAsync(slackUserDetails.UserId, _stringConstant.GroupNameStartsWith + slackChannelDetails.ChannelId, _stringConstant.Add + " " + _stringConstant.Channel + " " + slackChannelDetails.Name, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ChannelAddSuccess, actual);
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
            string actual = await _scrumBotRepository.ProcessMessagesAsync(slackUserDetails.UserId, _stringConstant.GroupName, _stringConstant.Add + " " + _stringConstant.Channel + " " + _stringConstant.GroupName, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.OnlyPrivateChannel, actual);
        }


        /// <summary>
        /// Method AddChannelManually Testing with No User
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task AddChannelNoUser()
        {
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(slackUserDetails.UserId, _stringConstant.GroupNameStartsWith + slackChannelDetails.ChannelId, _stringConstant.Add + " " + _stringConstant.Channel + " " + _stringConstant.GroupName, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.YouAreNotInExistInOAuthServer, actual);
        }


        #endregion


        #region Link Channel


        /// <summary>
        /// Method List links Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task ListLink()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            slackUserDetails.UserId = _stringConstant.IdForTest;
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            testSlackUserDetails.UserId = _stringConstant.StringIdForTest;
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await UserProjectSetup();

            await _botQuestionRepository.AddQuestionAsync(question);
            _scrumDataRepository.Insert(scrum);
            await _scrumDataRepository.SaveChangesAsync();
            await _scrumBotRepository.AddTemporaryScrumDetailsAsync(1, _stringConstant.StringIdForTest, 0, question.Id);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ListLinks, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.NoLinks, actual);
        }


        //future
        /// <summary>
        /// Method link channel with wrong command
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LinkChannelWrongCommand()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            slackUserDetails.UserId = _stringConstant.IdForTest;
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            testSlackUserDetails.UserId = _stringConstant.StringIdForTest;
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await UserProjectSetup();

            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.LinkTest + _stringConstant.NameClaimType, _stringConstant.ScrumBotName);
            Assert.Empty(actual);
        }


        /// <summary>
        /// Method link channel with wrong command but channel not linked yet
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LinkChannelWrongCommandChannelNotLinked()
        {
            slackChannelDetails.ProjectId = null;
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            slackUserDetails.UserId = _stringConstant.IdForTest;
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            testSlackUserDetails.UserId = _stringConstant.StringIdForTest;
            await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            await UserProjectSetup();

            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, "link \"\"", _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.ProjectChannelNotLinked, actual);
        }


        /// <summary>
        /// Method List links Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LinkChannelNoUser()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.LinkTest + _stringConstant.NameClaimType, _stringConstant.ScrumBotName);
            Assert.Empty(actual);
        }


        /// <summary>
        /// Method List links Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LinkChannelNoUserValidCommand()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ListLinks, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.SlackUserNotFound, actual);
        }


        /// <summary>
        /// Method link channel no channel Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LinkChannelNoUserNoChannel()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, "link \"\"", _stringConstant.ScrumBotName);
            Assert.Empty(actual);
        }


        /// <summary>
        /// Method link channel no channel Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task LinkChannelNoUserHasChannel()
        {
            await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
            string actual = await _scrumBotRepository.ProcessMessagesAsync(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.LinkTest, _stringConstant.ScrumBotName);
            Assert.Equal(_stringConstant.SlackUserNotFound, actual);
        }

        #endregion


        #endregion


    }
}