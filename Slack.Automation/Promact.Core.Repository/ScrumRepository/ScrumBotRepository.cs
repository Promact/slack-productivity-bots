using Autofac.Extras.NLog;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.OauthCallsRepository;
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
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IHttpClientRepository _httpClientRepository;
        private readonly ISlackUserRepository _slackUserDetails;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IRepository<SlackBotUserDetail> _slackBotUserDetail;


        #endregion


        #region Constructor


        public ScrumBotRepository(IRepository<ScrumAnswer> scrumAnswerRepository,
            IOauthCallsRepository oauthCallsRepository,
            IRepository<Scrum> scrumRepository, IAttachmentRepository attachmentRepository,
            IRepository<Question> questionRepository, IHttpClientRepository httpClientRepository,
            IRepository<ApplicationUser> applicationUser, ISlackChannelRepository slackChannelRepository,
            ISlackUserRepository slackUserDetails, IStringConstantRepository stringConstant,
            IRepository<SlackBotUserDetail> slackBotUserDetail)
        {
            _scrumAnswerRepository = scrumAnswerRepository;
            _scrumRepository = scrumRepository;
            _questionRepository = questionRepository;
            _oauthCallsRepository = oauthCallsRepository;
            _slackChannelRepository = slackChannelRepository;
            _applicationUser = applicationUser;
            _attachmentRepository = attachmentRepository;
            _httpClientRepository = httpClientRepository;
            _slackUserDetails = slackUserDetails;
            _slackBotUserDetail = slackBotUserDetail;
            _stringConstant = stringConstant;
        }


        #endregion


        #region Public Method 


        /// <summary>
        /// This will process the messages from slack and use appropriate methods to give a suitable response through Bot
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="channelId"></param>
        /// <param name="message"></param>
        /// <returns>reply message</returns>
        public async Task<string> ProcessMessagesAsync(string userId, string channelId, string message)
        {
            string replyText = string.Empty;
            SlackUserDetails user = await _slackUserDetails.GetByIdAsync(userId);
            SlackChannelDetails channel = await _slackChannelRepository.GetByIdAsync(channelId);
            //the command is split to individual words
            //commnads ex: "scrum time", "later @userId"
            string[] messageArray = message.Split(null);
            if (user != null && !user.Deleted && String.Compare(message, _stringConstant.ScrumHelp, true) == 0)
                replyText = _stringConstant.ScrumHelpMessage;
            else if (user != null && !user.Deleted && channel != null && !channel.Deleted)
            {
                //commands could be"scrum time" or "scrum halt" or "scrum resume"
                if (String.Compare(message, _stringConstant.ScrumTime, true) == 0 || String.Compare(message, _stringConstant.ScrumHalt, true) == 0 || String.Compare(message, _stringConstant.ScrumResume, true) == 0)
                    replyText = await ScrumAsync(channel.Name, user.Name, messageArray[1].ToLower(), user.UserId);
                //a particular employee is on leave, getting marked as later or asked question again
                //command would be like "leave <@userId>"
                else if ((String.Compare(messageArray[0], _stringConstant.Leave, true) == 0) && messageArray.Length == 2)
                {
                    int fromIndex = message.IndexOf("<@") + "<@".Length;
                    int toIndex = message.LastIndexOf(">");
                    if (toIndex > 0 && fromIndex > 1)
                    {
                        //the userId is fetched
                        string applicantId = message.Substring(fromIndex, toIndex - fromIndex);
                        //fetch the user of the given userId
                        SlackUserDetails applicant = await _slackUserDetails.GetByIdAsync(applicantId);
                        if (applicant != null)
                        {
                            string applicantName = applicant.Name;
                            replyText = await LeaveAsync(channel.Name, user.Name, user.UserId, applicantName, applicantId);
                        }
                        else
                            replyText = _stringConstant.NotAUser;
                    }
                    else
                        replyText = await AddScrumAnswerAsync(user.Name, message, channel.Name, user.UserId);
                }
                //all other texts
                else
                    replyText = await AddScrumAnswerAsync(user.Name, message, channel.Name, user.UserId);
            }
            //If channel is not registered in the database
            else if (user != null && !user.Deleted)
            {
                //If channel is not registered in the database and the command encountered is "add channel channelname"
                if (channel == null && String.Compare(messageArray[0], _stringConstant.Add, true) == 0 && String.Compare(messageArray[1], _stringConstant.Channel, true) == 0)
                    replyText = await AddChannelManuallyAsync(messageArray[2], user.Name, channelId, user.UserId);
                else if (((String.Compare(messageArray[0], _stringConstant.Leave, true) == 0) && messageArray.Length == 2) || String.Compare(message, _stringConstant.ScrumTime, true) == 0 || String.Compare(message, _stringConstant.ScrumHalt, true) == 0 || String.Compare(message, _stringConstant.ScrumResume, true) == 0)
                    replyText = _stringConstant.ChannelAddInstruction;
            }
            else if (user == null)
            {
                SlackBotUserDetail botUser = await _slackBotUserDetail.FirstOrDefaultAsync(x => x.UserId == userId);
                if (botUser == null && (((String.Compare(messageArray[0], _stringConstant.Leave, true) == 0) && messageArray.Length == 2) || String.Compare(message, _stringConstant.ScrumTime, true) == 0 || String.Compare(message, _stringConstant.ScrumHalt, true) == 0 || String.Compare(message, _stringConstant.ScrumResume, true) == 0))
                    replyText = _stringConstant.SlackUserNotFound;
            }

            return replyText;
        }


        #endregion


        #region Private Methods


        /// <summary>
        ///  This method is called whenever a message other than the default keywords is written in the group. - JJ
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <param name="groupName"></param>
        /// <param name="userId"></param>
        /// <returns>the next question statement</returns>
        private async Task<string> AddScrumAnswerAsync(string userName, string message, string groupName, string userId)
        {
            string reply = string.Empty;
            //scrum of the group 
            List<Scrum> scrumList = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, groupName, true) == 0).Result.ToList();
            if (scrumList.Count > 0)
            {
                //today's scrum of the group
                Scrum scrum = scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);
                if (scrum != null && scrum.IsOngoing && !scrum.IsHalted)
                {
                    // getting user name from user's slack name
                    ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == userId);
                    // getting access token for that user
                    if (applicationUser != null)
                    {
                        // get access token of user for promact oauth server
                        string accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);
                        //list of scrum questions. Type =1
                        List<Question> questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();
                        if (questions.Count > 0)
                        {
                            //employees of the given group name fetched from the oauth server
                            List<User> employees = await _oauthCallsRepository.GetUsersByGroupName(groupName, accessToken);
                            int questionCount = questions.Count();
                            //scrum answer of that day's scrum
                            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id).Result.ToList();
                            //status would be empty if the interacting user is same as the expected user.
                            string status = await ExpectedUserAsync(scrum.Id, questions, employees, userName, userId);
                            if (status == string.Empty)
                            {

                                #region Normal Scrum

                                if ((employees.Count() * questionCount) > scrumAnswers.Count())
                                {
                                    Question firstQuestion = questions.FirstOrDefault();
                                    ScrumAnswer lastScrumAnswer = scrumAnswers.OrderByDescending(x => x.Id).FirstOrDefault();
                                    //scrum answers of the given employee
                                    int answerListCount = scrumAnswers.Count(x => x.EmployeeId == lastScrumAnswer.EmployeeId);

                                    if (scrumAnswers.Any())
                                    {
                                        if (answerListCount < questionCount)
                                        {
                                            //not all questions have been answered
                                            Question prevQuestion = questions.FirstOrDefault(x => x.Id == lastScrumAnswer.QuestionId);
                                            Question question = questions.FirstOrDefault(x => x.OrderNumber == prevQuestion.OrderNumber + 1);
                                            await AddAnswerAsync(lastScrumAnswer.ScrumId, question.Id, lastScrumAnswer.EmployeeId, message);
                                        }
                                        else
                                        {
                                            //A particular employee's first answer
                                            //list of Employee ids who have not answered yet                       
                                            List<string> idList = employees.Where(x => !scrumAnswers.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                                            if (idList != null && idList.Count > 0)
                                            {
                                                //now fetch the first question to the next employee
                                                User user = employees.FirstOrDefault(x => x.Id == idList.FirstOrDefault());
                                                await AddAnswerAsync(lastScrumAnswer.ScrumId, firstQuestion.Id, user.Id, message);
                                            }
                                        }
                                        //get the next question 
                                        //donot shift message                                         
                                        reply = await GetQuestionAsync(scrum.Id, groupName, questions, employees, scrum.ProjectId, accessToken);
                                    }
                                    else
                                    {
                                        //First Employee's first answer
                                        User user = employees.FirstOrDefault();
                                        await AddAnswerAsync(scrum.Id, firstQuestion.Id, user.Id, message);
                                        //get the next question . donot shift message 
                                        reply = await GetQuestionAsync(scrum.Id, groupName, questions, employees, scrum.ProjectId, accessToken);
                                    }
                                }


                                #endregion

                            }
                            //the user interacting is not the expected user
                            else if ((status != _stringConstant.ScrumConcludedButLater) && (status != _stringConstant.ScrumComplete))
                                return status;
                        }
                        else
                            return _stringConstant.NoQuestion;
                    }
                    else
                        // if user doesn't exist then this message will be shown to user
                        return _stringConstant.YouAreNotInExistInOAuthServer;
                }
            }
            return reply;
        }


        /// <summary>
        /// This method will be called when the keyword "scrum time" or "scrum halt" or "scrum resume" is encountered
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userName"></param>
        /// <param name="parameter"></param>      
        /// <param name="userId"></param>
        /// <returns>The question or the status of the scrum</returns>
        private async Task<string> ScrumAsync(string groupName, string userName, string parameter, string userId)
        {
            // getting user name from user's slack name
            ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == userId);
            // getting access token for that user
            if (applicationUser != null)
            {
                // get access token of user for promact oauth server
                string accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);

                Scrum scrum = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, groupName, true) == 0).Result.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);
                ScrumActions scrumStage = (ScrumActions)Enum.Parse(typeof(ScrumActions), parameter);
                switch (scrumStage)
                {
                    case ScrumActions.halt:
                        return await ScrumHaltAsync(scrum, groupName, accessToken);

                    case ScrumActions.resume:
                        return await ScrumResumeAsync(scrum, groupName, accessToken);

                    case ScrumActions.time:
                        //keyword encountered is "scrum time"
                        return await StartScrumAsync(groupName, userName, accessToken);

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
        /// <param name="groupName"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <param name="applicant"></param>
        /// <param name="applicantId"></param>
        /// <param name="parameter"></param>
        /// <returns>Question to the next person or other scrum status</returns>
        private async Task<string> LeaveAsync(string groupName, string userName, string userId, string applicant, string applicantId)
        {
            string returnMsg = string.Empty;
            List<Scrum> scrumList = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, groupName, true) == 0).Result.ToList();
            Scrum scrum = scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);
            if (scrumList.Count > 0 && scrum != null)
            {
                if (scrum.IsOngoing)
                {
                    if (!scrum.IsHalted)
                    {
                        // getting user name from user's slack name
                        ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == userId);
                        // getting access token for that user
                        if (applicationUser != null)
                        {
                            // get access token of user for promact oauth server
                            string accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);

                            List<Question> questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();
                            List<User> employees = await _oauthCallsRepository.GetUsersByGroupName(groupName, accessToken);

                            ScrumStatus scrumStatus = await FetchScrumStatusAsync(groupName, accessToken, null, employees, questions);

                            if (scrumStatus == ScrumStatus.OnGoing)
                            {
                                List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id).Result.ToList();
                                //keyword "leave @username" 
                                returnMsg = await AsyncLeave(scrumAnswer, employees, scrum.Id, applicant, questions, groupName, scrum.ProjectId, userName, accessToken, userId, applicantId);
                            }
                            else
                                returnMsg = ReplyToClient(scrumStatus);
                        }
                        else
                            // if user doesn't exist then this message will be shown to user
                            returnMsg = _stringConstant.YouAreNotInExistInOAuthServer;
                    }
                    else
                        returnMsg = _stringConstant.ScrumIsHalted;
                }
                else
                    returnMsg = _stringConstant.ScrumAlreadyConducted;
            }
            else
                returnMsg = _stringConstant.ScrumNotStarted;
            return returnMsg;
        }


        /// <summary>
        /// Used to add channel manually by command "add channel channelname"
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="channelId"></param>
        /// <param name="username"></param>
        /// <param name="userId"></param>
        /// <returns>status message</returns>
        private async Task<string> AddChannelManuallyAsync(string channelName, string username, string channelId, string userId)
        {
            string returnMsg = string.Empty;
            //Checks whether channelId starts with "G". This is done inorder to make sure that only private channels are added manually
            if (IsPrivateChannel(channelId))
            {
                // getting user name from user's slack name
                ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == userId);
                // getting access token for that user
                if (applicationUser != null)
                {
                    // get access token of user for promact oauth server
                    string accessToken = await _attachmentRepository.AccessToken(applicationUser.UserName);
                    //get the project details of the given channel name
                    ProjectAc project = await _oauthCallsRepository.GetProjectDetails(channelName, accessToken);
                    //add channel details only if the channel has been registered as project in OAuth server
                    if (project != null && project.Id > 0)
                    {
                        SlackChannelDetails channel = new SlackChannelDetails();
                        channel.ChannelId = channelId;
                        channel.CreatedOn = DateTime.UtcNow;
                        channel.Deleted = false;
                        channel.Name = channelName;
                        await _slackChannelRepository.AddSlackChannelAsync(channel);
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
        /// <param name="channelId"></param>
        /// <returns>true if private channel</returns>
        private bool IsPrivateChannel(string channelId)
        {
            if (channelId.StartsWith(_stringConstant.GroupNameStartsWith, StringComparison.Ordinal))
                return true;
            else
                return false;
        }


        /// <summary>
        /// This method is used to add Scrum answer to the database
        /// </summary>
        /// <param name="scrumId"></param>
        /// <param name="questionId"></param>
        /// <param name="employeeId"></param>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <returns>true if scrum answer is added successfully</returns>
        private async Task<bool> AddAnswerAsync(int scrumId, int questionId, string employeeId, string message)
        {
            ScrumAnswer answer = new ScrumAnswer();
            answer.Answer = message;
            answer.AnswerDate = DateTime.UtcNow;
            answer.CreatedOn = DateTime.UtcNow;
            answer.EmployeeId = employeeId;
            answer.QuestionId = questionId;
            answer.ScrumId = scrumId;
            _scrumAnswerRepository.Insert(answer);
            await _scrumAnswerRepository.SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// This method will be called when the keyword "scrum time" is encountered
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns>The next question or the scrum complete message</returns>
        private async Task<string> StartScrumAsync(string groupName, string userName, string accessToken)
        {
            string replyMessage = string.Empty;
            ProjectAc project = await _oauthCallsRepository.GetProjectDetails(groupName, accessToken);
            List<User> employees = await _oauthCallsRepository.GetUsersByGroupName(groupName, accessToken);
            List<Question> questionList = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();

            ScrumStatus scrumStatus = await FetchScrumStatusAsync(groupName, accessToken, project, employees, questionList);
            if (scrumStatus == ScrumStatus.NotStarted)
            {
                Question question = questionList.FirstOrDefault();
                Scrum scrum = new Scrum();
                scrum.CreatedOn = DateTime.UtcNow;
                scrum.GroupName = groupName;
                scrum.ScrumDate = DateTime.UtcNow.Date;
                scrum.ProjectId = project.Id;
                scrum.TeamLeaderId = project.TeamLeaderId;
                scrum.IsHalted = false;
                scrum.IsOngoing = true;
                _scrumRepository.Insert(scrum);
                await _scrumRepository.SaveChangesAsync();
                User firstEmployee = employees.FirstOrDefault();
                //first employee is asked questions along with the previous day status (if any)
                replyMessage = _stringConstant.GoodDay + "<@" + userName + ">!\n" + FetchPreviousDayStatusAsync(firstEmployee.Id, project.Id).Result + question.QuestionStatement;
            }

            else if (scrumStatus == ScrumStatus.OnGoing)
            {
                //if scrum meeting was interrupted. "scrum time" is written to resume scrum meeting. So next question is fetched.
                List<Scrum> scrumList = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, groupName, true) == 0).Result.ToList();
                replyMessage = await GetQuestionAsync(scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date).Id, groupName, null, null, project.Id, accessToken);
            }
            else
                return ReplyToClient(scrumStatus);
            return replyMessage;
        }


        /// <summary>
        /// This method is used when an employee is on leave or can asnwer only later
        /// </summary>
        /// <param name="scrumAnswer"></param>
        /// <param name="employees"></param>
        /// <param name="scrumId"></param>
        /// <param name="applicant"></param>
        /// <param name="questions"></param>
        /// <param name="groupName"></param>
        /// <param name="projectId"></param>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task<string> AsyncLeave(List<ScrumAnswer> scrumAnswer, List<User> employees, int scrumId, string applicant, List<Question> questions, string groupName, int projectId, string userName, string accessToken, string userId, string applicantId)
        {
            string returnMsg = string.Empty;
            string status = await ExpectedUserAsync(scrumId, questions, employees, applicant, applicantId);//checks whether the applicant is the expected user
            if (status == string.Empty)//if the interacting user is the expected user
            {
                string EmployeeId = employees.FirstOrDefault(x => x.SlackUserId == applicantId).Id;
                if (String.Compare(userId, applicantId, true) == 0)
                    return _stringConstant.LeaveError;
                else
                {
                    if (scrumAnswer.Any())
                        //fetch the scrum answer of the employee given on that day
                        scrumAnswer = scrumAnswer.Where(x => x.EmployeeId == EmployeeId).ToList();
                    //If no anmswer
                    //from the employee has been obtained yet.
                    if (scrumAnswer.Count() == 0)
                    {
                        //all the scrum questions are answered as "leave"
                        foreach (var question in questions)
                        {
                            await AddAnswerAsync(scrumId, question.Id, EmployeeId, _stringConstant.Leave);
                        }
                    }
                    else
                        //If the applicant has already answered questions
                        returnMsg = string.Format(_stringConstant.AlreadyAnswered, applicant);
                }
            }
            else
                return status;
            //fetches the next question or status and returns
            return returnMsg + Environment.NewLine + GetQuestionAsync(scrumId, groupName, questions, employees, projectId, accessToken).Result;
        }


        /// <summary>
        /// Used to fetch the next question based on the given parameters
        /// </summary>
        /// <param name="scrumId"></param>
        /// <param name="groupName"></param>
        /// <param name="employees"></param>
        /// <param name="questions"></param>
        ///<param name="projectId"></param>
        ///<param name="accessToken"></param>
        /// <returns>The next question or the scrum complete message</returns>
        private async Task<string> GetQuestionAsync(int scrumId, string groupName, List<Question> questions, List<User> employees, int projectId, string accessToken)
        {
            string returnMsg = string.Empty;
            List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();

            if (questions == null)
                questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();

            if (employees == null)
                employees = await _oauthCallsRepository.GetUsersByGroupName(groupName, accessToken);

            if (scrumAnswer.Any())
            {
                #region Normal Get Question

                int questionCount = questions.Count();
                //last acrum answer of the given scrum id.
                ScrumAnswer lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                //no. of answers given by the employee who gave the last scrum answer.
                int answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrumAnswer.EmployeeId).Count();
                if (answerListCount == questionCount)
                {
                    //all questions have been asked to the previous employee                        
                    //list of Employee ids who have not answer yet                       
                    List<string> idList = employees.Where(x => !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                    if (idList != null && idList.Count > 0)
                    {
                        //now fetch the first question to the next employee
                        User user = employees.FirstOrDefault(x => x.Id == idList.FirstOrDefault());
                        SlackUserDetails slackUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                        returnMsg = _stringConstant.GoodDay + "<@" + slackUser.Name + ">!\n" + FetchPreviousDayStatusAsync(user.Id, projectId).Result + FetchQuestionAsync(null, true).Result;
                    }
                    else
                    {
                        if (await MarkScrumCompleteAsync(lastScrumAnswer.ScrumId) == 0)
                            //answers of all the employees has been recorded
                            returnMsg = _stringConstant.ScrumComplete;
                        else
                            returnMsg = _stringConstant.ErrorMsg;
                    }
                }
                else
                {
                    //as not all questions have been answered by the last employee,the next question to that employee will be asked
                    User user = employees.FirstOrDefault(x => x.Id == lastScrumAnswer.EmployeeId);
                    SlackUserDetails slackUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                    returnMsg = "<@" + slackUser.Name + "> " + FetchQuestionAsync(lastScrumAnswer.QuestionId, false).Result;
                }

                #endregion
            }
            else
            {
                //no scrum answer has been recorded yet. So first question to the first employee
                SlackUserDetails slackUser = await _slackUserDetails.GetByIdAsync(employees.FirstOrDefault().SlackUserId);
                returnMsg = _stringConstant.GoodDay + "<@" + slackUser.Name + ">!\n" + FetchPreviousDayStatusAsync(employees.FirstOrDefault().Id, projectId).Result + questions.FirstOrDefault().QuestionStatement;
            }
            return returnMsg;
        }


        /// <summary>
        /// Used to mark scrum as completed
        /// </summary>
        /// <param name="scrumId"></param>
        private async Task<int> MarkScrumCompleteAsync(int scrumId)
        {
            Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => x.Id == scrumId);
            scrum.IsOngoing = false;
            _scrumRepository.Update(scrum);
            await _scrumRepository.SaveChangesAsync();
            return 0;
        }


        /// <summary>
        /// Used to fetch the next question based on the given parameters
        /// </summary>
        /// <param name="scrumId"></param>
        /// <param name="employees"></param>
        /// <param name="questions"></param>
        ///<param name="projectId"></param>
        ///<param name="applicantId"></param>
        /// <returns>empty string if the expected user is same as the applicant</returns>
        private async Task<string> ExpectedUserAsync(int scrumId, List<Question> questions, List<User> employees, string applicant, string applicantId)
        {
            //List of scrum answer of the given scrumId.
            List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();
            User user = new User();

            if (scrumAnswer.Any())
            {
                int questionCount = questions.Count();
                //last acrum answer of the given scrum id.
                ScrumAnswer lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                //no. of answers given by the employee who gave the last scrum answer.
                int answerListCount = scrumAnswer.Count(x => x.EmployeeId == lastScrumAnswer.EmployeeId);
                if (answerListCount == questionCount)
                {
                    //all questions have been asked to the previous employee 
                    //list of Employee ids who have not answer yet                       
                    List<string> idList = employees.Where(x => !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                    if (idList != null && idList.Count > 0)
                        //now the next employee
                        user = employees.FirstOrDefault(x => x.Id == idList.FirstOrDefault());
                    else
                        return _stringConstant.ScrumComplete;
                }
                else
                    //as not all questions have been answered by the last employee,so to that employee itself
                    user = employees.FirstOrDefault(x => x.Id == lastScrumAnswer.EmployeeId);
            }
            else
                //no scrum answer has been recorded yet. So first employee
                user = employees.FirstOrDefault();

            if (user != null && user.SlackUserId == applicantId)
                return string.Empty;
            else if (user == null)
                return string.Format(_stringConstant.NotExpected, applicant);
            else
            {
                SlackUserDetails slackUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                return string.Format(_stringConstant.PleaseAnswer, slackUser.Name);
            }
        }


        /// <summary>
        /// This method fetches the Question statement of next order of the given questionId or the first question statement
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="isFirstQuestion"></param>
        /// <returns>Question statement</returns>
        private async Task<string> FetchQuestionAsync(int? questionId, bool isFirstQuestion)
        {
            Question question = new Question();
            if (isFirstQuestion)
            {
                //fetch the first question statement
                List<Question> questionList = _questionRepository.FetchAsync(x => x.Type == 1).Result.ToList();
                if (questionList.Count > 0)
                {
                    question = questionList.OrderBy(x => x.OrderNumber).FirstOrDefault();
                    return question.QuestionStatement;
                }
                else
                    return _stringConstant.NoQuestion;
            }
            else
            {
                //order number of the given question 
                question = await _questionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
                if (question != null)
                {
                    int orderNumber = question.OrderNumber;
                    //question with the next order
                    question = await _questionRepository.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber + 1 && x.Type == 1);
                    if (question != null)
                        return question.QuestionStatement;
                    else
                        return _stringConstant.NoQuestion;
                }
                else
                    return _stringConstant.NoQuestion;
            }
        }


        /// <summary>
        /// Fetches the previous day's questions and answers of the employee of the given id for the given project
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="projectId"></param>
        /// <returns>previous day status</returns>
        private async Task<string> FetchPreviousDayStatusAsync(string employeeId, int projectId)
        {
            string previousDayStatus = string.Empty;
            //previous scrums
            IEnumerable<Scrum> scrumList = await _scrumRepository.FetchAsync(x => x.ProjectId == projectId);
            if (scrumList != null)
            {
                scrumList = scrumList.OrderByDescending(x => x.ScrumDate).ToList();
                //previous scrum
                Scrum previousScrum = scrumList.FirstOrDefault(y => y.ScrumDate < DateTime.UtcNow.Date);
                if (previousScrum != null)
                {
                    List<Question> questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();
                    List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == previousScrum.Id && x.EmployeeId == employeeId).Result.ToList();
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
            }
            return previousDayStatus;
        }


        /// <summary>
        /// Fetch the status of the scrum
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="accessToken"></param>
        /// <param name="project"></param>
        /// <param name="employees"></param>
        /// <param name="questions"></param>
        /// <returns>object of ScrumStatus</returns>
        private async Task<ScrumStatus> FetchScrumStatusAsync(string groupName, string accessToken, ProjectAc project, List<User> employees, List<Question> questions)
        {
            if (project == null)
                project = await _oauthCallsRepository.GetProjectDetails(groupName, accessToken);
            if (project != null && project.Id > 0)
            {
                if (project.IsActive)
                {
                    if (employees == null || employees.Count == 0)
                        employees = await _oauthCallsRepository.GetUsersByGroupName(groupName, accessToken);
                    if (employees.Count > 0)
                    {
                        if (questions == null || questions.Count() == 0)
                            questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.ToList();
                        if (questions.Count() > 0)
                        {
                            List<Scrum> scrumList = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, groupName, true) == 0).Result.ToList();
                            if (scrumList.Count() > 0)
                            {
                                Scrum scrum = scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);
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
                                return ScrumStatus.NotStarted;
                        }
                        else
                            return ScrumStatus.NoQuestion;
                    }
                    else
                        return ScrumStatus.NoEmployee;
                }
                else
                    return ScrumStatus.InActiveProject;
            }
            else
                return ScrumStatus.NoProject;
        }


        /// <summary>
        /// Halt the scrum meeting 
        /// </summary>
        /// <param name="scrum"></param>
        /// <param name="groupName"></param>
        /// <param name="accessToken"></param>
        /// <returns>scrum halted message</returns>
        private async Task<string> ScrumHaltAsync(Scrum scrum, string groupName, string accessToken)
        {
            ScrumStatus status = await FetchScrumStatusAsync(groupName, accessToken, null, null, null);
            //keyword encountered is "scrum halt"
            if (status == (ScrumStatus.OnGoing))
            {
                //scrum halted
                scrum.IsHalted = true;
                _scrumRepository.Update(scrum);
                await _scrumRepository.SaveChangesAsync();
                return _stringConstant.ScrumHalted;
            }
            else if (status == (ScrumStatus.Halted))
                return _stringConstant.ScrumAlreadyHalted;
            else
                return ReplyToClient(status) + _stringConstant.ScrumCannotBeHalted;
        }


        /// <summary>
        /// Resume the scrum meeting
        /// </summary>
        /// <param name="scrum"></param>
        /// <param name="groupName"></param>
        /// <param name="accessToken"></param>
        /// <returns>scrum resume message along with the next question</returns>
        private async Task<string> ScrumResumeAsync(Scrum scrum, string groupName, string accessToken)
        {
            ScrumStatus status = await FetchScrumStatusAsync(groupName, accessToken, null, null, null);
            var returnMsg = string.Empty;
            //keyword encountered is "scrum resume"      
            if (status == (ScrumStatus.Halted))
            {
                //scrum resumed
                scrum.IsHalted = false;
                _scrumRepository.Update(scrum);
                await _scrumRepository.SaveChangesAsync();
                //when the scrum is resumed then, the next question is to be asked
                returnMsg += _stringConstant.ScrumResumed + GetQuestionAsync(scrum.Id, groupName, null, null, scrum.ProjectId, accessToken).Result;
                return returnMsg;
            }
            else if (status == (ScrumStatus.OnGoing))
            {
                returnMsg += _stringConstant.ScrumNotHalted + GetQuestionAsync(scrum.Id, groupName, null, null, scrum.ProjectId, accessToken).Result;
                return returnMsg;
            }
            else
                return ReplyToClient(status) + _stringConstant.ScrumCannotBeResumed;
        }


        /// <summary>
        /// Select the appropriate reply to the client
        /// </summary>
        /// <param name="scrumStatus"></param>
        /// <returns>appropriate message indicating the status of scrum</returns>
        private string ReplyToClient(ScrumStatus scrumStatus)
        {
            string returnMessage = string.Empty;
            switch (scrumStatus)
            {
                case ScrumStatus.Completed:
                    returnMessage = _stringConstant.ScrumAlreadyConducted;
                    break;
                case ScrumStatus.Halted:
                    returnMessage = _stringConstant.ScrumIsHalted;
                    break;
                case ScrumStatus.NoEmployee:
                    returnMessage = _stringConstant.NoEmployeeFound;
                    break;
                case ScrumStatus.NoProject:
                    returnMessage = _stringConstant.NoProjectFound;
                    break;
                case ScrumStatus.InActiveProject:
                    returnMessage = _stringConstant.ProjectInActive;
                    break;
                case ScrumStatus.NoQuestion:
                    returnMessage = _stringConstant.NoQuestion;
                    break;
                case ScrumStatus.NotStarted:
                    returnMessage = _stringConstant.ScrumNotStarted;
                    break;
                case ScrumStatus.OnGoing:
                    returnMessage = _stringConstant.ScrumInProgress;
                    break;
                default: return null;
            }
            return returnMessage;
        }


        #endregion


    }
}