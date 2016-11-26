using Autofac;
using Moq;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ScrumReportRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class ScrumReportRepositoryTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IScrumReportRepository _scrumReportRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private readonly IRepository<Scrum> _scrumDataRepository;
        private readonly IRepository<ScrumAnswer> _scrumAnswerDataRepository;
        private readonly IRepository<Question> _questionDataRepository;
        private readonly IStringConstantRepository _stringConstant;

        private Scrum scrum = new Scrum();
        private ScrumAnswer scrumAnswer = new ScrumAnswer();
        private Question questionOne = new Question();
        private Question questionTwo = new Question();
        private Question questionThree = new Question();
        #endregion

        #region Constructor
        public ScrumReportRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumReportRepository = _componentContext.Resolve<IScrumReportRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _scrumDataRepository = _componentContext.Resolve<IRepository<Scrum>>();
            _scrumAnswerDataRepository = _componentContext.Resolve<IRepository<ScrumAnswer>>();
            _questionDataRepository = _componentContext.Resolve<IRepository<Question>>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Method to test GetProjects when the logged in person is admin
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void GetProjectsAdminTest()
        {
            var response = Task.FromResult(_stringConstant.AdminLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.LoginUserDetail, _stringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForAdminFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.TestAccessToken)).Returns(responseProjects);
            var projects = _scrumReportRepository.GetProjectsAsync(_stringConstant.TestUserName, _stringConstant.TestAccessToken).Result;
            Assert.Equal(1, projects.Count());

        }

        /// <summary>
        /// Method to test GetProjects when the logged in person is team leader
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void GetProjectsTeamLeaderTest()
        {
            var response = Task.FromResult(_stringConstant.TeamLeaderLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.LoginUserDetail, _stringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForTeamLeaderFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.TestAccessToken)).Returns(responseProjects);
            var projects = _scrumReportRepository.GetProjectsAsync(_stringConstant.TestUserName, _stringConstant.TestAccessToken).Result;
            Assert.Equal(1, projects.Count());

        }

        /// <summary>
        /// Method to test GetProjects when the logged in person is employee
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void GetProjectsEmployeeTest()
        {
            var response = Task.FromResult(_stringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.LoginUserDetail, _stringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var responseProjects = Task.FromResult(_stringConstant.ProjectDetailsForEmployeeFromOauth);
            var requestUrlProjects = _stringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestUrlProjects, _stringConstant.TestAccessToken)).Returns(responseProjects);
            var projects = _scrumReportRepository.GetProjectsAsync(_stringConstant.TestUserName, _stringConstant.TestAccessToken).Result;
            Assert.Equal(1, projects.Count());

        }


        /// <summary>
        /// Method to test ScrumReportDetails when the person is not available on the scrum date
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void ScrumReportDetailsPersonUnavailableTest()
        {
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 15);
            var response = Task.FromResult(_stringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.LoginUserDetail, _stringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.TestAccessToken)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.TestUserName, _stringConstant.TestAccessToken).Result;
            Assert.NotNull(scrumProjectDetails);
        }

        /// <summary>
        /// Method to test ScrumReportDetails when the logged in person is employee 
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void ScrumReportDetailsEmployeeTest()
        {
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 19);
            var response = Task.FromResult(_stringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.LoginUserDetail, _stringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.TestAccessToken)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            _questionDataRepository.Insert(questionOne);
            _questionDataRepository.Save();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Save();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.TestUserName, _stringConstant.TestAccessToken).Result;
            Assert.NotNull(scrumProjectDetails);
        }


        /// <summary>
        /// Method to test ScrumReportDetails when the logged in person is admin 
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void ScrumReportDetailsAdminTest()
        {
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 19);
            var response = Task.FromResult(_stringConstant.AdminLogin);
            var requestUrl = string.Format("{0}{1}", _stringConstant.LoginUserDetail, _stringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}",  testProjectId,_stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.TestAccessToken)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            _questionDataRepository.Insert(questionTwo);
            _questionDataRepository.Save();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Save();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.TestUserName, _stringConstant.TestAccessToken).Result;
            Assert.NotNull(scrumProjectDetails);
        }

        /// <summary>
        /// Method to test ScrumReportDetails when the logged in person is teamLeader 
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void ScrumReportDetailsTeamLeaderTest()
        {
            int testProjectId = 1012;
            DateTime scrumDate = new DateTime(2016, 9, 19);
            var response = Task.FromResult(_stringConstant.TeamLeaderLoginDetails);
            var requestUrl = string.Format("{0}{1}", _stringConstant.LoginUserDetail, _stringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.UserUrl, requestUrl, _stringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(_stringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", testProjectId, _stringConstant.GetProjectDetails);
            _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, requestProjectUrl, _stringConstant.TestAccessToken)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            _questionDataRepository.Insert(questionThree);
            _questionDataRepository.Save();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Save();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetailsAsync(testProjectId, scrumDate, _stringConstant.TestUserName, _stringConstant.TestAccessToken).Result;
            Assert.NotNull(scrumProjectDetails);
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialize()
        {
            scrum.GroupName = _stringConstant.TestGroupName;
            scrum.ScrumDate = new DateTime(2016, 9, 19);
            scrum.ProjectId = 1012;
            scrum.TeamLeaderId = _stringConstant.TestId;

            scrumAnswer.EmployeeId = _stringConstant.EmployeeIdForTest;
            scrumAnswer.ScrumId = 1;
            scrumAnswer.QuestionId = 1;
            scrumAnswer.Answer = _stringConstant.TestAnswer;
            scrumAnswer.AnswerDate = new DateTime(2016, 9, 19);
            scrumAnswer.CreatedOn = DateTime.UtcNow;
            scrumAnswer.Id = 1;

            questionOne.QuestionStatement = _stringConstant.ScrumFirstQuestion;
            questionOne.Type = 1;

            questionTwo.QuestionStatement = _stringConstant.ScrumSecondQuestion;
            questionTwo.Type = 1;

            questionThree.QuestionStatement = _stringConstant.ScrumThirdQuestion;
            questionThree.Type = 1;
        }
        #endregion
    }
}
