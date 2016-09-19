using Autofac;
using Moq;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ScrumReportRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
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
        private IComponentContext _componentContext;
        private IScrumReportRepository _scrumReportRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        private IRepository<Scrum> _scrumDataRepository;
        private IRepository<ScrumAnswer> _scrumAnswerDataRepository;
        private IRepository<Question> _questionDataRepository;

        public ScrumReportRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumReportRepository = _componentContext.Resolve<IScrumReportRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _scrumDataRepository = _componentContext.Resolve<IRepository<Scrum>>(); 
            _scrumAnswerDataRepository = _componentContext.Resolve<IRepository<ScrumAnswer>>();
            _questionDataRepository = _componentContext.Resolve<IRepository<Question>>();
        }

        /// <summary>
        /// Method to test GetProjects when the logged in person is admin
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void GetProjectsAdminTest()
        {
            var response = Task.FromResult(StringConstant.AdminLogin);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var responseProjects = Task.FromResult(StringConstant.ProjectDetailsForAdminFromOauth);
            var requestUrlProjects = StringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, requestUrlProjects, StringConstant.TestAccessToken)).Returns(responseProjects);
            var projects = _scrumReportRepository.GetProjects(StringConstant.TestUserName, StringConstant.TestAccessToken).Result;
            Assert.Equal(1, projects.Count());  

        }

        /// <summary>
        /// Method to test GetProjects when the logged in person is team leader
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void GetProjectsTeamLeaderTest()
        {
            var response = Task.FromResult(StringConstant.TeamLeaderLogin);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var responseProjects = Task.FromResult(StringConstant.ProjectDetailsForTeamLeaderFromOauth);
            var requestUrlProjects = StringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, requestUrlProjects, StringConstant.TestAccessToken)).Returns(responseProjects);
            var projects = _scrumReportRepository.GetProjects(StringConstant.TestUserName, StringConstant.TestAccessToken).Result;
            Assert.Equal(1, projects.Count());

        }

        /// <summary>
        /// Method to test GetProjects when the logged in person is employee
        /// </summary>
        [Fact Trait("Category", "Required")]
        public void GetProjectsEmployeeTest()
        {
            var response = Task.FromResult(StringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var responseProjects = Task.FromResult(StringConstant.ProjectDetailsForEmployeeFromOauth);
            var requestUrlProjects = StringConstant.AllProjectUrl;
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, requestUrlProjects, StringConstant.TestAccessToken)).Returns(responseProjects);
            var projects = _scrumReportRepository.GetProjects(StringConstant.TestUserName, StringConstant.TestAccessToken).Result;
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
            var response = Task.FromResult(StringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(StringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", StringConstant.GetProjectDetails, testProjectId);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, requestProjectUrl, StringConstant.TestAccessToken)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetails(testProjectId,scrumDate, StringConstant.TestUserName, StringConstant.TestAccessToken).Result;
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
            var response = Task.FromResult(StringConstant.EmployeeLogin);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(StringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", StringConstant.GetProjectDetails, testProjectId);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, requestProjectUrl, StringConstant.TestAccessToken)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            _questionDataRepository.Insert(questionOne);
            _questionDataRepository.Save();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Save();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetails(testProjectId, scrumDate, StringConstant.TestUserName, StringConstant.TestAccessToken).Result;
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
            var response = Task.FromResult(StringConstant.AdminLogin);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(StringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", StringConstant.GetProjectDetails, testProjectId);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, requestProjectUrl, StringConstant.TestAccessToken)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            _questionDataRepository.Insert(questionTwo);
            _questionDataRepository.Save();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Save();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetails(testProjectId, scrumDate, StringConstant.TestUserName, StringConstant.TestAccessToken).Result;
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
            var response = Task.FromResult(StringConstant.TeamLeaderLoginDetails);
            var requestUrl = string.Format("{0}{1}", StringConstant.LoginUserDetail, StringConstant.TestUserName);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.UserUrl, requestUrl, StringConstant.TestAccessToken)).Returns(response);
            var responseProject = Task.FromResult(StringConstant.ProjectDetail);
            var requestProjectUrl = string.Format("{0}{1}", StringConstant.GetProjectDetails, testProjectId);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUrl, requestProjectUrl, StringConstant.TestAccessToken)).Returns(responseProject);
            _scrumDataRepository.Insert(scrum);
            _scrumDataRepository.Save();
            _questionDataRepository.Insert(questionThree);
            _questionDataRepository.Save();
            _scrumAnswerDataRepository.Insert(scrumAnswer);
            _scrumAnswerDataRepository.Save();
            var scrumProjectDetails = _scrumReportRepository.ScrumReportDetails(testProjectId, scrumDate, StringConstant.TestUserName, StringConstant.TestAccessToken).Result;
            Assert.NotNull(scrumProjectDetails);
        }

        /// <summary>
        /// Creating a mock scrum object
        /// </summary>
        Scrum scrum = new Scrum()
        {
            GroupName = StringConstant.TestGroupName,
            ScrumDate = new DateTime(2016,9,19),
            ProjectId = 1012,
            TeamLeaderId = StringConstant.TestId
        };

        /// <summary>
        /// Creating a mock ScrumAnswer object
        /// </summary>
        ScrumAnswer scrumAnswer = new ScrumAnswer()
        {
            EmployeeId = StringConstant.EmployeeIdForTest,
            ScrumId = 1,
            QuestionId = 1,
            Answer = StringConstant.TestAnswer,
            AnswerDate = new DateTime(2016,9,19),
            CreatedOn = DateTime.UtcNow,
            Id = 1
        };

        /// <summary>
        /// Creating the first question  for scrum
        /// </summary>
        Question questionOne = new Question
        {
            QuestionStatement = StringConstant.QuestionOne,
            Type = 1
        };

        /// <summary>
        /// Creating the second question for scrum
        /// </summary>
        Question questionTwo = new Question
        {
            QuestionStatement = StringConstant.QuestionTwo,
            Type = 1
        };

        /// <summary>
        /// Creating the third question for scrum
        /// </summary>
        Question questionThree = new Question
        {
            QuestionStatement = StringConstant.QuestionThree,
            Type = 1
        };
    }
}
