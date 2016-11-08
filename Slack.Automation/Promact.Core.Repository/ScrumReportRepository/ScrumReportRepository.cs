using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumReportRepository
{
    public class ScrumReportRepository : IScrumReportRepository
    {
        private readonly IRepository<Scrum> _scrumDataRepository;
        private readonly IRepository<ScrumAnswer> _scrumAnswerDataRepository;
        private readonly IProjectUserCallRepository _projectUserCallRepository;
        private readonly IStringConstantRepository _stringConstant;

        public ScrumReportRepository(IRepository<Scrum> scrumDataRepository, IRepository<ScrumAnswer> scrumAnswerDataRepository, IStringConstantRepository stringConstant, IProjectUserCallRepository projectUserCallRepository)
        {
            _scrumDataRepository = scrumDataRepository;
            _scrumAnswerDataRepository = scrumAnswerDataRepository;
            _projectUserCallRepository = projectUserCallRepository;
            _stringConstant = stringConstant;
        }

        /// <summary>
        /// Method to return the list of projects depending on the role of the logged in user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns>List of projects</returns>
        public async Task<IEnumerable<ProjectAc>> GetProjects(string userName, string accessToken)
        {
            //getting the details of the logged in user from Oauth server
            User loginUser = await _projectUserCallRepository.GetUserByUserName(userName, accessToken);
            List<ProjectAc> projects = await _projectUserCallRepository.GetAllProjects(accessToken);

            //returning list of projects as per the role of the loggeed in user
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

            if (loginUser.Role.Equals(_stringConstant.TeamLeader))
            {
                List<ProjectAc> leaderProjects = projects.FindAll(x => x.TeamLeaderId == loginUser.Id).ToList();
                return leaderProjects;
            }
            return null;
        }


        /// <summary>
        /// Method to return the details of scrum for a particular project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="scrumDate"></param>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns>Details of the scrum</returns>
        public async Task<ScrumProjectDetails> ScrumReportDetails(int projectId, DateTime scrumDate,string userName, string accessToken)
        {
            //getting details of the logged in user from Oauth server
            User loginUser = await _projectUserCallRepository.GetUserByUserName(userName, accessToken);
            //getting details of the specific project from Oauth server
            ProjectAc project = await _projectUserCallRepository.GetProjectDetails(projectId, accessToken);
            Scrum scrum = _scrumDataRepository.FirstOrDefault(x => x.ProjectId == project.Id);
            ScrumProjectDetails scrumProjectDetail = new ScrumProjectDetails();
            scrumProjectDetail.ScrumDate = scrumDate.ToString(_stringConstant.FormatForDate);
            scrumProjectDetail.ProjectCreationDate = project.CreatedDate;
            //getting scrum answers of employees in a specific project
            scrumProjectDetail.EmployeeScrumAnswers = GetEmployeeScrumDetails(project,scrum,loginUser, scrumDate);
            return scrumProjectDetail;
        }

        /// <summary>
        /// Method to return list of employees in the project with their scrum answers based on role of logged in user
        /// </summary>
        /// <param name="project"></param>
        /// <param name="scrum"></param>
        /// <param name="loginUser"></param>
        /// <param name="scrumDate"></param>
        /// <returns>Object with list of employees in project with answers to scrum questions</returns>
        private IList<EmployeeScrumDetails> GetEmployeeScrumDetails(ProjectAc project, Scrum  scrum, User loginUser, DateTime scrumDate)
        {
            List<EmployeeScrumDetails> employeeScrumDetails = new List<EmployeeScrumDetails>();
            //assigning answers of scrum to employees in the project based on their role
            //if logged user is an employee it will return only his scrum answers
            if (loginUser.Role.Equals(_stringConstant.Employee))
            {
                foreach (var user in project.ApplicationUsers)
                {
                    if (user.Id.Equals(loginUser.Id))
                    {
                        EmployeeScrumDetails employeeScrumDetail = AssignAnswers(scrum, scrumDate, user);
                        employeeScrumDetails.Add(employeeScrumDetail);
                    }
                }
            }
            //if logged user is admin or teamleader it will return scrum answers for entire team
            else
            {
                foreach (var user in project.ApplicationUsers)
                {
                    EmployeeScrumDetails employeeScrumDetail = AssignAnswers(scrum, scrumDate, user);
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
        private EmployeeScrumDetails AssignAnswers(Scrum scrum, DateTime scrumDate, User user)
        {
            EmployeeScrumDetails employeeScrumDetail = new EmployeeScrumDetails();
            List<ScrumAnswer> scrumAnswers = _scrumAnswerDataRepository.Fetch(x => x.EmployeeId == user.Id).ToList();
            List<ScrumAnswer> todayScrumAnswers = scrumAnswers.FindAll(x => x.AnswerDate == scrumDate && x.ScrumId == scrum.Id).ToList();
            employeeScrumDetail.EmployeeName = string.Format("{0} {1}", user.FirstName, user.LastName);
            //assigning answers to specific scrum questions
            if (todayScrumAnswers.Count() == 0)
            {
                employeeScrumDetail.Status = _stringConstant.PersonNotAvailable;
            }
            foreach (var todayScrumAnswer in todayScrumAnswers)
            {
                if (todayScrumAnswer.Question.QuestionStatement.Equals(_stringConstant.ScrumFirstQuestion))
                {
                    employeeScrumDetail.Answer1 = todayScrumAnswer.Answer.Split('\n');
                }
                if (todayScrumAnswer.Question.QuestionStatement.Equals(_stringConstant.ScrumSecondQuestion))
                {
                    employeeScrumDetail.Answer2 = todayScrumAnswer.Answer.Split('\n');
                }
                if (todayScrumAnswer.Question.QuestionStatement.Equals(_stringConstant.ScrumThirdQuestion))
                {
                    employeeScrumDetail.Answer3 = todayScrumAnswer.Answer.Split('\n');
                }
            }
            return employeeScrumDetail;
        }
    }
}


