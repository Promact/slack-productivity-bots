using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Promact.Core.Repository.OauthCallsRepository;
using System.Data.Entity;

namespace Promact.Core.Repository.ScrumReportRepository
{
    public class ScrumReportRepository : IScrumReportRepository
    {
        #region Private Variables
        private readonly IRepository<Scrum> _scrumDataRepository;
        private readonly IRepository<ScrumAnswer> _scrumAnswerDataRepository;
        private readonly IOauthCallHttpContextRespository _oauthCallsRepository;
        private readonly IStringConstantRepository _stringConstant;
        #endregion

        #region Constructor
        public ScrumReportRepository(IRepository<Scrum> scrumDataRepository, 
            IRepository<ScrumAnswer> scrumAnswerDataRepository, IStringConstantRepository stringConstant, 
            IOauthCallHttpContextRespository oauthCallsRepository)
        {
            _scrumDataRepository = scrumDataRepository;
            _scrumAnswerDataRepository = scrumAnswerDataRepository;
            _oauthCallsRepository = oauthCallsRepository;
            _stringConstant = stringConstant;
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Method to return list of employees in the project with their scrum answers based on role of logged in user
        /// </summary>
        /// <param name="project"></param>
        /// <param name="scrum"></param>
        /// <param name="loginUser"></param>
        /// <param name="scrumDate"></param>
        /// <returns>Object with list of employees in project with answers to scrum questions</returns>
        private async Task<IList<EmployeeScrumDetails>> GetEmployeeScrumDetailsAsync(ProjectAc project, Scrum scrum, User loginUser, DateTime scrumDate)
        {
            List<EmployeeScrumDetails> employeeScrumDetails = new List<EmployeeScrumDetails>();
            //Assigning answers of scrum to employees in the project based on their role
            //If logged user is an employee it will return only his scrum answers
            if (loginUser.Role.Equals(_stringConstant.Employee))
            {
                foreach (var user in project.ApplicationUsers)
                {
                    if (user.Id.Equals(loginUser.Id))
                    {
                        EmployeeScrumDetails employeeScrumDetail = await AssignAnswersAsync(scrum, scrumDate, user);
                        employeeScrumDetails.Add(employeeScrumDetail);
                    }
                }
            }
            //If logged user is admin or teamleader it will return scrum answers for the entire team
            else
            {
                foreach (var user in project.ApplicationUsers)
                {
                    EmployeeScrumDetails employeeScrumDetail = await AssignAnswersAsync(scrum, scrumDate, user);
                    employeeScrumDetails.Add(employeeScrumDetail);
                }
            }
            return employeeScrumDetails;
        }


        /// <summary>
        /// Method to assign scrum answers for a specific date to a particular employee
        /// </summary>
        /// <param name="scrum"></param>
        /// <param name="scrumDate"></param>
        /// <param name="user"></param>
        /// <returns>object with scrum answers for an employee</returns>
        private async Task<EmployeeScrumDetails> AssignAnswersAsync(Scrum scrum, DateTime scrumDate, User user)
        {
            EmployeeScrumDetails employeeScrumDetail = new EmployeeScrumDetails();
            //Fetch all the scrum answers for a particular employee
            List<ScrumAnswer> scrumAnswers = (await _scrumAnswerDataRepository.FetchAsync(x => x.EmployeeId == user.Id && DbFunctions.TruncateTime(x.AnswerDate) == scrumDate)).ToList();
            //Find scrum answers for a particular employee of a particular project on a specific date 
            List<ScrumAnswer> todayScrumAnswers = scrumAnswers.FindAll(x => x.ScrumId == scrum.Id).ToList();
            employeeScrumDetail.EmployeeName = string.Format("{0} {1}", user.FirstName, user.LastName);
            //Assigning answers to specific scrum questions
            if (!todayScrumAnswers.Any())
            {
                employeeScrumDetail.Status = string.Format(_stringConstant.PersonNotAvailable, scrumDate.ToString(_stringConstant.FormatForDate));
            }
            foreach (var todayScrumAnswer in todayScrumAnswers)
            {
                if (todayScrumAnswer.Question.Type == BotQuestionType.Scrum && todayScrumAnswer.Question.OrderNumber == QuestionOrder.Yesterday)
                {
                    employeeScrumDetail.Answer1 = SplitScrumAnswer(todayScrumAnswer.Answer);
                }
                if (todayScrumAnswer.Question.Type == BotQuestionType.Scrum && todayScrumAnswer.Question.OrderNumber == QuestionOrder.Today)
                {
                    employeeScrumDetail.Answer2 = SplitScrumAnswer(todayScrumAnswer.Answer);
                }
                if (todayScrumAnswer.Question.Type == BotQuestionType.Scrum && todayScrumAnswer.Question.OrderNumber == QuestionOrder.RoadBlock)
                {
                    employeeScrumDetail.Answer3 = SplitScrumAnswer(todayScrumAnswer.Answer);
                }
            }
            return employeeScrumDetail;
        }

        /// <summary>
        /// Method to split the multiple scrum answers into an array
        /// </summary>
        /// <param name="answer"></param>
        /// <returns>scrum answer</returns>
        private string[] SplitScrumAnswer(string answer)
        {
            return answer.Split(new string[] { "\\n" }, StringSplitOptions.None);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Method to return the list of projects depending on the role of the logged in user
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>List of projects</returns>
        public async Task<IEnumerable<ProjectAc>> GetProjectsAsync(string userId)
        {
            //Getting the details of the logged in user from Oauth server
            User loginUser = await _oauthCallsRepository.GetUserByEmployeeIdAsync(userId);
            //Fetch list of all the projects from oauth server
            List<ProjectAc> projects = await _oauthCallsRepository.GetAllProjectsAsync();
            //Checking if there are projects returned from oauth server or not
            if (projects.Any())
            {
                //Returning list of projects as per the role of the loggeed in user
                if (loginUser.Role.Equals(_stringConstant.Admin))
                {
                    return projects;
                }

                else if (loginUser.Role.Equals(_stringConstant.Employee))
                {
                    List<ProjectAc> employeeProjects = new List<ProjectAc>();
                    foreach (var project in projects)
                    {
                        foreach (var user in project.ApplicationUsers)
                        {
                            if (user.Id == loginUser.Id)
                            {
                                employeeProjects.Add(project);
                            }
                        }
                    }
                    return employeeProjects;
                }

                else if (loginUser.Role.Equals(_stringConstant.TeamLeader))
                {
                    List<ProjectAc> leaderProjects = projects.FindAll(x => x.TeamLeaderId == loginUser.Id).ToList();
                    return leaderProjects;
                }
            }
            return projects;
        }


        /// <summary>
        /// Method to return the details of scrum for a particular project
        /// </summary>
        /// <param name="projectId">project Id</param>
        /// <param name="scrumDate">Date of scrum</param>
        /// <param name="userId">userId of user</param>
        /// <returns>Details of the scrum</returns>
        public async Task<ScrumProjectDetails> ScrumReportDetailsAsync(int projectId, DateTime scrumDate, string userId)
        {
            //Getting details of the logged in user from Oauth server
            User loginUser = await _oauthCallsRepository.GetUserByEmployeeIdAsync(userId);
            //Getting details of the specific project from Oauth server
            ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(projectId);
            //Getting scrum for a specific project
            Scrum scrum = await _scrumDataRepository.FirstOrDefaultAsync(x => x.ProjectId == project.Id && DbFunctions.TruncateTime(x.ScrumDate) == scrumDate);
            ScrumProjectDetails scrumProjectDetail = new ScrumProjectDetails();
            scrumProjectDetail.ScrumDate = scrumDate.ToString(_stringConstant.FormatForDate);
            scrumProjectDetail.ProjectCreationDate = project.CreatedDate;
            //getting scrum answers of employees in a specific project
            scrumProjectDetail.EmployeeScrumAnswers = await GetEmployeeScrumDetailsAsync(project, scrum, loginUser, scrumDate);
            return scrumProjectDetail;
        }

        #endregion
    }
}
