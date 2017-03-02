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
        #endregion

        #region Constructor
        public MailSettingDetailsByProjectAndModuleRepository(IRepository<MailSetting> mailSettingDataRepository,
            IRepository<MailSettingMapping> mailSettingMappingDataRepository, IRepository<GroupEmailMapping> groupEmailMappingDataRepository)
        {
            _mailSettingDataRepository = mailSettingDataRepository;
            _mailSettingMappingDataRepository = mailSettingMappingDataRepository;
            _groupEmailMappingDataRepository = groupEmailMappingDataRepository;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method used to get mail setting for a module by projectId
        /// </summary>
        /// <param name="projectId">project Id</param>
        /// <param name="module">mail setting module</param>
        /// <returns>mail setting details</returns>
        public async Task<MailSettingAC> GetMailSettingAsync(int projectId, string module)
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
                    var mailSettingMappings = (await _mailSettingMappingDataRepository.FetchAsync(x => x.MailSettingId == mailSettingDetails.Id)).ToList();
                    if (mailSettingMappings.Any())
                    {
                        foreach (var mailSettingMapping in mailSettingMappings)
                        {
                            if (mailSettingMapping.IsTo)
                            {
                                if (string.IsNullOrEmpty(mailSettingMapping.Email))
                                    mailSetting.To.AddRange((await _groupEmailMappingDataRepository.FetchAsync(x => x.GroupId == mailSettingMapping.GroupId)).Select(x=>x.Email));
                                else
                                    mailSetting.To.Add(mailSettingMapping.Email);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(mailSettingMapping.Email))
                                    mailSetting.CC.AddRange((await _groupEmailMappingDataRepository.FetchAsync(x => x.GroupId == mailSettingMapping.GroupId)).Select(x=>x.Email));
                                else
                                    mailSetting.CC.Add(mailSettingMapping.Email);
                            }
                        }
                    }
                }
            }
            return mailSetting;
        }
        #endregion
    }
}
