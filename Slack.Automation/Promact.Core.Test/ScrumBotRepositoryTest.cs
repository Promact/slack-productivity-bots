using Autofac;
using Moq;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Threading.Tasks;
using Xunit;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Microsoft.AspNet.Identity;

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

        private ApplicationUser user = new ApplicationUser()
        {
            Email = StringConstant.EmailForTest,
            UserName = StringConstant.EmailForTest,
            SlackUserName = StringConstant.UserNameForTest,
        };

        private Question question = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 1,
            QuestionStatement = StringConstant.ScrumQuestionForTest,
            Type = 1
        };

        private Question question1 = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 2,
            QuestionStatement = StringConstant.ScrumQuestionForTest,
            Type = 1
        };

        private Scrum scrum = new Scrum()
        {
            CreatedOn = DateTime.UtcNow,
            ProjectId = Convert.ToInt32(StringConstant.ProjectIdForTest),
            GroupName = StringConstant.GroupName,
            ScrumDate = DateTime.UtcNow,
            TeamLeaderId = StringConstant.TeamLeaderIdForTest,
            IsHalted = false
        };

        private ScrumAnswer scrumAnswer = new ScrumAnswer()
        {
            Answer = StringConstant.NoQuestion,
            CreatedOn = DateTime.UtcNow,
            AnswerDate = DateTime.UtcNow,
            EmployeeId = StringConstant.UserIdForTest,
            Id = 1,
            QuestionId = 1,
            ScrumAnswerStatus = ScrumAnswerStatus.Answered,
            ScrumId = 1
        };

        private ApplicationUser testUser = new ApplicationUser()
        {
            Email = StringConstant.EmailForTest,
            UserName = StringConstant.EmailForTest,
            SlackUserName = StringConstant.TestUser,
        };

        public ScrumBotRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumBotRepository = _componentContext.Resolve<IScrumBotRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _scrumDataRepository = _componentContext.Resolve<IRepository<Scrum>>();
            _scrumAnswerDataRepository = _componentContext.Resolve<IRepository<ScrumAnswer>>();
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
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            _scrumDataRepository.Insert(scrum);
            _botQuestionRepository.AddQuestion(question);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal(msg, StringConstant.GoodDay + "<@" + StringConstant.LeaveApplicant + ">!\n" + StringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method StartScrum Testing without registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumNoUser()
        {
            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal(msg, StringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum and Scrum Answer with AnswerNow status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateAnswerNow()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            _scrumDataRepository.Insert(scrum);
            _botQuestionRepository.AddQuestion(question);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal(msg, StringConstant.NextQuestion);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoQuestion()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal(msg, StringConstant.NoQuestion);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoProject()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            var msg = await _scrumBotRepository.Scrum(StringConstant.UserNameForTest, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal(StringConstant.NoProjectFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with No Employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoEmployee()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal(StringConstant.NoEmployeeFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiate()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            _botQuestionRepository.AddQuestion(question);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal(StringConstant.GoodDay + "<@" + StringConstant.LeaveApplicant + ">!\n" + StringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrum()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            _botQuestionRepository.AddQuestion(question);

            _scrumDataRepository.Insert(scrum);
            scrum.ScrumDate = DateTime.UtcNow.Date.AddDays(-1);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            scrumAnswer.AnswerDate = DateTime.UtcNow.Date.AddDays(-1);
            scrumAnswer.ScrumId = 2;
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            string previousDayStatus = Environment.NewLine + StringConstant.PreviousDayStatus + Environment.NewLine;
            previousDayStatus += "*_Q_*: " + StringConstant.ScrumQuestionForTest + Environment.NewLine + "*_A_*: _" + StringConstant.NoQuestion + "_" + Environment.NewLine;
            previousDayStatus += Environment.NewLine + StringConstant.AnswerToday + Environment.NewLine + Environment.NewLine;

            Assert.Equal(StringConstant.GoodDay + "<@" + StringConstant.TestUser + ">!\n" + previousDayStatus + StringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing halted scrum 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasHaltedScrum()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal(StringConstant.ResumeScrum, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrumQuestionRemaining()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            _botQuestionRepository.AddQuestion(question);
            Question question1 = new Question
            {
                CreatedOn = DateTime.UtcNow,
                OrderNumber = 2,
                QuestionStatement = StringConstant.ScrumQuestionForTest,
                Type = 1
            };
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal("<@" + StringConstant.LeaveApplicant + "> " + StringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum but no question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrumNoQuestion()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Time);
            Assert.Equal(StringConstant.NoQuestion, msg);
        }


        #endregion


        #region Halt Scrum


        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumHalt()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Halt);
            Assert.Equal(StringConstant.ScrumHalted, msg);
        }


        /// <summary>
        /// Method Scrum Testing for already halted
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAlreadyHalted()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Halt);
            Assert.Equal(StringConstant.ScrumAlreadyHalted, msg);
        }



        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumHaltNoScrum()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Halt);
            Assert.Equal(StringConstant.ScrumNotStarted, msg);
        }


        #endregion


        #region Resume Scrum


        /// <summary>
        /// Method Scrum Testing for resume (getquestion)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResume()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Resume);
            var expectedString = string.Format(StringConstant.QuestionToNextEmployee, StringConstant.LeaveApplicant);
            Assert.Equal(StringConstant.ScrumResumed + expectedString, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume without a scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeNoScrum()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Resume);
            Assert.Equal(StringConstant.ScrumNotStarted, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume for unhalted scrum (getquestion)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeNotHalted()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = false;
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Resume);
            Assert.Equal(StringConstant.ScrumNotHalted + StringConstant.NoEmployeeFound, msg);
        }


        /// <summary>
        /// Method Scrum Testing for scrum exception case
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumException()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.UserName);
            Assert.Equal(StringConstant.ServerClosed, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume with previous day scrum status
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeWithPreviuosDayStatus()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            scrum.ScrumDate = scrum.ScrumDate.AddDays(-1);
            _scrumDataRepository.Insert(scrum);

            scrumAnswer.ScrumId = 2;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Scrum(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.Resume);
            Assert.Equal(StringConstant.PreviousDayStatusForTest, msg);
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
            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.LeaveApplicant, StringConstant.Leave);
            Assert.Equal(StringConstant.YouAreNotInExistInOAuthServer, msg);
        }


        /// <summary>
        /// Method Leave Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumHalted()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.LeaveApplicant, StringConstant.Leave);
            Assert.Equal(StringConstant.ResumeScrum, msg);
        }


        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void Leave()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.TestUser, StringConstant.LeaveApplicant, StringConstant.Leave);
            Assert.Equal(Environment.NewLine + StringConstant.GoodDay + "<@" + StringConstant.TestUser + ">!\n" + StringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant and applying employee are same
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveWithSameApplicant()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.LeaveApplicant, StringConstant.Leave);
            Assert.Equal(StringConstant.LeaveError, msg);
        }


        /// <summary>
        /// Method Leave Testing with no scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveNoScrum()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.LeaveApplicant, StringConstant.Leave);
            Assert.Equal(StringConstant.ScrumNotStarted, msg);
        }


        /// <summary>
        /// Method Leave Testing (where leave is applied on a group where scrum is already complete)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumAlreadyComplete()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);

            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.LeaveApplicant, StringConstant.Leave);
            Assert.Equal(StringConstant.ScrumAlreadyConducted, msg);
        }


        /// <summary>
        /// Method Leave Testing where applicant has already answered a question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumWithAnswer()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.UserNameForTest, StringConstant.Leave);
            string compareString = string.Format(StringConstant.AlreadyAnswered, StringConstant.UserNameForTest) + Environment.NewLine;
            Assert.Equal(compareString + StringConstant.NextQuestion, msg);
        }


        #endregion


        #region Later Test Cases 


        /// <summary>
        /// Method LeaveLater Testing with wrong employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLaterWithWrongEmployee()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.TestUser, StringConstant.TestUser, StringConstant.Later);
            string compareString = string.Format(StringConstant.PleaseAnswer, StringConstant.UserNameForTest);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method LeaveLater Testing with no scrum answer yet and answer expected message
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLaterWithoutScrumAnswer()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.TestUser, StringConstant.UserNameForTest, StringConstant.Later);
            //  string compareString = string.Format(StringConstant.PleaseAnswer, StringConstant.UserNameForTest) + Environment.NewLine;
            string questionString = Environment.NewLine + string.Format(StringConstant.QuestionToNextEmployee, StringConstant.TestUser);
            Assert.Equal(msg, questionString);
        }


        /// <summary>
        /// Method LeaveLater Testing with scrum answer already marked as later or to be answered now 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLater()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.TestUser, StringConstant.UserNameForTest, StringConstant.Later);
            string compareString = Environment.NewLine + string.Format(StringConstant.QuestionToNextEmployee, StringConstant.TestUser);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method Later Testing all answers are given but some marked as later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterWithSomeLaterAnswer()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);

            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.Later;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.TestUser, StringConstant.UserNameForTest, StringConstant.Later);
            Assert.Equal(StringConstant.ScrumConcludedButLater, msg);
        }


        #endregion


        #region Scrum Later Test Cases 


        /// <summary>
        /// Method LaterScrum Testing
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrum()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.Answer = StringConstant.Later;
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.TestUser, StringConstant.Scrum);
            var expectedString = string.Format("<@{0}> ", StringConstant.TestUser);
            Assert.Equal(msg, expectedString + StringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method LaterScrum Testing with Unrecognized employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumUnrecognizedEmployee()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.Answer = StringConstant.Later;
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.TestUserName, StringConstant.Scrum);
            Assert.Equal(msg, StringConstant.Unrecognized);
        }


        /// <summary>
        /// Method LaterScrum Testing with wrong employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumWrongEmployee()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.Answer = StringConstant.Later;
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.UserNameForTest, StringConstant.Scrum);
            string expectedString = string.Format(StringConstant.PleaseAnswer, StringConstant.TestUser) + Environment.NewLine;
            Assert.Equal(msg, expectedString);
        }


        /// <summary>
        /// Method LaterScrum Testing with few answered already
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumWithFewAnswered()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            scrumAnswer.Answer = StringConstant.Later;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.UserNameForTest, StringConstant.Scrum);
            Assert.Equal(msg, StringConstant.NextQuestion);
        }


        /// <summary>
        /// Method LaterScrum Testing with all answers already answered
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumWithAllAnswers()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.UserNameForTest, StringConstant.Scrum);
            string compareString = Environment.NewLine + string.Format(StringConstant.QuestionToNextEmployee, StringConstant.TestUser);
            Assert.Equal(msg, StringConstant.AllAnswerRecorded + compareString);
        }


        /// <summary>
        /// Method LaterScrum Testing with not marked as later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumNotLater()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.UserNameForTest, StringConstant.Scrum);
            string expectedString = string.Format(StringConstant.NotLaterYet, StringConstant.UserNameForTest);
            // expectedString += StringConstant.ScrumQuestionForTest;
            Assert.Equal(msg, expectedString + StringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method LaterScrum Testing with not marked as later with wrong applicant
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumNotLaterWrongApplicant()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.TestUser, StringConstant.Scrum);
            string expectedString = string.Format(StringConstant.PleaseAnswer, StringConstant.UserNameForTest) + Environment.NewLine;
            Assert.Equal(msg, expectedString);
        }


        /// <summary>
        /// Method LaterScrum Testing with Answers with status AnswerNow
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AnswerNowScrum()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.TestUser, StringConstant.Scrum);
            var expectedString = string.Format(StringConstant.PleaseAnswer, StringConstant.TestUser) + Environment.NewLine;
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
            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.Equal(msg, StringConstant.YouAreNotInExistInOAuthServer);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with first employee's first answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerFirstEmployeeFirstAnswer()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);

            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.Equal(msg, StringConstant.NextQuestion);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next answer of an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerNextAnswer()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);

            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
            string compareString = string.Format(StringConstant.QuestionToNextEmployee, StringConstant.TestUser);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next question to same employee as return message
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerNextQuestion()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            Question question2 = new Question()
            {
                CreatedOn = DateTime.UtcNow,
                OrderNumber = 3,
                QuestionStatement = StringConstant.ScrumQuestionForTest,
                Type = 1
            };
            _botQuestionRepository.AddQuestion(question2);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.Equal(msg, StringConstant.NextQuestion);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing (update answer) with only One Answer marked to be asked later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void UpdateScrumAnswer()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.Equal(msg, StringConstant.UpdateAnswer);
        }


        /// <summary>
        /// Method UpdateScrumAnswer Testing with more than one answer marked to be asked later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void UpdateScrumAnswers()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
            string compareString = "<@" + StringConstant.UserNameForTest + "> " + StringConstant.NoQuestion;
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next employee's first answer and scrum complete
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerScrumComplete()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.TestUser, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.Equal(msg, StringConstant.ScrumComplete);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with unexpected employee's answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumUnexpectedEmployeeAnswer()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
            string expectedString = string.Format(StringConstant.PleaseAnswer, StringConstant.TestUser);
            Assert.Equal(msg, expectedString);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with scrum complete but some employees marked as later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerScrumCompleteButLater()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.Later;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.TestUser, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.Equal(msg, StringConstant.ScrumConcludedButLater);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with normal conversation on slack channel
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void NormalConversation()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.TestUser, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.Equal(msg, string.Empty);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddScrumAnswerHaltedScrum()
        {
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.TestUser, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.Equal(msg, string.Empty);
        }


        #endregion


        #endregion


    }
}