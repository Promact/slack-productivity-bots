using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.MailSettingDetailsByProjectAndModule
{
    public class MailSettingDetailsByProjectAndModuleRepository : IMailSettingDetailsByProjectAndModuleRepository
    {
        #region Private Variables
        private readonly IRepository<MailSetting> _mailSettingDataRepository;
        private readonly IRepository<MailSettingMapping> _mailSettingMappingDataRepository;
        private readonly IRepository<GroupEmailMapping> _groupEmailMappingDataRepository;
        private readonly IRepository<Group> _groupDataRepository;
        private readonly IOauthCallsRepository _oauthCallRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ApplicationUserManager _userManager;
        #endregion

        #region Constructor
        public MailSettingDetailsByProjectAndModuleRepository(IRepository<MailSetting> mailSettingDataRepository,
            IRepository<MailSettingMapping> mailSettingMappingDataRepository, IRepository<GroupEmailMapping> groupEmailMappingDataRepository,
            IRepository<Group> groupDataRepository, IOauthCallsRepository oauthCallRepository, IAttachmentRepository attachmentRepository,
            ApplicationUserManager userManager)
        {
            _mailSettingDataRepository = mailSettingDataRepository;
            _mailSettingMappingDataRepository = mailSettingMappingDataRepository;
            _groupEmailMappingDataRepository = groupEmailMappingDataRepository;
            _groupDataRepository = groupDataRepository;
            _oauthCallRepository = oauthCallRepository;
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Method used to get mail setting for a module by projectId
        /// </summary>
        /// <param name="projectId">project Id</param>
        /// <param name="module">mail setting module</param>
        /// <param name="userId">user's user Id</param>
        /// <returns>mail setting details</returns>
        public async Task<MailSettingAC> GetMailSettingAsync(int projectId, string module, string userId)
        {
            MailSettingAC mailSetting = new MailSettingAC();
            mailSetting.To = new List<string>();
            mailSetting.CC = new List<string>();
            if (_mailSettingDataRepository.Any(x => x.ProjectId == projectId && x.Module == module))
            {
                var mailSettingDetails = await _mailSettingDataRepository.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.SendMail && x.Module == module);
                if (mailSettingDetails != null)
                {
                    mailSetting.Id = mailSettingDetails.Id;
                    mailSetting.Module = mailSettingDetails.Module;
                    mailSetting.ProjectId = mailSettingDetails.ProjectId;
                    mailSetting.SendMail = mailSettingDetails.SendMail;
                    mailSetting.To = await GetListOfEmailByMailSettingAsync(true, mailSettingDetails.Id, userId, projectId);
                    mailSetting.CC = await GetListOfEmailByMailSettingAsync(false, mailSettingDetails.Id, userId, projectId);
                }
            }
            return mailSetting;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Method to get list of emails for a mail setting
        /// </summary>
        /// <param name="isTo">is To or CC</param>
        /// <param name="mailSettingId">mail setting Id</param>
        /// <param name="userId">user's user Id</param>
        /// <param name="projectId">project Id</param>
        /// <returns></returns>
        private async Task<List<string>> GetListOfEmailByMailSettingAsync(bool isTo, int mailSettingId, string userId, int projectId)
        {
            List<string> emails = new List<string>();
            var mailSettingMappings = (await _mailSettingMappingDataRepository.FetchAsync(x => x.MailSettingId == mailSettingId && x.IsTo == isTo)).ToList();
            if (mailSettingMappings.Any())
            {
                foreach (var mailSetingMapping in mailSettingMappings)
                {
                    if (!string.IsNullOrEmpty(mailSetingMapping.Email))
                        emails.Add(mailSetingMapping.Email);
                    else
                    {
                        var groupName = (await _groupDataRepository.FirstAsync(x => x.Id == mailSetingMapping.GroupId)).Name;
                        var accessToken = await _attachmentRepository.UserAccessTokenAsync((await _userManager.FindByIdAsync(userId)).UserName);
                        switch (groupName)
                        {
                            case "Team Leader":
                                {
                                    emails.AddRange((await _oauthCallRepository.GetTeamLeaderUserIdAsync(userId, accessToken)).Select(x => x.Email));
                                }
                                break;
                            case "Management":
                                {
                                    emails.AddRange((await _groupEmailMappingDataRepository.FetchAsync(x => x.GroupId == mailSetingMapping.GroupId)).Select(x => x.Email));
                                }
                                break;
                            case "Team Members":
                                {
                                    emails.AddRange((await _oauthCallRepository.GetAllTeamMemberByProjectId(projectId, accessToken)).Select(x => x.Email));
                                }
                                break;
                            default:
                                {
                                    emails.AddRange((await _groupEmailMappingDataRepository.FetchAsync(x => x.GroupId == mailSetingMapping.GroupId)).Select(x => x.Email));
                                }
                                break;
                        }
                    }
                }
            }
            return emails;
        }
        #endregion
    }
}
