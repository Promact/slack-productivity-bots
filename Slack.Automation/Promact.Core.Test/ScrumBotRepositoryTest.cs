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

            Initialization();
        }

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

            question1.CreatedOn = DateTime.UtcNow;
            question1.OrderNumber = 2;
            question1.QuestionStatement = _stringConstant.ScrumQuestionForTest;
            question1.Type = 1;

            user.Email = _stringConstant.EmailForTest;
            user.UserName = _stringConstant.EmailForTest;
            user.SlackUserName = _stringConstant.UserNameForTest;

            question.CreatedOn = DateTime.UtcNow;
            question.OrderNumber = 1;
            question.QuestionStatement = _stringConstant.ScrumQuestionForTest;
            question.Type = 1;

            testUser.Email = _stringConstant.EmailForTest;
            testUser.UserName = _stringConstant.EmailForTest;
            testUser.SlackUserName = _stringConstant.TestUser;

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

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.GoodDay + "<@" + _stringConstant.UserNameForTest + ">!\n" + _stringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method StartScrum Testing without registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumNoUser()
        {
            AddChannelUser();
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.YouAreNotInExistInOAuthServer);
        }




        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoQuestion()
        {
            AddChannelUser();
            UserProjectSetup();

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(msg, _stringConstant.NoQuestion);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoProject()
        {
            slackChannelDetails.Name = _stringConstant.UserNameForTest;
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);
            _slackUserRepository.AddSlackUser(slackUserDetails);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", _stringConstant.ProjectDetailsUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.NoProjectFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with No Employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoEmployee()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", _stringConstant.ProjectDetailsUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
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

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal(_stringConstant.GoodDay + "<@" + _stringConstant.UserNameForTest + ">!\n" + _stringConstant.ScrumQuestionForTest, msg);
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
            scrumAnswer.EmployeeId = _stringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            string previousDayStatus = Environment.NewLine + _stringConstant.PreviousDayStatus + Environment.NewLine;
            previousDayStatus += "*_Q_*: " + _stringConstant.ScrumQuestionForTest + Environment.NewLine + "*_A_*: _" + _stringConstant.NoQuestion + "_" + Environment.NewLine;
            previousDayStatus += Environment.NewLine + _stringConstant.AnswerToday + Environment.NewLine + Environment.NewLine;

            Assert.Equal(_stringConstant.GoodDay + "<@" + _stringConstant.TestUser + ">!\n" + previousDayStatus + _stringConstant.ScrumQuestionForTest, msg);
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

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumTime);
            Assert.Equal("<@" + _stringConstant.UserNameForTest + "> " + _stringConstant.ScrumQuestionForTest, msg);
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
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumHalted, msg);
        }


        /// <summary>
        /// Method Scrum Testing for already halted
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAlreadyHalted()
        {
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);
            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumAlreadyHalted, msg);
        }



        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumHaltNoScrum()
        {
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumHalt);
            Assert.Equal(_stringConstant.ScrumNotStarted + _stringConstant.ScrumCannotBeHalted, msg);
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

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume);
            var expectedString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.UserNameForTest);
            Assert.Equal(_stringConstant.ScrumResumed + expectedString, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume without a scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeNoScrum()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume);
            Assert.Equal(_stringConstant.NoProjectFound + _stringConstant.ScrumCannotBeResumed, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume for unhalted scrum (getquestion)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeNotHalted()
        {
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = false;
            _scrumDataRepository.Insert(scrum);
            string compareString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.UserNameForTest);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume);
            Assert.Equal(_stringConstant.ScrumNotHalted + compareString, msg);
        }


        #endregion


        #endregion


        #region Leave Test Cases 


        /// <summary>
        /// Method Leave Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumHaltedProcessMessage()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.ScrumIsHalted, msg);
        }


        /// <summary>
        /// Method Leave Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void NotUserProcessMessage()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.Scrum + ">");
            Assert.Equal(_stringConstant.NotAUser, msg);
        }


        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void Leave()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl,  _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", _stringConstant.ProjectDetailsUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);


            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(Environment.NewLine + _stringConstant.GoodDay + "<@" + _stringConstant.TestUser + ">!\n" + _stringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveNoProject()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl,  _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.NoProjectFound, msg);
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

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.LeaveError, msg);
        }


        /// <summary>
        /// Method Leave Testing with no scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveNoScrum()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
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

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.ScrumAlreadyConducted, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant has already answered a question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumWithAnswer()
        {
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            testSlackUserDetails.UserId = _stringConstant.TestUserId;
            _slackUserRepository.AddSlackUser(testSlackUserDetails);
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.EmployeeId = _stringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            string compareString = string.Format(_stringConstant.AlreadyAnswered, _stringConstant.UserNameForTest) + Environment.NewLine;
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

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName );
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.IdForTest + ">");
            string compareString = string.Format(_stringConstant.PleaseAnswer, _stringConstant.UserNameForTest);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method LeaveLater Testing with no scrum answer yet and answer expected message
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLaterWithoutScrumAnswer()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            string questionString = Environment.NewLine + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser);
            Assert.Equal(msg, questionString);
        }


        /// <summary>
        /// Method Leave Testing where applicant has already answered a question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApplicantNotExpected()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName );
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Later + " <@" + _stringConstant.StringIdForTest + ">");

            string compareString = Environment.NewLine + string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser);
            Assert.Equal(msg, compareString);
        }

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", _stringConstant.ProjectDetailsUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.EmployeeId = _stringConstant.StringIdForTest;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            string compareString = string.Format(_stringConstant.NotExpected, _stringConstant.UserNameForTest);
            Assert.Equal(compareString, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant is not in OAuth
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveNotInOAuth()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Leave + " <@" + _stringConstant.StringIdForTest + ">");
            Assert.Equal(_stringConstant.YouAreNotInExistInOAuthServer, msg);
        }


        #endregion


        #region AddScrumAnswer Test Cases 


        /// <summary>
        /// Not is Slack
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void NoSlackDetails()
        {
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.NoSlackDetails);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing without registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerNoUser()
        {
            AddChannelUser();
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
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

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, "<@" + _stringConstant.UserNameForTest + "> " + _stringConstant.ScrumQuestionForTest);
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

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            string compareString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.TestUser);
            Assert.Equal(msg, compareString);
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

            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            string msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            string expectedMsg = string.Format(_stringConstant.WrongPerson, _stringConstant.TestUser);
            Assert.Equal(msg, expectedMsg);
        }
                  

        /// <summary>
        /// Method AddScrumAnswer Testing with next employee's first answer and scrum complete
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerScrumComplete()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
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

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            string expectedString = string.Format(_stringConstant.PleaseAnswer, _stringConstant.TestUser);
            Assert.Equal(msg, expectedString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with scrum complete but some employees marked as later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerScrumCompleteButLater()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl , _stringConstant.GroupName );
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, usersListRequestUrl, _stringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.Later;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
            Assert.Equal(msg, _stringConstant.ScrumConcludedButLater);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with normal conversation on slack channel
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void NormalConversation()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.IdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.Scrum);
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
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.GroupName, _stringConstant.ScrumHelp);
            Assert.Equal(msg, _stringConstant.ScrumHelpMessage);
        }


        /// <summary>
        /// Method Scrum Testing for in active project
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ProjectInActive()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.InActiveProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", _stringConstant.ProjectDetailsUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = false;
            _scrumDataRepository.Insert(scrum);
            string compareString = string.Format(_stringConstant.QuestionToNextEmployee, _stringConstant.UserNameForTest);

            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.SlackChannelIdForTest, _stringConstant.ScrumResume);
            Assert.Equal(_stringConstant.ProjectInActive+_stringConstant.ScrumCannotBeResumed, msg);
        }



        /// <summary>
        /// Method ProcessMessages Testing with not registered channel
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerProcessNoChannel()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var msg = await _scrumBotRepository.ProcessMessages(_stringConstant.StringIdForTest, _stringConstant.GroupName, _stringConstant.GroupDetailsResponseText);
            Assert.Equal(msg, _stringConstant.ChannelAddInstruction);
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

            var msg = await _scrumBotRepository.ProcessMessages(slackUserDetails.UserId, _stringConstant.GroupNameStartsWith + slackChannelDetails.ChannelId, _stringConstant.Add + " " + _stringConstant.Channel + " " + slackChannelDetails.Name);
            Assert.Equal(msg, _stringConstant.ChannelAddSuccess);
        }


        /// <summary>
        /// Method AddChannelManually Testing with Not Privaet Group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddChannelNotPrivateGroup()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(slackUserDetails.UserId, _stringConstant.GroupName, _stringConstant.Add + " " + _stringConstant.Channel + " " + _stringConstant.GroupName);
            Assert.Equal(msg, _stringConstant.OnlyPrivateChannel);
        }


        /// <summary>
        /// Method AddChannelManually Testing with No User
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddChannelNoUser()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);

            var msg = await _scrumBotRepository.ProcessMessages(slackUserDetails.UserId, _stringConstant.GroupNameStartsWith + slackChannelDetails.ChannelId, _stringConstant.Add + " " + _stringConstant.Channel + " " + _stringConstant.GroupName);
            Assert.Equal(msg, _stringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// Method AddChannelManually Testing with no project added
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddChannelNoProject()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(slackUserDetails.UserId, _stringConstant.GroupNameStartsWith + slackChannelDetails.ChannelId, _stringConstant.Add + " " + _stringConstant.Channel + " " + _stringConstant.GroupName);
            Assert.Equal(msg, _stringConstant.ProjectNotInOAuth);
        }


        #endregion


        #endregion

        private void AddChannelUser()
        {
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _slackUserRepository.AddSlackUser(testSlackUserDetails);
        }

        private async void UserProjectSetup()
        {
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(_stringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UsersDetailByGroupUrl ,_stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", _stringConstant.ProjectDetailsUrl, _stringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

        }


    }
}