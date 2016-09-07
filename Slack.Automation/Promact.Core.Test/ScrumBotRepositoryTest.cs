using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Threading.Tasks;
using Xunit;

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
            TeamLeaderId = StringConstant.TeamLeaderIdForTest
        };

        private ScrumAnswer scrumAnswer = new ScrumAnswer()
        {
            Answer = StringConstant.ScrumHelpMessage,
            CreatedOn = DateTime.UtcNow,
            AnswerDate = DateTime.UtcNow,
            EmployeeId = StringConstant.UserIdForTest,
            Id = 1,
            QuestionId = 1,
            ScrumId = 1
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


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoQuestion()
        {
            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserNameForTest);
            Assert.Equal(msg, StringConstant.NoQuestion);
        }


        /// <summary>
        /// Method StartScrum Testing with No Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoProject()
        {
            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.UserNameForTest, StringConstant.UserNameForTest);
            Assert.Equal(StringConstant.NoProjectFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with No Employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoEmployee()
        {
            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserNameForTest);
            Assert.Equal(StringConstant.NoEmployeeFound, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiate()
        {
            var projectResponse = Task.FromResult(StringConstant.ProjectDetailsFromOauth);
            var projectRequestUrl = string.Format("{0}{1}", StringConstant.ProjectDetailsUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, projectRequestUrl, StringConstant.AccessTokenForTest)).Returns(projectResponse);

            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            _botQuestionRepository.AddQuestion(question);
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserNameForTest);
            Assert.Equal(StringConstant.GoodDay + "<@" + StringConstant.LeaveApplicant + ">!\n" + StringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrum()
        {
            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);
            _scrumAnswerDataRepository.Insert(scrumAnswer);

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserNameForTest);
            Assert.Equal(StringConstant.GoodDay + "<@" + StringConstant.TestUser + ">!\n" + StringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrumQuestionRemaining()
        {
            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

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

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserNameForTest);
            Assert.Equal("<@" + StringConstant.LeaveApplicant + "> " + StringConstant.ScrumQuestionForTest, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum but no employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrumNoEmployee()
        {
            _scrumDataRepository.Insert(scrum);
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserNameForTest);
            Assert.Equal(msg, StringConstant.NoEmployeeFound);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum but no question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrumNoQuestion()
        {
            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            _scrumDataRepository.Insert(scrum);

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserNameForTest);
            Assert.Equal(StringConstant.NoQuestion, msg);
        }


        /// <summary>
        /// Method StartScrum Testing with existing scrum but no scrum answer yet
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateHasScrumNoAnswer()
        {
            var userResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);

            _scrumDataRepository.Insert(scrum);
            _botQuestionRepository.AddQuestion(question);

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserNameForTest);
            Assert.Equal(msg, StringConstant.GoodDay + "<@" + StringConstant.LeaveApplicant + ">!\n" + StringConstant.ScrumQuestionForTest);
        }


        /// <summary>
        /// Method Leave Testing 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void Leave()
        {
            {
                var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
                var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
                _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

                _botQuestionRepository.AddQuestion(question);
                _scrumDataRepository.Insert(scrum);

                UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user.Id, info);

                var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.LeaveApplicant);
                Assert.Equal(StringConstant.GoodDay + "<@" + StringConstant.TestUser + ">!\n" + StringConstant.ScrumQuestionForTest, msg);
            }
        }


        /// <summary>
        /// Method Leave Testing with no scrum
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveNoScrum()
        {
            {
                UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user.Id, info);

                var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.LeaveApplicant);
                Assert.Equal(StringConstant.ScrumNotStarted, msg);
            }
        }


        /// <summary>
        /// Method Leave Testing (where leave is applied on a group where scrum is already complete)
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumAlreadyComplete()
        {
            {
                var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
                var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
                _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

                _botQuestionRepository.AddQuestion(question);

                _scrumDataRepository.Insert(scrum);
                _scrumAnswerDataRepository.Insert(scrumAnswer);
                _scrumAnswerDataRepository.Insert(scrumAnswer);

                UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user.Id, info);

                var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.LeaveApplicant);
                Assert.Equal(StringConstant.ScrumAlreadyConducted, msg);
            }
        }


        /// <summary>
        /// Method Leave Testing where applicant has already answered a question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveScrumWithAnswer()
        {
            {
                var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
                var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
                _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

                _botQuestionRepository.AddQuestion(question);
                _botQuestionRepository.AddQuestion(question1);
                _scrumDataRepository.Insert(scrum);
                _scrumAnswerDataRepository.Insert(scrumAnswer);

                UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user.Id, info);

                var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserNameForTest, StringConstant.UserNameForTest);
                string compareString = string.Format(StringConstant.AlreadyAnswered, StringConstant.UserNameForTest);
                Assert.Equal(compareString + StringConstant.NextQuestion, msg);
            }
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with only One Question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddScrumAnswer()
        {
            var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
            var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

            _botQuestionRepository.AddQuestion(question);
            _scrumDataRepository.Insert(scrum);

            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
            //msg = "Good Day <@pranali>!\nWhat did you do yesterday?";
            Assert.Equal(StringConstant.QuestionToNextEmployee, msg);
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with first employee's first answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerFirstEmployeeFirstAnswer()
        {
            {
                var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
                var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
                _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

                _botQuestionRepository.AddQuestion(question);
                _botQuestionRepository.AddQuestion(question1);

                _scrumDataRepository.Insert(scrum);
                _scrumDataRepository.Save();

                UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user.Id, info);

                var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
                Assert.Equal(msg, StringConstant.NextQuestion);
            }
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next answer of an employee
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerNextAnswer()
        {
            {
                var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
                var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
                _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

                _botQuestionRepository.AddQuestion(question);
                _botQuestionRepository.AddQuestion(question1);

                _scrumDataRepository.Insert(scrum);
                _scrumAnswerDataRepository.Insert(scrumAnswer);

                UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user.Id, info);

                var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
                Assert.Equal(msg, StringConstant.QuestionToNextEmployee);
            }
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next question to same employee as return message
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerNextQuestion()
        {
            {
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

                UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user.Id, info);

                var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserNameForTest, StringConstant.AnswerStatement, StringConstant.GroupName);
                Assert.Equal(msg, StringConstant.NextQuestion);
            }
        }


        /// <summary>
        /// Method AddScrumAnswer Testing with next employee's first answer
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumAnswerScrumComplete()
        {
            {
                var usersListResponse = Task.FromResult(StringConstant.EmployeesListFromOauth);
                var usersListRequestUrl = string.Format("{0}{1}", StringConstant.UsersDetailByGroupUrl, StringConstant.GroupName);
                _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, usersListRequestUrl, StringConstant.AccessTokenForTest)).Returns(usersListResponse);

                _botQuestionRepository.AddQuestion(question);
                _scrumDataRepository.Insert(scrum);
                _scrumAnswerDataRepository.Insert(scrumAnswer);

                UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
                ApplicationUser user1 = user;
                user1.SlackUserName = StringConstant.TestUser;
                await _userManager.CreateAsync(user1);
                await _userManager.AddLoginAsync(user1.Id, info);

                var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.TestUser, StringConstant.AnswerStatement, StringConstant.GroupName);
                Assert.Equal(msg, StringConstant.ScrumComplete);
            }
        }


        #endregion


    }
}



