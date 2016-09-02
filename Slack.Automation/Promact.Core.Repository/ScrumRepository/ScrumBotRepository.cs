using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumRepository
{
    public class ScrumBotRepository : IScrumBotRepository
    {

        #region Private Variable

        private readonly IRepository<ScrumAnswer> _scrumAnswerRepository;
        private readonly IRepository<Scrum> _scrumRepository;
        private readonly IRepository<ApplicationUser> _applicationUser;
        private readonly IRepository<Question> _questionRepository;
        private readonly IProjectUserCallRepository _projectUser;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IHttpClientRepository _httpClientRepository;

        #endregion


        #region Constructor

        public ScrumBotRepository(IRepository<ScrumAnswer> scrumAnswerRepository, IProjectUserCallRepository projectUser, IRepository<Scrum> scrumRepository,
            IAttachmentRepository attachmentRepository, IRepository<Question> questionRepository, IHttpClientRepository httpClientRepository,
            IRepository<ApplicationUser> applicationUser, IClient clientRepository)
        {
            _scrumAnswerRepository = scrumAnswerRepository;
            _scrumRepository = scrumRepository;
            _questionRepository = questionRepository;
            _projectUser = projectUser;
            _applicationUser = applicationUser;
            _attachmentRepository = attachmentRepository;
            _clientRepository = clientRepository;
            _httpClientRepository = httpClientRepository;
        }

        #endregion


        #region Public Methods   


        /// <summary>
        /// This method is called whenever a message other than "scrumn time" or "leave username" is written in the group during scrum meeting. - JJ
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Message"></param>
        /// <param name="GroupName"></param>
        /// <returns>The next Question Statement</returns>
        public async Task<string> AddScrumAnswer(string UserName, string Message, string GroupName)
        {
            try
            {
                var scrum = _scrumRepository.Fetch(x => x.GroupName.Equals(GroupName) && x.ScrumDate.Date == DateTime.Now.Date).ToList();
                string message = "";
                if (scrum.Any())
                {
                    // getting user name from user's slack name
                    var applicationUser = _applicationUser.FirstOrDefault(x => x.SlackUserName == UserName);
                    // getting access token for that user
                    var accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);

                    var questions = _questionRepository.Fetch(x => x.Type == 1).ToList();
                    var questionCount = questions.Count();
                    var employees = await _projectUser.GetUsersByGroupName(GroupName, accessToken);

                    var scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumId == scrum.FirstOrDefault().Id).ToList();
                    if ((employees.Count() * questionCount) > scrumAnswer.Count())
                    {
                        var firstQuestion = questions.OrderBy(x => x.OrderNumber).FirstOrDefault();
                        if (scrumAnswer.Any())
                        {
                            var lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                            var answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrumAnswer.EmployeeId).Count();
                            var questionStatement = "";
                            if (answerListCount < questionCount)
                            {
                                var prevQuestion = _questionRepository.FirstOrDefault(x => x.Id == lastScrumAnswer.QuestionId);
                                var question = _questionRepository.FirstOrDefault(x => x.Type == 1 && x.OrderNumber == prevQuestion.OrderNumber + 1);
                                AddAnswer(lastScrumAnswer.ScrumId, question.Id, lastScrumAnswer.EmployeeId, Message);

                                if (questionCount == answerListCount + 1)
                                {
                                    //if the employee who answered now has answered all the questions
                                    var list = scrumAnswer.Select(x => x.EmployeeId).ToList();
                                    var idlist = employees.Where(x => !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                                    if (idlist.FirstOrDefault() != null)
                                    {
                                        //first question to the next employee
                                        var user = await _projectUser.GetUserById(idlist.FirstOrDefault(), accessToken);
                                        questionStatement = StringConstant.GoodDay + "<@" + user.SlackUserName + ">!\n" + FetchQuestion(null, true);
                                    }
                                    else
                                        //all employees have been asked questions
                                        questionStatement = StringConstant.ScrumComplete;
                                }
                                else
                                {
                                    //next question to the same employee
                                    var user = await _projectUser.GetUserById(lastScrumAnswer.EmployeeId, accessToken);
                                    questionStatement = "<@" + user.SlackUserName + ">" + " " + FetchQuestion(question.Id, false);
                                }
                                message = questionStatement;
                            }
                            else
                            {
                                //A particular employee's first answer
                                var user = await _projectUser.GetUserBySlackUserName(UserName, accessToken);
                                AddAnswer(lastScrumAnswer.ScrumId, firstQuestion.Id, user.Id, Message);
                                message = StringConstant.GoodDay + "<@" + user.SlackUserName + ">!\n" + FetchQuestion(firstQuestion.Id, false);
                            }
                        }
                        else
                        {
                            //First Employee's first answer
                            var user = await _projectUser.GetUserBySlackUserName(UserName, accessToken);
                            AddAnswer(scrum.FirstOrDefault().Id, firstQuestion.Id, user.Id, Message);
                            message = "<@" + user.SlackUserName + ">" + " " + FetchQuestion(firstQuestion.Id, false);
                        }
                    }
                }
                return message;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }


        /// <summary>
        /// This method will be called when the keyword "scrum time" is encountered
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public async Task<string> StartScrum(string GroupName, string UserName)
        {
            try
            {
                var scrumList = _scrumRepository.Fetch(x => x.GroupName.Equals(GroupName) && x.ScrumDate.Date == DateTime.Now.Date).ToList();
                string message = "";
                // getting user name from user's slack name
                var applicationUser = _applicationUser.FirstOrDefault(x => x.SlackUserName == UserName);
                // getting access token for that user
                var accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);
                var questionList = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.OrderNumber).ToList();
                if (!(scrumList.Any()))
                {
                    ProjectAc project;
                    try
                    {
                        project = await _projectUser.GetProjectDetails(GroupName, accessToken);
                    }
                    catch (Exception)
                    {
                        return StringConstant.ServerClosed;
                    }
                    if (project != null)
                    {
                        List<User> employees;
                        try
                        {
                            employees = await _projectUser.GetUsersByGroupName(GroupName, accessToken);
                        }
                        catch (Exception)
                        {
                            return StringConstant.ServerClosed;
                        }
                        if (employees.Count != 0)
                        {
                            Scrum scrum = new Scrum();
                            scrum.CreatedOn = DateTime.UtcNow;
                            scrum.GroupName = GroupName;
                            scrum.ScrumDate = DateTime.UtcNow.Date;
                            scrum.ProjectId = project.Id;
                            scrum.TeamLeaderId = project.TeamLeaderId;
                            _scrumRepository.Insert(scrum);
                            _scrumRepository.Save();

                            var firstEmployee = employees.FirstOrDefault();
                            var question = questionList.FirstOrDefault();
                            if (question != null)
                                message = StringConstant.GoodDay + "<@" + firstEmployee.SlackUserName + ">!\n" + question.QuestionStatement;
                            else
                                message = StringConstant.NoQuestion;
                        }
                        else
                            message = StringConstant.NoEmployeeFound;
                    }
                    else
                        message = StringConstant.NoProjectFound;

                }
                else
                    message = await GetQuestion(scrumList.FirstOrDefault().Id, GroupName, accessToken);
                return message;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// This method will be called when the keyword "leave" is received as reply of a group member. - JJ
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns>Question to the next person</returns>
        public async Task<string> Leave(string GroupName, string UserName, string LeaveApplicant)
        {
            try
            {
                var returnMsg = "";
                var scrum = _scrumRepository.Fetch(x => x.GroupName.Equals(GroupName) && x.ScrumDate.Date == DateTime.Now.Date).FirstOrDefault();
                if (scrum != null)
                {
                    // getting user name from user's slack name
                    var applicationUser = _applicationUser.FirstOrDefault(x => x.SlackUserName == UserName);
                    // getting access token for that user
                    var accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);

                    var employee = await _projectUser.GetUserBySlackUserName(LeaveApplicant, accessToken);
                    if (employee == null)
                        return "Sorry." + LeaveApplicant + " is not a member of this project.";
                    else
                    {
                        var questionList = _questionRepository.Fetch(x => x.Type == 1).ToList();
                        foreach (var question in questionList)
                        {
                            var answer = new ScrumAnswer();
                            answer.Answer = StringConstant.Leave;
                            answer.AnswerDate = DateTime.UtcNow;
                            answer.CreatedOn = DateTime.UtcNow;
                            answer.EmployeeId = employee.Id;
                            answer.QuestionId = question.Id;
                            answer.ScrumId = scrum.Id;
                            _scrumAnswerRepository.Insert(answer);
                            _scrumAnswerRepository.Save();
                        }
                    }

                    returnMsg = await GetQuestion(scrum.Id, GroupName, accessToken);
                }
                return returnMsg;
            }
            catch (Exception)
            {
                return StringConstant.ServerClosed;
            }
        }



        /// <summary>
        /// This method is used to add Scrum answer to the database
        /// </summary>
        /// <param name="ScrumID"></param>
        /// <param name="QuestionId"></param>
        /// <param name="EmployeeId"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public bool AddAnswer(int ScrumID, int QuestionId, string EmployeeId, string Message)
        {
            try
            {
                var answer = new ScrumAnswer();
                answer.Answer = Message;
                answer.AnswerDate = DateTime.UtcNow;
                answer.CreatedOn = DateTime.UtcNow;
                answer.EmployeeId = EmployeeId;
                answer.QuestionId = QuestionId;
                answer.ScrumId = ScrumID;
                _scrumAnswerRepository.Insert(answer);
                _scrumAnswerRepository.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Used to fetch the next question based on the given parameters
        /// </summary>
        /// <param name="ScrumId"></param>
        /// <param name="GroupName"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task<string> GetQuestion(int ScrumId, string GroupName, string accessToken)
        {
            try
            {
                var returnMsg = "";
                var scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumId == ScrumId).ToList();
                if (scrumAnswer.Any())
                {
                    var questions = _questionRepository.Fetch(x => x.Type == 1).ToList();
                    var questionCount = questions.Count();
                    var lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                    var answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrumAnswer.EmployeeId).Count();
                    var employees = await _projectUser.GetUsersByGroupName(GroupName, accessToken);
                    if (answerListCount == questionCount)
                    {
                        var list = scrumAnswer.Select(x => x.EmployeeId).ToList();
                        var idList = employees.Where(x => !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                        if (idList != null && idList.Count > 0)
                        {
                            var user = await _projectUser.GetUserById(idList.FirstOrDefault(), accessToken);
                            returnMsg = StringConstant.GoodDay + "<@" + user.SlackUserName + ">!\n" + FetchQuestion(null, true);
                        }
                        else
                            returnMsg = StringConstant.ScrumComplete;
                    }
                    else
                    {
                        var user = await _projectUser.GetUserById(lastScrumAnswer.EmployeeId, accessToken);
                        returnMsg = "<@" + user.SlackUserName + ">" + " " + FetchQuestion(lastScrumAnswer.QuestionId, false);
                    }
                }
                return returnMsg;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }


        /// <summary>
        /// This method fetches the Question statement of next question of the given questionId
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <param name="isFirstQuestion"></param>
        /// <returns></returns>
        private string FetchQuestion(int? QuestionId, bool isFirstQuestion)
        {
            try
            {
                if (isFirstQuestion)
                {
                    var question = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.OrderNumber).FirstOrDefault();
                    return question.QuestionStatement;
                }
                else
                {
                    var orderNumber = _questionRepository.FirstOrDefault(x => x.Id == QuestionId).OrderNumber;
                    var question = _questionRepository.FirstOrDefault(x => x.OrderNumber == orderNumber + 1 && x.Type == 1);
                    if (question != null)
                        return question.QuestionStatement;
                    else
                        return String.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
