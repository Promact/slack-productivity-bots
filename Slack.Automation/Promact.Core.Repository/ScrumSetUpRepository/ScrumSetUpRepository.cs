using NLog;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.BaseRepository;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringLiteral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumSetUpRepository
{
    public class ScrumSetUpRepository : RepositoryBase, IScrumSetUpRepository
    {

        #region Private Variable 


        private readonly IRepository<ApplicationUser> _applicationUser;
        private readonly ISlackChannelRepository _slackChannelRepository;
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly AppStringLiteral _stringConstant;
        private readonly ILogger _logger;


        #endregion


        #region Constructor


        public ScrumSetUpRepository(
                      ISlackChannelRepository slackChannelRepository,
            IOauthCallsRepository oauthCallsRepository,
            ISingletonStringLiteral stringConstant,
            IRepository<ApplicationUser> applicationUser,
            IAttachmentRepository attachmentRepository)
            : base(applicationUser, attachmentRepository)
        {
            _slackChannelRepository = slackChannelRepository;
            _oauthCallsRepository = oauthCallsRepository;
            _stringConstant = stringConstant.StringConstant;
            _applicationUser = applicationUser;
            _logger = LogManager.GetLogger("ScrumBotModule");
        }


        #endregion


        #region Public Method


        /// <summary>
        /// It is called when the message is "list links","link "project"" or "unlink "project"" - JJ
        /// </summary>
        /// <param name="slackUserId">UserId of slack user</param>
        /// <param name="slackChannel">slack channel from which message is send</param>
        /// <param name="message">message from slack</param>
        /// <returns>appropriate message</returns>
        public async Task<string> ProcessSetUpMessagesAsync(string slackUserId, SlackChannelDetails slackChannel, string message)
        {
            if (String.Compare(message, _stringConstant.ListLinks, StringComparison.OrdinalIgnoreCase) == 0)
                return await ListLinkAsync(slackUserId);

            else
            {
                string[] messageArray = message.Split(null);
                int messageLength = message.Length - 1;
                int first = message.IndexOf('"') + 1; //first index of ".
                int last = message.IndexOf('"', message.IndexOf('"') + 1);//last index of "
                int projectNameStartIndex = messageArray[0].Length + 2;// index from where the name of the project starts

                if (messageLength == last && first == projectNameStartIndex)
                {
                    //fetch the project name from the message string
                    string name = message.Substring(first, last - first);
                    if (string.IsNullOrEmpty(name))// ex. link ""
                        return null;// it will be considered as a normal message
                    return await ProcessCommandsAsync(slackUserId, slackChannel.ChannelId, name, messageArray[0]);
                }
                else
                {
                    if (slackChannel.ProjectId != null)
                        return null;
                    return _stringConstant.ProjectChannelNotLinked;
                }
            }
        }


        /// <summary>
        /// Used to add channel manually by command "add channel channelname". - JJ
        /// </summary>
        /// <param name="slackChannelName">slack channel name from which message is send</param>
        /// <param name="slackChannelId">slack channel id from which message is send</param>
        /// <param name="slackUserId">slack user id of interacting user</param>
        /// <returns>status message</returns>
        public async Task<string> AddChannelManuallyAsync(string slackChannelName, string slackChannelId, string slackUserId)
        {
            //Checks whether channelId starts with "G" or "C". This is done inorder to make sure that only gruops or channels are added manually
            if (IsPrivateChannel(slackChannelId))
            {
                var accessToken = await GetAccessToken(slackUserId);
                if (accessToken != null)
                {
                    SlackChannelDetails slackChannelDetails = new SlackChannelDetails();
                    slackChannelDetails.ChannelId = slackChannelId;
                    slackChannelDetails.CreatedOn = DateTime.UtcNow;
                    slackChannelDetails.Deleted = false;
                    slackChannelDetails.Name = slackChannelName;
                    await _slackChannelRepository.AddSlackChannelAsync(slackChannelDetails);
                    return _stringConstant.ChannelAddSuccess;
                }
                else
                    // if user doesn't exist then this message will be shown to user
                    return _stringConstant.YouAreNotInExistInOAuthServer;
            }
            else
                return _stringConstant.OnlyPrivateChannel;
        }


        #endregion


        #region Private Method


        /// <summary>
        /// Used to check whether channelId is of a private channel. - JJ
        /// </summary>
        /// <param name="slackChannelId">Id of the slack channel</param>
        /// <returns>true if private channel</returns>
        private bool IsPrivateChannel(string slackChannelId)
        {
            return (slackChannelId.StartsWith(_stringConstant.GroupNameStartsWith, StringComparison.Ordinal) || slackChannelId.StartsWith("C", StringComparison.Ordinal)) ? true : false;
        }


        /// <summary>
        /// Check various conditions before linking or unlinking slack channels to OAuth projects- JJ
        /// </summary>
        /// <param name="slackUserId">UserId of slack user</param>
        /// <param name="slackChannelId">slack channel id from which message is send</param>
        /// <param name="givenProjectName">the project name mentioned in the slack message</param>
        /// <param name="command">link or unlink</param>
        /// <returns>appropriate message</returns>
        private async Task<string> ProcessCommandsAsync(string slackUserId, string slackChannelId, string givenProjectName, string command)
        {
            //Checks whether channelId starts with "G" or "C". This is done inorder to make sure that only gruops or channels are added manually
            if (IsPrivateChannel(slackChannelId))
            {
                var accessToken = await GetAccessToken(slackUserId);
                if (accessToken != null)
                {
                    ApplicationUser appUser = await _applicationUser.FirstAsync(x => x.SlackUserId == slackUserId);
                    //the project which is mentioned by user
                    ProjectAc project = (await _oauthCallsRepository.GetListOfProjectsEnrollmentOfUserByUserIdAsync(appUser.Id, accessToken)).FirstOrDefault(x => String.Compare(givenProjectName, x.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    if (project != null)
                    {
                        //check whether user is the team leader of the project
                        if (project.TeamLeader != null && project.TeamLeaderId == appUser.Id)
                        {
                            if (project.TeamLeader.IsActive)
                            {
                                //command to link
                                if (String.Compare(command, _stringConstant.Link, StringComparison.OrdinalIgnoreCase) == 0)
                                    return await LinkAsync(slackChannelId, givenProjectName, project);

                                //command to unlink
                                else if (String.Compare(command, _stringConstant.Unlink, StringComparison.OrdinalIgnoreCase) == 0)
                                    return await UnlinkAsync(slackChannelId, givenProjectName, project);
                                return string.Empty;

                            }
                            else
                                return string.Format(_stringConstant.NotActiveUser, slackUserId);

                        }
                        else
                            return string.Format(_stringConstant.NotTeamLeader, slackUserId);

                    }
                    else
                        return string.Format(_stringConstant.NotTeamLeaderOfProject, givenProjectName, slackUserId);
                }
                else
                    // if user doesn't exist then this message will be shown to user
                    return _stringConstant.YouAreNotInExistInOAuthServer;
            }
            else
                return _stringConstant.OnlyPrivateChannel;
        }


        /// <summary>
        /// Used to link Slack Channel to OAuth Project - JJ
        /// </summary>
        /// <param name="slackChannelId">slack channel id from which message is send</param>
        /// <param name="givenProjectName">the project name mentioned in the slack message</param>
        /// <param name="project">OAuth Project of the given name</param>
        /// <returns>status message</returns>
        private async Task<string> LinkAsync(string slackChannelId, string givenProjectName, ProjectAc project)
        {
            if (project.IsActive)
            {
                SlackChannelDetails alreadyLinkedChannel = await _slackChannelRepository.FetchChannelByProjectIdAsync(project.Id);
                if (alreadyLinkedChannel == null)
                {
                    //add the project id to the slack channel
                    SlackChannelDetails slackChannel = await _slackChannelRepository.GetByIdAsync(slackChannelId);
                    if (slackChannel.ProjectId == null)
                    {
                        _logger.Debug("\nLinkAsync method : not linked to any project yet\n");
                        slackChannel.ProjectId = project.Id;
                        await _slackChannelRepository.UpdateSlackChannelAsync(slackChannel);

                        return string.Format(_stringConstant.ProjectLinked, givenProjectName, slackChannel.Name);
                    }
                    else
                        return string.Format(_stringConstant.UnLinkFirst, givenProjectName);
                }
                else
                    return _stringConstant.AlreadyLinked;
            }
            else
                return _stringConstant.InActiveProject;
        }


        /// <summary>
        /// Used to unlink Slack Channel from OAuth Project - JJ
        /// </summary>
        /// <param name="slackChannelId">slack channel id from which message is send</param>
        /// <param name="givenProjectName">the project name mentioned in the slack message</param>
        /// <param name="project">OAuth Project of the given name</param>
        /// <returns>status message</returns>
        private async Task<string> UnlinkAsync(string slackChannelId, string givenProjectName, ProjectAc project)
        {
            SlackChannelDetails slackChannel = await _slackChannelRepository.GetByIdAsync(slackChannelId);
            if (slackChannel.ProjectId == null)
                return string.Format(_stringConstant.NotLinkedYet, givenProjectName);
            else
            {
                if (slackChannel.ProjectId == project.Id)
                {
                    _logger.Debug("\nUnLinkAsync method : matching project found\n");
                    slackChannel.ProjectId = null;
                    await _slackChannelRepository.UpdateSlackChannelAsync(slackChannel);

                    return string.Format(_stringConstant.UnlinkedSuccessfully, givenProjectName, slackChannel.Name);
                }
                else
                    return string.Format(_stringConstant.NotLinkedToChannel, givenProjectName);
            }
        }


        /// <summary>
        /// Used to fetch list of Active OAuth projects in which the user is team leader and their linked slack channels - JJ
        /// </summary>
        /// <param name="slackUserId">UserId of slack user</param>
        /// <returns>List of Active OAuth projects in which the user is team leader and their linked slack channels</returns>
        private async Task<string> ListLinkAsync(string slackUserId)
        {
            string reply = string.Empty;
            var accessToken = await GetAccessToken(slackUserId);
            if (accessToken != null)
            {
                ApplicationUser appUser = await _applicationUser.FirstAsync(x => x.SlackUserId == slackUserId);
                List<ProjectAc> projectList = await _oauthCallsRepository.GetListOfProjectsEnrollmentOfUserByUserIdAsync(appUser.Id, accessToken);
                IEnumerable<SlackChannelDetails> slackChannelList = await _slackChannelRepository.FetchChannelAsync();
             
                projectList.Where(y => y.IsActive && y.TeamLeader != null && y.TeamLeader.IsActive && y.TeamLeaderId == appUser.Id)
                    .Select(proj => new
                    {
                        SlackChannel = slackChannelList.FirstOrDefault(x => x.ProjectId == proj.Id),
                        ProjectName = proj.Name
                    }).ToList().ForEach(x =>
                    {
                        reply += Environment.NewLine + "*" + x.ProjectName + (x.SlackChannel != null ? "* - `" + x.SlackChannel.Name + "`" : "* - none") + Environment.NewLine;
                    });

                _logger.Debug("\nproject channel list :" + reply);

                if (string.IsNullOrEmpty(reply))
                    reply = _stringConstant.NoLinks;

                else
                    reply = _stringConstant.Links + Environment.NewLine + reply;

            }
            else
                // if user doesn't exist then this message will be shown to user
                reply = _stringConstant.YouAreNotInExistInOAuthServer;
            return reply;
        }


        #endregion
    }
}