using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;

namespace Promact.Core.Repository.ScrumRepository
{
    public class ScrumBotRepository : IScrumBotRepository
    {

        #region Private Variable


        private readonly IRepository<TemporaryScrumDetails> _tempScrumDetailsRepository;
        private readonly IRepository<ScrumAnswer> _scrumAnswerRepository;
        private readonly IRepository<Scrum> _scrumRepository;
        private readonly IRepository<ApplicationUser> _applicationUser;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<SlackBotUserDetail> _slackBotUserDetailRepository;
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ISlackUserRepository _slackUserDetailRepository;
        private readonly IStringConstantRepository _stringConstant;


        #endregion


        #region Constructor


        public ScrumBotRepository(IRepository<TemporaryScrumDetails> tempScrumDetailsRepository,
            IRepository<ScrumAnswer> scrumAnswerRepository, IRepository<Scrum> scrumRepository,
            IRepository<ApplicationUser> applicationUser, IRepository<Question> questionRepository,
            IRepository<SlackBotUserDetail> slackBotUserDetailRepository, IRepository<SlackUserDetails> slackUserDetails,
            ISlackChannelRepository slackChannelRepository,
            IOauthCallsRepository oauthCallsRepository, IAttachmentRepository attachmentRepository,
            ISlackUserRepository slackUserDetailRepository, IStringConstantRepository stringConstant)
        {
            _tempScrumDetailsRepository = tempScrumDetailsRepository;
            _scrumAnswerRepository = scrumAnswerRepository;
            _scrumRepository = scrumRepository;
            _applicationUser = applicationUser;
            _questionRepository = questionRepository;
            _slackBotUserDetailRepository = slackBotUserDetailRepository;
            _slackUserDetailRepository = slackUserDetailRepository;
            _slackChannelRepository = slackChannelRepository;
            _oauthCallsRepository = oauthCallsRepository;
            _attachmentRepository = attachmentRepository;
            _slackUserDetails = slackUserDetails;
            _stringConstant = stringConstant;
        }


        #endregion


        #region Public Method 


        /// <summary>
        /// This will process the messages from slack and use appropriate methods to give a suitable response through Bot - JJ
        /// </summary>
        /// <param name="slackUserId">UserId of slack user</param>
        /// <param name="slackChannelId">Slack Channel Id</param>
        /// <param name="message">message from slack</param>
        /// <returns>reply message</returns>      
        public async Task<string> ProcessMessagesAsync(string slackUserId, string slackChannelId, string message)
        {
            string replyText = string.Empty;
            SlackUserDetailAc slackUserDetail = await _slackUserDetailRepository.GetByIdAsync(slackUserId);
            SlackChannelDetails slackChannelDetail = await _slackChannelRepository.GetByIdAsync(slackChannelId);
            //the command is split to individual words
            //commnads ex: "scrum time", "leave @userId"
            string[] messageArray = message.Split(null);

            #region Added temporarily for testing purpose

            if (messageArray[0] == "delete")
            {
                var date = DateTime.UtcNow.Date;
                ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
                // getting access token for that user
                if (applicationUser != null)
                {
                    // get access token of user for promact oauth server
                    string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);

                    ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(slackChannelDetail.Name, accessToken);
                    if (project != null && project.Id > 0)
                    {
                        Scrum scrum = _scrumRepository.FirstOrDefault(x => x.ProjectId == project.Id && x.ScrumDate == date);
                        if (scrum != null)
                        {
                            _scrumRepository.Delete(scrum.Id);
                            int scrumDelete = await _scrumRepository.SaveChangesAsync();
                            if (scrumDelete == 1)
                                replyText += "scrum has been deleted\n";
                            else
                                replyText += "scrum has not been deleted\n";
                            TemporaryScrumDetails temp = _tempScrumDetailsRepository.FirstOrDefault(x => x.ScrumId == scrum.Id);
                            if (temp != null)
                            {
                                _tempScrumDetailsRepository.Delete(temp.Id);
                                int deleteTemp = await _tempScrumDetailsRepository.SaveChangesAsync();
                                if (deleteTemp == 1)
                                    replyText += "temp data has been deleted\n";
                                else
                                    replyText += "temp data has not been deleted\n";
                            }
                        }
                        else
                            replyText += "no scrum cud be deleted\n";
                    }
                    else
                        replyText = "Project not found in OAuth\n";
                }
                else
                    replyText = "Please login with OAuth\n";
                return replyText;
            }

            #endregion

            if (slackUserDetail != null)
            {
                if (String.Compare(message, _stringConstant.ScrumHelp, StringComparison.OrdinalIgnoreCase) == 0) //when the message obtained is "scrum help"
                {
                    replyText = _stringConstant.ScrumHelpMessage;
                }
                else if (slackChannelDetail != null)
                {
                    //commands could be"scrum time" or "scrum halt" or "scrum resume"
                    if (String.Compare(message, _stringConstant.ScrumTime, StringComparison.OrdinalIgnoreCase) == 0 || 
                        String.Compare(message, _stringConstant.ScrumHalt, StringComparison.OrdinalIgnoreCase) == 0 || 
                        String.Compare(message, _stringConstant.ScrumResume, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        replyText = await ScrumAsync(slackChannelId, slackChannelDetail.Name, slackUserDetail.Name, messageArray[1].ToLower(), slackUserDetail.UserId);
                    }
                    //a particular user is on leave. command would be like "leave <@userId>"
                    else if ((String.Compare(messageArray[0], _stringConstant.Leave, StringComparison.OrdinalIgnoreCase) == 0) && messageArray.Length == 2)
                    {
                        //"<@".Length is 2
                        int fromIndex = message.IndexOf("<@", StringComparison.Ordinal) + 2;
                        int toIndex = message.LastIndexOf(">", StringComparison.Ordinal);
                        if (toIndex > 0 && fromIndex > 1)
                        {
                            //the slack userId is fetched
                            string applicantId = message.Substring(fromIndex, toIndex - fromIndex);
                            //fetch the user of the given userId
                            SlackUserDetailAc applicantDetails = await _slackUserDetailRepository.GetByIdAsync(applicantId);
                            replyText = applicantDetails != null ? await LeaveAsync(slackChannelId, slackChannelDetail.Name, slackUserDetail.Name, slackUserDetail.UserId, applicantDetails.Name, applicantId) : _stringConstant.NotAUser;
                        }
                        else //when command would be like "leave <@>"
                        {
                            replyText = await AddScrumAnswerAsync(slackUserDetail.Name, message, slackChannelId, slackChannelDetail.Name, slackUserDetail.UserId);
                        }
                    }
                    else  //all other texts
                    {
                        replyText = await AddScrumAnswerAsync(slackUserDetail.Name, message, slackChannelId, slackChannelDetail.Name, slackUserDetail.UserId);
                    }
                }
                else   //channel is not registered in the database
                {
                    //If channel is not registered in the database and the command encountered is "add channel channelname"
                    if (String.Compare(messageArray[0], _stringConstant.Add, StringComparison.OrdinalIgnoreCase) == 0 && 
                        String.Compare(messageArray[1], _stringConstant.Channel, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        replyText = await AddChannelManuallyAsync(messageArray[2], slackChannelId, slackUserDetail.UserId);
                    }
                    //If any of the commands which scrum bot recognizes is encountered
                    else if (((String.Compare(messageArray[0], _stringConstant.Leave, StringComparison.OrdinalIgnoreCase) == 0) && 
                              messageArray.Length == 2) || 
                             String.Compare(message, _stringConstant.ScrumTime, StringComparison.OrdinalIgnoreCase) == 0 || 
                             String.Compare(message, _stringConstant.ScrumHalt, StringComparison.OrdinalIgnoreCase) == 0 || 
                             String.Compare(message, _stringConstant.ScrumResume, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        replyText = _stringConstant.ChannelAddInstruction;
                    }
                }
            }
            else //user == null
            {
                SlackBotUserDetail botUserDetail = await _slackBotUserDetailRepository.FirstOrDefaultAsync(x => x.UserId == slackUserId);
                if (botUserDetail == null)
                    replyText = _stringConstant.SlackUserNotFound;
            }
            return replyText;
        }


        #region Temporary Scrum Details


        /// <summary>
        /// Store the scrum details temporarily in a database - JJ
        /// </summary>
        /// <param name="scrumId">Id of scrum of the channel for the day</param>
        /// <param name="slackUserId">UserId of slack user</param>
        /// <param name="answerCount">Number of answers of the user</param>
        /// <param name="questionId">The Id of the last question asked to the user</param>
        /// <returns></returns>
        public async Task AddTemporaryScrumDetailsAsync(int scrumId, string slackUserId, int answerCount, int questionId)
        {
            TemporaryScrumDetails tempScrumDetails = await _tempScrumDetailsRepository.FirstOrDefaultAsync(x => x.ScrumId == scrumId && DbFunctions.TruncateTime(x.CreatedOn) == DateTime.UtcNow.Date);
            if (tempScrumDetails == null)
            {
                TemporaryScrumDetails temporaryScrumDetails = new TemporaryScrumDetails();
                temporaryScrumDetails.ScrumId = scrumId;
                temporaryScrumDetails.SlackUserId = slackUserId;
                temporaryScrumDetails.AnswerCount = answerCount;
                temporaryScrumDetails.QuestionId = questionId;
                temporaryScrumDetails.CreatedOn = DateTime.UtcNow.Date;
                _tempScrumDetailsRepository.Insert(temporaryScrumDetails);
                await _tempScrumDetailsRepository.SaveChangesAsync();
            }
        }


        #endregion


        #endregion


        #region Private Methods


        #region Temporary Scrum Details


        /// <summary>
        /// Fetch the temporary scrum details of the given projectId for today. - JJ
        /// </summary>
        /// <param name="scrumId">Id of scrum of the channel for the day</param>
        /// <returns>object of TemporaryScrumDetails</returns>
        private async Task<TemporaryScrumDetails> FetchTemporaryScrumDetailsAsync(int scrumId)
        {
            DateTime today = DateTime.UtcNow.Date;
            TemporaryScrumDetails temporaryScrumDetails = await _tempScrumDetailsRepository.FirstOrDefaultAsync(x => DbFunctions.TruncateTime(x.CreatedOn) == today && x.ScrumId == scrumId);
            return temporaryScrumDetails;
        }


        /// <summary>
        /// Remove all the temporary data of the scrum of the given scrumId from the list of the given day. - JJ
        /// </summary>
        /// <param name="scrumId">Id of scrum of the channel for the day</param>
        /// <returns></returns>
        private async Task RemoveTemporaryScrumDetailsAsync(int scrumId)
        {
            DateTime today = DateTime.UtcNow.Date;
            TemporaryScrumDetails temporaryScrumDetails = await _tempScrumDetailsRepository.FirstOrDefaultAsync(x => x.ScrumId == scrumId && DbFunctions.TruncateTime(x.CreatedOn) == today);
            if (temporaryScrumDetails != null)
            {
                _tempScrumDetailsRepository.Delete(temporaryScrumDetails.Id);
                await _tempScrumDetailsRepository.SaveChangesAsync();
            }
        }


        /// <summary>
        /// Get the slack user who was last asked question to. - JJ
        /// </summary>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <returns>object of SlackUserDetails</returns>
        private async Task<SlackUserDetailAc> GetSlackUserAsync(int scrumId, List<User> users)
        {
            TemporaryScrumDetails temporaryScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
            SlackUserDetailAc slackUserDetails = new SlackUserDetailAc();
            if (temporaryScrumDetails != null)
            {
                User user = users.FirstOrDefault(x => x.SlackUserId == temporaryScrumDetails.SlackUserId);
                slackUserDetails = await _slackUserDetailRepository.GetByIdAsync(temporaryScrumDetails.SlackUserId);
                if (!string.IsNullOrEmpty(slackUserDetails.UserId))
                {
                    if (user != null)
                    {
                        slackUserDetails.IsActive = user.IsActive;
                        slackUserDetails.Deleted = false;
                    }
                    else
                        slackUserDetails.Deleted = true;
                }
            }
            return slackUserDetails;
        }


        /// <summary>
        /// Update the scrum details temporarily stored in the database. - JJ
        /// </summary>
        /// <param name="slackUserId">Slack user's Id</param>
        /// <param name="scrumId">scrum id of the project for the day</param>
        /// <param name="users">List of users of the project</param>
        /// <param name="questionId">Id of last question asked to the user</param>
        private async Task UpdateTemporaryScrumDetailsAsync(string slackUserId, int scrumId, List<User> users, int? questionId)
        {
            DateTime today = DateTime.UtcNow.Date;
            TemporaryScrumDetails temporaryScrumDetails = await _tempScrumDetailsRepository.FirstOrDefaultAsync(x => x.ScrumId == scrumId && DbFunctions.TruncateTime(x.CreatedOn) == today);
            if (temporaryScrumDetails != null)
            {
                User user = users.FirstOrDefault(x => x.SlackUserId == slackUserId);
                int answerCount = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId && x.EmployeeId == user.Id).Result.Count();
                temporaryScrumDetails.SlackUserId = slackUserId;
                temporaryScrumDetails.AnswerCount = answerCount;
                temporaryScrumDetails.QuestionId = questionId;
                _tempScrumDetailsRepository.Update(temporaryScrumDetails);
                await _tempScrumDetailsRepository.SaveChangesAsync();
            }
        }


        #endregion


        /// <summary>
        ///  This method is called whenever a message other than the default keywords is written in the channel. - JJ
        /// </summary>
        /// <param name="slackUserName">slack user name of the interacting user</param>
        /// <param name="message">the message that interacting user sends</param>
        /// <param name="slackChannelId">slack channel Id</param>
        /// <param name="slackChannelName">slack channel name from which the message has been send</param>
        /// <param name="slackUserId">slack user Id of the interacting user</param>
        /// <returns>the next question statement</returns>
        private async Task<string> AddScrumAnswerAsync(string slackUserName, string message, string slackChannelId, string slackChannelName, string slackUserId)
        {
            string reply = string.Empty;
            DateTime today = DateTime.UtcNow.Date;
            //today's scrum of the channel 
            Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => String.Compare(x.SlackChannelId, slackChannelId, StringComparison.OrdinalIgnoreCase) == 0 && 
            DbFunctions.TruncateTime(x.ScrumDate) == today);
            if (scrum != null && scrum.IsOngoing && !scrum.IsHalted)
            {
                // getting user name from user's slack name
                ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
                // getting access token for that user
                if (applicationUser != null)
                {
                    // get access token of user for promact oauth server
                    string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);
                    //list of scrum questions. Type = BotQuestionType.Scrum
                    List<Question> questions = _questionRepository.FetchAsync(x => x.Type == BotQuestionType.Scrum).Result.OrderBy(x => x.OrderNumber).ToList();
                    //users of the given channel name fetched from the oauth server
                    List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(slackChannelName, accessToken);
                    ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(slackChannelName, accessToken);
                    ScrumStatus scrumStatus = await FetchScrumStatusAsync(project, users, questions);
                    //scrumStatus could be anything like the project is in-active
                    if (scrumStatus == ScrumStatus.OnGoing)
                    {
                        int questionCount = questions.Count();
                        //scrum answer of that day's scrum
                        List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id).Result.ToList();
                        //status would be empty if the interacting user is same as the expected user.
                        string status = await ExpectedUserAsync(scrum.Id, questions, users, slackUserName, slackUserId, scrum.ProjectId);
                        if (string.IsNullOrEmpty(status))
                        {
                            TemporaryScrumDetails temporaryScrumDetails = await FetchTemporaryScrumDetailsAsync(scrum.Id);
                            User user = users.FirstOrDefault(x => x.SlackUserId == temporaryScrumDetails.SlackUserId);
                            if (temporaryScrumDetails.QuestionId != null)
                                await AddUpdateAnswerAsync(scrum.Id, (int)temporaryScrumDetails.QuestionId, user.Id, message, ScrumAnswerStatus.Answered);
                            else
                                return _stringConstant.AnswerNotRecorded;

                            //update the details in temporary table 
                            await UpdateTemporaryScrumDetailsAsync(slackUserId, scrum.Id, users, null);
                            //get the next question
                            reply = await GetQuestionAsync(scrum.Id, questions, users, scrum.ProjectId);
                        }
                        //the user interacting is not the expected user
                        else
                            reply = status;
                    }
                    else
                        reply = ReplyStatusofScrumToClient(scrumStatus);
                }
                else
                    // if user doesn't exist then this message will be shown to user
                    reply = _stringConstant.YouAreNotInExistInOAuthServer;
            }
            return reply;
        }


        /// <summary>
        /// This method will be called when the keyword "scrum time" or "scrum halt" or "scrum resume" is encountered. - JJ
        /// </summary>
        /// <param name="slackChannelId">slack channel id</param>
        /// <param name="slackChannelName">slack channel name from which the message has been send</param>
        /// <param name="slackUserName">slack user name of the interacting user</param>
        /// <param name="parameter">the keyword(second word) send by the user</param>      
        /// <param name="slackUserId">slack userId of the interacting user</param>
        /// <returns>The question or the status of the scrum</returns>
        private async Task<string> ScrumAsync(string slackChannelId, string slackChannelName, string slackUserName, string parameter, string slackUserId)
        {
            // getting user name from user's slack name
            ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
            // getting access token for that user
            if (applicationUser != null)
            {
                // get access token of user for promact oauth server
                string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);
                List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(slackChannelName, accessToken);
                if (users != null && users.Any())
                {
                    ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(slackChannelName, accessToken);
                    DateTime today = DateTime.UtcNow.Date;
                    Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => String.Compare(x.SlackChannelId, slackChannelId, StringComparison.OrdinalIgnoreCase) == 0 && 
                    DbFunctions.TruncateTime(x.ScrumDate) == today);
                    ScrumStatus scrumStatus = await FetchScrumStatusAsync(project, users, null);
                    ScrumActions scrumCommand = (ScrumActions)Enum.Parse(typeof(ScrumActions), parameter);
                    User user = users.FirstOrDefault(x => x.SlackUserId == slackUserId);
                    if (user != null && user.IsActive)
                    {
                        switch (scrumCommand)
                        {
                            case ScrumActions.halt:
                                //keyword encountered is "scrum halt"
                                return await ScrumHaltAsync(scrum, scrumStatus);
                            case ScrumActions.resume:
                                //keyword encountered is "scrum resume"
                                return await ScrumResumeAsync(scrum, users, scrumStatus);
                            case ScrumActions.time:
                                //keyword encountered is "scrum time"
                                return await StartScrumAsync(slackChannelId, users, project, scrumStatus);
                            default:
                                return string.Empty;
                        }
                    }
                    //if user is in-active
                    string returnMessage = string.Empty;
                    switch (scrumStatus)
                    {
                        case ScrumStatus.Halted:
                            returnMessage = (scrumCommand == ScrumActions.resume ? _stringConstant.ScrumCannotBeResumed : string.Empty) + string.Format(_stringConstant.InActiveInOAuth, slackUserName);
                            break;

                        //scrum is in progress
                        case ScrumStatus.OnGoing:
                            List<Question> questions = _questionRepository.FetchAsync(x => x.Type == BotQuestionType.Scrum).Result.OrderBy(x => x.OrderNumber).ToList();
                            returnMessage = await GetReplyToUserAsync(users, project.Id, scrum.Id, slackUserId, slackUserName, questions);

                            if (scrumCommand == ScrumActions.resume)
                                returnMessage = _stringConstant.ScrumNotHalted + Environment.NewLine + returnMessage;
                            else if (scrumCommand == ScrumActions.halt)
                                returnMessage = _stringConstant.ScrumCannotBeHalted + Environment.NewLine + returnMessage;
                            break;

                        //for all other status of the scrum
                        default:
                            returnMessage = string.Format(_stringConstant.InActiveInOAuth, slackUserName) + ReplyStatusofScrumToClient(scrumStatus);
                            break;
                    }
                    return returnMessage;
                }
                return _stringConstant.NoEmployeeFound;
            }
            return _stringConstant.YouAreNotInExistInOAuthServer;
        }


        /// <summary>
        /// This method will be called when the keyword "leave @username" is received as reply from a channel member. - JJ
        /// </summary>
        /// <param name="slackChannelId">slack channel id</param>
        /// <param name="slackChannelName">slack channel name from which the message has been send</param>
        /// <param name="slackUserName">slack user name of the interacting user</param>
        /// <param name="slackUserId">slack user Id of the interacting user</param>
        /// <param name="applicant">slack user name of the user who is being marked on leave</param>
        /// <param name="applicantId">slack user id of the user who is being marked on leave</param>
        /// <returns>Question to the next person or other scrum status</returns>
        private async Task<string> LeaveAsync(string slackChannelId, string slackChannelName, string slackUserName, string slackUserId, string applicant, string applicantId)
        {
            string returnMsg;
            DateTime today = DateTime.UtcNow.Date;
            //we will have to check whether the scrum is on going or not before calling FetchScrumStatus()
            //because any command outside the scrum time must not be entertained except with the replies like "scrum is concluded","scrum has not started" or "scrum has not started".
            Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => String.Compare(x.SlackChannelId, slackChannelId, StringComparison.OrdinalIgnoreCase) == 0 && 
            DbFunctions.TruncateTime(x.ScrumDate) == today);
            if (scrum != null)
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

                            List<Question> questions = _questionRepository.FetchAsync(x => x.Type == BotQuestionType.Scrum).Result.OrderBy(x => x.OrderNumber).ToList();
                            List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(slackChannelName, accessToken);
                            ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(slackChannelName, accessToken);

                            ScrumStatus scrumStatus = await FetchScrumStatusAsync(project, users, questions);
                            if (scrumStatus == ScrumStatus.OnGoing)
                            {
                                User user = users.FirstOrDefault(x => x.SlackUserId == slackUserId);
                                if (user != null && user.IsActive)
                                    returnMsg = await MarkLeaveAsync(users, scrum.Id, applicant, questions, scrum.ProjectId, slackUserId, applicantId);
                                else
                                    //when the applicant is not in OAuth or not user of the project or is in-active inOAuth
                                    returnMsg = await GetReplyToUserAsync(users, project.Id, scrum.Id, slackUserId, slackUserName, questions);
                            }
                            else
                                returnMsg = ReplyStatusofScrumToClient(scrumStatus);
                        }
                        else
                            // if user doesn't exist in OAuth or hasn't logged in with Promact OAuth then this message will be shown to user
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
        /// Used to add channel manually by command "add channel channelname". - JJ
        /// </summary>
        /// <param name="slackChannelName">slack channel name from which message is send</param>
        /// <param name="slackChannelId">slack channel id from which message is send</param>
        /// <param name="slackUserId">slack user id of interacting user</param>
        /// <returns>status message</returns>
        private async Task<string> AddChannelManuallyAsync(string slackChannelName, string slackChannelId, string slackUserId)
        {
            string returnMsg;
            //Checks whether channelId starts with "G". This is done inorder to make sure that only private channels are added manually
            if (IsPrivateChannel(slackChannelId))
            {
                // getting user name from user's slack name
                ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
                // getting access token for that user
                if (applicationUser != null)
                {
                    // get access token of user for promact oauth server
                    string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);
                    //get the project details of the given channel name
                    ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(slackChannelName, accessToken);
                    //add channel details only if the channel has been registered as project in OAuth server
                    if (project != null && project.Id > 0)
                    {
                        if (project.IsActive)
                        {
                            SlackChannelDetails slackChannelDetails = new SlackChannelDetails();
                            slackChannelDetails.ChannelId = slackChannelId;
                            slackChannelDetails.CreatedOn = DateTime.UtcNow;
                            slackChannelDetails.Deleted = false;
                            slackChannelDetails.Name = slackChannelName;
                            await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
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
        /// Used to check whether channelId is of a private channel. - JJ
        /// </summary>
        /// <param name="slackChannelId">Id of the slack channel</param>
        /// <returns>true if private channel</returns>
        private bool IsPrivateChannel(string slackChannelId)
        {
            return (slackChannelId.StartsWith(_stringConstant.GroupNameStartsWith, StringComparison.Ordinal)) ? true : false;
        }


        /// <summary>
        /// This method is used to add/update Scrum answer to/in the database. - JJ
        /// </summary>
        /// <param name="scrumId">Id of scrum of the channel for that day</param>
        /// <param name="questionId">Id of the question which is answered</param>
        /// <param name="userId">Id of the user who has answered</param>
        /// <param name="message">answer</param>
        /// <param name="scrumAnswerStatus">the status of the answer like. Answered,Leave,etc</param>
        private async Task AddUpdateAnswerAsync(int scrumId, int questionId, string userId, string message, ScrumAnswerStatus scrumAnswerStatus)
        {
            ScrumAnswer scrumAnswer = await _scrumAnswerRepository.FirstOrDefaultAsync(x => x.ScrumId == scrumId && x.EmployeeId == userId && x.QuestionId == questionId);
            if (scrumAnswer != null)
            {
                scrumAnswer.Answer = message;
                scrumAnswer.ScrumAnswerStatus = scrumAnswerStatus;
                _scrumAnswerRepository.Update(scrumAnswer);
                await _scrumAnswerRepository.SaveChangesAsync();
            }
            else
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
            }
        }


        /// <summary>
        /// This method will be called when the keyword "scrum time" is encountered. - JJ
        /// </summary>
        /// <param name="slackChannelId">slack channel id</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="project">project(in OAuth) corresponding to slack channel</param>
        ///<param name="scrumStatus">status of scrum</param>
        /// <returns>The next question or the scrum complete message</returns>
        private async Task<string> StartScrumAsync(string slackChannelId, List<User> users, ProjectAc project, ScrumStatus scrumStatus)
        {
            string replyMessage = string.Empty;
            List<Question> questionList = _questionRepository.FetchAsync(x => x.Type == BotQuestionType.Scrum).Result.OrderBy(x => x.OrderNumber).ToList();
            //only if scrum has not been conducted in the day can scrum start.
            if (scrumStatus == ScrumStatus.NotStarted)
            {
                Question question = questionList.First();
                User firstUser = users.FirstOrDefault(x => x.IsActive);
                if (firstUser != null)
                {
                    SlackUserDetailAc slackUserDetailAc = await _slackUserDetailRepository.GetByIdAsync(firstUser.SlackUserId);
                    if (slackUserDetailAc == null)
                    {
                        SlackUserDetails slackUserDetail = _slackUserDetails.FirstOrDefault(x => users.Where(y => y.IsActive).Select(y => y.SlackUserId).Contains(x.UserId));
                        if (slackUserDetail != null)
                            firstUser = users.First(x => x.SlackUserId == slackUserDetail.UserId);
                        else
                            return _stringConstant.NoEmployeeFound;
                    }
                    Scrum scrum = new Scrum();
                    scrum.CreatedOn = DateTime.UtcNow.Date;
                    scrum.SlackChannelId = slackChannelId;
                    scrum.ScrumDate = DateTime.UtcNow.Date;
                    scrum.ProjectId = project.Id;
                    scrum.TeamLeaderId = project.TeamLeaderId;
                    scrum.IsHalted = false;
                    scrum.IsOngoing = true;
                    _scrumRepository.Insert(scrum);
                    await _scrumRepository.SaveChangesAsync();

                    //add the scrum details to the temporary table
                    await AddTemporaryScrumDetailsAsync(scrum.Id, firstUser.SlackUserId, 0, question.Id);
                    //first user is asked questions along with the previous day status (if any)
                    replyMessage = string.Format(_stringConstant.GoodDay, slackUserDetailAc.Name) + FetchPreviousDayStatus(firstUser.Id, project.Id, questionList) + question.QuestionStatement + Environment.NewLine;
                }
                else
                    //no active users are found
                    replyMessage = _stringConstant.NoEmployeeFound;
            }
            else if (scrumStatus == ScrumStatus.OnGoing)
            {
                DateTime today = DateTime.UtcNow.Date;
                Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => String.Compare(x.SlackChannelId, slackChannelId, StringComparison.OrdinalIgnoreCase) == 0 && 
                DbFunctions.TruncateTime(x.ScrumDate) == today);
                //user to whom the last question was asked
                SlackUserDetailAc prevUser = await GetSlackUserAsync(scrum.Id, users);
                if (!string.IsNullOrEmpty(prevUser?.Name))
                {
                    if (prevUser.Deleted)
                        //user is not part of the project in OAuth
                        replyMessage = string.Format(_stringConstant.UserNotInProject, prevUser.Name);

                    else if (!prevUser.IsActive)
                    {
                        //user to whom the last question was asked. but this user is in active now
                        User userDetail = users.FirstOrDefault(x => x.SlackUserId == prevUser.UserId);
                        List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id && x.EmployeeId == userDetail.Id).Result.ToList();
                        replyMessage = string.Format(_stringConstant.InActiveInOAuth, prevUser.Name);
                    }
                }
                //if scrum meeting was interrupted. "scrum time" is written to resume scrum meeting. So next question is fetched.
                replyMessage += await GetQuestionAsync(scrum.Id, questionList, users, project.Id);
            }
            else
                //for all other statuses.
                return ReplyStatusofScrumToClient(scrumStatus);
            return replyMessage;
        }


        /// <summary>
        /// This method is used to mark a user's answer on leave. - JJ
        /// </summary>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="applicant">slack user name of the user who is being marked on leave</param>
        /// <param name="questions">List of questions to be asked in scrum</param>
        /// <param name="projectId">Id of project(in OAuth) corresponding to slack channel</param>
        /// <param name="slackUserId">slack userId of the interacting user</param>
        /// <param name="applicantId">slack user id of the user who is being marked on leave</param>
        /// <returns>Question to the next user or status of the request</returns>
        private async Task<string> MarkLeaveAsync(List<User> users, int scrumId, string applicant, List<Question> questions, int projectId, string slackUserId, string applicantId)
        {
            string returnMsg = string.Empty;
            User user = users.FirstOrDefault(x => x.SlackUserId == applicantId);
            if (user != null)
            {
                if (user.IsActive)
                {
                    //checks whether the applicant is the expected user
                    string status = await ExpectedUserAsync(scrumId, questions, users, applicant, applicantId, projectId);
                    //if the interacting user is the expected user
                    if (string.IsNullOrEmpty(status))
                    {
                        //if applying user tries to mark himself/herself as on leave
                        if (String.Compare(slackUserId, applicantId, StringComparison.OrdinalIgnoreCase) == 0)
                            return _stringConstant.LeaveError;
                        string expectedUserId = users.First(x => x.SlackUserId == applicantId).Id;
                        //fetch the scrum answer of the user given on that day
                        List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId && x.EmployeeId == expectedUserId && x.ScrumAnswerStatus == ScrumAnswerStatus.Answered).Result.ToList();
                        //If no answer from the user has been obtained yet.
                        if (!scrumAnswer.Any())
                        {
                            //all the scrum questions are answered as "leave"
                            foreach (Question question in questions)
                            {
                                await AddUpdateAnswerAsync(scrumId, question.Id, expectedUserId, _stringConstant.Leave, ScrumAnswerStatus.Leave);
                            }
                            await UpdateTemporaryScrumDetailsAsync(applicantId, scrumId, users, null);
                        }
                        else
                            //If the applicant has already answered questions
                            returnMsg = string.Format(_stringConstant.AlreadyAnswered, applicant);
                    }
                    else
                        return status;
                }
                else
                    return await GetReplyToUserAsync(users, projectId, scrumId, applicantId, applicant, questions);
            }
            else
                returnMsg = string.Format(_stringConstant.UserNotInProject, applicant);
            //fetches the next question or status and returns
            return returnMsg + await GetQuestionAsync(scrumId, questions, users, projectId);
        }


        /// <summary>
        /// Used to fetch the next question based on the given parameters. JJ
        /// </summary>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="questions">List of questions to be asked in scrum</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="projectId">Id of project(in OAuth) corresponding to slack channel</param>
        /// <returns>The next question or the scrum complete message</returns>
        private async Task<string> GetQuestionAsync(int scrumId, List<Question> questions, List<User> users, int projectId)
        {
            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();
            TemporaryScrumDetails temporaryScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
            //user to whom the last question was asked
            User prevUser = users.FirstOrDefault(x => x.SlackUserId == temporaryScrumDetails.SlackUserId && x.IsActive);
            User user = new User();
            //list of active users who have not answered yet                       
            List<User> activeUnAnsweredUserList = users.Where(x => x.IsActive && !scrumAnswers.Select(y => y.EmployeeId).ToList().Contains(x.Id)).ToList();
            if (scrumAnswers.Any())
            {
                int questionCount = questions.Count();
                if (temporaryScrumDetails.AnswerCount == 0 || temporaryScrumDetails.AnswerCount == questionCount)
                {
                    //all questions have been asked to the previous user      
                    user = activeUnAnsweredUserList.FirstOrDefault();
                    //now fetch the first question to the next user
                    if (prevUser != null)
                        user = activeUnAnsweredUserList.FirstOrDefault(x => x.SlackUserId == prevUser.SlackUserId);
                    //temporaryScrumDetails.AnswerCount == questionCount - because if the previous user has answered all
                    //his questions then next user must be asked question
                    if (temporaryScrumDetails.AnswerCount == questionCount || user == null)
                        user = activeUnAnsweredUserList.FirstOrDefault();
                }
                else
                {
                    //as not all questions have been answered by the last user,the next question to that user will be asked
                    if (prevUser != null)
                    {
                        SlackUserDetailAc slackUserDetail = await _slackUserDetailRepository.GetByIdAsync(prevUser.SlackUserId);
                        if (slackUserDetail != null)
                        {
                            //last scrum answer of the given scrum id.
                            ScrumAnswer lastScrumAnswer = scrumAnswers.OrderByDescending(x => x.Id).First(x => x.EmployeeId == prevUser.Id);
                            Question question = await FetchQuestionAsync(lastScrumAnswer.QuestionId);
                            if (question != null)
                            {
                                await UpdateTemporaryScrumDetailsAsync(prevUser.SlackUserId, scrumId, users, question.Id);
                                return string.Format(_stringConstant.NameFormat, slackUserDetail.Name) + question.QuestionStatement + Environment.NewLine;
                            }
                            return _stringConstant.NoQuestion;
                        }
                    }
                    user = activeUnAnsweredUserList.FirstOrDefault();
                }
            }
            else
                user = prevUser ?? activeUnAnsweredUserList.FirstOrDefault();  //preveUser == null, if a user was asked a question before but at present is not active

            if (user != null)
            {
                SlackUserDetailAc slackUserAc = await _slackUserDetailRepository.GetByIdAsync(user.SlackUserId);
                if (slackUserAc != null)
                {
                    Question firstQuestion = questions.First();
                    //update the temporary scrum details with the next id of the question to be asked
                    await UpdateTemporaryScrumDetailsAsync(user.SlackUserId, scrumId, users, firstQuestion.Id);
                    //as it is the first question to the user also fetch the previous day scrum status.
                    return string.Format(_stringConstant.GoodDay, slackUserAc.Name) + FetchPreviousDayStatus(user.Id, projectId, questions) + firstQuestion.QuestionStatement + Environment.NewLine;
                }
            }
            return await MarkScrumCompleteAsync(scrumId, users, questions.Count());
        }


        /// <summary>
        /// Used to check and mark scrum as completed. - JJ
        /// </summary>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="users">list of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="questionCount">number of questions asked during scrum to a user</param>
        /// <returns>reply to user</returns>
        /// <remarks>If scrum is completed then message saying that the scrum is complete 
        /// or if any active emplpoyee is pending to answer then that question</remarks>
        private async Task<string> MarkScrumCompleteAsync(int scrumId, List<User> users, int questionCount)
        {
            //list of scrum answers of the given scrumId            
            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.Fetch(x => x.ScrumId == scrumId).OrderBy(x => x.Id).ToList();
            User user = new User();
            Question question = new Question();
            SlackUserDetailAc slackUserDetail = new SlackUserDetailAc();
            string nextQuestion = string.Empty;

            var scrumAnswersInComplete = scrumAnswers.GroupBy(m => m.EmployeeId)
                .Select(g => new
                {
                    AnswerCount = g.Count(), g.First().EmployeeId,
                    Answers = g
                }).ToList();

            if (scrumAnswersInComplete.Any(x => x.AnswerCount < questionCount))
            {
                var userIdObjects = scrumAnswersInComplete.FindAll(x => x.AnswerCount < questionCount);
                foreach (var userId in userIdObjects)
                {
                    user = users.FirstOrDefault(x => x.Id == userId.EmployeeId && x.IsActive);
                    //check whether those who didn't answer now are active or not
                    if (!string.IsNullOrEmpty(user?.Id))
                    {
                        slackUserDetail = await _slackUserDetailRepository.GetByIdAsync(user.SlackUserId);
                        if (slackUserDetail != null)
                        {
                            question = await FetchQuestionAsync(userId.Answers.OrderByDescending(x => x.Id).First().QuestionId);
                            nextQuestion = question.QuestionStatement;
                            break;
                        }
                    }
                }
            }
            //if the nextQuestion is fetched then it means that there are questions to be asked to user
            if (!string.IsNullOrEmpty(nextQuestion))
            {
                await UpdateTemporaryScrumDetailsAsync(user.SlackUserId, scrumId, users, question.Id);
                return string.Format(_stringConstant.MarkedInActive, slackUserDetail.Name) + nextQuestion;
            }
            //if no questions are pending then scrum is marked to be complete
            if (await UpdateScrumAsync(scrumId, false, false) == 1)
                //answers of all the users has been recorded            
                return _stringConstant.ScrumComplete;
            return _stringConstant.ErrorMsg;
        }


        /// <summary>
        /// Update scrum status to not in progress scrum. JJ
        /// </summary>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="isHalted">bit indicating whether scrum is halted</param>
        /// <param name="isOngoing">bit indicating whether scrum is in progress</param>
        /// <returns>1 if successfully updated</returns>
        private async Task<int> UpdateScrumAsync(int scrumId, bool isOngoing, bool isHalted)
        {
            if (!isOngoing)
                await RemoveTemporaryScrumDetailsAsync(scrumId);
            Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => x.Id == scrumId);
            scrum.IsOngoing = isOngoing;
            scrum.IsHalted = isHalted;
            _scrumRepository.Update(scrum);
            return await _scrumRepository.SaveChangesAsync();
        }


        /// <summary>
        /// Used to check whether the applicant is the expected user. - JJ
        /// </summary>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="questions">List of questions to be asked in scrum</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="applicant">slack user name</param>
        /// <param name="applicantId">slack user id</param>
        /// <param name="projectId">Id of project(in OAuth) corresponding to slack channel</param>
        /// <returns>empty string if the expected user is same as the applicant</returns>
        private async Task<string> ExpectedUserAsync(int scrumId, List<Question> questions, List<User> users, string applicant, string applicantId, int projectId)
        {
            //List of scrum answer of the given scrumId.
            List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();
            User user = new User();
            if (scrumAnswer.Any())
            {
                int questionCount = questions.Count();
                TemporaryScrumDetails temporaryScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
                //list of user ids who have not answer yet and are still active                     
                List<User> activeUserList = users.Where(x => x.IsActive && !scrumAnswer.Select(y => y.EmployeeId).ToList().Contains(x.Id)).ToList();
                User prevUser = users.FirstOrDefault(x => x.SlackUserId == temporaryScrumDetails.SlackUserId);
                if (prevUser != null)
                {
                    if (temporaryScrumDetails.AnswerCount == 0 || temporaryScrumDetails.AnswerCount == questionCount)
                    {
                        //all questions have been asked to the previous user 
                        if (activeUserList.Any())
                        {
                            //now the next user
                            TemporaryScrumDetails tempScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
                            user = activeUserList.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId);
                            if (user == null)//the previous user is either in-active or not a member of the project in OAuth
                                user = activeUserList.First();
                        }
                        else
                        {
                            string reply = await GetReplyToUserAsync(users, projectId, scrumId, applicantId, applicant, questions);

                            if (string.IsNullOrEmpty(reply))//when scrum concludes
                                reply = await GetQuestionAsync(scrumId, questions, users, projectId);

                            return reply;
                        }
                    }
                    else
                        //as not all questions have been answered by the last user,so to that user itself
                        user = prevUser;
                }
                else
                    user = activeUserList.FirstOrDefault();
            }
            else
            {
                //no scrum answer has been recorded yet. So first user
                TemporaryScrumDetails tempScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
                user = users.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId && x.IsActive);
                if (user == null)
                    user = users.FirstOrDefault(x => x.IsActive);
            }

            return await ProcessExpectedUserResultAsync(user, applicantId, users, projectId, applicant, scrumId, questions);
        }


        /// <summary>
        /// Gets the appropraite reply to the next user. JJ
        /// </summary>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="projectId">Id of project(in OAuth) corresponding to slack channel</param>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="applicantId">slack user id</param>
        /// <param name="applicant">slack user name</param>
        /// <param name="questions">List of questions to be asked in scrum</param>
        /// <returns>reply to the question, next question or the scrum status</returns>
        private async Task<string> GetReplyToUserAsync(List<User> users, int projectId, int scrumId, string applicantId, string applicant, List<Question> questions)
        {
            bool fetchQuestion = false;
            User unexpectedUser = users.FirstOrDefault(x => x.SlackUserId == applicantId);
            //the user to whom the last question was asked. This user must be called before GetQuestionAsync() is called because if scrum is complete then temporary data is deleted and this user cannot be fetched.
            SlackUserDetailAc prevUser = await GetSlackUserAsync(scrumId, users);
            string reply = await GetQuestionAsync(scrumId, questions, users, projectId);
            if (unexpectedUser != null && !unexpectedUser.IsActive)
            {
                fetchQuestion = true;
                reply = string.Format(_stringConstant.InActiveInOAuth, applicant) + reply;
            }
            bool isPreviousUserNull = string.IsNullOrEmpty(prevUser?.Name);
            //if unexpectedUser is null it means that the user is not a member of the project in OAuth
            //in that case even the user who user who was asked the last question to(i.e prevUser) is same as this user, it is alright
            if (!isPreviousUserNull && (prevUser.UserId != applicantId || unexpectedUser == null))
            {
                //the user who user who was asked the last question to(i.e prevUser) is not a member of the project in OAuth
                if (prevUser.Deleted)
                {
                    fetchQuestion = true;
                    reply = string.Format(_stringConstant.UserNotInProject, prevUser.Name) + reply;
                }
                else if (!prevUser.IsActive)
                {
                    fetchQuestion = true;
                    reply = string.Format(_stringConstant.InActiveInOAuth, prevUser.Name) + reply;
                }
            }

            //issue is : scrum starts and first question is asked to first user. Remove first and second users from project.Let second user write scrum halt. Third user is asked question. Second user writes scrum halt again.
            //when the unexpectedUser user is null(user is not a member of project or not in OAuth) and previous user is not the interacting user right now
            //or when both unexpectedUser user and previous users are null.
            if ((unexpectedUser == null && !isPreviousUserNull && prevUser.UserId != applicantId) || (unexpectedUser == null && isPreviousUserNull))
            {
                fetchQuestion = true;
                reply = string.Format(_stringConstant.UserNotInProject, applicant) + reply;
            }
            if (fetchQuestion)
                return reply;
            return string.Empty;
        }


        /// <summary>
        /// Check whether the given user can answer now. - JJ
        /// </summary>
        /// <param name="user">User who is expected to interact</param>
        /// <param name="applicantId">slack user id</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="projectId">Id of project(in OAuth) corresponding to slack channel</param>
        /// <param name="applicant">slack user name</param>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="questions">List of questions to be asked in scrum</param>
        /// <returns>status</returns>
        private async Task<string> ProcessExpectedUserResultAsync(User user, string applicantId, List<User> users, int projectId, string applicant, int scrumId, List<Question> questions)
        {
            //the expected user and the interacting user are same and is active
            if (user != null && user.IsActive && user.SlackUserId == applicantId)
            {
                TemporaryScrumDetails tempScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
                //the expected interacting user is not the user to whom the last question was asked
                if (tempScrumDetails.SlackUserId != applicantId)
                {
                    //last question was asked to this user.
                    SlackUserDetailAc tempSlackUser = await _slackUserDetailRepository.GetByIdAsync(tempScrumDetails.SlackUserId);
                    if (tempSlackUser != null)
                    {
                        User userDetail = users.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId);
                        tempScrumDetails.SlackUserId = applicantId;

                        if (userDetail == null)
                            // User is either not a member of the project or not in OAuth
                            return string.Format(_stringConstant.UserNotInProject, tempSlackUser.Name) + await GetQuestionAsync(scrumId, questions, users, projectId);
                        if (!userDetail.IsActive)
                            return string.Format(_stringConstant.InActiveInOAuth, tempSlackUser.Name) + await GetQuestionAsync(scrumId, questions, users, projectId);
                    }
                    else
                        return _stringConstant.UserNotInSlack + await GetQuestionAsync(scrumId, questions, users, projectId);
                }
                return string.Empty;
            }
            string reply = await GetReplyToUserAsync(users, projectId, scrumId, applicantId, applicant, questions);
            if (user != null)
            {
                SlackUserDetailAc expectedUser = await _slackUserDetailRepository.GetByIdAsync(user.SlackUserId);
                if (expectedUser != null)
                {
                    if (!user.IsActive)
                        //the expected user is marked as in-active in OAuth. So mark the answers as in active and fetch question to the next user
                        return string.Format(_stringConstant.InActiveInOAuth, expectedUser.Name) + await GetQuestionAsync(scrumId, questions, users, projectId);
                    reply += string.Format(_stringConstant.PleaseAnswer, expectedUser.Name);
                }
                else
                    return _stringConstant.UserNotInSlack + await GetQuestionAsync(scrumId, questions, users, projectId);
            }
            return reply;
        }


        /// <summary>
        /// This method fetches the Question of next order of the given questionId - JJ
        /// </summary>
        /// <param name="questionId">Id of question to be fetched</param>
        /// <returns>object of Question</returns>
        private async Task<Question> FetchQuestionAsync(int questionId)
        {
            Question question = await _questionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
            if (question != null)
            {
                //order number of the given question 
                int orderNumber = (int)question.OrderNumber;
                //question with the next order
                question = await _questionRepository.FirstOrDefaultAsync(x => x.OrderNumber == (QuestionOrder)(orderNumber + 1) && x.Type == BotQuestionType.Scrum);
            }
            return question;
        }


        /// <summary>
        /// Fetches the previous day's questions and answers of the user of the given id for the given project - JJ
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="projectId">Id of project(in OAuth) corresponding to slack channel</param>
        /// <param name="questions">List of questions of scrum </param>
        /// <returns>previous day status</returns>
        private string FetchPreviousDayStatus(string userId, int projectId, List<Question> questions)
        {
            string previousDayStatus = string.Empty;
            DateTime today = DateTime.UtcNow.Date;
            //previous scrums of this channel(project)
            List<int> scrumList = _scrumRepository.FetchAsync(x => x.ProjectId == projectId && DbFunctions.TruncateTime(x.ScrumDate) < today).Result.Select(x => x.Id).ToList();
            //answers in which user was not on leave.
            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => scrumList.Contains(x.ScrumId) && x.EmployeeId == userId && x.ScrumAnswerStatus == ScrumAnswerStatus.Answered).Result.OrderByDescending(x => x.AnswerDate).ToList();
            if (scrumAnswers.Any() && questions.Any())
            {
                DateTime scrumDate = new DateTime();
                foreach (Question question in questions)
                {
                    //Question and the corresponding answer appended
                    ScrumAnswer scrumAnswer = scrumAnswers.FirstOrDefault(x => x.QuestionId == question.Id);
                    if (scrumAnswer != null)
                    {
                        if (string.IsNullOrEmpty(previousDayStatus))
                        {
                            scrumDate = scrumAnswer.AnswerDate;
                            previousDayStatus = Environment.NewLine + string.Format(_stringConstant.PreviousDayStatus, scrumAnswer.AnswerDate.ToShortDateString()) + Environment.NewLine;
                        }
                        if (scrumDate.Date == scrumAnswer.AnswerDate.Date)
                            previousDayStatus += string.Format(_stringConstant.PreviousDayScrumAnswer, question.QuestionStatement, scrumAnswer.Answer);
                    }
                }
            }
            if (!string.IsNullOrEmpty(previousDayStatus))
                return previousDayStatus + Environment.NewLine + _stringConstant.AnswerToday + Environment.NewLine + Environment.NewLine;

            return previousDayStatus;
        }


        /// <summary>
        /// Fetch the status of the scrum - JJ
        /// </summary>
        /// <param name="project">project(in OAuth) corresponding to slack channel</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="questions">List of questions to be asked in scrum</param>
        /// <returns>object of ScrumStatus</returns>
        private async Task<ScrumStatus> FetchScrumStatusAsync(ProjectAc project, List<User> users, List<Question> questions)
        {
            if (project != null && project.Id > 0)
            {
                if (project.IsActive)
                {
                    if (users != null && users.Any())
                    {
                        if (questions == null || !questions.Any())
                            questions = _questionRepository.FetchAsync(x => x.Type == BotQuestionType.Scrum).Result.ToList();
                        if (questions.Any())
                        {
                            DateTime today = DateTime.UtcNow.Date;
                            Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => x.ProjectId == project.Id && DbFunctions.TruncateTime(x.ScrumDate) == today);
                            if (scrum != null)
                            {
                                if (!scrum.IsHalted)
                                    return scrum.IsOngoing ? ScrumStatus.OnGoing : ScrumStatus.Completed;

                                return ScrumStatus.Halted;
                            }
                            return ScrumStatus.NotStarted;
                        }
                        return ScrumStatus.NoQuestion;
                    }
                    return ScrumStatus.NoEmployee;
                }
                return ScrumStatus.InActiveProject;
            }
            return ScrumStatus.NoProject;
        }


        /// <summary>
        /// Halt the scrum meeting - JJ
        /// </summary>
        /// <param name="scrum">scrum of the channel for that day</param>
        /// <param name="status">status of scrum</param>
        /// <returns>scrum halted message</returns>
        private async Task<string> ScrumHaltAsync(Scrum scrum, ScrumStatus status)
        {
            //keyword encountered is "scrum halt"
            if (status == (ScrumStatus.OnGoing))
            {
                //scrum halted
                await UpdateScrumAsync(scrum.Id, true, true);
                return _stringConstant.ScrumHalted;
            }
            if (status == (ScrumStatus.Halted))
                return _stringConstant.ScrumAlreadyHalted;
            return ReplyStatusofScrumToClient(status) + _stringConstant.ScrumCannotBeHalted;
        }


        /// <summary>
        /// Resume the scrum meeting - JJ
        /// </summary>
        /// <param name="scrum">scrum of the channel for that day</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="status">status of scrum</param>
        /// <returns>scrum resume message along with the next question</returns>
        private async Task<string> ScrumResumeAsync(Scrum scrum, List<User> users, ScrumStatus status)
        {
            List<Question> questionList = _questionRepository.FetchAsync(x => x.Type == BotQuestionType.Scrum).Result.ToList();
            //keyword encountered is "scrum resume"      
            if (status == (ScrumStatus.Halted) || status == (ScrumStatus.OnGoing))
            {
                string returnMsg;
                if (status == (ScrumStatus.Halted))
                {
                    //scrum resumed
                    await UpdateScrumAsync(scrum.Id, true, false);
                    returnMsg = _stringConstant.ScrumResumed;
                }
                else
                    returnMsg = _stringConstant.ScrumNotHalted;

                //user to whom the last question was asked
                SlackUserDetailAc prevUser = await GetSlackUserAsync(scrum.Id, users);
                if (!string.IsNullOrEmpty(prevUser?.Name))
                {
                    if (prevUser.Deleted)//the previous user is not part of the project in OAuth
                        returnMsg += string.Format(_stringConstant.UserNotInProject, prevUser.Name);

                    else if (!prevUser.IsActive && status == (ScrumStatus.Halted))
                        returnMsg += string.Format(_stringConstant.InActiveInOAuth, prevUser.Name);
                }
                //next question is fetched
                returnMsg += await GetQuestionAsync(scrum.Id, questionList, users, scrum.ProjectId);
                return returnMsg;
            }
            return ReplyStatusofScrumToClient(status) + _stringConstant.ScrumCannotBeResumed;
        }


        /// <summary>
        /// Select the appropriate reply to the client - JJ
        /// </summary>
        /// <param name="scrumStatus">Status of the scrum</param>
        /// <returns>appropriate message indicating the status of scrum</returns>
        private string ReplyStatusofScrumToClient(ScrumStatus scrumStatus)
        {
            string returnMessage;
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