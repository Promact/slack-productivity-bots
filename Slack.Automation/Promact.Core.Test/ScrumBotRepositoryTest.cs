using Autofac;
using Moq;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Erp.DomainModel.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Microsoft.AspNet.Identity;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util.StringConstants;

namespace Promact.Core.Test
{
    public class ScrumBotRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<Scrum> _scrumDataRepository;
        private readonly IRepository<ScrumAnswer> _scrumAnswerDataRepository;
        private readonly IScrumBotRepository _scrumBotRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly ISlackChannelRepository _slackChannelReposiroty;
        private readonly IStringConstantRepository _stringConstant;

        private static string _emailForTest;
        private static string _slackUserName;
        private static string _scrumQuestionForTest;
        private static int _projectIdForTest;
        private static string _groupName;
        private static string _teamLeaderIdForTest;
        private static string _noQuestion;
        private static string _userIdForTest;
        private static string _testUser;
        private static string _testUserId;
        private static string _phoneForTest;
        private static string _stringIdForTest;
        private static string _promactStringName;
        private static string _idForTest;
        private static string _slackChannelIdForTest;
        private static Question question = new Question();
        private static ApplicationUser testUser = new ApplicationUser();
        private static SlackProfile profile = new SlackProfile();
        private static Scrum scrum = new Scrum();
        private static SlackUserDetails slackUserDetails = new SlackUserDetails();
        private static SlackUserDetails testSlackUserDetails = new SlackUserDetails();
        private static SlackChannelDetails slackChannelDetails = new SlackChannelDetails();
        private static ApplicationUser user = new ApplicationUser();
        private static Question question1 = new Question();
        private static ScrumAnswer scrumAnswer = new ScrumAnswer();
       
        public ScrumBotRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumBotRepository = _componentContext.Resolve<IScrumBotRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _scrumDataRepository = _componentContext.Resolve<IRepository<Scrum>>();
            _scrumAnswerDataRepository = _componentContext.Resolve<IRepository<ScrumAnswer>>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _slackChannelReposiroty = _componentContext.Resolve<ISlackChannelRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();

            _emailForTest = _stringConstant.EmailForTest;
            _slackUserName = _stringConstant.UserNameForTest;
            _scrumQuestionForTest = _stringConstant.ScrumQuestionForTest;
            _projectIdForTest = _stringConstant.ProjectIdForTest;
            _groupName = _stringConstant.GroupName;
            _teamLeaderIdForTest = _stringConstant.TeamLeaderIdForTest;
            _noQuestion = _stringConstant.NoQuestion;
            _userIdForTest = _stringConstant.UserIdForTest;
            _testUser = _stringConstant.TestUser;
            _testUserId = _stringConstant.TestUserId;
            _phoneForTest = _stringConstant.PhoneForTest;
            _stringIdForTest = _stringConstant.StringIdForTest;
            _promactStringName = _stringConstant.PromactStringName;
            _idForTest = _stringConstant.IdForTest;
            _slackChannelIdForTest = _stringConstant.SlackChannelIdForTest;
            Initialization();
        }

        public static void Initialization()
        {
            scrumAnswer.Answer = _noQuestion;
            scrumAnswer.CreatedOn = DateTime.UtcNow;
            scrumAnswer.AnswerDate = DateTime.UtcNow;
            scrumAnswer.EmployeeId = _userIdForTest;
            scrumAnswer.Id = 1;
            scrumAnswer.QuestionId = 1;
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.Answered;
            scrumAnswer.ScrumId = 1;

            question1.CreatedOn = DateTime.UtcNow;
            question1.OrderNumber = 2;
            question1.QuestionStatement = _scrumQuestionForTest;
            question1.Type = 1;

            user.Email = _emailForTest;
            user.UserName = _emailForTest;
            user.SlackUserName = _slackUserName;

            question.CreatedOn = DateTime.UtcNow;
            question.OrderNumber = 1;
            question.QuestionStatement = _scrumQuestionForTest;
            question.Type = 1;

            testUser.Email = _emailForTest;
            testUser.UserName = _emailForTest;
            testUser.SlackUserName = _testUser;

            profile.Skype = _testUserId;
            profile.Email = _emailForTest;
            profile.FirstName = _slackUserName;
            profile.LastName = _testUser;
            profile.Phone = _phoneForTest;
            profile.Title = _slackUserName;

            scrum.CreatedOn = DateTime.UtcNow;
            scrum.ProjectId = _projectIdForTest;
            scrum.GroupName = _groupName;
            scrum.ScrumDate = DateTime.UtcNow;
            scrum.TeamLeaderId = _teamLeaderIdForTest;
            scrum.IsHalted = false;

            slackUserDetails.UserId = _stringIdForTest;
            slackUserDetails.Name = _slackUserName;
            slackUserDetails.TeamId = _promactStringName;
            slackUserDetails.CreatedOn = DateTime.UtcNow;
            slackUserDetails.Deleted = false;
            slackUserDetails.IsAdmin = false;
            slackUserDetails.IsBot = false;
            slackUserDetails.IsPrimaryOwner = false;
            slackUserDetails.IsOwner = false;
            slackUserDetails.IsRestrictedUser = false;
            slackUserDetails.IsUltraRestrictedUser = false;
            slackUserDetails.Profile = profile;
            slackUserDetails.RealName = _slackUserName + _testUser;

            testSlackUserDetails.UserId = _idForTest;
            testSlackUserDetails.Name = _testUser;
            testSlackUserDetails.TeamId = _promactStringName;
            testSlackUserDetails.Profile = profile;

            slackChannelDetails.ChannelId = _slackChannelIdForTest;
            slackChannelDetails.Deleted = false;
            slackChannelDetails.CreatedOn = DateTime.UtcNow;
            slackChannelDetails.Name = _groupName;

        }


        #region Test Cases


        #region Scrum Test Cases 


        #region Start Scrum


        /// <summary>
        /// Method StartScrum Testing with existing scrum but no scrum answer given yet
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrumNoAnswer()
        {
            AddChannelUser();
            UserProjectSetup();

            _scrumDataRepository.Insert(scrum);
            _botQuestionRepository.AddQuestion(question);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.GoodDay + "<@" + _slackUserName + ">!\n" + _stringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method StartScrum Testing without registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumNoUser()
        {
            AddChannelUser();
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum and Scrum Answer with AnswerNow status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateAnswerNow()
        {
            AddChannelUser();
            UserProjectSetup();

            _scrumDataRepository.Insert(scrum);
            _botQuestionRepository.AddQuestion(question);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.NextQuestion);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoQuestion()
        {
            AddChannelUser();
            UserProjectSetup();

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _noQuestion);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoProject()
        {
            slackChannelDetails.Name = _slackUserName;
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);
            _slackUserRepository.AddSlackUser(slackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", _stringConstant.ProjectDetailsUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.NoProjectFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with No Employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoEmployee()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", _stringConstant.ProjectDetailsUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.NoEmployeeFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiate()
        {
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.GoodDay + "<@" + _slackUserName + ">!\n" + _stringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrum()
        {
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrum.ScrumDate = DateTime.UtcNow.Date.AddDays(-1);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            scrumAnswer.AnswerDate = DateTime.UtcNow.Date.AddDays(-1);
            scrumAnswer.ScrumId = 2;
            scrumAnswer.EmployeeId = _testUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            string previousDayStatus = Environment.NewLine + _stringConstant.PreviousDayStatus + Environment.NewLine;
            previousDayStatus += "*_Q_*: " + _stringConstant.ScrumQuestionForTest + Environment.NewLine + "*_A_*: _" + _noQuestion + "_" + Environment.NewLine;
            previousDayStatus += Environment.NewLine + _stringConstant.AnswerToday + Environment.NewLine + Environment.NewLine;

            Assert.Equal(_stringConstant.GoodDay + "<@" + _stringConstant.UserNameForTest + ">!\n" + previousDayStatus + _stringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing halted scrum 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasHaltedScrum()
        {
            AddChannelUser();

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            UserProjectSetup();

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.ResumeScrum, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrumQuestionRemaining()
        {
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            Question question1 = new Question
            {
                CreatedOn = DateTime.UtcNow,
                OrderNumber = 2,
                QuestionStatement = _stringConstant.ScrumQuestionForTest,
                Type = 1
            };
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal("<@" + _slackUserName + "> " + _stringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum but no question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrumNoQuestion()
        {
            AddChannelUser();
            UserProjectSetup();
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_noQuestion, msg);
        }


        #endregion


        #region Halt Scrum


        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumHalt()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumHalted, msg);
        }


        /// <summary>
        /// Method Scrum Testing for already halted
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAlreadyHalted()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumAlreadyHalted, msg);
        }



        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumHaltNoScrum()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumNotStarted, msg);
        }


        #endregion


        #region Resume Scrum


        /// <summary>
        /// Method Scrum Testing for resume (getquestion)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResume()
        {
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumResume);
            var expectedString = string.Format(_stringConstant.QuestionToNextEmployee, _slackUserName);
            Assert.Equal(_stringConstant.ScrumResumed + expectedString, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume without a scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeNoScrum()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumResume);
            Assert.Equal(_stringConstant.ScrumNotStarted, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume for unhalted scrum (getquestion)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeNotHalted()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = false;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumResume);
            Assert.Equal(_stringConstant.ScrumNotHalted + _stringConstant.NoEmployeeFound, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume with previous day scrum status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeWithPreviuosDayStatus()
        {
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            scrum.ScrumDate = scrum.ScrumDate.AddDays(-1);
            _scrumDataRepository.Insert(scrum);

            scrumAnswer.ScrumId = 2;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.ScrumResume);
            Assert.Equal(_stringConstant.PreviousDayStatusForTest, msg);
        }


        #endregion


        #endregion


        #region Leave and other functionalities 


        #region Leave Test Cases 


        /// <summary>
        /// Method Leave Testing without registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveNoUser()
        {
            AddChannelUser();
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            Assert.Equal(_stringConstant.YouAreNotInExistInOAuthServer, msg);
        }


        /// <summary>
        /// Method Leave Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumHalted()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            Assert.Equal(_stringConstant.ResumeScrum, msg);
        }


        /// <summary>
        /// Method Leave Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumHaltedProcessMessage()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Leave + " " + _slackUserName);
            Assert.Equal(String.Empty, msg);
        }



        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void Leave()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            Assert.Equal(Environment.NewLine + _stringConstant.GoodDay + "<@" + _testUser + ">!\n" + _stringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant and applying employee are same
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveWithSameApplicant()
        {
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            Assert.Equal(_stringConstant.LeaveError, msg);
        }


        /// <summary>
        /// Method Leave Testing with no scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveNoScrum()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            Assert.Equal(_stringConstant.ScrumNotStarted, msg);
        }


        /// <summary>
        /// Method Leave Testing (where leave is applied on a group where scrum is already complete)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumAlreadyComplete()
        {
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            Assert.Equal(_stringConstant.ScrumAlreadyConducted, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant has already answered a question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumWithAnswer()
        {
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            string compareString = string.Format(_stringConstant.AlreadyAnswered, _slackUserName) + Environment.NewLine;
            Assert.Equal(compareString + _stringConstant.NextQuestion, msg);
        }


        #endregion


        #region Later Test Cases 


        /// <summary>
        /// Method LeaveLater Testing with wrong employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLaterWithWrongEmployee()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _idForTest + ">");
            string compareString = string.Format(_stringConstant.PleaseAnswer, _slackUserName);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method LeaveLater Testing with no scrum answer yet and answer expected message
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLaterWithoutScrumAnswer()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            string questionString = Environment.NewLine + string.Format(_stringConstant.QuestionToNextEmployee, _testUser);
            Assert.Equal(msg, questionString);
        }


        /// <summary>
        /// Method LeaveLater Testing with scrum answer already marked as later or to be answered now 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLater()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Later + " <@" + _stringIdForTest + ">");

            string compareString = Environment.NewLine + string.Format(_stringConstant.QuestionToNextEmployee, _testUser);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method LeaveLater Testing with scrum answer already marked as later or to be answered now but with unexpected user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLaterNotExpected()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            scrumAnswer.EmployeeId = _idForTest + _stringIdForTest;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            string compareString = string.Format(_stringConstant.NotExpected, _slackUserName);
            Assert.Equal(msg, compareString);
        }



        /// <summary>
        /// Method Later Testing all answers are given but some marked as later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterWithSomeLaterAnswer()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.Later;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            scrumAnswer.EmployeeId = _testUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Leave + " <@" + _stringIdForTest + ">");
            Assert.Equal(_stringConstant.ScrumConcludedButLater, msg);
        }


        #endregion


        #region Scrum Later Test Cases 


        /// <summary>
        /// Method LaterScrum Testing
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrum()
        {
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.Answer = _stringConstant.Later;
            scrumAnswer.EmployeeId = _testUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum + " <@" + _idForTest + ">");
            var expectedString = string.Format("<@{0}> ", _testUser);
            Assert.Equal(msg, expectedString + _stringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method LaterScrum Testing with applicant not logged in with promact
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumApplicantNotLoggedIn()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);

            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum + " <@" + _idForTest + ">");
            Assert.Equal(msg, _stringConstant.NotAUser);
        }



        /// <summary>
        /// Method LaterScrum Testing with Unrecognized employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumUnrecognizedEmployee()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            slackUserDetails.Name = _stringConstant.TestUserName;
            slackUserDetails.UserId = _idForTest;
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.Answer = _stringConstant.Later;
            scrumAnswer.EmployeeId = _testUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum + " <@" + _idForTest + ">");
            Assert.Equal(msg, _stringConstant.Unrecognized);
        }


        /// <summary>
        /// Method LaterScrum Testing with wrong employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumWrongEmployee()
        {
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);

            scrumAnswer.Answer = _stringConstant.Later;
            scrumAnswer.EmployeeId = _testUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum + " <@" + _stringIdForTest + ">");
            string expectedString = string.Format(_stringConstant.PleaseAnswer, _testUser) + Environment.NewLine;
            Assert.Equal(msg, expectedString);
        }


        /// <summary>
        /// Method LaterScrum Testing with few answered already
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumWithFewAnswered()
        {
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);

            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            scrumAnswer.Answer = _stringConstant.Later;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum + " <@" + _stringIdForTest + ">");
            Assert.Equal(msg, _stringConstant.NextQuestion);
        }


        /// <summary>
        /// Method LaterScrum Testing with all answers already answered
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumWithAllAnswers()
        {
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum + " <@" + _stringIdForTest + ">");
            string compareString = Environment.NewLine + string.Format(_stringConstant.QuestionToNextEmployee, _testUser);
            Assert.Equal(msg, _stringConstant.AllAnswerRecorded + compareString);
        }


        /// <summary>
        /// Method LaterScrum Testing with not marked as later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumNotLater()
        {
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum + " <@" + _stringIdForTest + ">");
            string expectedString = string.Format(_stringConstant.NotLaterYet, _slackUserName);
            Assert.Equal(msg, expectedString + _stringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method LaterScrum Testing with not marked as later with wrong applicant
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumNotLaterWrongApplicant()
        {
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum + " <@" + _idForTest + ">");
            string expectedString = string.Format(_stringConstant.PleaseAnswer, _slackUserName) + Environment.NewLine;
            Assert.Equal(msg, expectedString);
        }


        /// <summary>
        /// Method LaterScrum Testing with Answers with status AnswerNow
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AnswerNowScrum()
        {
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            scrumAnswer.EmployeeId = _testUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum + " <@" + _idForTest + ">");
            var expectedString = string.Format(_stringConstant.PleaseAnswer, _testUser) + Environment.NewLine;
            Assert.Equal(msg, expectedString);
        }


        #endregion


        #endregion


        #region AddScrumAnswer Test Cases 


        /// <summary>
        /// Method AddScrumAnswer Testing without registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerNoUser()
        {
            AddChannelUser();
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with first employee's first answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerFirstEmployeeFirstAnswer()
        {
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.NextQuestion);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next answer of an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerNextAnswer()
        {
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);

            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            string compareString = string.Format(_stringConstant.QuestionToNextEmployee, _testUser);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next question to same employee as return message
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerNextQuestion()
        {
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            Question question2 = new Question()
            {
                CreatedOn = DateTime.UtcNow,
                OrderNumber = 3,
                QuestionStatement = _stringConstant.ScrumQuestionForTest,
                Type = 1
            };
            _botQuestionRepository.AddQuestion(question2);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.NextQuestion);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing (update answer) with only One Answer marked to be asked later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void UpdateScrumAnswer()
        {
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.UpdateAnswer);
        }


        /// <summary>
        /// Method UpdateScrumAnswer Testing with more than one answer marked to be asked later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void UpdateScrumAnswers()
        {
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            string compareString = "<@" + _slackUserName + "> " + _stringConstant.NoQuestion;
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next employee's first answer and scrum complete
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerScrumComplete()
        {
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.ScrumComplete);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with unexpected employee's answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumUnexpectedEmployeeAnswer()
        {
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            string expectedString = string.Format(_stringConstant.PleaseAnswer, _testUser);
            Assert.Equal(msg, expectedString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with scrum complete but some employees marked as later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerScrumCompleteButLater()
        {
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.Later;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.ScrumConcludedButLater);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with normal conversation on slack channel
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void NormalConversation()
        {
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, string.Empty);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddScrumAnswerHaltedScrum()
        {
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_idForTest, _slackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, string.Empty);
        }


        #endregion


        #region ProcessMessages Test Cases


        /// <summary>
        /// Method ProcessMessages Testing with scrum help
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerProcess()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _groupName, _stringConstant.ScrumHelp);
            Assert.Equal(msg, _stringConstant.ScrumHelpMessage);
        }


        /// <summary>
        /// Method ProcessMessages Testing with not registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerProcessNoUser()
        {
            var msg = await _scrumBotRepository.ProcessMessages(_stringIdForTest, _groupName, _stringConstant.ScrumHelp);
            Assert.Equal(msg, _stringConstant.NotAUser);
        }


        /// <summary>
        /// Method ProcessMessages Testing with not registered channel
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerProcessNoChannel()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.GroupName, StringConstant.GroupDetailsResponseText);
            Assert.Equal(msg, StringConstant.ChannelAddInstruction);
        }


        #endregion


        #region AddChannel Test Cases


        /// <summary>
        /// Method AddChannelManually Testing
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddChannel()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            UserProjectSetup();

            var msg = await _scrumBotRepository.ProcessMessages(slackUserDetails.UserId, slackChannelDetails.ChannelId, StringConstant.Add + " " + StringConstant.Channel + " " + slackChannelDetails.Name);
            Assert.Equal(msg, StringConstant.ChannelAddSuccess);
        }


        /// <summary>
        /// Method AddChannelManually Testing with Not Privaet Group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddChannelNotPrivateGroup()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(slackUserDetails.UserId, StringConstant.GroupName, StringConstant.Add + " " + StringConstant.Channel + " " + StringConstant.GroupName);
            Assert.Equal(msg, StringConstant.OnlyPrivateChannel);
        }


        /// <summary>
        /// Method AddChannelManually Testing with No User
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddChannelNoUser()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);

            var msg = await _scrumBotRepository.ProcessMessages(slackUserDetails.UserId, slackChannelDetails.ChannelId, StringConstant.Add + " " + StringConstant.Channel + " " + StringConstant.GroupName);
            Assert.Equal(msg, StringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// Method AddChannelManually Testing with no project added
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddChannelNoProject()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(slackUserDetails.UserId, slackChannelDetails.ChannelId, StringConstant.Add + " " + StringConstant.Channel + " " + StringConstant.GroupName);
            Assert.Equal(msg, StringConstant.ProjectNotInOAuth);
        }


        #endregion


        #endregion

        //private ApplicationUser user = new ApplicationUser()
        //{
        //    Email = _emailForTest,
        //    UserName = _emailForTest,
        //    SlackUserName = _slackUserName
        //};

        //private Question question = new Question()
        //{
        //    CreatedOn = DateTime.UtcNow,
        //    OrderNumber = 1,
        //    QuestionStatement = _scrumQuestionForTest,
        //    Type = 1
        //};

        //private Question question1 = new Question()
        //{
        //    CreatedOn = DateTime.UtcNow,
        //    OrderNumber = 2,
        //    QuestionStatement = _scrumQuestionForTest,
        //    Type = 1
        //};

        //private Scrum scrum = new Scrum()
        //{
        //    CreatedOn = DateTime.UtcNow,
        //    ProjectId = _projectIdForTest,
        //    GroupName = _groupName,
        //    ScrumDate = DateTime.UtcNow,
        //    TeamLeaderId = _teamLeaderIdForTest,
        //    IsHalted = false
        //};

        //private ScrumAnswer scrumAnswer = new ScrumAnswer()
        //{
        //    Answer = _noQuestion,
        //    CreatedOn = DateTime.UtcNow,
        //    AnswerDate = DateTime.UtcNow,
        //    EmployeeId = _userIdForTest,
        //    Id = 1,
        //    QuestionId = 1,
        //    ScrumAnswerStatus = ScrumAnswerStatus.Answered,
        //    ScrumId = 1
        //};

        //private ApplicationUser testUser = new ApplicationUser()
        //{
        //    Email = _emailForTest,
        //    UserName = _emailForTest,
        //    //SlackUserId = "df3432",
        //    SlackUserName = _testUser
        //};

        //private static SlackProfile profile = new SlackProfile()
        //{
        //    Skype = _testUserId,
        //    Email = _emailForTest,
        //    FirstName = _slackUserName,
        //    LastName = _testUser,
        //    Phone = _phoneForTest,
        //    Title = _slackUserName
        //};

        //private SlackUserDetails slackUserDetails = new SlackUserDetails()
        //{
        //    UserId = _stringIdForTest,
        //    Name = _slackUserName,
        //    TeamId = _promactStringName,
        //    CreatedOn = DateTime.UtcNow,
        //    Deleted = false,
        //    IsAdmin = false,
        //    IsBot = false,
        //    IsPrimaryOwner = false,
        //    IsOwner = false,
        //    IsRestrictedUser = false,
        //    IsUltraRestrictedUser = false,
        //    Profile = profile,
        //    RealName = _slackUserName + _testUser
        //};

        //private SlackUserDetails testSlackUserDetails = new SlackUserDetails()
        //{
        //    UserId = _idForTest,
        //    Name = _testUser,
        //    TeamId = _promactStringName,
        //    Profile = profile
        //};

        //private SlackChannelDetails slackChannelDetails = new SlackChannelDetails()
        //{
        //    ChannelId = _slackChannelIdForTest,
        //    Deleted = false,
        //    CreatedOn = DateTime.UtcNow,
        //    Name = _groupName
        //};

        private void AddChannelUser()
        {
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _slackUserRepository.AddSlackUser(testSlackUserDetails);
        }

        private async void UserProjectSetup()
        {
            UserLoginInfo info = new UserLoginInfo(_promactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", _stringConstant.ProjectDetailsUrl, _groupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

        }



    }
}