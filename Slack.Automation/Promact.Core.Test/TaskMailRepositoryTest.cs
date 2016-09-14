using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.Email;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class TaskMailRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ITaskMailRepository _taskMailRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<TaskMail> _taskMailDataRepository;
        private readonly IRepository<TaskMailDetails> _taskMailDetailsDataRepository;
        private readonly Mock<IEmailService> _mockEmailService;

        public TaskMailRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _taskMailRepository = _componentContext.Resolve<ITaskMailRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _taskMailDataRepository = _componentContext.Resolve<IRepository<TaskMail>>();
            _taskMailDetailsDataRepository = _componentContext.Resolve<IRepository<TaskMailDetails>>();
            _mockEmailService = _componentContext.Resolve<Mock<IEmailService>>();
        }

        /// <summary>
        /// Test case for task mail start and ask first question for true result, First question of task mail
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void StartTaskMail()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            var responses = await _taskMailRepository.StartTaskMail(StringConstant.FirstNameForTest);
            Assert.Equal(responses, firstQuestion.QuestionStatement);
        }

        /// <summary>
        /// Test case for conduct task mail after started for true result, Request to task mail
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, null);
            Assert.Equal(response, StringConstant.RequestToStartTaskMail);
        }

        /// <summary>
        /// Test case for task mail start and ask first question for already start task mail scenario
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void StartTaskMailAlreadyStart()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            _taskMailDataRepository.Insert(taskMail);
            taskMailDetails.QuestionId = firstQuestion.Id;
            taskMailDetails.TaskId = taskMail.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            var responses = await _taskMailRepository.StartTaskMail(StringConstant.FirstNameForTest);
            Assert.Equal(responses, firstQuestion.QuestionStatement);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not answered first question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerFirstNotAnswered()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, null);
            Assert.Equal(response, StringConstant.FirstQuestionForTest);
        }

        /// <summary>
        /// Test case for task mail start for already mail send for task mail scenario
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void StartTaskMailAlreadyMailSend()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            taskMailDetails.QuestionId = SeventhQuestion.Id;
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.SendEmailConfirmation = SendEmailConfirmation.yes;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            var responses = await _taskMailRepository.StartTaskMail(StringConstant.FirstNameForTest);
            Assert.Equal(responses, StringConstant.AlreadyMailSend);
        }

        /// <summary>
        /// Test case for task mail start for User Does Not Exist for task mail
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void StartTaskMailUserDoesNotExist()
        {
            var responses = await _taskMailRepository.StartTaskMail(StringConstant.FirstNameForTest);
            Assert.Equal(responses, StringConstant.YouAreNotInExistInOAuthServer);
        }

        /// <summary>
        /// Test case for Question And Answer for User Does Not Exist for task mail
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerUserDoesNotExist()
        {
            var responses = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, null);
            Assert.Equal(responses, StringConstant.YouAreNotInExistInOAuthServer);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after first question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterFirstAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            _botQuestionRepository.AddQuestion(secondQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.TaskMailDescription);
            Assert.Equal(response, StringConstant.SecondQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered second question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerSecondNotAnsweredOrWrongAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(secondQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = secondQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, null);
            var text = string.Format("{0}{1}{2}", StringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, StringConstant.SecondQuestionForTest);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after second question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterSecondAnswerForStringAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(secondQuestion);
            _botQuestionRepository.AddQuestion(thirdQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = secondQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.TaskMailDescription);
            var text = string.Format("{0}{1}{2}", StringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, StringConstant.SecondQuestionForTest);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after second question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterSecondAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(secondQuestion);
            _botQuestionRepository.AddQuestion(thirdQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = secondQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.HourSpentForTest);
            Assert.Equal(response, StringConstant.ThirdQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered third question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerThirdNotAnsweredOrWrongAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(thirdQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = thirdQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, null);
            var text = string.Format("{0}{1}{2}", StringConstant.TaskMailBotStatusErrorMessage, Environment.NewLine, StringConstant.ThirdQuestionForTest);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after third question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterThirdAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(thirdQuestion);
            _botQuestionRepository.AddQuestion(forthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = thirdQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.StatusOfWorkForTest);
            Assert.Equal(response, StringConstant.ForthQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered forth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerForthNotAnsweredOrWrongAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(forthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = forthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, null);
            Assert.Equal(response, forthQuestion.QuestionStatement);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after forth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterForthAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(forthQuestion);
            _botQuestionRepository.AddQuestion(fifthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = forthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.StatusOfWorkForTest);
            Assert.Equal(response, StringConstant.FifthQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered fifth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerFifthNotAnsweredOrWrongAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(fifthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = fifthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, null);
            var text = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, fifthQuestion.QuestionStatement);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after fifth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterFifthAnswerForYes()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(fifthQuestion);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = fifthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.SendEmailYesForTest);
            Assert.Equal(response, StringConstant.SixthQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after fifth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterFifthAnswerForNo()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(fifthQuestion);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = fifthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.SendEmailNoForTest);
            Assert.Equal(response, StringConstant.ThankYou);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered sixth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerSixthNotAnsweredOrWrongAnswer()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = SixthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, null);
            var text = string.Format("{0}{1}{2}", StringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, SixthQuestion.QuestionStatement);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after sixth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterSixthAnswerForYes()
        {
            await mockAndUserCreate();
            _mockEmailService.Setup(x => x.Send(It.IsAny<EmailApplication>()));
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = SixthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.SendEmailYesForTest);
            Assert.Equal(response, StringConstant.ThankYou);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after sixth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterSixthAnswerForNo()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = SixthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.SendEmailNoForTest);
            Assert.Equal(response, StringConstant.ThankYou);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after sixth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void QuestionAndAnswerAfterSendingMail()
        {
            await mockAndUserCreate();
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = SeventhQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswer(StringConstant.FirstNameForTest, StringConstant.SendEmailNoForTest);
            Assert.Equal(response, StringConstant.RequestToStartTaskMail);
        }

        /// <summary>
        /// Mocking and User create used in all test cases
        /// </summary>
        /// <returns></returns>
        private async Task mockAndUserCreate()
        {
            var userResponse = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, userRequestUrl, StringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(StringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", StringConstant.TeamLeaderDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, teamLeaderRequestUrl, StringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(StringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", StringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, managementRequestUrl, StringConstant.AccessTokenForTest)).Returns(managementResponse);
            UserLoginInfo info = new UserLoginInfo(StringConstant.PromactStringName, StringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

        }

        /// <summary>
        /// Private variable slack user details
        /// </summary>
        private SlackUserDetails slackUserDetails = new SlackUserDetails()
        {
            UserId = StringConstant.StringIdForTest,
            Name = StringConstant.FirstNameForTest,
            TeamId = StringConstant.PromactStringName
        };

        /// <summary>
        /// Private variable question to be used for test cases
        /// </summary>
        private Question firstQuestion = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 1,
            QuestionStatement = StringConstant.FirstQuestionForTest,
            Type = 2
        };

        /// <summary>
        /// Private user to be used in test cases
        /// </summary>
        private ApplicationUser user = new ApplicationUser()
        {
            Email = StringConstant.EmailForTest,
            UserName = StringConstant.EmailForTest,
            SlackUserName = StringConstant.FirstNameForTest,
        };

        /// <summary>
        /// Private variable task mail to be use in test cases
        /// </summary>
        private TaskMail taskMail = new TaskMail()
        {
            CreatedOn = DateTime.UtcNow,
            EmployeeId = StringConstant.StringIdForTest
        };

        /// <summary>
        /// Private variable task mail details to be used in test cases
        /// </summary>
        private TaskMailDetails taskMailDetails = new TaskMailDetails()
        {
            Comment = StringConstant.CommentAndDescriptionForTest,
            Description = StringConstant.CommentAndDescriptionForTest,
            Hours = Convert.ToDecimal(StringConstant.StringHourForTest),
            SendEmailConfirmation = SendEmailConfirmation.no,
            Status = TaskMailStatus.completed,
        };

        /// <summary>
        /// Private variable question to be used for test cases
        /// </summary>
        private Question secondQuestion = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 2,
            QuestionStatement = StringConstant.SecondQuestionForTest,
            Type = 2
        };

        /// <summary>
        /// Private variable question to be used for test cases
        /// </summary>
        private Question thirdQuestion = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 3,
            QuestionStatement = StringConstant.ThirdQuestionForTest,
            Type = 2
        };

        /// <summary>
        /// Private variable question to be used for test cases
        /// </summary>
        private Question forthQuestion = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 4,
            QuestionStatement = StringConstant.ForthQuestionForTest,
            Type = 2
        };

        /// <summary>
        /// Private variable question to be used for test cases
        /// </summary>
        private Question fifthQuestion = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 5,
            QuestionStatement = StringConstant.FifthQuestionForTest,
            Type = 2
        };

        /// <summary>
        /// Private variable question to be used for test cases
        /// </summary>
        private Question SixthQuestion = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 6,
            QuestionStatement = StringConstant.SixthQuestionForTest,
            Type = 2
        };

        /// <summary>
        /// Private variable question to be used for test cases
        /// </summary>
        private Question SeventhQuestion = new Question()
        {
            CreatedOn = DateTime.UtcNow,
            OrderNumber = 7,
            QuestionStatement = StringConstant.SeventhQuestionForTest,
            Type = 2
        };

        EmailApplication email = new EmailApplication()
        {
            From = StringConstant.ManagementEmailForTest,
            Subject = StringConstant.TaskMailSubject
        };
    }
}
