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
using System.Data.Entity;
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
        /// This will process the messages from slack and use appropriate methods to give a suitable response through Bot - JJ
        /// </summary>
        /// <param name="userId">UserId of slack user</param>
        /// <param name="channelId">Slack Channel Id</param>
        /// <param name="message">message from slack</param>
        /// <returns>reply message</returns>
        public async Task<string> ProcessMessagesAsync(string userId, string channelId, string message)
        {
            string replyText = string.Empty;
            SlackUserDetailAc user = await _slackUserDetails.GetByIdAsync(userId);
            SlackChannelDetails channel = await _slackChannelRepository.GetByIdAsync(channelId);
            //the command is split to individual words
            //commnads ex: "scrum time", "leave @userId"
            string[] messageArray = message.Split(null);

            #region Added temporarily for testing purpose

            if (messageArray[0] == "delete")
            {
                var date = DateTime.UtcNow.Date;
                ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == userId);
                // getting access token for that user
                if (applicationUser != null)
                {
                    // get access token of user for promact oauth server
                    string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);

                    ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(channel.Name, accessToken);
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

            else
            {
                if (user != null && string.Compare(message, _stringConstant.ScrumHelp, true) == 0)
                {
                    replyText = _stringConstant.ScrumHelpMessage;
                }
                else if (user != null && channel != null)
                {
                    //commands could be"scrum time" or "scrum halt" or "scrum resume"
                    if (string.Compare(message, _stringConstant.ScrumTime, true) == 0 || string.Compare(message, _stringConstant.ScrumHalt, true) == 0 || string.Compare(message, _stringConstant.ScrumResume, true) == 0)
                    {
                        replyText = await ScrumAsync(channelId, channel.Name, user.Name, messageArray[1].ToLower(), user.UserId);
                    }
                    //a particular user is on leave
                    //command would be like "leave <@userId>"
                    else if ((string.Compare(messageArray[0], _stringConstant.Leave, true) == 0) && messageArray.Length == 2)
                    {
                        //"<@".Length is 2
                        int fromIndex = message.IndexOf("<@") + 2;
                        int toIndex = message.LastIndexOf(">");
                        if (toIndex > 0 && fromIndex > 1)
                        {
                            //the userId is fetched
                            string applicantId = message.Substring(fromIndex, toIndex - fromIndex);
                            //fetch the user of the given userId
                            SlackUserDetailAc applicant = await _slackUserDetails.GetByIdAsync(applicantId);
                            if (applicant != null)
                            {
                                string applicantName = applicant.Name;
                                replyText = await LeaveAsync(channelId, channel.Name, user.Name, user.UserId, applicantName, applicantId);
                            }
                            else
                            {
                                replyText = _stringConstant.NotAUser;
                            }
                        }
                        else
                        {
                            //when command would be like "leave <@>"
                            replyText = await AddScrumAnswerAsync(user.Name, message, channelId, channel.Name, user.UserId);
                        }
                    }
                    //all other texts
                    else
                    {
                        replyText = await AddScrumAnswerAsync(user.Name, message, channelId, channel.Name, user.UserId);
                    }
                }
                //If channel is not registered in the database
                else if (user != null)
                {
                    //If channel is not registered in the database and the command encountered is "add channel channelname"
                    if (channel == null && string.Compare(messageArray[0], _stringConstant.Add, true) == 0 && string.Compare(messageArray[1], _stringConstant.Channel, true) == 0)
                    {
                        replyText = await AddChannelManuallyAsync(messageArray[2], channelId, user.UserId);
                    }
                    //If any of the commands which scrum bot recognizes is encountered
                    else if (((string.Compare(messageArray[0], _stringConstant.Leave, true) == 0) && messageArray.Length == 2) || string.Compare(message, _stringConstant.ScrumTime, true) == 0 || string.Compare(message, _stringConstant.ScrumHalt, true) == 0 || string.Compare(message, _stringConstant.ScrumResume, true) == 0)
                    {
                        replyText = _stringConstant.ChannelAddInstruction;
                    }
                }
                else if (user == null)
                {
                    SlackBotUserDetail botUser = await _slackBotUserDetail.FirstOrDefaultAsync(x => x.UserId == userId);
                    //If any of the commands which scrum bot recognizes is encountered
                    if (botUser == null && (((string.Compare(messageArray[0], _stringConstant.Leave, true) == 0) && messageArray.Length == 2) || string.Compare(message, _stringConstant.ScrumTime, true) == 0 || string.Compare(message, _stringConstant.ScrumHalt, true) == 0 || string.Compare(message, _stringConstant.ScrumResume, true) == 0))
                    {
                        replyText = _stringConstant.SlackUserNotFound;
                    }
                }
                return replyText;
            }
        }


        #region Temporary


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
                TemporaryScrumDetails temp = new TemporaryScrumDetails();
                temp.ScrumId = scrumId;
                temp.SlackUserId = slackUserId;
                temp.AnswerCount = answerCount;
                temp.QuestionId = questionId;
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
            TemporaryScrumDetails tempScrumDetails = await _tempScrumDetailsRepository.FirstOrDefaultAsync(x => x.ScrumId == scrumId && DbFunctions.TruncateTime(x.CreatedOn) == today);
            if (tempScrumDetails != null)
            {
                _tempScrumDetailsRepository.Delete(tempScrumDetails.Id);
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
            TemporaryScrumDetails tempScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
            SlackUserDetailAc slackUserDetails = new SlackUserDetailAc();
            if (tempScrumDetails != null)
            {
                User user = users.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId);
                slackUserDetails = await _slackUserDetails.GetByIdAsync(tempScrumDetails.SlackUserId);
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
        /// <param name="questionId">Id of last question asked to the user</param>
        /// <param name="scrumId">scrum id of the project for the day</param>
        /// <param name="users">List of users of the project</param>
        private async Task UpdateTemporaryScrumDetailsAsync(string slackUserId, int scrumId, List<User> users, int? questionId)
        {
            DateTime today = DateTime.UtcNow.Date;
            TemporaryScrumDetails tempScrumDetails = await _tempScrumDetailsRepository.FirstOrDefaultAsync(x => x.ScrumId == scrumId && DbFunctions.TruncateTime(x.CreatedOn) == today);
            if (tempScrumDetails != null)
            {
                User user = users.FirstOrDefault(x => x.SlackUserId == slackUserId);
                int answerCount = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId && x.EmployeeId == user.Id).Result.Count();
                tempScrumDetails.SlackUserId = slackUserId;
                tempScrumDetails.AnswerCount = answerCount;
                tempScrumDetails.QuestionId = questionId;
                _tempScrumDetailsRepository.Update(tempScrumDetails);
                await _tempScrumDetailsRepository.SaveChangesAsync();
            }
        }


        #endregion


        /// <summary>
        ///  This method is called whenever a message other than the default keywords is written in the channel. - JJ
        /// </summary>
        /// <param name="slackUserName">slack user name of the interacting user</param>
        /// <param name="message">the message that interacting user sends</param>
        /// <param name="channelId">slack channel Id</param>
        /// <param name="channelName">slack channel name from which the message has been send</param>
        /// <param name="slackUserId">slack user Id of the interacting user</param>
        /// <returns>the next question statement</returns>
        private async Task<string> AddScrumAnswerAsync(string slackUserName, string message, string channelId, string channelName, string slackUserId)
        {
            string reply = string.Empty;
            DateTime today = DateTime.UtcNow.Date;
            //today's scrum of the channel 
            Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => string.Compare(x.SlackChannelId, channelId, true) == 0 && DbFunctions.TruncateTime(x.ScrumDate) == today);
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
                    List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(channelName, accessToken);
                    ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(channelName, accessToken);
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
                            //scrum answers of users who were in active before, are now to be answered
                            List<ScrumAnswer> nowReadyScrumsAnswers = scrumAnswers.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).OrderBy(x => x.Id).ToList();
                            if (nowReadyScrumsAnswers.Any())
                            {
                                ScrumAnswer answerNow = nowReadyScrumsAnswers.First();
                                //update the answer which was marked as in - active before with the answer
                                await AddUpdateAnswerAsync(scrum.Id, answerNow.QuestionId, answerNow.EmployeeId, message, ScrumAnswerStatus.Answered);
                            }
                            else
                            {
                                TemporaryScrumDetails temporaryScrumDetails = await FetchTemporaryScrumDetailsAsync(scrum.Id);
                                User user = users.FirstOrDefault(x => x.SlackUserId == temporaryScrumDetails.SlackUserId);
                                if (temporaryScrumDetails.QuestionId != null)
                                    await AddUpdateAnswerAsync(scrum.Id, (int)temporaryScrumDetails.QuestionId, user.Id, message, ScrumAnswerStatus.Answered);
                                else
                                    return _stringConstant.AnswerNotRecorded;
                            }
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
        /// <param name="channelId">slack channel id</param>
        /// <param name="channelName">slack channel name from which the message has been send</param>
        /// <param name="slackUserName">slack user name of the interacting user</param>
        /// <param name="parameter">the keyword(second word) send by the user</param>      
        /// <param name="slackUserId">slack userId of the interacting user</param>
        /// <returns>The question or the status of the scrum</returns>
        private async Task<string> ScrumAsync(string channelId, string channelName, string slackUserName, string parameter, string slackUserId)
        {
            // getting user name from user's slack name
            ApplicationUser applicationUser = await _applicationUser.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
            // getting access token for that user
            if (applicationUser != null)
            {
                // get access token of user for promact oauth server
                string accessToken = await _attachmentRepository.UserAccessTokenAsync(applicationUser.UserName);
                List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(channelName, accessToken);
                if (users != null && users.Any())
                {
                    ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(channelName, accessToken);
                    DateTime today = DateTime.UtcNow.Date;
                    Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => string.Compare(x.SlackChannelId, channelId, true) == 0 && DbFunctions.TruncateTime(x.ScrumDate) == today);
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
                                return await StartScrumAsync(channelId, users, project, scrumStatus);
                            default:
                                return string.Empty;
                        }
                    }
                    else
                    {
                        //if user is in-active
                        string returnMessage = string.Empty;
                        switch (scrumStatus)
                        {
                            case ScrumStatus.Halted:
                                // as scrum is halted no need to mark that person as inactive.
                                if (scrumCommand == ScrumActions.resume)
                                    returnMessage = string.Format(_stringConstant.InActiveInOAuth, slackUserName) + _stringConstant.ScrumCannotBeResumed;
                                else
                                    returnMessage = _stringConstant.ScrumIsHalted + Environment.NewLine + string.Format(_stringConstant.InActiveInOAuth, slackUserName);
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
                }
                else
                    return _stringConstant.NoEmployeeFound;
            }
            else
                // if user doesn't exist or hasn't logged in with Promact OAuth then this message will be shown to user
                return _stringConstant.YouAreNotInExistInOAuthServer;
        }


        /// <summary>
        /// This method will be called when the keyword "leave @username" is received as reply from a channel member. - JJ
        /// </summary>
        /// <param name="channelId">slack channel id</param>
        /// <param name="channelName">slack channel name from which the message has been send</param>
        /// <param name="slackUserName">slack user name of the interacting user</param>
        /// <param name="slackUserId">slack user Id of the interacting user</param>
        /// <param name="applicant">slack user name of the user who is being marked on leave</param>
        /// <param name="applicantId">slack user id of the user who is being marked on leave</param>
        /// <returns>Question to the next person or other scrum status</returns>
        private async Task<string> LeaveAsync(string channelId, string channelName, string slackUserName, string slackUserId, string applicant, string applicantId)
        {
            string returnMsg = string.Empty;
            DateTime today = DateTime.UtcNow.Date;
            //we will have to check whether the scrum is on going or not before calling FetchScrumStatus()
            //because any command outside the scrum time must not be entertained except with the replies like "scrum is concluded","scrum has not started" or "scrum has not started".
            Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => string.Compare(x.SlackChannelId, channelId, true) == 0 && DbFunctions.TruncateTime(x.ScrumDate) == today);
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
                            List<User> users = await _oauthCallsRepository.GetUsersByChannelNameAsync(channelName, accessToken);
                            ProjectAc project = await _oauthCallsRepository.GetProjectDetailsAsync(channelName, accessToken);

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
        /// <param name="channelName">slack channel name from which message is send</param>
        /// <param name="channelId">slack channel id from which message is send</param>
        /// <param name="slackUserId">slack user id of interacting user</param>
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
        /// Used to check whether channelId is of a private channel. - JJ
        /// </summary>
        /// <param name="channelId">Id of the slack channel</param>
        /// <returns>true if private channel</returns>
        private bool IsPrivateChannel(string channelId)
        {
            if (channelId.StartsWith(_stringConstant.GroupNameStartsWith, StringComparison.Ordinal))
                return true;
            else
                return false;
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
        /// <param name="channelId">slack channel id</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="project">project(in OAuth) corresponding to slack channel</param>
        ///<param name="scrumStatus">status of scrum</param>
        /// <returns>The next question or the scrum complete message</returns>
        private async Task<string> StartScrumAsync(string channelId, List<User> users, ProjectAc project, ScrumStatus scrumStatus)
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
                    Scrum scrum = new Scrum();
                    scrum.CreatedOn = DateTime.UtcNow;
                    scrum.SlackChannelId = channelId;
                    scrum.ScrumDate = DateTime.UtcNow.Date;
                    scrum.ProjectId = project.Id;
                    scrum.TeamLeaderId = project.TeamLeaderId;
                    scrum.IsHalted = false;
                    scrum.IsOngoing = true;
                    _scrumRepository.Insert(scrum);
                    await _scrumRepository.SaveChangesAsync();

                    SlackUserDetailAc slackUser = await _slackUserDetails.GetByIdAsync(firstUser.SlackUserId);
                    //add the scrum details to the temporary table
                    await AddTemporaryScrumDetailsAsync(scrum.Id, firstUser.SlackUserId, 0, question.Id);
                    //first user is asked questions along with the previous day status (if any)
                    replyMessage = _stringConstant.GoodDay + string.Format(_stringConstant.NameFormatWithNewLine, slackUser.Name) + FetchPreviousDayStatus(firstUser.Id, project.Id, questionList) + question.QuestionStatement + Environment.NewLine;
                }
                else
                    //no active users are found
                    replyMessage = _stringConstant.NoEmployeeFound;
            }
            else if (scrumStatus == ScrumStatus.OnGoing)
            {
                DateTime today = DateTime.UtcNow.Date;
                Scrum scrum = await _scrumRepository.FirstOrDefaultAsync(x => string.Compare(x.SlackChannelId, channelId, true) == 0 && DbFunctions.TruncateTime(x.ScrumDate) == today);
                //user to whom the last question was asked
                SlackUserDetailAc prevUser = await GetSlackUserAsync(scrum.Id, users);
                if (prevUser != null && !string.IsNullOrEmpty(prevUser.Name))
                {
                    if (prevUser.Deleted)
                        //user is not part of the project in OAuth
                        replyMessage = string.Format(_stringConstant.UserNotInProject, prevUser.Name);

                    else if (!prevUser.IsActive)
                    {
                        //user to whom the last question was asked. but this user is in active now
                        User userDetail = users.FirstOrDefault(x => x.SlackUserId == prevUser.UserId);
                        List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id && x.EmployeeId == userDetail.Id).Result.ToList();
                        return string.Format(_stringConstant.InActiveInOAuth, prevUser.Name) + await MarkAsInActiveAsync(scrumAnswer, users, scrum.Id, questionList, project.Id, prevUser.UserId, true);
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
                        string expectedUserId = users.FirstOrDefault(x => x.SlackUserId == applicantId).Id;
                        //if applying user tries to mark himself/herself as on leave
                        if (string.Compare(slackUserId, applicantId, true) == 0)
                            return _stringConstant.LeaveError;
                        else
                        {
                            List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId && x.ScrumAnswerStatus == ScrumAnswerStatus.Answered).Result.ToList();
                            if (scrumAnswer.Any())
                                //fetch the scrum answer of the user given on that day
                                scrumAnswer = scrumAnswer.Where(x => x.EmployeeId == expectedUserId).ToList();
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
            string returnMsg = string.Empty;
            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();
            if (scrumAnswers.Any())
            {
                //scrum answers which were marked as in-active, are now to be answered
                List<ScrumAnswer> answerNow = scrumAnswers.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).OrderBy(x => x.Id).ToList();
                if (answerNow.Any())
                {
                    ScrumAnswer answer = answerNow.First();
                    User user = users.FirstOrDefault(x => x.Id == answer.EmployeeId);
                    //the user is not a member of the project now
                    if (user == null)
                    {
                        foreach (ScrumAnswer ans in answerNow)
                        {
                            _scrumAnswerRepository.Delete(ans.Id);
                            await _scrumAnswerRepository.SaveChangesAsync();
                        }
                        return _stringConstant.UserNotInOAuthOrProject + Environment.NewLine + await MarkScrumCompleteAsync(scrumId, users);
                    }
                    else if (!user.IsActive)
                        return await MarkAsInActiveAsync(scrumAnswers, users, scrumId, questions, projectId, user.SlackUserId, true);

                    else
                    {
                        SlackUserDetailAc slackUserDetails = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                        await UpdateTemporaryScrumDetailsAsync(slackUserDetails.UserId, scrumId, users, answer.QuestionId);
                        //the first question which is to be answered now is asked
                        return string.Format(_stringConstant.NameFormat, slackUserDetails.Name) + questions.FirstOrDefault(x => x.Id == answer.QuestionId).QuestionStatement + Environment.NewLine;
                    }
                }
                else
                    return await GetQuestionActiveUser(questions, users, scrumAnswers, scrumId, projectId);
            }
            else
            {
                //no scrum answer has been recorded yet. So first question to the first user
                TemporaryScrumDetails tempScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
                User user = users.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId && x.IsActive);
                if (user == null)
                    //if a user was asked a question before but at present is not active
                    user = users.FirstOrDefault(x => x.IsActive);

                SlackUserDetailAc slackUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                Question firstQuestion = questions.First();
                await UpdateTemporaryScrumDetailsAsync(user.SlackUserId, scrumId, users, firstQuestion.Id);
                returnMsg = _stringConstant.GoodDay + string.Format(_stringConstant.NameFormat, slackUser.Name) + FetchPreviousDayStatus(user.Id, projectId, questions) + firstQuestion.QuestionStatement + Environment.NewLine;
            }
            return returnMsg;
        }


        /// <summary>
        /// Fetch Questions for next user (there are no users who were marked as in-active before and are now active). - JJ
        /// </summary>
        /// <param name="questions">List of questions to be asked in scrum</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="scrumAnswers">Scrum answers of users for that day</param>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="projectId">Id of project(in OAuth) corresponding to slack channel</param>
        /// <returns>question statement or scrum status</returns>
        private async Task<string> GetQuestionActiveUser(List<Question> questions, List<User> users, List<ScrumAnswer> scrumAnswers, int scrumId, int projectId)
        {
            string returnMsg = string.Empty;
            int questionCount = questions.Count();
            TemporaryScrumDetails temporaryScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
            //user to whom the last question was asked
            User prevUser = users.FirstOrDefault(x => x.SlackUserId == temporaryScrumDetails.SlackUserId);
            User user = new User();
            //list of active users who have not answer yet                       
            List<User> activeUserList = users.Where(x => x.IsActive && !scrumAnswers.Select(y => y.EmployeeId).ToList().Contains(x.Id)).ToList();

            if (temporaryScrumDetails.AnswerCount == 0 || temporaryScrumDetails.AnswerCount == questionCount)
            {
                //all questions have been asked to the previous user      
                if (activeUserList != null && activeUserList.Any())
                {
                    user = activeUserList.First();
                    //now fetch the first question to the next user
                    if (prevUser != null)
                        user = activeUserList.FirstOrDefault(x => x.SlackUserId == prevUser.SlackUserId);

                    //temporaryScrumDetails.AnswerCount == questionCount - because if the previous user has answered all
                    //his questions then next user must be asked question
                    if (temporaryScrumDetails.AnswerCount == questionCount || user == null)
                        user = activeUserList.First();
                    //the returnMsg will be fetched in the end of this method.
                }
                else
                    //no active users are left to be asked questions to.
                    return await MarkScrumCompleteAsync(scrumId, users);
            }
            else
            {
                //as not all questions have been answered by the last user,the next question to that user will be asked
                if (prevUser != null)
                {
                    //last scrum answer of the given scrum id.
                    ScrumAnswer lastScrumAnswer = scrumAnswers.OrderByDescending(x => x.Id).First(x => x.EmployeeId == prevUser.Id);
                    if (prevUser.IsActive)
                    {
                        SlackUserDetailAc slackUser = await _slackUserDetails.GetByIdAsync(prevUser.SlackUserId);
                        Question question = await FetchQuestionAsync(lastScrumAnswer.QuestionId);
                        if (question != null)
                        {
                            await UpdateTemporaryScrumDetailsAsync(prevUser.SlackUserId, scrumId, users, question.Id);
                            return string.Format(_stringConstant.NameFormat, slackUser.Name) + question.QuestionStatement + Environment.NewLine;
                        }
                        else
                            return _stringConstant.NoQuestion;
                    }
                    else
                        return await MarkAsInActiveAsync(scrumAnswers, users, scrumId, questions, projectId, prevUser.SlackUserId, true);
                }
                else
                {
                    //the previous user is not part of the project in OAuth. So fetch question to the next active user.
                    if (activeUserList != null && activeUserList.Any())
                        user = activeUserList.First();

                    else
                        //no active users are left to be asked questions to.
                        return await MarkScrumCompleteAsync(scrumId, users);
                }
            }

            Question firstQuestion = questions.First();
            //update the temporary scrum details with the next id of the question to be asked
            await UpdateTemporaryScrumDetailsAsync(user.SlackUserId, scrumId, users, firstQuestion.Id);
            SlackUserDetailAc slackUserAc = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
            //as it is the first question to the user also fetch the previous day scrum status.
            returnMsg = _stringConstant.GoodDay + string.Format(_stringConstant.NameFormatWithNewLine, slackUserAc.Name) + FetchPreviousDayStatus(user.Id, projectId, questions) + firstQuestion.QuestionStatement + Environment.NewLine;

            return returnMsg;
        }


        /// <summary>
        /// Mark a user as inactive during the scrum and get the next question to the next user. -JJ
        /// </summary>
        /// <param name="scrumAnswers">Scrum answers of users for that day</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="questions">List of questions to be asked in scrum</param>
        /// <param name="projectId">Id of project(in OAuth) corresponding to slack channel</param>
        /// <param name="applicantId">slack user id</param>
        /// <param name="getQuestion">flag inbdicating whether to fetch the next question</param>
        /// <returns>reply that applicant is marked as in-active and the next question to the next user</returns>
        private async Task<string> MarkAsInActiveAsync(List<ScrumAnswer> scrumAnswers, List<User> users, int scrumId, List<Question> questions, int projectId, string applicantId, bool getQuestion)
        {
            string returnMsg = string.Empty;
            User user = users.FirstOrDefault(x => x.SlackUserId == applicantId);
            //scrum answer of the user
            scrumAnswers = scrumAnswers.Where(x => x.EmployeeId == user.Id).ToList();
            //id of questions which were not answered by the user
            List<int> questionIds = questions.Where(x => x.Type == BotQuestionType.Scrum && !scrumAnswers.Where(a => a.ScrumAnswerStatus == ScrumAnswerStatus.InActive || a.ScrumAnswerStatus == ScrumAnswerStatus.Answered).Select(y => y.QuestionId).ToList()
            .Contains(x.Id))
            .OrderBy(i => i.OrderNumber)
            .Select(z => z.Id).ToList();

            foreach (int questionId in questionIds)
            {
                //mark all the remaining answers of the user as inactive
                await AddUpdateAnswerAsync(scrumId, questionId, user.Id, _stringConstant.InActive, ScrumAnswerStatus.InActive);
            }

            TemporaryScrumDetails temporaryScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
            if (temporaryScrumDetails != null && temporaryScrumDetails.SlackUserId == applicantId)
                await UpdateTemporaryScrumDetailsAsync(applicantId, scrumId, users, null);

            if (getQuestion)
                //fetches the next question or status and returns
                return returnMsg + Environment.NewLine + await GetQuestionAsync(scrumId, questions, users, projectId);
            return returnMsg;
        }


        /// <summary>
        /// Used to check and mark scrum as completed. - JJ
        /// </summary>
        /// <param name="scrumId">id of scrum of the channel for that day</param>
        /// <param name="users">List of users of the project(in OAuth) corresponding to slack channel</param>
        /// <returns>reply to user</returns>
        /// <remarks>If scrum is completed then message saying that the scrum is complete 
        /// or if any active emplpoyee is pending to answer then that question</remarks>
        private async Task<string> MarkScrumCompleteAsync(int scrumId, List<User> users)
        {
            //list of scrum answers of users who were in active during scrum meeting
            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.Fetch(x => x.ScrumId == scrumId && x.ScrumAnswerStatus == ScrumAnswerStatus.InActive).OrderBy(x => x.Id).ToList();
            User user = new User();
            Question question = new Question();
            string nextQuestion = string.Empty;
            if (scrumAnswers.Any())
            {
                //Id of users who were in active during scrum meeting
                List<string> userIds = scrumAnswers.Select(x => x.EmployeeId).Distinct().ToList();

                foreach (string userId in userIds)
                {
                    user = users.FirstOrDefault(x => x.Id == userId);
                    //check whether those in active before are active now
                    if (user != null && !string.IsNullOrEmpty(user.Id) && user.IsActive)
                    {
                        scrumAnswers = scrumAnswers.Where(x => x.EmployeeId == userId).ToList();
                        foreach (ScrumAnswer scrumAns in scrumAnswers)
                        {
                            // the next question is fetched if it has already not been fetched
                            if (string.IsNullOrEmpty(nextQuestion))
                            {
                                question = await _questionRepository.FirstOrDefaultAsync(x => x.Id == scrumAns.QuestionId);
                                nextQuestion = question.QuestionStatement;
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
                    await UpdateTemporaryScrumDetailsAsync(user.SlackUserId, scrumId, users, question.Id);
                    return string.Format(_stringConstant.MarkedInActive, _slackUserDetails.GetByIdAsync(user.SlackUserId).Result.Name) + nextQuestion;
                }
            }

            //if no questions are pending then scrum is marked to be complete
            if (await UpdateScrumAsync(scrumId, false, false) == 1)
                //answers of all the users has been recorded            
                return _stringConstant.ScrumComplete;
            else
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
                //scrum answers of users who were marked as in active during their scrum but now are available for scrum.
                List<ScrumAnswer> scrumAnswers = scrumAnswer.Where(x => x.ScrumAnswerStatus == ScrumAnswerStatus.AnswerNow).ToList();

                if (scrumAnswers.Any())
                    user = users.FirstOrDefault(x => x.Id == scrumAnswers.First().EmployeeId);
                else
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
                            if (activeUserList != null && activeUserList.Any())
                            {
                                //now the next user
                                TemporaryScrumDetails tempScrumDetails = await FetchTemporaryScrumDetailsAsync(scrumId);
                                user = activeUserList.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId);
                                if (user == null)//the previous user is either in-active or not a member of the project in OAuth
                                    user = activeUserList.First();
                            }
                            else
                            {
                                string reply = string.Empty;
                                reply = await GetReplyToUserAsync(users, projectId, scrumId, applicantId, applicant, questions);

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
            string reply = string.Empty;
            List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();
            User unexpectedUser = users.FirstOrDefault(x => x.SlackUserId == applicantId);
            //the user to whom the last question was asked. This user must be called before MarkAsInActiveAsync() is called because if scrum is complete then temporary data is deleted and this user cannot be fetched.
            SlackUserDetailAc prevUser = await GetSlackUserAsync(scrumId, users);
            if (unexpectedUser != null && !unexpectedUser.IsActive)
                reply += string.Format(_stringConstant.InActiveInOAuth, applicant) + await MarkAsInActiveAsync(scrumAnswers, users, scrumId, questions, projectId, unexpectedUser.SlackUserId, true);

            bool isPreviousUserNull = prevUser != null && !string.IsNullOrEmpty(prevUser.Name) ? false : true;
            //if unexpectedUser is null it means that the user is not a member of the project in OAuth
            //in that case even the user who user who was asked the last question to(i.e prevUser) is same as this user, it is alright
            if (!isPreviousUserNull && (prevUser.UserId != applicantId || unexpectedUser == null))
            {
                //the user who user who was asked the last question to(i.e prevUser) is not a member of the project in OAuth
                if (prevUser.Deleted)
                {
                    if (string.IsNullOrEmpty(reply))
                        reply = await GetQuestionAsync(scrumId, questions, users, projectId);

                    reply = string.Format(_stringConstant.UserNotInProject, prevUser.Name) + reply;
                }
                else if (!prevUser.IsActive)
                    reply = string.Format(_stringConstant.InActiveInOAuth, prevUser.Name) + await MarkAsInActiveAsync(scrumAnswers, users, scrumId, questions, projectId, prevUser.UserId, string.IsNullOrEmpty(reply)) + reply;
            }

            //issue is : scrum starts and first question is asked to first user. Remove first and second users from project.Let second user write scrum halt. Third user is asked question. Second user writes scrum halt again.
            //when the unexpectedUser user is null(user is not a member of project or not in OAuth) and previous user is not the interacting user right now
            //or when both unexpectedUser user and previous users are null.
            if ((unexpectedUser == null && !isPreviousUserNull && prevUser.UserId != applicantId) || (unexpectedUser == null && isPreviousUserNull))
            {
                if (string.IsNullOrEmpty(reply))
                    reply = await GetQuestionAsync(scrumId, questions, users, projectId);

                reply = string.Format(_stringConstant.UserNotInProject, applicant) + reply;
            }

            return reply;
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
                    SlackUserDetailAc tempUser = await _slackUserDetails.GetByIdAsync(tempScrumDetails.SlackUserId);
                    User userDetail = users.FirstOrDefault(x => x.SlackUserId == tempScrumDetails.SlackUserId);
                    tempScrumDetails.SlackUserId = applicantId;

                    if (userDetail == null)
                        // User is either not a member of the project or not in OAuth
                        return string.Format(_stringConstant.UserNotInProject, tempUser.Name) + await GetQuestionAsync(scrumId, questions, users, projectId);
                    else if (userDetail != null && !userDetail.IsActive)
                    {
                        //the user to whom the last question was asked is in-active now
                        List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId && x.EmployeeId == userDetail.Id).Result.ToList();
                        return string.Format(_stringConstant.InActiveInOAuth, tempUser.Name) + await MarkAsInActiveAsync(scrumAnswer, users, scrumId, questions, projectId, tempUser.UserId, true);
                    }
                }
                return string.Empty;
            }
            else
            {
                //scrum answers of users in that day's scrum
                List<ScrumAnswer> scrumAnswers = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrumId).Result.ToList();
                if (user != null && !user.IsActive)
                {
                    //the expected user is marked as in-active in OAuth. So mark the answers as in active and fetch question to the next user
                    SlackUserDetailAc expectedUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                    return string.Format(_stringConstant.InActiveInOAuth, expectedUser.Name) + await MarkAsInActiveAsync(scrumAnswers, users, scrumId, questions, projectId, expectedUser.UserId, true);
                }
                else
                {
                    string reply = await GetReplyToUserAsync(users, projectId, scrumId, applicantId, applicant, questions);
                    if (user != null)
                    {
                        //expected user is active
                        SlackUserDetailAc expectedUser = await _slackUserDetails.GetByIdAsync(user.SlackUserId);
                        reply += string.Format(_stringConstant.PleaseAnswer, expectedUser.Name);
                    }
                    // no need of fetching question in else condition as it is done in GetReplyToUserAsync() itself
                    return reply;
                }
            }
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
            else if (status == (ScrumStatus.Halted))
                return _stringConstant.ScrumAlreadyHalted;
            else
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
            string returnMsg = string.Empty;
            //keyword encountered is "scrum resume"      
            if (status == (ScrumStatus.Halted) || status == (ScrumStatus.OnGoing))
            {
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
                if (prevUser != null && !string.IsNullOrEmpty(prevUser.Name))
                {
                    if (prevUser.Deleted)//the previous user is not part of the project in OAuth
                        returnMsg += string.Format(_stringConstant.UserNotInProject, prevUser.Name);

                    else if (!prevUser.IsActive && status == (ScrumStatus.Halted))
                    {
                        //user to whom the last question was asked. but this user is in active now
                        User userDetail = users.FirstOrDefault(x => x.SlackUserId == prevUser.UserId);
                        List<ScrumAnswer> scrumAnswer = _scrumAnswerRepository.FetchAsync(x => x.ScrumId == scrum.Id && x.EmployeeId == userDetail.Id).Result.ToList();
                        return _stringConstant.ScrumResumed + string.Format(_stringConstant.InActiveInOAuth, prevUser.Name)
                            + await MarkAsInActiveAsync(scrumAnswer, users, scrum.Id, questionList, scrum.ProjectId, prevUser.UserId, true);
                    }
                }
                //next question is fetched
                returnMsg += await GetQuestionAsync(scrum.Id, questionList, users, scrum.ProjectId);
                return returnMsg;
            }
            else
                return ReplyStatusofScrumToClient(status) + _stringConstant.ScrumCannotBeResumed;
        }


        /// <summary>
        /// Select the appropriate reply to the client - JJ
        /// </summary>
        /// <param name="scrumStatus">Status of the scrum</param>
        /// <returns>appropriate message indicating the status of scrum</returns>
        private string ReplyStatusofScrumToClient(ScrumStatus scrumStatus)
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