using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
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
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IProjectUserCallRepository _projectUser;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly ISlackUserRepository _slackUserDetails;
        private readonly IStringConstantRepository _stringConstant;

        #endregion


        #region Constructor


        public ScrumBotRepository(IRepository<ScrumAnswer> scrumAnswerRepository, IProjectUserCallRepository projectUser,
            IRepository<Scrum> scrumRepository, IAttachmentRepository attachmentRepository, IRepository<Question> questionRepository,
            IHttpClientRepository httpClientRepository, IRepository<ApplicationUser> applicationUser,
            ISlackChannelRepository slackChannelRepository, ISlackUserRepository slackUserDetails, IStringConstantRepository stringConstant)
        {
            _scrumAnswerRepository = scrumAnswerRepository;
            _scrumRepository = scrumRepository;
            _questionRepository = questionRepository;
            _projectUser = projectUser;
            _slackChannelRepository = slackChannelRepository;
            _applicationUser = applicationUser;
            _attachmentRepository = attachmentRepository;
            _httpClientRepository = httpClientRepository;
            _slackUserDetails = slackUserDetails;
            _stringConstant = stringConstant;
        }


        #endregion


        #region Public Method 


        public async Task<string> ProcessMessages(string userId, string channelId, string message)
        {
            string replyText = string.Empty;
            SlackUserDetails user = _slackUserDetails.GetById(userId);
            SlackChannelDetails channel = _slackChannelRepository.GetById(channelId);
            //the command is split to individual words
            //commnads ex: "scrum time", "later @userId"
            var messageArray = message.Split(null);
            if (user != null && String.Compare(message, _stringConstant.ScrumHelp, true).Equals(0))
                replyText = _stringConstant.ScrumHelpMessage;
            else if (user != null && channel != null)
            {
                //commands could be"scrum time" or "scrum halt" or "scrum resume"
                if (String.Compare(message, _stringConstant.ScrumTime, true).Equals(0) || String.Compare(message, _stringConstant.ScrumHalt, true).Equals(0) || String.Compare(message, _stringConstant.ScrumResume, true).Equals(0))
                    replyText = await Scrum(channel.Name, user.Name, messageArray[1].ToLower());
                //a particular employee is on leave, getting marked as later or asked question again
                //commands would be "leave @userId"
                else if ((String.Compare(messageArray[0], _stringConstant.Leave, true).Equals(0)) && messageArray.Length == 2)
                {
                    int fromIndex = message.IndexOf("<@") + "<@".Length;
                    int toIndex = message.LastIndexOf(">");
                    if (toIndex > 0)
                    {
                        try
                        {
                            //the userId is fetched
                            string applicantId = message.Substring(fromIndex, toIndex - fromIndex);
                            //fetch the user of the given userId
                            SlackUserDetails applicant = _slackUserDetails.GetById(applicantId);
                            if (applicant != null)
                            {
                                string applicantName = applicant.Name;
                                replyText = await Leave(channel.Name, user.Name, applicantName, messageArray[0].ToLower());
                            }
                            else
                                replyText = _stringConstant.NotAUser;
                        }
                        catch (Exception)
                        {
                            replyText = _stringConstant.ScrumHelpMessage;
                        }
                    }
                    else
                        replyText = await AddScrumAnswer(user.Name, message, channel.Name);
                }
                //all other texts
                else
                    replyText = await AddScrumAnswer(user.Name, message, channel.Name);
            }
            //If channel is not registered in the database
            else if (user != null)
            {
                //If channel is not registered in the database and the command encountered is "add channel channelname"
                if (channel == null && String.Compare(messageArray[0], _stringConstant.Add, true).Equals(0) && String.Compare(messageArray[1], _stringConstant.Channel, true).Equals(0))
                    replyText = AddChannelManually(messageArray[2], user.Name, channelId).Result;
                else
                    replyText = _stringConstant.ChannelAddInstruction;
            }
            else if (user == null)
                replyText = _stringConstant.NotAUser;

            return replyText;
        }


        #endregion


        #region Private Methods


        /// <summary>
        /// This method is called whenever a message other than the default keywords is written in the group. - JJ
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Message"></param>
        /// <param name="GroupName"></param>
        /// <returns>The next Question Statement</returns>
        private async Task<string> AddScrumAnswer(string UserName, string Message, string GroupName)
        {
            string message = string.Empty;
            // getting user name from user's slack name
            var applicationUser = _applicationUser.FirstOrDefault(x => x.SlackUserName == UserName);
            // getting access token for that user
            if (applicationUser != null)
            {
                // get access token of user for promact oauth server
                var accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);
                //today's scrum of the group 
                List<Scrum> scrum = _scrumRepository.Fetch(x => String.Compare(x.GroupName, GroupName, true).Equals(0) && x.ScrumDate.Date == DateTime.UtcNow.Date).ToList();
                //list of scrum questions. Type =1
                List<Question> questions = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.OrderNumber).ToList();
                //employees of the given group name fetched from the oauth server
                List<User> employees = await _projectUser.GetUsersByGroupName(GroupName, accessToken);

                ScrumStatus scrumStatus = FetchScrumStatus(GroupName, accessToken, null, employees, questions).Result;
                if (scrumStatus == ScrumStatus.OnGoing)
                {
                    int firstScrumId = scrum.FirstOrDefault().Id;
                    int questionCount = questions.Count();
                    //scrum answer of that day's scrum
                    List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumId == firstScrumId).ToList();
                    //status would be empty if the interacting user is same as the expected user.
                    var status = ExpectedUser(scrum.FirstOrDefault().Id, questions, employees, UserName);
                    if (status == string.Empty)
                    {
                        ////scrum answers which were marked as later, are now to be answered
                        //var nowReadyScrumsAnswers = scrumAnswer.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).OrderBy(x => x.Id).ToList();

                        //if (nowReadyScrumsAnswers.Any())
                        //{
                        //    message = UpdateAnswer(nowReadyScrumsAnswers, Message, UserName);
                        //    if (nowReadyScrumsAnswers.Count == 1)
                        //        //scrum answers which were marked to be answered later are all answered
                        //        return message + Environment.NewLine + GetQuestion(firstScrumId, GroupName, questions, employees, scrum.FirstOrDefault().ProjectId, accessToken).Result;
                        //    else
                        //        //return the message which may contain the next question
                        //        return message;
                        //}
                        //else
                        //{
                        #region Normal Scrum

                        if ((employees.Count() * questionCount) > scrumAnswer.Count)
                        {
                            Question firstQuestion = questions.OrderBy(x => x.OrderNumber).FirstOrDefault();
                            ScrumAnswer lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                            //scrum answers of the given employee
                            int answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrumAnswer.EmployeeId).Count();

                            if (scrumAnswer.Any())
                            {
                                if (answerListCount < questionCount)
                                {
                                    //not all questions have been answered
                                    Question prevQuestion = _questionRepository.FirstOrDefault(x => x.Id == lastScrumAnswer.QuestionId);
                                    Question question = _questionRepository.FirstOrDefault(x => x.Type == 1 && x.OrderNumber == prevQuestion.OrderNumber + 1);
                                    AddAnswer(lastScrumAnswer.ScrumId, question.Id, lastScrumAnswer.EmployeeId, Message);
                                }
                                else
                                {
                                    //A particular employee's first answer
                                    var idList = employees.Where(x => !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                                    if (idList != null && idList.Count > 0)
                                    {
                                        //now fetch the first question to the next employee
                                        User user = employees.FirstOrDefault(x => x.Id == idList.FirstOrDefault());
                                        AddAnswer(lastScrumAnswer.ScrumId, firstQuestion.Id, user.Id, Message);
                                    }
                                }
                                //get the next question 
                                //donot shift message                                         
                                message = await GetQuestion(firstScrumId, GroupName, questions, employees, scrum.FirstOrDefault().ProjectId, accessToken);
                            }
                            else
                            {
                                //First Employee's first answer
                                User user = employees.FirstOrDefault();
                                AddAnswer(firstScrumId, firstQuestion.Id, user.Id, Message);
                                //get the next question . donot shift message 
                                message = await GetQuestion(firstScrumId, GroupName, questions, employees, scrum.FirstOrDefault().ProjectId, accessToken);
                            }
                        }

                        #endregion
                        // }
                    }
                    //the user interacting is not the expected user
                    else if ((status != _stringConstant.ScrumConcludedButLater) && (status != _stringConstant.ScrumComplete))
                        return status;
                }
                //else
                //    message = ReplyToClient(scrumStatus);

            }

            //else
            //    // if user doesn't exist then this message will be shown to user
            //    message = _stringConstant.YouAreNotInExistInOAuthServer;

            //    }
            //}
            return message;
        }



        /// <summary>
        /// This method will be called when the keyword "scrum time" or "scrum halt" or "scrum resume" is encountered
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserName"></param>
        /// <param name="Parameter"></param>
        /// <returns>The question or the status of the scrum</returns>
        private async Task<string> Scrum(string GroupName, string UserName, string Parameter)
        {
            // getting user name from user's slack name
            var applicationUser = _applicationUser.FirstOrDefault(x => x.SlackUserName == UserName);
            // getting access token for that user
            if (applicationUser != null)
            {
                // get access token of user for promact oauth server
                var accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);

                var scrumList = _scrumRepository.Fetch(x => String.Compare(x.GroupName, GroupName, true).Equals(0)).ToList();
                var scrum = scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);

                var scrumStage = (ScrumActions)Enum.Parse(typeof(ScrumActions), Parameter);
                switch (scrumStage)
                {
                    case ScrumActions.halt:
                        return ScrumHalt(scrum, GroupName, accessToken);

                    case ScrumActions.resume:
                        return ScrumResume(scrum, GroupName, accessToken);

                    case ScrumActions.time:
                        //keyword encountered is "scrum time"
                        return StartScrum(GroupName, UserName, accessToken).Result;

                    default:
                        return string.Empty;
                }
            }
            else
                // if user doesn't exist then this message will be shown to user
                return _stringConstant.YouAreNotInExistInOAuthServer;

        }

        /// <summary>
        /// This method will be called when the keyword "leave @username" or "later @username" or "scrum @username" is received as reply from a group member. - JJ
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserName"></param>
        /// <param name="LeaveApplicant"></param>
        /// <param name="Parameter"></param>
        /// <returns>Question to the next person or other scrum status</returns>
        private async Task<string> Leave(string GroupName, string UserName, string Applicant, string Parameter)
        {
            var returnMsg = string.Empty;
            // getting user name from user's slack name
            var applicationUser = _applicationUser.FirstOrDefault(x => x.SlackUserName == UserName);
            // getting access token for that user
            if (applicationUser != null)
            {
                // get access token of user for promact oauth server
                var accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);
                List<Question> questions = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.OrderNumber).ToList();
                List<User> employees = await _projectUser.GetUsersByGroupName(GroupName, accessToken);

                ScrumStatus scrumStatus = FetchScrumStatus(GroupName, accessToken, null, employees, questions).Result;

                if (scrumStatus == ScrumStatus.OnGoing)
                {
                    var scrum = _scrumRepository.Fetch(x => String.Compare(x.GroupName, GroupName, true).Equals(0) && x.ScrumDate.Date == DateTime.UtcNow.Date).FirstOrDefault();
                    List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumId == scrum.Id).ToList();

                    //if (Parameter == _stringConstant.Scrum)
                    //{
                    //    //keyword "scrum @username" i.e. what we actually get is "scrum @userId" is encountered
                    //    var status = CheckUser(scrumAnswer, employees, questions.Count, Applicant);
                    //    if (status.Equals(string.Empty))//true condition
                    //    {
                    //        var employee = employees.FirstOrDefault(x => x.SlackUserName == Applicant);
                    //        if (employee != null)
                    //            return LaterScrum(scrumAnswer, employee.Id, Applicant, GroupName, scrum.ProjectId, employees, questions, accessToken);
                    //        else
                    //            returnMsg = _stringConstant.Unrecognized;
                    //    }
                    //    else
                    //        return status + Environment.NewLine;
                    //}
                    //else
                    //keyword "leave @username" or "later @username" is encountered.(@userId is obtained)
                    returnMsg = LeaveLater(scrumAnswer, employees, Parameter, scrum.Id, Applicant, questions, GroupName, scrum.ProjectId, UserName, accessToken);
                }
                else
                    returnMsg = ReplyToClient(scrumStatus);
            }
            else
                // if user doesn't exist then this message will be shown to user
                returnMsg = _stringConstant.YouAreNotInExistInOAuthServer;
            return returnMsg;
        }


        /// <summary>
        /// Used to add channel manually by command "add channel channelname"
        /// </summary>
        /// <param name="ChannelName"></param>
        /// <param name="ChannelId"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        private async Task<string> AddChannelManually(string ChannelName, string Username, string ChannelId)
        {
            var returnMsg = string.Empty;
            //Checks whether channelId starts with "G". This is done inorder to make sure that only private channels are added manually
            if (IsPrivateChannel(ChannelId))
            {
                // getting user name from user's slack name
                var applicationUser = _applicationUser.FirstOrDefault(x => x.SlackUserName == Username);
                // getting access token for that user
                if (applicationUser != null)
                {
                    // get access token of user for promact oauth server
                    var accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);
                    //get the project details of the given channel name
                    var project = await _projectUser.GetProjectDetails(ChannelName, accessToken);
                    //add channel details only if the channel has been registered as project in OAuth server
                    if (project != null && project.Id > 0)
                    {
                        SlackChannelDetails channel = new SlackChannelDetails();
                        channel.ChannelId = ChannelId;
                        channel.CreatedOn = DateTime.UtcNow;
                        channel.Deleted = false;
                        channel.Name = ChannelName;
                        _slackChannelRepository.AddSlackChannel(channel);
                        returnMsg = _stringConstant.ChannelAddSuccess;
                    }
                    else
                        returnMsg = _stringConstant.ProjectNotInOAuth;
                }
                else
                    // if user doesn't exist then this message will be shown to user
                    returnMsg = _stringConstant.YouAreNotInExistInOAuthServer;
            }
            else
                return _stringConstant.OnlyPrivateChannel;

            return returnMsg;
        }

        /// <summary>
        /// Used to check whether channelId is of a private channel
        /// </summary>
        /// <param name="ChannelId"></param>
        /// <returns></returns>
        private bool IsPrivateChannel(string ChannelId)
        {
            if (ChannelId.StartsWith(_stringConstant.GroupNameStartsWith, StringComparison.Ordinal))
                return true;
            else
                return false;
        }

        ///// <summary>
        ///// Used to update the scrum answer
        ///// </summary>
        ///// <param name="scrumAnswers"></param>
        ///// <param name="Message"></param>
        ///// <param name="UserName"></param>
        ///// <returns></returns>
        //private string UpdateAnswer(List<ScrumAnswer> ScrumAnswers, string Message, string UserName)
        //{
        //    ScrumAnswer answer = ScrumAnswers.FirstOrDefault();
        //    answer.CreatedOn = DateTime.UtcNow;
        //    answer.AnswerDate = DateTime.UtcNow;
        //    answer.Answer = Message;
        //    _scrumAnswerRepository.Update(answer);
        //    if (ScrumAnswers.Count == 1)
        //    {
        //        //all the answers which were marked to be answered later and after some time marked as ready to be answered now are answered
        //        var returnMsg = string.Format(_stringConstant.ScrumLaterDone, UserName);
        //        return returnMsg;
        //    }
        //    else
        //        return "<@" + UserName + "> " + FetchQuestion(answer.QuestionId, false);
        //}


        /// <summary>
        /// Check which user is supposed to answer the question
        /// </summary>
        /// <param name="ScrumAnswers"></param>
        /// <param name="Employees"></param>
        /// <param name="QuestionCount"></param>
        /// <returns>status</returns>
        private string CheckUser(List<ScrumAnswer> ScrumAnswers, List<User> Employees, int QuestionCount, string Applicant)
        {
            ////scrum answers which were marked as later, are now to be answered
            //var nowReadyScrumsAnswers = ScrumAnswers.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).OrderBy(x => x.Id).ToList();
            //if (nowReadyScrumsAnswers.Any())
            //{
            //    ScrumAnswer answer = nowReadyScrumsAnswers.FirstOrDefault();
            //    User user = Employees.FirstOrDefault(x => x.Id == answer.EmployeeId);
            //    //no keywords are expected, just answers. So ask the person who is to answer now, to answer.
            //    return string.Format(_stringConstant.PleaseAnswer, user.SlackUserName);
            //}
            //else
            //{
            ScrumAnswer scrumAnswer = ScrumAnswers.OrderByDescending(x => x.Id).FirstOrDefault();
            if (scrumAnswer != null)
            {
                var answerCount = ScrumAnswers.Where(x => x.EmployeeId == scrumAnswer.EmployeeId).Count();
                if (answerCount == QuestionCount)
                    //as all the answers of the previous employee has been obtained, any person can be asked question to 
                    return string.Empty;
                else
                {
                    var employee = Employees.Where(x => x.Id == scrumAnswer.EmployeeId).FirstOrDefault();
                    if (employee.SlackUserName == Applicant)
                        return string.Empty;
                    else
                        return string.Format(_stringConstant.PleaseAnswer, employee.SlackUserName);
                }
            }
            else
            {
                //no scrum answers yet. Thus the first employee was asked question. So he is to answer
                var employee = Employees.FirstOrDefault();
                if (employee.SlackUserName == Applicant)
                    return string.Empty;
                else
                    //wrong employee
                    return string.Format(_stringConstant.PleaseAnswer, employee.SlackUserName);
            }
            //  }
        }


        /// <summary>
        /// This method is used to add Scrum answer to the database
        /// </summary>
        /// <param name="ScrumID"></param>
        /// <param name="QuestionId"></param>
        /// <param name="EmployeeId"></param>
        /// <param name="Message"></param>
        /// <param name="Status"></param>
        /// <returns>true if scrum answer is added successfully</returns>
        private bool AddAnswer(int ScrumID, int QuestionId, string EmployeeId, string Message)
        {
            ScrumAnswer answer = new ScrumAnswer();
            answer.Answer = Message;
            answer.AnswerDate = DateTime.UtcNow;
            answer.CreatedOn = DateTime.UtcNow;
            answer.EmployeeId = EmployeeId;
            answer.QuestionId = QuestionId;
            answer.ScrumId = ScrumID;
            _scrumAnswerRepository.Insert(answer);
            return true;
        }


        /// <summary>
        /// This method will be called when the keyword "scrum time" is encountered
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserName"></param>
        /// <param name="AccessToken"></param>
        /// <returns>The next question or the scrum complete message</returns>
        private async Task<string> StartScrum(string GroupName, string UserName, string AccessToken)
        {
            string message = string.Empty;
            ProjectAc project = await _projectUser.GetProjectDetails(GroupName, AccessToken);
            List<User> employees = await _projectUser.GetUsersByGroupName(GroupName, AccessToken);
            List<Question> questionList = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.OrderNumber).ToList();
            ScrumStatus scrumStatus = FetchScrumStatus(GroupName, AccessToken, project, employees, questionList).Result;
            if (scrumStatus == ScrumStatus.NotStarted)
            {
                Question question = questionList.FirstOrDefault();
                Scrum scrum = new Scrum();
                scrum.CreatedOn = DateTime.UtcNow;
                scrum.GroupName = GroupName;
                scrum.ScrumDate = DateTime.UtcNow.Date;
                scrum.ProjectId = project.Id;
                scrum.TeamLeaderId = project.TeamLeaderId;
                scrum.IsHalted = false;
                _scrumRepository.Insert(scrum);

                User firstEmployee = employees.FirstOrDefault();
                //first employee is asked questions along with the previous day status (if any)
                message = _stringConstant.GoodDay + "<@" + firstEmployee.SlackUserName + ">!\n" + FetchPreviousDayStatus(firstEmployee.Id, project.Id) + question.QuestionStatement;
            }
            //handle this message in the else part
            //else if (scrumStatus == ScrumStatus.Halted)
            //    //scrum is halted
            //    message = _stringConstant.ResumeScrum;

            else if (scrumStatus == ScrumStatus.OnGoing)
            {
                //if scrum meeting was interrupted. "scrum time" is written to resume scrum meeting. So next question is fetched.
                var scrumList = _scrumRepository.Fetch(x => String.Compare(x.GroupName, GroupName, true).Equals(0) && x.ScrumDate.Date == DateTime.UtcNow.Date).ToList();
                message = await GetQuestion(scrumList.FirstOrDefault().Id, GroupName, null, null, project.Id, AccessToken);
            }
            else
                return ReplyToClient(scrumStatus);
            return message;
        }


        /// <summary>
        /// This method is used when an employee is on leave or can asnwer only later
        /// </summary>
        /// <param name="ScrumAnswer"></param>
        /// <param name="Employees"></param>
        /// <param name="Parameter"></param>
        /// <param name="ScrumId"></param>
        /// <param name="Applicant"></param>
        /// <param name="Questions"></param>
        /// <param name="GroupName"></param>
        /// <param name="ProjectId"></param>
        /// <param name="UserName"></param>
        /// <param name="AccessToken"></param>
        /// <returns></returns>
        private string LeaveLater(List<ScrumAnswer> ScrumAnswer, List<User> Employees, string Parameter, int ScrumId, string Applicant, List<Question> Questions, string GroupName, int ProjectId, string UserName, string AccessToken)
        {
            string returnMsg = string.Empty;
            var status = ExpectedUser(ScrumId, Questions, Employees, Applicant);//checks whether the applicant is the expected user
            if (status == string.Empty)//if the interacting user is the expected user
            {
                string EmployeeId = Employees.FirstOrDefault(x => x.SlackUserName == Applicant).Id;
                if (ScrumAnswer.Any())
                    //fetch the scrum answer of the employee given on that day
                    ScrumAnswer = ScrumAnswer.Where(x => x.EmployeeId == EmployeeId).ToList();
                //If no anmswer from the employee has been obtained yet.
                if (ScrumAnswer.Count() == 0 && String.Compare(Parameter, _stringConstant.Leave, true).Equals(0))
                {
                    if (String.Compare(UserName, Applicant, true).Equals(0))
                    {
                        return _stringConstant.LeaveError;
                    }
                    else
                    {
                        //all the scrum questions are answered as "leave"
                        foreach (var question in Questions)
                        {
                            AddAnswer(ScrumId, question.Id, EmployeeId, _stringConstant.Leave);
                        }
                    }
                }
                //else if (String.Compare(Parameter, _stringConstant.Later, true).Equals(0))
                //{
                //    //all the questions which are not answered by the employee are fetched
                //    List<Question> questionsNotAnswered = Questions.Where(x => !ScrumAnswer.Select(y => y.QuestionId).ToList().Contains(x.Id)).ToList();
                //    if (questionsNotAnswered.Any())
                //    {
                //        //the scrum questions are answered as "later"
                //        foreach (var question in questionsNotAnswered)
                //        {
                //            AddAnswer(ScrumId, question.Id, EmployeeId, _stringConstant.Later, ScrumAnswerStatus.Later);
                //        }
                //    }
                //    else
                //    {
                //        //if execution reaches here, it is can be understood that scrumAnswer will have answer of the given EmployeeId only.
                //        //checks if there are any scrum answers whose status is not Answered
                //        var answerlist = ScrumAnswer.Where(x => x.ScrumAnswerStatus != ScrumAnswerStatus.Answered).ToList();
                //        if (answerlist.Any())
                //        {
                //            foreach (var answer in answerlist)
                //            {
                //                answer.ScrumAnswerStatus = ScrumAnswerStatus.Later;
                //                _scrumAnswerRepository.Update(answer);
                //            }
                //        }
                //        else
                //            // No test case
                //            //all answers are answered or marked as later earlier
                //            return _stringConstant.AlreadyMarkedAsAnswered;
                //    }
                //}
                else
                    //If the applicant has already answered questions
                    returnMsg = string.Format(_stringConstant.AlreadyAnswered, Applicant);
            }
            else
                return status;
            //fetches the next question or status and returns
            return returnMsg + Environment.NewLine + GetQuestion(ScrumId, GroupName, Questions, Employees, ProjectId, AccessToken).Result;
        }


        ///// <summary>
        ///// This method is used to conduct scrum of a individual person
        ///// </summary>
        ///// <param name="EmployeeId"></param>
        ///// <param name="LaterUserName"></param>
        ///// <param name="ScrumAnswer"></param>
        ///// <param name="GroupName"></param>
        ///// <param name="ProjectId"></param>
        ///// <param name="Employees"></param>
        ///// <param name="Questions"></param>
        ///// <param name="AccessToken"></param>
        ///// <returns>Question statement</returns>
        //private string LaterScrum(List<ScrumAnswer> ScrumAnswer, string EmployeeId, string LaterUserName, string GroupName, int ProjectId, List<User> Employees, List<Question> Questions, string AccessToken)
        //{
        //    string returnMsg = string.Empty;
        //    //scrum answer for the day of the given employee id.
        //    List<ScrumAnswer> employeeScrumAnswers = ScrumAnswer.Where(x => x.EmployeeId == EmployeeId).ToList();
        //    if (employeeScrumAnswers.Any())
        //    {
        //        //scrum answers which are marked as later
        //        List<ScrumAnswer> employeeScrumAnswerWithLater = employeeScrumAnswers.Where(x => x.Answer.Equals(_stringConstant.Later)).ToList();

        //        if (employeeScrumAnswerWithLater.Any())
        //        {
        //            foreach (var answer in employeeScrumAnswerWithLater)
        //            {
        //                //update all these answers to with the status Answer now.
        //                answer.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
        //                _scrumAnswerRepository.Update(answer);
        //            }
        //            if (employeeScrumAnswers.Count == employeeScrumAnswerWithLater.Count)//all the answers are marked as later
        //                                                                                 //so fetch the first question
        //                returnMsg = "<@" + LaterUserName + "> " + FetchQuestion(null, true);
        //            else
        //            {
        //                // only some answers are marked as later. So the last question which was answered is fetched and the next question is asked.
        //                var questionId = employeeScrumAnswers.Where(x => !employeeScrumAnswerWithLater.Select(y => y.Id).ToList().Contains(x.Id)).OrderByDescending(x => x.Id).Select(x => x.QuestionId).FirstOrDefault();
        //                returnMsg = "<@" + LaterUserName + "> " + FetchQuestion(questionId, false);
        //            }
        //        }
        //        else
        //            //if no answer is marked as later
        //            returnMsg = _stringConstant.AllAnswerRecorded + Environment.NewLine + GetQuestion(ScrumAnswer.First().ScrumId, GroupName, Questions, Employees, ProjectId, AccessToken).Result;
        //    }
        //    else
        //        //if no answer of the employee is recorded yet
        //        returnMsg = string.Format(_stringConstant.NotLaterYet, LaterUserName) + FetchQuestion(null, true);

        //    return returnMsg;
        //}


        /// <summary>
        /// Used to fetch the next question based on the given parameters
        /// </summary>
        /// <param name="ScrumId"></param>
        /// <param name="GroupName"></param>
        /// <param name="Employees"></param>
        /// <param name="Questions"></param>
        ///<param name="ProjectId"></param>
        ///<param name="AccessToken"></param>
        /// <returns>The next question or the scrum complete message</returns>
        private async Task<string> GetQuestion(int ScrumId, string GroupName, List<Question> Questions, List<User> Employees, int ProjectId, string AccessToken)
        {
            string returnMsg = string.Empty;
            List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumId == ScrumId).ToList();
            if (Questions == null)
                Questions = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.OrderNumber).ToList();
            if (Employees == null)
                Employees = await _projectUser.GetUsersByGroupName(GroupName, AccessToken);

            if (scrumAnswer.Any())
            {
                ////scrum answers which were marked as later, are now to be answered
                //var laterAnswers = scrumAnswer.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).OrderBy(x => x.Id).ToList();
                //if (laterAnswers.Any())
                //{
                //    ScrumAnswer ans = laterAnswers.FirstOrDefault();
                //    //the first question which is to be answered now is asked
                //    return "<@" + Employees.FirstOrDefault(x => x.Id == ans.EmployeeId).SlackUserName + "> " + Questions.FirstOrDefault(x => x.Id == ans.QuestionId).QuestionStatement;
                //}
                //else
                //{
                #region Normal Get Question
                int questionCount = Questions.Count();
                //last acrum answer of the given scrum id.
                ScrumAnswer lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                //no. of answers given by the employee who gave the last scrum answer.
                int answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrumAnswer.EmployeeId).Count();
                if (answerListCount == questionCount)
                {
                    //all questions have been asked to the previous employee                        
                    var idList = Employees.Where(x => !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                    if (idList != null && idList.Count > 0)
                    {
                        //now fetch the first question to the next employee
                        User user = Employees.FirstOrDefault(x => x.Id == idList.FirstOrDefault());
                        returnMsg = _stringConstant.GoodDay + "<@" + user.SlackUserName + ">!\n" + FetchPreviousDayStatus(user.Id, ProjectId) + FetchQuestion(null, true);
                    }
                    else
                    {
                        //var laterList = scrumAnswer.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.Later).ToList();
                        //if (laterList != null && laterList.Count > 0)
                        //    //some are still marked to be answered later
                        //    returnMsg = _stringConstant.ScrumConcludedButLater;
                        //else
                        //answers of all the employees has been recorded
                        returnMsg = _stringConstant.ScrumComplete;
                    }
                }
                else
                {
                    //as not all questions have been answered by the last employee,the next question to that employee will be asked
                    User user = Employees.FirstOrDefault(x => x.Id == lastScrumAnswer.EmployeeId);
                    returnMsg = "<@" + user.SlackUserName + "> " + FetchQuestion(lastScrumAnswer.QuestionId, false);
                }
                #endregion
                //}
            }
            else
                //no scrum answer has been recorded yet. So first question to the first employee
                returnMsg = _stringConstant.GoodDay + "<@" + Employees.FirstOrDefault().SlackUserName + ">!\n" + FetchPreviousDayStatus(Employees.FirstOrDefault().Id, ProjectId) + Questions.FirstOrDefault().QuestionStatement;

            return returnMsg;
        }


        /// <summary>
        /// Used to fetch the next question based on the given parameters
        /// </summary>
        /// <param name="ScrumId"></param>
        /// <param name="Employees"></param>
        /// <param name="Questions"></param>
        ///<param name="ProjectId"></param>
        /// <returns>The next question or the scrum complete message</returns>
        private string ExpectedUser(int ScrumId, List<Question> Questions, List<User> Employees, string Applicant)
        {
            //List of scrum answer of the given scrumId.
            List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.Fetch(x => x.ScrumId == ScrumId).ToList();
            User user = new User();

            if (scrumAnswer.Any())
            {
                ////scrum answers which were marked as later, are now to be answered
                //var readyToBeAnswered = scrumAnswer.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).ToList();
                //if (readyToBeAnswered.Any())
                //{
                //    ScrumAnswer answer = readyToBeAnswered.FirstOrDefault();
                //    user = Employees.FirstOrDefault(x => x.Id == answer.EmployeeId);
                //}
                //else
                //{
                int questionCount = Questions.Count();
                //last acrum answer of the given scrum id.
                ScrumAnswer lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                //no. of answers given by the employee who gave the last scrum answer.
                int answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrumAnswer.EmployeeId).Count();
                if (answerListCount == questionCount)
                {
                    //all questions have been asked to the previous employee                        
                    var idList = Employees.Where(x => !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                    if (idList != null && idList.Count > 0)
                        //now the next employee
                        user = Employees.FirstOrDefault(x => x.Id == idList.FirstOrDefault());
                    else
                        return _stringConstant.ScrumComplete;
                    //else
                    //{
                    //    var list = scrumAnswer.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.Later).ToList();
                    //    if (list != null && list.Count > 0)//some are still marked to be answered later
                    //        return _stringConstant.ScrumConcludedButLater;
                    //    else
                    //        return _stringConstant.ScrumComplete;
                    //}
                }
                else
                    //as not all questions have been answered by the last employee,so to that employee itself
                    user = Employees.FirstOrDefault(x => x.Id == lastScrumAnswer.EmployeeId);
                // }
            }
            else
                //no scrum answer has been recorded yet. So first employee
                user = Employees.FirstOrDefault();

            if (user != null && user.SlackUserName == Applicant)
                return string.Empty;
            else if (user == null)
                return string.Format(_stringConstant.NotExpected, Applicant);
            else
                return string.Format(_stringConstant.PleaseAnswer, user.SlackUserName);
        }


        /// <summary>
        /// This method fetches the Question statement of next order of the given questionId or the first question statement
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <param name="isFirstQuestion"></param>
        /// <returns></returns>
        private string FetchQuestion(int? QuestionId, bool isFirstQuestion)
        {
            if (isFirstQuestion)
            {
                //fetch the first question statement
                var question = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.OrderNumber).FirstOrDefault();
                return question.QuestionStatement;
            }
            else
            {
                //order number of the given question 
                var orderNumber = _questionRepository.FirstOrDefault(x => x.Id == QuestionId).OrderNumber;
                //question with the next order
                var question = _questionRepository.FirstOrDefault(x => x.OrderNumber == orderNumber + 1 && x.Type == 1);
                if (question != null)
                    return question.QuestionStatement;
                else
                    return _stringConstant.NoQuestion;
            }
        }


        /// <summary>
        /// Fetches the previous day's questions and answers of the employee of the given id for the given project
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        private string FetchPreviousDayStatus(string EmployeeId, int ProjectId)
        {
            string previousDayStatus = string.Empty;
            //previous scrums
            List<Scrum> scrumList = _scrumRepository.Fetch(x => x.ProjectId == ProjectId && x.ScrumDate < DateTime.UtcNow.Date).OrderByDescending(x => x.ScrumDate).ToList();
            if (scrumList.Any())
            {
                //previous scrum
                Scrum previousScrum = scrumList.FirstOrDefault();
                List<Question> questions = _questionRepository.Fetch(x => x.Type == 1).OrderBy(x => x.OrderNumber).ToList();
                List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.Fetch(x => x.ScrumId == previousScrum.Id && x.EmployeeId == EmployeeId).ToList();
                if (scrumAnswers.Any() && questions.Any())
                {
                    previousDayStatus = Environment.NewLine + _stringConstant.PreviousDayStatus + Environment.NewLine;
                    foreach (var question in questions)
                    {
                        //Question and the corresponding answer appended
                        previousDayStatus += "*_Q_*: " + question.QuestionStatement + Environment.NewLine + "*_A_*: _" + scrumAnswers.FirstOrDefault(x => x.QuestionId == question.Id).Answer + "_" + Environment.NewLine;
                    }
                    previousDayStatus += Environment.NewLine + _stringConstant.AnswerToday + Environment.NewLine + Environment.NewLine;
                }
            }
            return previousDayStatus;
        }



        private async Task<ScrumStatus> FetchScrumStatus(string groupName, string accessToken, ProjectAc project, List<User> employees, List<Question> questions)
        {
            if (project == null)
                project = await _projectUser.GetProjectDetails(groupName, accessToken);
            if (project != null && project.Id > 0)
            {
                if (employees.Count == 0)
                    employees = await _projectUser.GetUsersByGroupName(groupName, accessToken);
                if (employees.Count > 0)
                {
                    if (questions.Count == 0)
                        questions = _questionRepository.Fetch(x => x.Type == 1).ToList();
                    if (questions.Count > 0)
                    {
                        var scrumList = _scrumRepository.Fetch(x => String.Compare(x.GroupName, groupName, true).Equals(0)).ToList();
                        var scrum = scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);
                        if (scrum != null)
                        {
                            if (!scrum.IsHalted)
                            {
                                List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id).Result.ToList();
                                int questionCount = questions.Count();

                                if (scrumAnswers.Any())
                                {
                                    if (scrumAnswers.Count >= questionCount * employees.Count)
                                    {
                                        return ScrumStatus.Completed;
                                    }
                                    else
                                        return ScrumStatus.OnGoing;
                                }
                                else
                                    return ScrumStatus.OnGoing;
                            }
                            else
                                return ScrumStatus.Halted;
                        }
                        else
                            return ScrumStatus.NotStarted;
                    }
                    else
                        return ScrumStatus.NoQuestion;
                }
                else
                    return ScrumStatus.NoEmployee;
            }
            else
                return ScrumStatus.NoProject;
        }


        private string ScrumHalt(Scrum scrum, string groupName, string accessToken)
        {
            ScrumStatus status = FetchScrumStatus(groupName, accessToken, null, null, null).Result;
            if (status == (ScrumStatus.OnGoing))
            {
                //keyword encountered is "scrum halt"

                if (scrum.IsHalted)
                    return _stringConstant.ScrumAlreadyHalted;
                else
                {
                    //scrum halted
                    scrum.IsHalted = true;
                    return _stringConstant.ScrumHalted;
                }

            }
            else
            {
                return "Scrum cannot be halted." + ReplyToClient(status);
            }
        }


        private string ScrumResume(Scrum scrum, string groupName, string accessToken)
        {
            ScrumStatus status = FetchScrumStatus(groupName, accessToken, null, null, null).Result;
            if (status == (ScrumStatus.OnGoing))
            {
                //keyword encountered is "scrum resume"               
                var returnMsg = string.Empty;
                if (scrum.IsHalted)
                {
                    //scrum resumed
                    scrum.IsHalted = false;
                    returnMsg = _stringConstant.ScrumResumed;
                }
                else
                    returnMsg = _stringConstant.ScrumNotHalted;
                //when the scrum is resumed then, the next question is to be asked
                returnMsg += GetQuestion(scrum.Id, groupName, null, null, scrum.ProjectId, accessToken).Result;
                return returnMsg;
            }
            else
                return ReplyToClient(status);
        }


        private string ReplyToClient(ScrumStatus scrumStatus)
        {
            string returnMessage = string.Empty;
            switch (scrumStatus)
            {
                case ScrumStatus.Completed:
                    returnMessage = "Scrum for today has been concluded";
                    break;
                case ScrumStatus.Halted:
                    returnMessage = "Scrum is halted";
                    break;
                case ScrumStatus.NoEmployee:
                    returnMessage = _stringConstant.NoEmployeeFound;
                    break;
                case ScrumStatus.NoProject:
                    returnMessage = _stringConstant.NoProjectFound;
                    break;
                case ScrumStatus.NoQuestion:
                    returnMessage = _stringConstant.NoQuestion;
                    break;
                case ScrumStatus.NotStarted:
                    returnMessage = _stringConstant.ScrumNotStarted;
                    break;
                case ScrumStatus.OnGoing:
                    returnMessage = "Scrum is in progress";
                    break;
                default: return null;
            }
            return returnMessage;

        }


        #endregion


    }
}