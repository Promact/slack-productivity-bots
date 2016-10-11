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
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

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
            SlackUserName = StringConstant.TestUser
        };

        private SlackUserDetails slackUserDetails = new SlackUserDetails()
        {
            UserId = StringConstant.StringIdForTest,
            Name = StringConstant.UserNameForTest,
            TeamId = StringConstant.PromactStringName
        };

        private SlackUserDetails testSlackUserDetails = new SlackUserDetails()
        {
            UserId = StringConstant.IdForTest,
            Name = StringConstant.TestUser,
            TeamId = StringConstant.PromactStringName
        };

        private SlackChannelDetails slackChannelDetails = new SlackChannelDetails()
        {
            ChannelId = StringConstant.SlackChannelIdForTest,
            Deleted = false,
            CreatedOn = DateTime.UtcNow,
            Name = StringConstant.GroupName
        };

        private void AddChannelUser()
        {
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _slackUserRepository.AddSlackUser(testSlackUserDetails);
        }

        private async void UserProjectSetup()
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

        }

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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
            Assert.Equal(msg, StringConstant.GoodDay + "<@" + StringConstant.LeaveApplicant + ">!\n" + StringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method StartScrum Testing without registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumNoUser()
        {
            AddChannelUser();
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
            Assert.Equal(msg, StringConstant.YouAreNotInExistInOAuthServer);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
            Assert.Equal(msg, StringConstant.NextQuestion);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoQuestion()
        {
            AddChannelUser();
            UserProjectSetup();

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
            Assert.Equal(msg, StringConstant.NoQuestion);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoProject()
        {
            slackChannelDetails.Name = StringConstant.UserNameForTest;
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);
            _slackUserRepository.AddSlackUser(slackUserDetails);

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
            Assert.Equal(StringConstant.NoProjectFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with No Employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoEmployee()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
            Assert.Equal(StringConstant.NoEmployeeFound, msg);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
            Assert.Equal(StringConstant.GoodDay + "<@" + StringConstant.LeaveApplicant + ">!\n" + StringConstant.ScrumQuestionForTest, msg);
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
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
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
            AddChannelUser();

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            UserProjectSetup();

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
            Assert.Equal(StringConstant.ResumeScrum, msg);
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
                QuestionStatement = StringConstant.ScrumQuestionForTest,
                Type = 1
            };
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
            Assert.Equal("<@" + StringConstant.LeaveApplicant + "> " + StringConstant.ScrumQuestionForTest, msg);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumTime);
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
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumHalt);
            Assert.Equal(StringConstant.ScrumHalted, msg);
        }


        /// <summary>
        /// Method Scrum Testing for already halted
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAlreadyHalted()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumHalt);
            Assert.Equal(StringConstant.ScrumAlreadyHalted, msg);
        }



        /// <summary>
        /// Method Scrum Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumHaltNoScrum()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumHalt);
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
            AddChannelUser();
            UserProjectSetup();
            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumResume);
            var expectedString = string.Format(StringConstant.QuestionToNextEmployee, StringConstant.LeaveApplicant);
            Assert.Equal(StringConstant.ScrumResumed + expectedString, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume without a scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeNoScrum()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumResume);
            Assert.Equal(StringConstant.ScrumNotStarted, msg);
        }


        /// <summary>
        /// Method Scrum Testing for resume for unhalted scrum (getquestion)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumResumeNotHalted()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            _botQuestionRepository.AddQuestion(question);

            scrum.IsHalted = false;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumResume);
            Assert.Equal(StringConstant.ScrumNotHalted + StringConstant.NoEmployeeFound, msg);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.ScrumResume);
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
            AddChannelUser();
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
            Assert.Equal(StringConstant.YouAreNotInExistInOAuthServer, msg);
        }


        /// <summary>
        /// Method Leave Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumHalted()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
            Assert.Equal(StringConstant.ResumeScrum, msg);
        }


        /// <summary>
        /// Method Leave Testing with halted scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumHaltedProcessMessage()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            scrum.IsHalted = true;
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " " + StringConstant.LeaveApplicant);
            Assert.Equal(String.Empty, msg);
        }



        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void Leave()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
            Assert.Equal(Environment.NewLine + StringConstant.GoodDay + "<@" + StringConstant.TestUser + ">!\n" + StringConstant.ScrumQuestionForTest, msg);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
            Assert.Equal(StringConstant.LeaveError, msg);
        }


        /// <summary>
        /// Method Leave Testing with no scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveNoScrum()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
            Assert.Equal(StringConstant.ScrumNotStarted, msg);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
            Assert.Equal(StringConstant.ScrumAlreadyConducted, msg);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
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
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.IdForTest + ">");
            string compareString = string.Format(StringConstant.PleaseAnswer, StringConstant.UserNameForTest);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method LeaveLater Testing with no scrum answer yet and answer expected message
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLaterWithoutScrumAnswer()
        {
            AddChannelUser();

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
            string questionString = Environment.NewLine + string.Format(StringConstant.QuestionToNextEmployee, StringConstant.TestUser);
            Assert.Equal(msg, questionString);
        }


        /// <summary>
        /// Method LeaveLater Testing with scrum answer already marked as later or to be answered now 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLater()
        {
            AddChannelUser();

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
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Later + " <@" + StringConstant.StringIdForTest + ">");

            string compareString = Environment.NewLine + string.Format(StringConstant.QuestionToNextEmployee, StringConstant.TestUser);
            Assert.Equal(msg, compareString);
        }


        /// <summary>
        /// Method LeaveLater Testing with scrum answer already marked as later or to be answered now but with unexpected user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumLeaveLaterNotExpected()
        {
            AddChannelUser();
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(testUser);
            await _userManager.AddLoginAsync(testUser.Id, info);

            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
            scrumAnswer.EmployeeId = StringConstant.IdForTest + StringConstant.StringIdForTest;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
            string compareString = string.Format(StringConstant.NotExpected, StringConstant.UserNameForTest);
            Assert.Equal(msg, compareString);
        }



        /// <summary>
        /// Method Later Testing all answers are given but some marked as later
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterWithSomeLaterAnswer()
        {
            AddChannelUser();
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Leave + " <@" + StringConstant.StringIdForTest + ">");
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
            AddChannelUser();
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _botQuestionRepository.AddQuestion(question1);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.Answer = StringConstant.Later;
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum + " <@" + StringConstant.IdForTest + ">");
            var expectedString = string.Format("<@{0}> ", StringConstant.TestUser);
            Assert.Equal(msg, expectedString + StringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method LaterScrum Testing with applicant not logged in with promact
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumApplicantNotLoggedIn()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum + " <@" + StringConstant.IdForTest + ">");
            Assert.Equal(msg, StringConstant.NotAUser);
        }



        /// <summary>
        /// Method LaterScrum Testing with Unrecognized employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LaterScrumUnrecognizedEmployee()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            slackUserDetails.Name = StringConstant.TestUserName;
            slackUserDetails.UserId = StringConstant.IdForTest;
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _slackChannelReposiroty.AddSlackChannel(slackChannelDetails);
            UserProjectSetup();

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            scrumAnswer.Answer = StringConstant.Later;
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum + " <@" + StringConstant.IdForTest + ">");
            Assert.Equal(msg, StringConstant.Unrecognized);
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

            scrumAnswer.Answer = StringConstant.Later;
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum + " <@" + StringConstant.StringIdForTest + ">");
            string expectedString = string.Format(StringConstant.PleaseAnswer, StringConstant.TestUser) + Environment.NewLine;
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
            scrumAnswer.Answer = StringConstant.Later;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum + " <@" + StringConstant.StringIdForTest + ">");
            Assert.Equal(msg, StringConstant.NextQuestion);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum + " <@" + StringConstant.StringIdForTest + ">");
            string compareString = Environment.NewLine + string.Format(StringConstant.QuestionToNextEmployee, StringConstant.TestUser);
            Assert.Equal(msg, StringConstant.AllAnswerRecorded + compareString);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum + " <@" + StringConstant.StringIdForTest + ">");
            string expectedString = string.Format(StringConstant.NotLaterYet, StringConstant.UserNameForTest);
            Assert.Equal(msg, expectedString + StringConstant.ScrumQuestionForTest);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum + " <@" + StringConstant.IdForTest + ">");
            string expectedString = string.Format(StringConstant.PleaseAnswer, StringConstant.UserNameForTest) + Environment.NewLine;
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
            scrumAnswer.EmployeeId = StringConstant.TestUserId;
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum + " <@" + StringConstant.IdForTest + ">");
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
            AddChannelUser();
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
            Assert.Equal(msg, StringConstant.YouAreNotInExistInOAuthServer);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
            Assert.Equal(msg, StringConstant.NextQuestion);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
            string compareString = string.Format(StringConstant.QuestionToNextEmployee, StringConstant.TestUser);
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
                QuestionStatement = StringConstant.ScrumQuestionForTest,
                Type = 1
            };
            _botQuestionRepository.AddQuestion(question2);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
            Assert.Equal(msg, StringConstant.NextQuestion);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
            Assert.Equal(msg, StringConstant.UpdateAnswer);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
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
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
            Assert.Equal(msg, StringConstant.ScrumComplete);
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

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
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
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
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

            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
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
            AddChannelUser();

            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.IdForTest, StringConstant.SlackChannelIdForTest, StringConstant.Scrum);
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
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.GroupName, StringConstant.ScrumHelp);
            Assert.Equal(msg, StringConstant.ScrumHelpMessage);
        }


        /// <summary>
        /// Method ProcessMessages Testing with not registered user
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerProcessNoUser()
        {
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.GroupName, StringConstant.ScrumHelp);
            Assert.Equal(msg, StringConstant.NotAUser);
        }


        /// <summary>
        /// Method ProcessMessages Testing with not registered channel
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerProcessNoChannel()
        {
            _slackUserRepository.AddSlackUser(slackUserDetails);
            var msg = await _scrumBotRepository.ProcessMessages(StringConstant.StringIdForTest, StringConstant.GroupName, StringConstant.GroupDetailsResponseText);
            Assert.Equal(msg, StringConstant.NotAProject);
        }


        #endregion


        #endregion


    }
}