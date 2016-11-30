using Autofac;
using Microsoft.AspNet.Identity;
using Moq;
using Promact.Core.Repository.BotQuestionRepository;

using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.Email;
using Promact.Erp.Util.HttpClient;
using Promact.Erp.Util.StringConstants;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class TaskMailRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly ITaskMailRepository _taskMailRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly Mock<IHttpClientService> _mockHttpClient;
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<TaskMail> _taskMailDataRepository;
        private readonly IRepository<TaskMailDetails> _taskMailDetailsDataRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly IStringConstantRepository _stringConstant;
        private SlackProfile profile = new SlackProfile();
        private SlackUserDetails slackUserDetails = new SlackUserDetails();
        private Question firstQuestion = new Question();
        private ApplicationUser user = new ApplicationUser();
        private TaskMail taskMail = new TaskMail();
        private TaskMail taskMailPrvious = new TaskMail();
        private TaskMailDetails taskMailDetails = new TaskMailDetails();
        private Question secondQuestion = new Question();
        private Question thirdQuestion = new Question();
        private Question forthQuestion = new Question();
        private Question fifthQuestion = new Question();
        private Question SixthQuestion = new Question();
        private Question SeventhQuestion = new Question();
        private EmailApplication email = new EmailApplication();
        #endregion

        #region Constructor
        public TaskMailRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _taskMailRepository = _componentContext.Resolve<ITaskMailRepository>();
            _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
            _botQuestionRepository = _componentContext.Resolve<IBotQuestionRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
            _userManager = _componentContext.Resolve<ApplicationUserManager>();
            _taskMailDataRepository = _componentContext.Resolve<IRepository<TaskMail>>();
            _taskMailDetailsDataRepository = _componentContext.Resolve<IRepository<TaskMailDetails>>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _mockEmailService = _componentContext.Resolve<Mock<IEmailService>>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test case for task mail start and ask first question for true result, First question of task mail
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task StartTaskMailAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            var responses = await _taskMailRepository.StartTaskMailAsync(_stringConstant.FirstNameForTest);
            Assert.Equal(responses, firstQuestion.QuestionStatement);
        }

        /// <summary>
        /// Test case for conduct task mail after started for true result, Request to task mail
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(null, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.RequestToStartTaskMail);
        }

        /// <summary>
        /// Test case for task mail start and ask first question for already start task mail scenario
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task StartTaskMailAlreadyStartAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            _taskMailDataRepository.Insert(taskMail);
            taskMailDetails.QuestionId = firstQuestion.Id;
            taskMailDetails.TaskId = taskMail.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            var responses = await _taskMailRepository.StartTaskMailAsync(_stringConstant.FirstNameForTest);
            Assert.Equal(responses, firstQuestion.QuestionStatement);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not answered first question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerFirstNotAnsweredAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(null, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.FirstQuestionForTest);
        }

        /// <summary>
        /// Test case for task mail start for already mail send for task mail scenario
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task StartTaskMailAlreadyMailSendAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            taskMailDetails.QuestionId = SeventhQuestion.Id;
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.SendEmailConfirmation = SendEmailConfirmation.yes;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            var responses = await _taskMailRepository.StartTaskMailAsync(_stringConstant.FirstNameForTest);
            Assert.Equal(responses, _stringConstant.AlreadyMailSend);
        }

        /// <summary>
        /// Test case for task mail start for User Does Not Exist for task mail
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task StartTaskMailUserDoesNotExistAsync()
        {
            var responses = await _taskMailRepository.StartTaskMailAsync(_stringConstant.FirstNameForTest);
            Assert.Equal(responses, _stringConstant.YouAreNotInExistInOAuthServer);
        }

        /// <summary>
        /// Test case for Question And Answer for User Does Not Exist for task mail
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerUserDoesNotExistAsync()
        {
            var responses = await _taskMailRepository.QuestionAndAnswerAsync(null, _stringConstant.FirstNameForTest);
            Assert.Equal(responses, _stringConstant.YouAreNotInExistInOAuthServer);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after first question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterFirstAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            _botQuestionRepository.AddQuestion(secondQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.TaskMailDescription, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.SecondQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered second question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerSecondNotAnsweredOrWrongAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(secondQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = secondQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(null, _stringConstant.FirstNameForTest);
            var text = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, _stringConstant.SecondQuestionForTest);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after second question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterSecondAnswerForStringAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(secondQuestion);
            _botQuestionRepository.AddQuestion(thirdQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = secondQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.TaskMailDescription, _stringConstant.FirstNameForTest);
            var text = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.TaskMailBotHourErrorMessage, Environment.NewLine, _stringConstant.SecondQuestionForTest);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after second question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterSecondAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(secondQuestion);
            _botQuestionRepository.AddQuestion(thirdQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = secondQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.HourSpentForTest, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.ThirdQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered third question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerThirdNotAnsweredOrWrongAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(thirdQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = thirdQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(null, _stringConstant.FirstNameForTest);
            var text = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.TaskMailBotStatusErrorMessage, Environment.NewLine, _stringConstant.ThirdQuestionForTest);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after third question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterThirdAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(thirdQuestion);
            _botQuestionRepository.AddQuestion(forthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = thirdQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.StatusOfWorkForTest, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.ForthQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered forth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerForthNotAnsweredOrWrongAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(forthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = forthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(null, _stringConstant.FirstNameForTest);
            Assert.Equal(response, forthQuestion.QuestionStatement);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after forth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterForthAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(forthQuestion);
            _botQuestionRepository.AddQuestion(fifthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = forthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.StatusOfWorkForTest, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.FifthQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered fifth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerFifthNotAnsweredOrWrongAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(fifthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = fifthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(null, _stringConstant.FirstNameForTest);
            var text = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, fifthQuestion.QuestionStatement);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after fifth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterFifthAnswerForYesAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(fifthQuestion);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = fifthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.SendEmailYesForTest, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.SixthQuestionForTest);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after fifth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterFifthAnswerForNoAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(fifthQuestion);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = fifthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.SendEmailNoForTest, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.ThankYou);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started but not or wrong answered sixth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerSixthNotAnsweredOrWrongAnswerAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = SixthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(null, _stringConstant.FirstNameForTest);
            var text = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.SendTaskMailConfirmationErrorMessage, Environment.NewLine, SixthQuestion.QuestionStatement);
            Assert.Equal(response, text);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after sixth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterSixthAnswerForYesAsync()
        {
            await mockAndUserCreateAsync();
            _mockEmailService.Setup(x => x.Send(It.IsAny<EmailApplication>()));
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = SixthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.SendEmailYesForTest, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.ThankYou);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after sixth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterSixthAnswerForNoAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = SixthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.SendEmailNoForTest, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.ThankYou);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after sixth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerAfterSendingMailAsync()
        {
            await mockAndUserCreateAsync();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = SeventhQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.SendEmailNoForTest, _stringConstant.FirstNameForTest);
            Assert.Equal(response, _stringConstant.RequestToStartTaskMail);
        }

        /// <summary>
        /// Mocking and User create used in all test cases
        /// </summary>
        /// <returns></returns>
        private async Task mockAndUserCreateAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = _stringConstant.ManagementDetailsUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
        }

        /// <summary>
        /// Test case for conduct task mail after started for task mail started after sixth question
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task QuestionAndAnswerForEmailServiceErrorAsync()
        {
            SmtpException ex = new SmtpException();
            await mockAndUserCreateAsync();
            _mockEmailService.Setup(x => x.Send(It.IsAny<EmailApplication>())).Throws<SmtpException>();
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(SixthQuestion);
            _botQuestionRepository.AddQuestion(SeventhQuestion);
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = SixthQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.SendEmailYesForTest, _stringConstant.FirstNameForTest);
            var expectReply = string.Format(_stringConstant.ReplyTextForSMTPExceptionErrorMessage, _stringConstant.ErrorOfEmailServiceFailureTaskMail, ex.Message.ToString());
            Assert.Equal(response, expectReply);
        }
        
        /// <summary>
        /// this test case for the task mail details 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void TaskMailDetailsReportAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", _stringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportAsync(user.Id, _stringConstant.RoleAdmin, _stringConstant.FirstNameForTest, user.Id);
            Assert.Equal(1, taskMailDetail.Count);
        }

        /// <summary>
        /// this test case for the task mail details 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task TaskMailDetailsReportForEmployeeAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", _stringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportAsync(user.Id, _stringConstant.RoleEmployee, _stringConstant.FirstNameForTest, user.Id);
            Assert.Equal(1, taskMailDetail.Count);
        }
        ///<summary>
        /// this test case for the task mail details for user role is team leader
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void TaskMailDetailsForSelectedDateForTeamLeaderAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", _stringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();

            var response = Task.FromResult(_stringConstant.TaskMailReportTeamLeader);
            var requestUrl = string.Format("{0}{1}", user.Id, _stringConstant.TeamMembersUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);


            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportAsync(user.Id, _stringConstant.RoleTeamLeader, _stringConstant.FirstNameForTest, user.Id);
            Assert.Equal(3, taskMailDetail.Count);
        }
        
        ///<summary>
        /// this test case for the task mail details for user role is team leader
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task TaskMailDetailsForSelectedDateForTeamLeaderAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", _stringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();

            var response = Task.FromResult(_stringConstant.TaskMailReportTeamLeader);
            var requestUrl = string.Format("{0}{1}", user.Id, _stringConstant.TeamMembersUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);

            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportSelectedDateAsync(user.Id, _stringConstant.FirstNameForTest, _stringConstant.RoleTeamLeader, Convert.ToString(DateTime.UtcNow), user.Id, Convert.ToString(DateTime.UtcNow));
            Assert.Equal(3, taskMailDetail.Count);
        }

        ///<summary>
        /// this test case for the task mail details 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task TaskMailDetailsForSelectedDateAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", _stringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            _slackUserRepository.AddSlackUser(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportSelectedDateAsync(user.Id, _stringConstant.FirstNameForTest,_stringConstant.RoleEmployee,Convert.ToString(DateTime.UtcNow), user.Id, Convert.ToString(DateTime.UtcNow));
            Assert.Equal(1, taskMailDetail.Count);
        }

            var response = Task.FromResult(_stringConstant.TaskMailReportTeamLeader);
            var requestUrl = string.Format("{0}{1}", user.Id, _stringConstant.TeamMembersUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);

            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportSelectedDateAsync(user.Id, _stringConstant.FirstNameForTest, _stringConstant.RoleTeamLeader, Convert.ToString(DateTime.UtcNow), user.Id, Convert.ToString(DateTime.UtcNow));
            Assert.Equal(3, taskMailDetail.Count);
        }

        ///<summary>
        /// this test case for the task mail details 
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task TaskMailDetailsForSelectedDateForAdminAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", _stringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportSelectedDateAsync(user.Id, _stringConstant.FirstNameForTest, _stringConstant.RoleAdmin, Convert.ToString(DateTime.UtcNow), user.Id, Convert.ToString(DateTime.UtcNow));
            Assert.Equal(1, taskMailDetail.Count);
        }

        /// <summary>
        /// get the user information.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void GetAllEmployeeAsync()
        {

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var response = Task.FromResult(_stringConstant.TaskMailReport);
            var requestUrl = string.Format("{0}{1}", user.Id, _stringConstant.UserRoleUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);

            var result = await _taskMailRepository.GetAllEmployeeAsync(user.Id);
            Assert.Equal(3, result.Count);
        }

        /// <summary>
        /// get the employee information for user role is TeamLeader.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetAllEmployeeForTeamLeaderAsync()
        {

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var response = Task.FromResult(_stringConstant.ListOfEmployeeForTeamLeader);
            var requestUrl = string.Format("{0}{1}", user.Id, _stringConstant.UserRoleUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);

            var result = await _taskMailRepository.GetAllEmployeeAsync(user.Id);
            Assert.Equal(3, result.Count);
        }

        /// <summary>
        /// get the employee information for user role is Employee.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetEmployeeInfromationAsync()
        {

            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);

            var response = Task.FromResult(_stringConstant.EmployeeInformation);
            var requestUrl = string.Format("{0}{1}", user.Id, _stringConstant.UserRoleUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.AccessTokenForTest)).Returns(response);

            var result = await _taskMailRepository.GetAllEmployeeAsync(user.Id);
            Assert.Equal(1, result.Count);
        }
        
        /// <summary>
        /// this test case for the task mail details for the selected date.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void TaskMailDetailsReportSelectedDateAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = _stringConstant.ManagementDetailsUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
             var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportSelectedDateAsync(user.Id, _stringConstant.FirstNameForTest, _stringConstant.RoleAdmin, Convert.ToString(DateTime.UtcNow), user.Id, Convert.ToString(DateTime.UtcNow));
            Assert.Equal(1, taskMailDetail.Count);
        }

        /// <summary>
        /// this test case for the task mail details for the next date.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void TaskMailDetailsReportNextPreviousDateAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = _stringConstant.ManagementDetailsUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();

            taskMailPrvious.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMailPrvious);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMailPrvious.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();

            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportNextPreviousDateAsync(user.Id, _stringConstant.FirstNameForTest, _stringConstant.RoleAdmin, Convert.ToString(DateTime.UtcNow), user.Id, _stringConstant.Previouspage);
            Assert.Equal(1, taskMailDetail.Count);
        }

        /// <summary>
        /// this test case for the task mail details for the next date.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task TaskMailDetailsReportNextPreviousDateForEmployeeAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = _stringConstant.ManagementDetailsUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMail.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();

            taskMailPrvious.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMailPrvious);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMailPrvious.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();

            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportNextPreviousDateAsync(user.Id, _stringConstant.FirstNameForTest, _stringConstant.RoleEmployee, Convert.ToString(DateTime.UtcNow), user.Id, _stringConstant.Previouspage);
            Assert.Equal(1, taskMailDetail.Count);
        }

        /// <summary>
        /// this test case for the task mail details for the next date.
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task TaskMailDetailsReportNextPreviousDateForTeamLeaderAsync()
        {
            var userResponse = Task.FromResult(_stringConstant.UserDetailsFromOauthServer);
            var userRequestUrl = string.Format("{0}{1}", _stringConstant.UserDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, userRequestUrl, _stringConstant.AccessTokenForTest)).Returns(userResponse);
            var teamLeaderResponse = Task.FromResult(_stringConstant.TeamLeaderDetailsFromOauthServer);
            var teamLeaderRequestUrl = string.Format("{0}{1}", _stringConstant.TeamLeaderDetailsUrl, _stringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, teamLeaderRequestUrl, _stringConstant.AccessTokenForTest)).Returns(teamLeaderResponse);
            var managementResponse = Task.FromResult(_stringConstant.ManagementDetailsFromOauthServer);
            var managementRequestUrl = string.Format("{0}", _stringConstant.ManagementDetailsUrl);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUserUrl, managementRequestUrl, _stringConstant.AccessTokenForTest)).Returns(managementResponse);
            await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
            _botQuestionRepository.AddQuestion(firstQuestion);
            UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, info);
            taskMail.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMail);
            _taskMailDataRepository.Save();

            taskMailPrvious.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMailPrvious);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMailPrvious.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();
            TaskMail newTaskMail = new TaskMail()
            {
                CreatedOn = DateTime.UtcNow,
                EmployeeId = _stringConstant.StringIdForTest
            };
            _taskMailDataRepository.Insert(newTaskMail);
            _taskMailDataRepository.Save();
            TaskMailDetails newTaskMailDetails = new TaskMailDetails();
            newTaskMailDetails.TaskId = newTaskMail.Id;
            newTaskMailDetails.QuestionId = secondQuestion.Id;
            _taskMailDetailsDataRepository.Insert(newTaskMailDetails);
            _taskMailDetailsDataRepository.Save();
            var expectedResponse = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.HourLimitExceed, Environment.NewLine, SixthQuestion.QuestionStatement);
            var response = await _taskMailRepository.QuestionAndAnswerAsync(_stringConstant.HourSpentForTesting, _stringConstant.FirstNameForTest);
            Assert.Equal(response, expectedResponse);
        }
            taskMailPrvious.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMailPrvious);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMailPrvious.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();

            taskMailPrvious.EmployeeId = user.Id;
            _taskMailDataRepository.Insert(taskMailPrvious);
            _taskMailDataRepository.Save();
            taskMailDetails.TaskId = taskMailPrvious.Id;
            taskMailDetails.QuestionId = firstQuestion.Id;
            _taskMailDetailsDataRepository.Insert(taskMailDetails);
            _taskMailDetailsDataRepository.Save();

            var taskMailDetail = await _taskMailRepository.TaskMailDetailsReportNextPreviousDateAsync(user.Id, _stringConstant.FirstNameForTest, _stringConstant.RoleAdmin, Convert.ToString(DateTime.UtcNow), user.Id, _stringConstant.Previouspage);
            Assert.Equal(1, taskMailDetail.Count);
        }

        #endregion

        #region Initialisation
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
            slackUserDetails.Profile = profile;

            firstQuestion.CreatedOn = DateTime.UtcNow;
            firstQuestion.OrderNumber = 1;
            firstQuestion.QuestionStatement = _stringConstant.FirstQuestionForTest;
            firstQuestion.Type = 2;

            user.Id = "1";
            user.Email = _stringConstant.EmailForTest;
            user.UserName = _stringConstant.EmailForTest;
            user.SlackUserId = _stringConstant.FirstNameForTest;
            user.Id = _stringConstant.TestUserId;

            taskMail.CreatedOn = DateTime.UtcNow;
            taskMail.EmployeeId = _stringConstant.StringIdForTest;

            taskMailPrvious.CreatedOn = DateTime.UtcNow.AddDays(-1);

            taskMailDetails.Comment = _stringConstant.CommentAndDescriptionForTest;
            taskMailDetails.Description = _stringConstant.CommentAndDescriptionForTest;
            taskMailDetails.Hours = Convert.ToDecimal(_stringConstant.StringHourForTest);
            taskMailDetails.SendEmailConfirmation = SendEmailConfirmation.no;
            taskMailDetails.Status = TaskMailStatus.completed;

            secondQuestion.CreatedOn = DateTime.UtcNow;
            secondQuestion.OrderNumber = 2;
            secondQuestion.QuestionStatement = _stringConstant.SecondQuestionForTest;
            secondQuestion.Type = 2;

            thirdQuestion.CreatedOn = DateTime.UtcNow;
            thirdQuestion.OrderNumber = 3;
            thirdQuestion.QuestionStatement = _stringConstant.ThirdQuestionForTest;
            thirdQuestion.Type = 2;

            forthQuestion.CreatedOn = DateTime.UtcNow;
            forthQuestion.OrderNumber = 4;
            forthQuestion.QuestionStatement = _stringConstant.ForthQuestionForTest;
            forthQuestion.Type = 2;

            fifthQuestion.CreatedOn = DateTime.UtcNow;
            fifthQuestion.OrderNumber = 5;
            fifthQuestion.QuestionStatement = _stringConstant.FifthQuestionForTest;
            fifthQuestion.Type = 2;



            SixthQuestion.CreatedOn = DateTime.UtcNow;
            SixthQuestion.OrderNumber = 6;
            SixthQuestion.QuestionStatement = _stringConstant.SixthQuestionForTest;
            SixthQuestion.Type = 2;


            SeventhQuestion.CreatedOn = DateTime.UtcNow;
            SeventhQuestion.OrderNumber = 7;
            SeventhQuestion.QuestionStatement = _stringConstant.SeventhQuestionForTest;
            SeventhQuestion.Type = 2;

            email.From = _stringConstant.ManagementEmailForTest;
            email.Subject = _stringConstant.TaskMailSubject;
        }
        #endregion
    }
}
