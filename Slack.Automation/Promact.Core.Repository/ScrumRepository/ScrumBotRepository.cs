using Newtonsoft.Json;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
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

        public void InitiateScrum(ScrumAnswer ScrumAnswer)
        {
            try
            {
                _scrumAnswerRepository.Insert(ScrumAnswer);
                _scrumAnswerRepository.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<string> AddScrumAnswer(string UserName, string Message, string GroupName)
        {
            try
            {
                var scrum = _scrumRepository.Fetch(x => x.GroupName.Equals(GroupName) && x.ScrumDate.Date == DateTime.Now.Date).ToList();
                var questionCount = _questionRepository.Fetch(x => x.Type == 1).Count();
                if (scrum.Any())
                {
                    var scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumID == scrum.FirstOrDefault().Id).ToList();
                    if (scrumAnswer.Any())
                    {
                        var lastScrum = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                        var answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrum.EmployeeId).Count();
                        if (questionCount <= answerListCount)
                        {
                            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailByUserNameUrl, UserName);
                            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.ProjectUserUrl, requestUrl);
                            var responseContent = response.Content.ReadAsStringAsync().Result;
                            var user = JsonConvert.DeserializeObject<User>(responseContent);
                        }
                        else
                        {

                            FetchQuestion(lastScrum.QuestionId + 1);

                        }
                    }
                    else
                    {

                    }
                }
                else
                {

                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetQuestion(string userName, string message, string groupName)
        {
            try
            {
                var scrum = _scrumRepository.Fetch(x => x.GroupName.Equals(groupName) && x.ScrumDate.Date == DateTime.Now.Date).ToList();
                if (scrum.Any())
                {
                    var scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumID == scrum.FirstOrDefault().Id).ToList();
                    if (scrumAnswer.Any())
                    {
                        var lastScrum = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                        if (FetchQuestion(lastScrum.QuestionId) == String.Empty)
                        {

                        }
                        else
                        {
                            return FetchQuestion(lastScrum.QuestionId);
                        }
                        //add to scrum answer with question id the next one and fetch employee id based on username
                        //fetch the question
                        // fetch the employee name by id

                    }
                    else
                    {
                        //   StartScrum(groupName);
                        //think what to do
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw;
            }
        }


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

        #endregion


        #region Private Methods

        private string FetchQuestion(int QuestionId)
        {
            try
            {
                var question = _questionRepository.FirstOrDefault(x => x.Id == QuestionId);
                if (question != null)
                    return question.QuestionStatement;
                else
                    return String.Empty;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
