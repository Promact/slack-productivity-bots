using Promact.Core.Repository.AttachmentRepository;
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


        private readonly IRepository<TemporaryScrumDetails> _tempScrumDetailsRepository;
        private readonly IRepository<ScrumAnswer> _scrumAnswerRepository;
        private readonly IRepository<Scrum> _scrumRepository;
        private readonly IRepository<ApplicationUser> _applicationUser;
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ISlackUserRepository _slackUserDetails;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IRepository<SlackBotUserDetail> _slackBotUserDetail;


        #endregion


        #region Constructor


        public ScrumBotRepository(IRepository<ScrumAnswer> scrumAnswerRepository,
            IOauthCallsRepository oauthCallsRepository, ISlackUserRepository slackUserDetails,
            IRepository<Question> questionRepository, IAttachmentRepository attachmentRepository,
            IRepository<ApplicationUser> applicationUser, ISlackChannelRepository slackChannelRepository,
            IRepository<Scrum> scrumRepository, IStringConstantRepository stringConstant,
            IRepository<SlackBotUserDetail> slackBotUserDetail, IRepository<TemporaryScrumDetails> tempScrumDetailsRepository)
        {
            _scrumAnswerRepository = scrumAnswerRepository;
            _scrumRepository = scrumRepository;
            _questionRepository = questionRepository;
            _oauthCallsRepository = oauthCallsRepository;
            _slackChannelRepository = slackChannelRepository;
            _applicationUser = applicationUser;
            _attachmentRepository = attachmentRepository;
            _slackUserDetails = slackUserDetails;
            _slackBotUserDetail = slackBotUserDetail;
            _stringConstant = stringConstant;
            _tempScrumDetailsRepository = tempScrumDetailsRepository;
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

            if (messageArray[0] == "delete" && userId == "U06NVGLPQ")
            {
                var date = DateTime.UtcNow.Date;
                var temp = _tempScrumDetailsRepository.FirstOrDefault(x => x.ProjectId == 2);
                if (temp != null)
                {
                    _tempScrumDetailsRepository.Delete(temp.Id);
                    await _tempScrumDetailsRepository.SaveChangesAsync();
                }
                var scrum = _scrumRepository.FirstOrDefault(x => x.ProjectId == 2 && x.ScrumDate == date);

                if (scrum != null)
                {
                    _scrumRepository.Delete(scrum.Id);
                    await _scrumRepository.SaveChangesAsync();
                    return "scrum has been deleted";
                }
                else
                    return "no scrum cud be deleted";
            }
            else
            {

                if (user != null && !user.Deleted && String.Compare(message, _stringConstant.ScrumHelp, true) == 0)
                    replyText = _stringConstant.ScrumHelpMessage;
                else if (user != null && !user.Deleted && channel != null && !channel.Deleted)
                {
                    //commands could be"scrum time" or "scrum halt" or "scrum resume"
                    if (String.Compare(message, _stringConstant.ScrumTime, true) == 0 || String.Compare(message, _stringConstant.ScrumHalt, true) == 0 || String.Compare(message, _stringConstant.ScrumResume, true) == 0)
                        replyText = await ScrumAsync(channel.Name, user.Name, messageArray[1].ToLower(), user.UserId);
                    //a particular user is on leave, getting marked as later or asked question again
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
                        replyText = await AddChannelManuallyAsync(messageArray[2], channelId, user.UserId);
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
        }


        #region Temporary


        /// <summary>
        /// Store the scrum details temporarily in a list
        /// </summary>
        /// <param name="projectId"></param>
        public async Task AddTemporaryScrumDetailsAsync(int projectId, string slackUserId)
        {
            TemporaryScrumDetails tempScrumDetails = _tempScrumDetailsRepository.FirstOrDefault(x => x.ProjectId == projectId && x.CreatedOn == DateTime.UtcNow.Date);
            if (tempScrumDetails == null)
            {
                TemporaryScrumDetails temp = new TemporaryScrumDetails();
                temp.ProjectId = projectId;
                temp.SlackUserId = slackUserId;
                temp.CreatedOn = DateTime.UtcNow.Date;
                _tempScrumDetailsRepository.Insert(temp);
                await _tempScrumDetailsRepository.SaveChangesAsync();
            }
        }


        #endregion


        #endregion


        #region Private Methods


        #region Temporary

        /// <summary>
        /// Fetch the temporary scrum details of the given projectId for today
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private async Task<TemporaryScrumDetails> FetchTemporaryScrumDetailsAsync(int projectId)
        {
            var date = DateTime.UtcNow.Date;
            TemporaryScrumDetails temporaryScrumDetails = await _tempScrumDetailsRepository.FirstOrDefaultAsync(x => x.CreatedOn == date && x.ProjectId == projectId);
            return temporaryScrumDetails;
        }


        /// <summary>
        /// Remove all the temporary data of the given project id from the list of the given day. 
        /// </summary>
        /// <param name="projectId"></param>
        private async Task RemoveTemporaryScrumDetailsAsync(int projectId)
        {
            var date = DateTime.UtcNow.Date;
            TemporaryScrumDetails tempScrumDetails = await _tempScrumDetailsRepository.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.CreatedOn == date);
            if (tempScrumDetails != null)
            {
                _tempScrumDetailsRepository.Delete(tempScrumDetails.Id);
                await _tempScrumDetailsRepository.SaveChangesAsync();
            }
        }


        #endregion

        /// <summary>
        /// Used to update the scrum answer
        /// </summary>
        /// <param name="scrumAnswers"></param>
        /// <param name="message"></param>        
        private async Task UpdateAnswerAsync(ScrumAnswer scrumAnswer, string message)
        {
            scrumAnswer.CreatedOn = DateTime.UtcNow;
            scrumAnswer.AnswerDate = DateTime.UtcNow;
            scrumAnswer.Answer = message;
            scrumAnswer.ScrumAnswerStatus = ScrumAnswerStatus.Answered;
            _scrumAnswerRepository.Update(scrumAnswer);
            await _scrumAnswerRepository.SaveChangesAsync();
        }


        /// <summary>
        ///  This method is called whenever a message other than the default keywords is written in the channel. - JJ
        /// </summary>
        /// <param name="slackUserName"></param>
        /// <param name="message"></param>
        /// <param name="channelName"></param>
        /// <param name="slackUserId"></param>
        /// <returns>the next question statement</returns>
        private async Task<string> AddScrumAnswerAsync(string slackUserName, string message, string channelName, string slackUserId)
        {
            string reply = string.Empty;
            //scrum of the channel 
            List<Scrum> scrumList = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, channelName, true) == 0).Result.ToList();
            if (scrumList.Any())
            {
                //today's scrum of the channel
                Scrum scrum = scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);
                if (scrum != null && scrum.IsOngoing && !scrum.IsHalted)
                {
                    // getting user name from user's slack name
                    ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
                    // getting access token for that user
                    if (applicationUser != null)
                    {
                        // get access token of user for promact oauth server
                        string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);
                        //list of scrum questions. Type =1
                        List<Question> questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();
                        if (questions.Any())
                        {
                            //users of the given channel name fetched from the oauth server
                            List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(channelName, accessToken);
                            int questionCount = questions.Count();
                            //scrum answer of that day's scrum
                            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id).Result.ToList();
                            //status would be empty if the interacting user is same as the expected user.
                            string status = await ExpectedUserAsync(scrum.Id, questions, users, slackUserName, slackUserId, channelName, scrum.ProjectId, accessToken);
                            if (status == string.Empty)
                            {
                                User currentUser = users.FirstOrDefault(x => x.SlackUserId == slackUserId && x.IsActive);
                                if (currentUser != null)
                                {
                                    List<ScrumAnswer> nowReadyScrumsAnswers = scrumAnswers.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).OrderBy(x => x.Id).ToList();
                                    //scrum answers of users who were in active before, are now to be answered
                                    if (nowReadyScrumsAnswers.Any())
                                    {
                                        await UpdateAnswerAsync(nowReadyScrumsAnswers.FirstOrDefault(), message);
                                        reply = await GetQuestionAsync(scrum.Id, channelName, questions, users, scrum.ProjectId, accessToken);
                                    }
                                    else
                                    {
                                        #region Normal Scrum

                                        if ((users.Count() * questionCount) > scrumAnswers.Count())
                                        {
                                            Question firstQuestion = questions.FirstOrDefault();

                                            ScrumAnswer lastScrumAnswer = scrumAnswers.OrderByDescending(x => x.Id).FirstOrDefault();
                                            //scrum answers of the given user
                                            int answerListCount = scrumAnswers.Count(x => x.EmployeeId == lastScrumAnswer.EmployeeId);
                                            if (scrumAnswers.Any())
                                            {
                                                if (answerListCount < questionCount)
                                                {
                                                    //not all questions have been answered
                                                    Question prevQuestion = questions.FirstOrDefault(x => x.Id == lastScrumAnswer.QuestionId);
                                                    Question question = questions.FirstOrDefault(x => x.OrderNumber == prevQuestion.OrderNumber + 1);
                                                    await AddAnswerAsync(scrum.Id, question.Id, lastScrumAnswer.EmployeeId, message, ScrumAnswerStatus.Answered);
                                                }
                                                else
                                                {
                                                    //A particular user's first answer
                                                    //list of user ids who have not answered yet                       
                                                    List<string> idList = users.Where(x => x.IsActive && !scrumAnswers.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                                                    if (idList != null && idList.Any())
                                                    {
                                                        //now fetch the first question to the next user
                                                        User user = users.FirstOrDefault(x => x.Id == idList.FirstOrDefault());
                                                        await AddAnswerAsync(scrum.Id, firstQuestion.Id, user.Id, message, ScrumAnswerStatus.Answered);
                                                    }
                                                }
                                                //get the next question 
                                                //donot shift message                                         
                                                reply = await GetQuestionAsync(scrum.Id, channelName, questions, users, scrum.ProjectId, accessToken);
                                            }
                                            else
                                            {
                                                //First User's first answer
                                                User user = users.FirstOrDefault(x => x.IsActive);
                                                await AddAnswerAsync(scrum.Id, firstQuestion.Id, user.Id, message, ScrumAnswerStatus.Answered);
                                                //get the next question . donot shift message 
                                                reply = await GetQuestionAsync(scrum.Id, channelName, questions, users, scrum.ProjectId, accessToken);
                                            }
                                        }


                                        #endregion
                                    }
                                }
                                else
                                {
                                    //Scenario - 3 questions in all
                                    // user answers 1 question becomes inactive. Then becomes active and answers a question. Is switched back to in-active again 
                                    List<ScrumAnswer> scrumAnswerNow = scrumAnswers.Where(x => x.EmployeeId == applicationUser.Id && x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).ToList();
                                    foreach (ScrumAnswer scrumAns in scrumAnswerNow)
                                    {
                                        scrumAns.ScrumAnswerStatus = ScrumAnswerStatus.InActive;
                                        _scrumAnswerRepository.Update(scrumAns);
                                        await _scrumAnswerRepository.SaveChangesAsync();
                                    }
                                    return string.Format(_stringConstant.InActiveInOAuth, slackUserName) + await MarkAsInActiveAsync(scrumAnswers, users, scrum.Id, questions, channelName, scrum.ProjectId, accessToken, slackUserId, true);
                                }
                            }
                            //the user interacting is not the expected user
                            else
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
        /// <param name="channelName"></param>
        /// <param name="slackUserName"></param>
        /// <param name="parameter"></param>      
        /// <param name="slackUserId"></param>
        /// <returns>The question or the status of the scrum</returns>
        private async Task<string> ScrumAsync(string channelName, string slackUserName, string parameter, string slackUserId)
        {
            // getting user name from user's slack name
            ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
            // getting access token for that user
            if (applicationUser != null)
            {
                // get access token of user for promact oauth server
                string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);
                List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(channelName, accessToken);

                User user = users.FirstOrDefault(x => x.SlackUserId == slackUserId);
                if (user != null && user.IsActive)
                {
                    Scrum scrum = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, channelName, true) == 0).Result.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);
                    ScrumActions scrumStage = (ScrumActions)Enum.Parse(typeof(ScrumActions), parameter);
                    switch (scrumStage)
                    {
                        case ScrumActions.halt:
                            //keyword encountered is "scrum halt"
                            return await ScrumHaltAsync(scrum, channelName, accessToken);

                        case ScrumActions.resume:
                            //keyword encountered is "scrum resume"
                            return await ScrumResumeAsync(scrum, channelName, accessToken);

                        case ScrumActions.time:
                            //keyword encountered is "scrum time"
                            return await StartScrumAsync(channelName, accessToken);

                        default:
                            return string.Empty;
                    }
                }
                else
                {
                    //also take the temp walas into consideration and fetch the next question
                    //but now I feellike it's not necessary
                    return string.Format(_stringConstant.InActiveInOAuth, slackUserName);
                }
            }
            else
                // if user doesn't exist then this message will be shown to user
                return _stringConstant.YouAreNotInExistInOAuthServer;
        }


        /// <summary>
        /// This method will be called when the keyword "leave @username" or "later @username" or "scrum @username" is received as reply from a channel member. - JJ
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="slackUserName"></param>
        /// <param name="slackUserId"></param>
        /// <param name="applicant"></param>
        /// <param name="applicantId"></param>
        /// <returns>Question to the next person or other scrum status</returns>
        private async Task<string> LeaveAsync(string channelName, string slackUserName, string slackUserId, string applicant, string applicantId)
        {
            string returnMsg = string.Empty;
            List<Scrum> scrumList = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, channelName, true) == 0).Result.ToList();
            Scrum scrum = scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);
            if (scrumList.Any() && scrum != null)
            {
                if (scrum.IsOngoing)
                {
                    if (!scrum.IsHalted)
                    {
                        // getting user name from user's slack name
                        ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
                        // getting access token for that user
                        if (applicationUser != null)
                        {
                            // get access token of user for promact oauth server
                            string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);

                            List<Question> questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();
                            List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(channelName, accessToken);
                            ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(channelName, accessToken);

                            ScrumStatus scrumStatus = await FetchScrumStatusAsync(channelName, accessToken, project, users, questions);
                            if (scrumStatus == ScrumStatus.OnGoing)
                            {
                                User user = users.FirstOrDefault(x => x.SlackUserId == slackUserId);
                                if (user != null)
                                {
                                    if (user.IsActive)
                                    {
                                        List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id).Result.ToList();
                                        //keyword "leave @username" 
                                        returnMsg = await MarkLeaveAsync(scrumAnswer, users, scrum.Id, applicant, questions, channelName, scrum.ProjectId, accessToken, slackUserId, applicantId);
                                    }
                                    else
                                        returnMsg = string.Format(_stringConstant.InActiveInOAuth, slackUserName) + await GetQuestionAsync(scrum.Id, channelName, questions, users, project.Id, accessToken);
                                }
                                else
                                    returnMsg = "<@" + slackUserName + "> is not in the project";
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
        /// <param name="slackUserId"></param>
        /// <returns>status message</returns>
        private async Task<string> AddChannelManuallyAsync(string channelName, string channelId, string slackUserId)
        {
            string returnMsg = string.Empty;
            //Checks whether channelId starts with "G". This is done inorder to make sure that only private channels are added manually
            if (IsPrivateChannel(channelId))
            {
                // getting user name from user's slack name
                ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
                // getting access token for that user
                if (applicationUser != null)
                {
                    // get access token of user for promact oauth server
                    string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);
                    //get the project details of the given channel name
                    ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(channelName, accessToken);
                    //add channel details only if the channel has been registered as project in OAuth server
                    if (project != null && project.Id > 0)
                    {
                        if (project.IsActive)
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
                            returnMsg = _stringConstant.ProjectInActive;
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
        /// Update the scrum details temporarily stored in a list
        /// </summary>
        /// <param name="slackUserId"></param>
        /// <param name="projectId"></param>
        private async Task UpdateTemporaryScrumDetailsAsync(string slackUserId, int projectId)
        {
            var date = DateTime.UtcNow.Date;
            TemporaryScrumDetails tempScrumDetails = await _tempScrumDetailsRepository.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.CreatedOn == date);
            if (tempScrumDetails != null)
            {
                tempScrumDetails.SlackUserId = slackUserId;
                _tempScrumDetailsRepository.Update(tempScrumDetails);
                await _tempScrumDetailsRepository.SaveChangesAsync();
            }
            //else
            //    await AddTemporaryScrumDetailsAsync(projectId, slackUserId);
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
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <param name="scrumAnswerStatus"></param>
        /// <returns>true if scrum answer is added successfully</returns>
        private async Task<bool> AddAnswerAsync(int scrumId, int questionId, string userId, string message, ScrumAnswerStatus scrumAnswerStatus)
        {
            ScrumAnswer answer = new ScrumAnswer();
            answer.Answer = message;
            answer.AnswerDate = DateTime.UtcNow;
            answer.CreatedOn = DateTime.UtcNow;
            answer.EmployeeId = userId;
            answer.QuestionId = questionId;
            answer.ScrumId = scrumId;
            answer.ScrumAnswerStatus = scrumAnswerStatus;
            _scrumAnswerRepository.Insert(answer);
            await _scrumAnswerRepository.SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// This method will be called when the keyword "scrum time" is encountered
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="accessToken"></param>
        /// <returns>The next question or the scrum complete message</returns>
        private async Task<string> StartScrumAsync(string channelName, string accessToken)
        {
            string replyMessage = string.Empty;
            ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(channelName, accessToken);
            List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(channelName, accessToken);
            List<Question> questionList = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();

            ScrumStatus scrumStatus = await FetchScrumStatusAsync(channelName, accessToken, project, users, questionList);
            if (scrumStatus == ScrumStatus.NotStarted)
            {
                Question question = questionList.FirstOrDefault();
                User firstUser = users.FirstOrDefault(x => x.IsActive);
                if (firstUser != null)
                {
                    Scrum scrum = new Scrum();
                    scrum.CreatedOn = DateTime.UtcNow;
                    scrum.GroupName = channelName;
                    scrum.ScrumDate = DateTime.UtcNow.Date;
                    scrum.ProjectId = project.Id;
                    scrum.TeamLeaderId = project.TeamLeaderId;
                    scrum.IsHalted = false;
                    scrum.IsOngoing = true;
                    _scrumRepository.Insert(scrum);
                    await _scrumRepository.SaveChangesAsync();
                    SlackUserDetails slackUser = await _slackUserDetails.GetByIdAsync(firstUser.SlackUserId);
                    await AddTemporaryScrumDetailsAsync(project.Id, slackUser.UserId);
                    //first user is asked questions along with the previous day status (if any)
                    replyMessage = _stringConstant.GoodDay + "<@" + slackUser.Name + ">!\n" + await FetchPreviousDayStatusAsync(firstUser.Id, project.Id) + question.QuestionStatement;
                }
                else
                    //no active users are found
                    replyMessage = _stringConstant.NoEmployeeFound;
            }
            else if (scrumStatus == ScrumStatus.OnGoing)
            {
                //user to whom the last question was asked
                SlackUserDetails slackUser = await GetSlackUserAsync(project.Id, users);
                if (slackUser != null && !string.IsNullOrEmpty(slackUser.Name))
                    replyMessage = string.Format(_stringConstant.InActiveInOAuth, slackUser.Name);

                //if scrum meeting was interrupted. "scrum time" is written to resume scrum meeting. So next question is fetched.
                List<Scrum> scrumList = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, channelName, true) == 0).Result.ToList();
                replyMessage += await GetQuestionAsync(scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date).Id, channelName, null, null, project.Id, accessToken);
            }
            else
                return ReplyToClient(scrumStatus);
            return replyMessage;
        }


        /// <summary>
        /// This method is used when an user is on leave or can asnwer only later
        /// </summary>
        /// <param name="scrumAnswer"></param>
        /// <param name="users"></param>
        /// <param name="scrumId"></param>
        /// <param name="applicant"></param>
        /// <param name="questions"></param>
        /// <param name="channelName"></param>
        /// <param name="projectId"></param>
        /// <param name="accessToken"></param>
        /// <param name="userId"></param>
        /// <param name="applicantId"></param>
        /// <returns>Question to the next user or status of the request</returns>
        private async Task<string> MarkLeaveAsync(List<ScrumAnswer> scrumAnswer, List<User> users, int scrumId, string applicant, List<Question> questions, string channelName, int projectId, string accessToken, string slackUserId, string applicantId)
        {
            string returnMsg = string.Empty;
            User user = users.FirstOrDefault(x => x.SlackUserId == applicantId);
            if (user != null)
            {
                if (user.IsActive)
                {
                    //checks whether the applicant is the expected user
                    string status = await ExpectedUserAsync(scrumId, questions, users, applicant, applicantId, channelName, projectId, accessToken);
                    //if the interacting user is the expected user
                    if (status == string.Empty)
                    {
                        string expectedUserId = users.FirstOrDefault(x => x.SlackUserId == applicantId).Id;
                        //if applying trying to self as on leave
                        if (String.Compare(slackUserId, applicantId, true) == 0)
                            return _stringConstant.LeaveError;
                        else
                        {
                            if (scrumAnswer.Any())
                                //fetch the scrum answer of the user given on that day
                                scrumAnswer = scrumAnswer.Where(x => x.EmployeeId == expectedUserId).ToList();
                            //If no answer from the user has been obtained yet.
                            if (!scrumAnswer.Any())
                            {
                                //all the scrum questions are answered as "leave"
                                foreach (Question question in questions)
                                {
                                    await AddAnswerAsync(scrumId, question.Id, expectedUserId, _stringConstant.Leave, ScrumAnswerStatus.Leave);
                                }
                            }
                            else
                                //If the applicant has already answered questions
                                returnMsg = string.Format(_stringConstant.AlreadyAnswered, applicant);
                        }
                    }
                    else
                        return status;
                }
                else
                    returnMsg = string.Format(_stringConstant.InActiveInOAuth, applicant);
            }
            else
                returnMsg = string.Format(_stringConstant.UserNotInProject, applicant);
            //fetches the next question or status and returns
            return returnMsg + await GetQuestionAsync(scrumId, channelName, questions, users, projectId, accessToken);
        }


        /// <summary>
        /// Used to fetch the next question based on the given parameters
        /// </summary>
        /// <param name="scrumId"></param>
        /// <param name="channelName"></param>
        /// <param name="users"></param>
        /// <param name="questions"></param>
        ///<param name="projectId"></param>
        ///<param name="accessToken"></param>
        /// <returns>The next question or the scrum complete message</returns>
        private async Task<string> GetQuestionAsync(int scrumId, string channelName, List<Question> questions, List<User> users, int projectId, string accessToken)
        {
            string returnMsg = string.Empty;
            List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();
            if (questions == null)
                questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();
            if (users == null)
                users = await _oauthCallsRepository.GetUsersByChannelNameAsync(channelName, accessToken);
            if (scrumAnswer.Any())
            {
                //scrum answers which were marked as later, are now to be answered
                List<ScrumAnswer> answerNow = scrumAnswer.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).OrderBy(x => x.Id).ToList();
                if (answerNow.Any())
                {
                    ScrumAnswer answer = answerNow.FirstOrDefault();
                    SlackUserDetails user = await _slackUserDetails.GetByIdAsync(users.FirstOrDefault(x => x.Id == answer.EmployeeId).SlackUserId);
                    await UpdateTemporaryScrumDetailsAsync(user.UserId, projectId);
                    //the first question which is to be answered now is asked
                    return "<@" + user.Name + "> " + questions.FirstOrDefault(x => x.Id == answer.QuestionId).QuestionStatement;
                }
                else
                {
                    #region Normal Get Question

                    int questionCount = questions.Count();
                    //last acrum answer of the given scrum id.
                    ScrumAnswer lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                    //no. of answers given by the user who gave the last scrum answer.
                    int answerListCount = scrumAnswer.FindAll(x => x.EmployeeId == lastScrumAnswer.EmployeeId).Count();
                    if (answerListCount == questionCount)
                    {
                        //all questions have been asked to the previous user                        
                        //list of user ids who have not answer yet                       
                        List<string> idList = users.Where(x => x.IsActive && !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();
                        if (idList != null && idList.Any())
                        {
                            //now fetch the first question to the next user
                            User user = users.FirstOrDefault(x => x.Id == idList.FirstOrDefault());
                            await UpdateTemporaryScrumDetailsAsync(user.SlackUserId, projectId);
                            SlackUserDetails slackUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                            returnMsg = _stringConstant.GoodDay + "<@" + slackUser.Name + ">!\n" + await FetchPreviousDayStatusAsync(user.Id, projectId) + await FetchQuestionAsync(null, true);
                        }
                        else
                            return await MarkScrumCompleteAsync(lastScrumAnswer.ScrumId, accessToken, projectId);
                    }
                    else
                    {
                        //as not all questions have been answered by the last user,the next question to that user will be asked
                        User user = users.FirstOrDefault(x => x.Id == lastScrumAnswer.EmployeeId);
                        if (user != null)
                        {
                            if (user.IsActive)
                            {
                                SlackUserDetails slackUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                                await UpdateTemporaryScrumDetailsAsync(slackUser.UserId, projectId);
                                returnMsg = "<@" + slackUser.Name + "> " + await FetchQuestionAsync(lastScrumAnswer.QuestionId, false);
                            }
                            else
                                return await MarkAsInActiveAsync(scrumAnswer, users, scrumId, questions, channelName, projectId, accessToken, user.SlackUserId, true);
                        }
                    }

                    #endregion
                }
            }
            else
            {
                //no scrum answer has been recorded yet. So first question to the first user
                User firstUser = users.FirstOrDefault(x => x.IsActive);
                SlackUserDetails slackUser = await _slackUserDetails.GetByIdAsync(firstUser.SlackUserId);
                await UpdateTemporaryScrumDetailsAsync(slackUser.UserId, projectId);
                returnMsg = _stringConstant.GoodDay + "<@" + slackUser.Name + ">!\n" + FetchPreviousDayStatusAsync(firstUser.Id, projectId).Result + questions.FirstOrDefault().QuestionStatement;
            }
            return returnMsg;
        }


        /// <summary>
        /// Mark a user as inactive during the scrum
        /// </summary>
        /// <param name="scrumAnswer"></param>
        /// <param name="users"></param>
        /// <param name="scrumId"></param>
        /// <param name="questions"></param>
        /// <param name="channelName"></param>
        /// <param name="projectId"></param>
        /// <param name="accessToken"></param>
        /// <param name="applicantId"></param>
        /// <returns>next question</returns>
        private async Task<string> MarkAsInActiveAsync(List<ScrumAnswer> scrumAnswer, List<User> users, int scrumId, List<Question> questions, string channelName, int projectId, string accessToken, string applicantId, bool getQuestion)
        {
            string returnMsg = string.Empty;
            User user = users.FirstOrDefault(x => x.SlackUserId == applicantId);
            //scrum answer of the user
            scrumAnswer = scrumAnswer.Where(x => x.EmployeeId == user.Id).ToList();
            //id of questions which were not answered by the user
            List<int> questionIds = questions.Where(x => x.Type == 1 && !scrumAnswer.Select(y => y.QuestionId).ToList().Contains(x.Id)).OrderBy(i => i.OrderNumber).Select(z => z.Id).ToList();
            foreach (int questionId in questionIds)
            {
                //mark all the remaining answers of the user as inactive
                await AddAnswerAsync(scrumId, questionId, user.Id, _stringConstant.InActive, ScrumAnswerStatus.InActive);
            }

            if (getQuestion)
                //fetches the next question or status and returns
                return returnMsg + Environment.NewLine + await GetQuestionAsync(scrumId, channelName, questions, users, projectId, accessToken);
            return returnMsg;
        }


        /// <summary>
        /// Used to mark scrum as completed
        /// </summary>
        /// <param name="scrumId"></param>
        /// <param name="accessToken"></param>
        /// <param name="projectId"></param>
        /// <returns>If scrum is completed then message saying that the scrum is complete 
        /// or if any active emplpoyee is pending to answer then that question</returns>
        private async Task<string> MarkScrumCompleteAsync(int scrumId, string accessToken, int projectId)
        {
            //list of scrum answers of users who were in active during scrum meeting
            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.Fetch(x => x.ScrumId == scrumId && x.ScrumAnswerStatus == ScrumAnswerStatus.InActive).OrderBy(x => x.Id).ToList();
            User user = new User();
            string nextQuestion = string.Empty;
            if (scrumAnswers.Any())
            {
                //Id of users who were in active during scrum meeting
                List<string> userIds = scrumAnswers.Select(x => x.EmployeeId).Distinct().ToList();
                foreach (string userId in userIds)
                {
                    user = await _oauthCallsRepository.GetUserByEmployeeIdAsync(userId, accessToken);
                    //check whether those in active before are active now
                    if (user != null && user.IsActive)
                    {
                        scrumAnswers = scrumAnswers.Where(x => x.EmployeeId == userId && x.ScrumAnswerStatus == ScrumAnswerStatus.InActive).ToList();
                        foreach (ScrumAnswer scrumAns in scrumAnswers)
                        {
                            // the next question is fetched if it has already not been fetched
                            if (string.IsNullOrEmpty(nextQuestion))
                            {
                                nextQuestion = _questionRepository.FirstOrDefaultAsync(x => x.Id == scrumAns.QuestionId).Result.QuestionStatement;
                            }
                            //all answers are marked to be answered now
                            scrumAns.ScrumAnswerStatus = ScrumAnswerStatus.AnswerNow;
                            _scrumAnswerRepository.Update(scrumAns);
                            await _scrumAnswerRepository.SaveChangesAsync();
                        }
                        break;
                    }
                }
                //if the nextQuestion is fetched then it means that there are questions to be asked to user
                if (!string.IsNullOrEmpty(nextQuestion))
                {
                    await UpdateTemporaryScrumDetailsAsync(user.SlackUserId, projectId);
                    return string.Format(_stringConstant.MarkedInActive, _slackUserDetails.GetByIdAsync(user.SlackUserId).Result.Name) + nextQuestion;
                }
            }
            //if no questions are pending then scrum is marked to be complete
            if (await UpdateScrumAsync(scrumId, projectId, false) == 0)
                //answers of all the users has been recorded            
                return _stringConstant.ScrumComplete;
            else
                return _stringConstant.ErrorMsg;
        }


        /// <summary>
        /// Update scrum status to not in progress scrum
        /// </summary>
        /// <param name="scrumId"></param>
        /// <returns>0 if no error</returns>
        private async Task<int> UpdateScrumAsync(int scrumId, int projectId, bool isOnGoing)
        {
            if (!isOnGoing)
                await RemoveTemporaryScrumDetailsAsync(projectId);
            Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => x.Id == scrumId);
            scrum.IsOngoing = isOnGoing;
            _scrumRepository.Update(scrum);
            await _scrumRepository.SaveChangesAsync();
            return 0;
        }


        /// <summary>
        /// Used to check whether the applicant is the expected user.
        /// </summary>
        /// <param name="scrumId"></param>
        /// <param name="questions"></param>
        /// <param name="users"></param>
        /// <param name="applicant"></param>
        /// <param name="applicantId"></param>
        /// <param name="channelName"></param>
        /// <param name="projectId"></param>
        /// <param name="accessToken"></param>
        /// <returns>empty string if the expected user is same as the applicant</returns>
        private async Task<string> ExpectedUserAsync(int scrumId, List<Question> questions, List<User> users, string applicant, string applicantId, string channelName, int projectId, string accessToken)
        {
            //List of scrum answer of the given scrumId.
            List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();
            User user = new User();
            if (scrumAnswer.Any())
            {
                int questionCount = questions.Count();
                //last acrum answer of the given scrum id.
                ScrumAnswer lastScrumAnswer = scrumAnswer.OrderByDescending(x => x.Id).FirstOrDefault();
                //no. of answers given by the user who gave the last scrum answer.
                int answerListCount = scrumAnswer.Count(x => x.EmployeeId == lastScrumAnswer.EmployeeId);
                if (answerListCount == questionCount)
                {
                    //scrum answers of users who were marked as in active during their scrum but now are available for scrum.
                    List<ScrumAnswer> scrumAnswers = scrumAnswer.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).ToList();

                    if (scrumAnswers.Any())
                    {
                        user = users.FirstOrDefault(x => x.Id == scrumAnswers.FirstOrDefault().EmployeeId);
                    }
                    else
                    {
                        //all questions have been asked to the previous user 
                        //list of user ids who have not answer yet                       
                        List<string> idList = users.Where(x => x.IsActive && !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).Select(x => x.Id).ToList();

                        if (idList != null && idList.Any())
                        {
                            //now the next user
                            TemporaryScrumDetails tempScrumDetails = await FetchTemporaryScrumDetailsAsync(projectId);
                            user = users.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId);
                            if (user == null)
                                user = users.FirstOrDefault(x => x.Id == idList.FirstOrDefault());
                        }
                        else
                        {
                            string reply = string.Empty;
                            SlackUserDetails expectedUser = await GetSlackUserAsync(projectId, users);
                            if (expectedUser != null && !string.IsNullOrEmpty(expectedUser.Name))
                                reply = string.Format(_stringConstant.InActiveInOAuth, expectedUser.Name) + await MarkAsInActiveAsync(scrumAnswer, users, scrumId, questions, channelName, projectId, accessToken, expectedUser.UserId, true);

                            else
                            {
                                if (await UpdateScrumAsync(scrumId, projectId, false) == 0)
                                    reply = _stringConstant.ScrumComplete;
                                else
                                    reply = _stringConstant.ErrorMsg;
                            }
                            return reply;
                        }
                    }
                }
                else
                {
                    //as not all questions have been answered by the last user,so to that user itself
                    user = users.FirstOrDefault(x => x.Id == lastScrumAnswer.EmployeeId);
                }
            }
            else
                //no scrum answer has been recorded yet. So first user
                user = users.FirstOrDefault(x => x.IsActive);

            return await ProcessExpectedUserResultAsync(user, applicantId, users, projectId, applicant, scrumId, channelName, accessToken, questions);
        }


        /// <summary>
        /// Gets the appropraite reply to the next user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="users"></param>
        /// <param name="projectId"></param>
        /// <param name="scrumId"></param>
        /// <param name="applicantId"></param>
        /// <param name="applicant"></param>
        /// <param name="questions"></param>
        /// <param name="channelName"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task<string> GetReplyToUser(User user, List<User> users, int projectId, int scrumId, string applicantId, string applicant, List<Question> questions, string channelName, string accessToken)
        {
            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();
            if (user != null && !user.IsActive)
            {
                SlackUserDetails expectedUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                return string.Format(_stringConstant.InActiveInOAuth, expectedUser.Name) + await MarkAsInActiveAsync(scrumAnswers, users, scrumId, questions, channelName, projectId, accessToken, expectedUser.UserId, true);
            }
            else
            {
                string reply = string.Empty;
                User unexpectedUser = users.FirstOrDefault(x => x.SlackUserId == applicantId);
                //the user to whom the last question was asked. This user must be called before MarkAsInActiveAsync() is called because if scrum is complete then temporary data is deleted and this user cannot be fetched.
                SlackUserDetails prevUser = await GetSlackUserAsync(projectId, users);
                if (unexpectedUser != null && !unexpectedUser.IsActive)
                    reply += string.Format(_stringConstant.InActiveInOAuth, applicant) + await MarkAsInActiveAsync(scrumAnswers, users, scrumId, questions, channelName, projectId, accessToken, unexpectedUser.SlackUserId, true);

                if (!string.IsNullOrEmpty(prevUser.Name) && prevUser.UserId != applicantId)
                {
                    if (string.IsNullOrEmpty(reply))
                        reply = string.Format(_stringConstant.InActiveInOAuth, prevUser.Name) + await MarkAsInActiveAsync(scrumAnswers, users, scrumId, questions, channelName, projectId, accessToken, prevUser.UserId, true) + reply;
                    else
                        reply = string.Format(_stringConstant.InActiveInOAuth, prevUser.Name) + await MarkAsInActiveAsync(scrumAnswers, users, scrumId, questions, channelName, projectId, accessToken, prevUser.UserId, false) + reply;
                }
                if (user != null)
                {
                    SlackUserDetails expectedUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                    reply += string.Format(_stringConstant.PleaseAnswer, expectedUser.Name);
                }
                return reply;
            }
        }


        /// <summary>
        /// Get the slack user who was last asked question to
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="users"></param>
        /// <returns>object of SlackUserDetails</returns>
        private async Task<SlackUserDetails> GetSlackUserAsync(int projectId, List<User> users)
        {
            TemporaryScrumDetails tempScrumDetails = await FetchTemporaryScrumDetailsAsync(projectId);
            SlackUserDetails slackUserDetails = new SlackUserDetails();
            if (tempScrumDetails != null)
            {
                User user = users.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId);

                if (!user.IsActive)
                    slackUserDetails = await _slackUserDetails.GetByIdAsync(tempScrumDetails.SlackUserId);
            }

            return slackUserDetails;
        }


        /// <summary>
        /// Check whether the given user can answer now.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="applicantId"></param>
        /// <param name="users"></param>
        /// <param name="projectId"></param>
        /// <param name="applicant"></param>
        /// <param name="scrumId"></param>
        /// <param name="channelName"></param>
        /// <param name="accessToken"></param>
        /// <param name="questions"></param>
        /// <returns>status</returns>
        private async Task<string> ProcessExpectedUserResultAsync(User user, string applicantId, List<User> users, int projectId, string applicant, int scrumId, string channelName, string accessToken, List<Question> questions)
        {
            if (user != null && user.IsActive && user.SlackUserId == applicantId)
            {
                TemporaryScrumDetails tempScrumDetails = await FetchTemporaryScrumDetailsAsync(projectId);
                if (tempScrumDetails.SlackUserId != applicantId)
                {
                    //last question was asked to this user.
                    SlackUserDetails tempUser = await _slackUserDetails.GetByIdAsync(tempScrumDetails.SlackUserId);
                    User userDetail = users.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId);
                    tempScrumDetails.SlackUserId = applicantId;

                    if (userDetail != null && !userDetail.IsActive)
                    {
                        List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId && x.EmployeeId == userDetail.Id).Result.ToList();
                        return string.Format(_stringConstant.InActiveInOAuth, tempUser.Name) + await MarkAsInActiveAsync(scrumAnswer, users, scrumId, questions, channelName, projectId, accessToken, tempUser.UserId, true);
                    }
                    else
                        return string.Format(_stringConstant.NotExpected, tempUser.Name) + await GetQuestionAsync(scrumId, channelName, questions, users, projectId, accessToken);
                }
                return string.Empty;
            }
            else
                return await GetReplyToUser(user, users, projectId, scrumId, applicantId, applicant, questions, channelName, accessToken);
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
                if (questionList.Any())
                {
                    question = questionList.OrderBy(x => x.OrderNumber).FirstOrDefault();
                    return question.QuestionStatement;
                }
                else
                    return _stringConstant.NoQuestion;
            }
            else
            {
                question = await _questionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
                if (question != null)
                {
                    //order number of the given question 
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
        /// Fetches the previous day's questions and answers of the user of the given id for the given project
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns>previous day status</returns>
        private async Task<string> FetchPreviousDayStatusAsync(string userId, int projectId)
        {
            string previousDayStatus = string.Empty;
            //scrums of this channel(project)
            IEnumerable<Scrum> scrumList = await _scrumRepository.FetchAsync(x => x.ProjectId == projectId);
            if (scrumList != null)
            {
                //previous scrums
                scrumList = scrumList.Where(x => x.ScrumDate < DateTime.UtcNow.Date).OrderByDescending(x => x.ScrumDate).ToList();
                List<Question> questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.OrderBy(x => x.OrderNumber).ToList();
                foreach (Scrum scrum in scrumList)
                {
                    //answers in which user was not on leave.
                    List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id && x.EmployeeId == userId && x.ScrumAnswerStatus != ScrumAnswerStatus.Leave).Result.ToList();
                    if (scrumAnswers.Any() && questions.Any())
                    {
                        previousDayStatus = Environment.NewLine + string.Format(_stringConstant.PreviousDayStatus, scrum.ScrumDate.ToShortDateString()) + Environment.NewLine;
                        foreach (Question question in questions)
                        {
                            //Question and the corresponding answer appended
                            previousDayStatus += "*_Q_*: " + question.QuestionStatement + Environment.NewLine + "*_A_*: _" + scrumAnswers.FirstOrDefault(x => x.QuestionId == question.Id).Answer + "_" + Environment.NewLine;
                        }
                        previousDayStatus += Environment.NewLine + _stringConstant.AnswerToday + Environment.NewLine + Environment.NewLine;
                    }
                    if (!string.IsNullOrEmpty(previousDayStatus))
                        return previousDayStatus;
                }
            }
            return previousDayStatus;
        }


        /// <summary>
        /// Fetch the status of the scrum
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="accessToken"></param>
        /// <param name="project"></param>
        /// <param name="users"></param>
        /// <param name="questions"></param>
        /// <returns>object of ScrumStatus</returns>
        private async Task<ScrumStatus> FetchScrumStatusAsync(string channelName, string accessToken, ProjectAc project, List<User> users, List<Question> questions)
        {
            if (project == null)
                project = await _oauthCallsRepository.GetProjectDetailsAsync(channelName, accessToken);
            if (project != null && project.Id > 0)
            {
                if (project.IsActive)
                {
                    if (users == null || !users.Any())
                        users = await _oauthCallsRepository.GetUsersByChannelNameAsync(channelName, accessToken);
                    if (users.Any())
                    {
                        if (questions == null || !questions.Any())
                            questions = _questionRepository.FetchAsync(x => x.Type == 1).Result.ToList();
                        if (questions.Any())
                        {
                            List<Scrum> scrumList = _scrumRepository.FetchAsync(x => String.Compare(x.GroupName, channelName, true) == 0).Result.ToList();
                            if (scrumList.Any())
                            {
                                Scrum scrum = scrumList.FirstOrDefault(x => x.ScrumDate.Date == DateTime.UtcNow.Date);
                                if (scrum != null)
                                {
                                    if (!scrum.IsHalted)
                                    {
                                        if (scrum.IsOngoing)
                                            return ScrumStatus.OnGoing;
                                        else
                                            return ScrumStatus.Completed;
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
        /// <param name="channelName"></param>
        /// <param name="accessToken"></param>
        /// <returns>scrum halted message</returns>
        private async Task<string> ScrumHaltAsync(Scrum scrum, string channelName, string accessToken)
        {
            ScrumStatus status = await FetchScrumStatusAsync(channelName, accessToken, null, null, null);
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
        /// <param name="channelName"></param>
        /// <param name="accessToken"></param>
        /// <returns>scrum resume message along with the next question</returns>
        private async Task<string> ScrumResumeAsync(Scrum scrum, string channelName, string accessToken)
        {
            ScrumStatus status = await FetchScrumStatusAsync(channelName, accessToken, null, null, null);
            string returnMsg = string.Empty;
            //keyword encountered is "scrum resume"      
            if (status == (ScrumStatus.Halted))
            {
                //scrum resumed
                scrum.IsHalted = false;
                _scrumRepository.Update(scrum);
                await _scrumRepository.SaveChangesAsync();
                //when the scrum is resumed then, the next question is to be asked
                returnMsg += _stringConstant.ScrumResumed + GetQuestionAsync(scrum.Id, channelName, null, null, scrum.ProjectId, accessToken).Result;
                return returnMsg;
            }
            else if (status == (ScrumStatus.OnGoing))
            {
                //scrum is not halted but is in progress 
                returnMsg += _stringConstant.ScrumNotHalted + GetQuestionAsync(scrum.Id, channelName, null, null, scrum.ProjectId, accessToken).Result;
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