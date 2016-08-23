using Newtonsoft.Json;
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
        private readonly IRepository<Question> _questionRepository;
        private readonly IProjectUserCallRepository _projectUser;
        private readonly IHttpClientRepository _httpClientRepository;

        #endregion


        #region Constructor

        public ScrumBotRepository(IRepository<ScrumAnswer> scrumAnswerRepository, IProjectUserCallRepository projectUser,
            IRepository<Scrum> scrumRepository, IRepository<Question> questionRepository, IHttpClientRepository httpClientRepository)
        {
            _scrumAnswerRepository = scrumAnswerRepository;
            _scrumRepository = scrumRepository;
            _questionRepository = questionRepository;
            _projectUser = projectUser;
            _httpClientRepository = httpClientRepository;
        }

        #endregion


        #region Public Methods          

        /// <summary>
        /// This method is called whenever a message other than "scrumn time" is written in the group during scrum meeting. - JJ
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Message"></param>
        /// <param name="GroupName"></param>
        /// <returns>Question statement</returns>
        public async Task<string> AddScrumAnswer(string UserName, string Message, string GroupName)
        {
            try
            {
                var scrum = _scrumRepository.Fetch(x => x.GroupName.Equals(GroupName) && x.ScrumDate.Date == DateTime.Now.Date).ToList();
                var questionCount = _questionRepository.Fetch(x => x.Type == 1).Count();
                var firstQuestion = _questionRepository.Fetch(x => x.Type == 1).FirstOrDefault();
                if (scrum.Any())
                {
                    if (CheckCondition(scrum.FirstOrDefault().Id, GroupName, questionCount))
                    {
                        var scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumID == scrum.FirstOrDefault().Id).ToList();
                        if (scrumAnswer.Any())
                        {
                            var lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                            var answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrumAnswer.EmployeeId).Count();
                            if (answerListCount < questionCount)
                            {
                                AddAnswer(lastScrumAnswer.ScrumID, lastScrumAnswer.QuestionId + 1, lastScrumAnswer.EmployeeId, Message);

                                if (questionCount == answerListCount + 1)
                                    return FetchQuestion(null, true);
                                else
                                    return FetchQuestion(lastScrumAnswer.QuestionId + 1, false);
                            }
                            else
                            {
                                var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailByUserNameUrl, UserName);
                                var response = await _httpClientRepository.GetAsync(AppSettingsUtil.UserUrl, requestUrl,null);
                                var responseContent = response.Content.ReadAsStringAsync().Result;
                                var user = JsonConvert.DeserializeObject<User>(responseContent);
                                var employeeId = "1";
                                AddAnswer(lastScrumAnswer.ScrumID, firstQuestion.Id, employeeId, Message);
                                return FetchQuestion(firstQuestion.Id + 1, false);
                            }
                        }
                        else
                        {
                            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailByUserNameUrl, UserName);
                            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.UserUrl, requestUrl,null);
                            var responseContent = response.Content.ReadAsStringAsync().Result;
                            var user = JsonConvert.DeserializeObject<User>(responseContent);
                            var employeeId = "1";
                            AddAnswer(scrum.FirstOrDefault().Id, firstQuestion.Id, employeeId, Message);
                            return FetchQuestion(firstQuestion.Id + 1, false);
                        }
                    }
                    else
                        return "Your scrum time has already been completed";
                }
                else
                    return "Sorry. Your scrum time has not been initiated.";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public string GetQuestion(string userName, string message, string groupName)
        //{
        //    try
        //    {
        //        var scrum = _scrumRepository.Fetch(x => x.GroupName.Equals(groupName) && x.ScrumDate.Date == DateTime.Now.Date).ToList();
        //        if (scrum.Any())
        //        {
        //            var scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumID == scrum.FirstOrDefault().Id).ToList();
        //            if (scrumAnswer.Any())
        //            {
        //                var lastScrum = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
        //                if (FetchQuestion(lastScrum.QuestionId, false) == String.Empty)
        //                {

        //                }
        //                else
        //                {
        //                    return FetchQuestion(lastScrum.QuestionId, false);
        //                }
        //                //add to scrum answer with question id the next one and fetch employee id based on username
        //                //fetch the question
        //                // fetch the employee name by id

        //            }
        //            else
        //            {
        //                //   StartScrum(groupName);
        //                //think what to do
        //            }
        //        }
        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// This method will be called when the keyword "scrum time" is encountered
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public async Task<string> StartScrum(string GroupName)
        {
            try
            {
                var scrumList = _scrumRepository.Fetch(x => x.GroupName.Equals(GroupName) && x.ScrumDate.Date == DateTime.Now.Date).ToList();
                if (!(scrumList.Any()))
                {
                    Scrum scrum = new Scrum();
                    scrum.CreatedOn = DateTime.UtcNow;
                    scrum.GroupName = GroupName;
                    scrum.ScrumDate = DateTime.UtcNow.Date;

                    var project = await _projectUser.GetProjectDetails(GroupName);
                    scrum.ProjectId = project.Id;
                    scrum.TeamLeaderId = project.TeamLeaderId;

                    _scrumRepository.Insert(scrum);
                    _scrumRepository.Save();

                    var question = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.Id).FirstOrDefault();
                    if (question != null)
                        return question.QuestionStatement;
                    else
                        return "Sorry I have nothing to ask you";
                }
                else
                    return "Sorry scrum time has already been started for this group";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will be called when the keyword "leave" is received as reply of a group member. - JJ
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns>Question to the next person</returns>
        public async Task<string> Leave(string GroupName, string Text)
        {
            try
            {
                var scrum = _scrumRepository.Fetch(x => x.GroupName.Equals(GroupName) && x.ScrumDate.Date == DateTime.Now.Date).FirstOrDefault();

                var name = Text.Split(new char[0]);
                var employee = await _projectUser.GetUsersByGroupName(GroupName, name[1]);
                if (employee == null)
                    return "Sorry." + name + " is not a member of this project.";
                else
                {
                    var questionList = _questionRepository.Fetch(x => x.Type == 1).ToList();
                    foreach (var question in questionList)
                    {
                        var answer = new ScrumAnswer();
                        answer.Answer = "leave";
                        answer.AnswerDate = DateTime.UtcNow;
                        answer.CreatedOn = DateTime.UtcNow;
                        answer.EmployeeId = employee.Id;
                        answer.QuestionId = question.Id;
                        answer.ScrumID = scrum.Id;
                        _scrumAnswerRepository.Insert(answer);
                        _scrumAnswerRepository.Save();
                    }
                }

                return FetchQuestion(null, true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion


        #region Private Methods

        private bool AddAnswer(int ScrumID, int QuestionId, string EmployeeId, string Message)
        {
            try
            {
                var answer = new ScrumAnswer();
                answer.Answer = Message;
                answer.AnswerDate = DateTime.UtcNow;
                answer.CreatedOn = DateTime.UtcNow;
                answer.EmployeeId = EmployeeId;
                answer.QuestionId = QuestionId;
                answer.ScrumID = ScrumID;
                _scrumAnswerRepository.Insert(answer);
                _scrumAnswerRepository.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string FetchQuestion(int? QuestionId, bool isFirstQuestion)
        {
            try
            {
                if (isFirstQuestion)
                {
                    var question = _questionRepository.Fetch(x => x.Type == 1).FirstOrDefault();
                    return question.QuestionStatement;
                }
                else
                {
                    var question = _questionRepository.FirstOrDefault(x => x.Id == QuestionId);
                    if (question != null)
                        return question.QuestionStatement;
                    else
                        return String.Empty;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool CheckCondition(int ScrumId, string GroupName, int QuestionCount)
        {
            try
            {
                var scrumAnswers = _scrumAnswerRepository.Fetch(x => x.ScrumID == ScrumId).ToList();

                //check the no. of emp[loyees in this group
                //multiply the question count with the no. of employees 
                // if they are the same return false

                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion

    }
}
