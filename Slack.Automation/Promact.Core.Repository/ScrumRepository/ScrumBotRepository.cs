using Newtonsoft.Json;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.Bot;
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
        private static IClient _clientRepository;

        #endregion


        #region Constructor

        public ScrumBotRepository(IRepository<ScrumAnswer> scrumAnswerRepository, IProjectUserCallRepository projectUser, IClient clientRepository,
            IRepository<Scrum> scrumRepository, IRepository<Question> questionRepository, IHttpClientRepository httpClientRepository)
        {
            _scrumAnswerRepository = scrumAnswerRepository;
            _scrumRepository = scrumRepository;
            _questionRepository = questionRepository;
            _projectUser = projectUser;
            _clientRepository = clientRepository;
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
                string message = "";
                if (scrum.Any())
                {
                    var questionCount = _questionRepository.Fetch(x => x.Type == 1).Count();
                    var firstQuestion = _questionRepository.Fetch(x => x.Type == 1).FirstOrDefault();
                    var employees = await _projectUser.GetUsersByGroupName(GroupName);
                    var scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumID == scrum.FirstOrDefault().Id).ToList();
                    if ((employees.Count() * questionCount) > scrumAnswer.Count())
                    {
                        if (scrumAnswer.Any())
                        {
                            var lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                            var answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrumAnswer.EmployeeId).Count();
                            var questionStatement = "";
                            if (answerListCount < questionCount)
                            {
                                AddAnswer(lastScrumAnswer.ScrumID, lastScrumAnswer.QuestionId + 1, lastScrumAnswer.EmployeeId, Message);

                                if (questionCount == answerListCount + 1)
                                {
                                    var list = scrumAnswer.Select(x => x.EmployeeId).ToList();
                                    var idlist = employees.Where(x => !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                                    if (idlist.FirstOrDefault() != null)
                                    {
                                        var user = await _projectUser.GetUserById(idlist.FirstOrDefault());
                                        questionStatement = user.UserName + " " + FetchQuestion(null, true);
                                    }
                                    else
                                        questionStatement = "Scrum of all employees has been recorded";
                                }
                                else
                                {
                                    var user = await _projectUser.GetUserById(lastScrumAnswer.EmployeeId);
                                    questionStatement = user.UserName + " " + FetchQuestion(lastScrumAnswer.QuestionId + 2, false);
                                }
                                message = questionStatement;
                            }
                            else
                            {
                                var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailByUserNameUrl, UserName);
                                var response = await _httpClientRepository.GetAsync(AppSettingsUtil.UserUrl, requestUrl);
                                var responseContent = response.Content.ReadAsStringAsync().Result;
                                var user = JsonConvert.DeserializeObject<User>(responseContent);
                                AddAnswer(lastScrumAnswer.ScrumID, firstQuestion.Id, user.Id, Message);
                                message = user.UserName + " " + FetchQuestion(firstQuestion.Id + 1, false);
                            }
                        }
                        else
                        {
                            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailByUserNameUrl, UserName);
                            var response = await _httpClientRepository.GetAsync(AppSettingsUtil.UserUrl, requestUrl);
                            var responseContent = response.Content.ReadAsStringAsync().Result;
                            var user = JsonConvert.DeserializeObject<User>(responseContent);
                            AddAnswer(scrum.FirstOrDefault().Id, firstQuestion.Id, user.Id, Message);
                            message = user.UserName + " " + FetchQuestion(firstQuestion.Id + 1, false);
                        }
                    }
                    //else
                    //    message = "Your scrum time has already been completed";
                }
                //else
                //    message = "Sorry. Your scrum time has not been initiated.";

                PostMessages(GroupName, "tsakmail", message);
                return message;
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
                string message = "";
                if (!(scrumList.Any()))
                {
                    var project = await _projectUser.GetProjectDetails(GroupName);
                    if (project != null)
                    {
                        var employees = await _projectUser.GetUsersByGroupName(GroupName);
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
                            var question = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.Id).FirstOrDefault();
                            if (question != null)
                                message = firstEmployee.UserName + " " + question.QuestionStatement;
                            else
                                message = "Sorry I have nothing to ask you.";
                        }
                        else
                            message = "Sorry. No employees found for this project.";
                    }
                    else
                        message = "No project found for this group.";
                }
                else
                    message = "Sorry scrum time has already been started for this group.";
                PostMessages(GroupName, "tsakmail", message);
                //  WriteMessage(message);
                return message;
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
                //    var employee = await _projectUser.GetUsersByGroupName(GroupName, name[1]);
                var employee = await _projectUser.GetUserByUsername(name[1]);
                if (employee == null)
                {
                    PostMessages(GroupName, "tsakmail", "Sorry." + name + " is not a member of this project.");
                    return "Sorry." + name + " is not a member of this project.";
                }
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

                var employees = await _projectUser.GetUsersByGroupName(GroupName);
                var scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumID == scrum.Id).ToList();

                var list = scrumAnswer.Select(x => x.EmployeeId).ToList();
                var idlist = employees.Where(x => !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                var user = await _projectUser.GetUserById(idlist.FirstOrDefault());
                var returnMsg = user.UserName + " " + FetchQuestion(null, true);
                PostMessages(GroupName, "tsakmail", returnMsg);
                return returnMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion


        //private void WriteMessage(string message)
        //{
        //    try
        //    {
        //        // ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
        //        //var bytes = new Byte[8192];
        //        //bytes = Encoding.ASCII.GetBytes(message);
        //        //var len = bytes.Length;
        //        // var buffer = new ArraySegment<byte>(bytes, 0, len-1);

        //        //ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
        //        //buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        //        //_webSocket = new ClientWebSocket();
        //        //Task tsk = _webSocket.ConnectAsync(new Uri("wss://mpmulti-0yj2.slack-msgs.com/websocket/3PAzcxoBhkXzN9iBvwT1gSZy4En0QBmb9ht8DJVEPHugTiEXk1qdk4jGj2jdET3oXoeOJ2SDyLR1YEP3SrOMLNydmB7rae-z2aSVWlzYDrK3kaSvWqmmN9x_esGpSJ-6Ps-UQtiY6RpfKxlTVeXbww=="), System.Threading.CancellationToken.None);
        //        //tsk.Wait();
        //        //await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

        //        BotClient _botClient = new BotClient("xoxb-61375498279-ZBxCBFUkvnlR4muKNiUh7tCG");
        //        _botClient.WriteMessage(message);

        //        //while (1 == 1)
        //        //{
        //        //    await _webSocket.SendAsync(buffer, WebSocketMessageType.Binary, false, CancellationToken.None);
        //        //}
        //    }
        //    catch (System.Net.WebSockets.WebSocketException ex)
        //    {
        //        //DisconnectSlack();
        //        throw ex;
        //    }
        //}

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

        private async void PostMessages(string GroupName, string UserName, string Message)
        {
            try
            {
                PostMessageArguments args = new PostMessageArguments();
                args.Channel = GroupName;
                args.Username = UserName;
                args.Text = Message;
                args.as_user = true;
                args.parse = "full";
                args.link_names = 1;
                args.unfurl_links = true;
                args.unfurl_media = true;
                args.icon_url = "";
                args.icon_emoji = "";

                string strURL =
                    "https://slack.com/api/chat.postMessage?token=" + "xoxb-61375498279-ZBxCBFUkvnlR4muKNiUh7tCG" +
                    "&channel=" + System.Web.HttpUtility.UrlEncode(args.Channel) +
                    "&text=" + System.Web.HttpUtility.UrlEncode(args.Text) +
                     "&as_user=" + args.as_user.ToString() +
                  "&parse=" + System.Web.HttpUtility.UrlEncode(args.parse) +
                  "&link_names=" + System.Web.HttpUtility.UrlEncode(args.link_names.ToString()) +
                  "&unfurl_links=" + args.unfurl_links.ToString() +
                  "&unfurl_media=" + args.unfurl_media.ToString();
                _clientRepository.WebRequestMethod("hi", strURL);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

               
        #endregion

    }
}


