using AutoMapper;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.MailSettingRepository
{
    public class MailSettingRepository : IMailSettingRepository
    {
        #region Private Variables
        private readonly IRepository<MailSetting> _mailSettingDataRepository;
        private readonly IOauthCallHttpContextRespository _oauthCallRepository;
        private readonly IRepository<MailSettingMapping> _mailSettingMappingDataRepository;
        private readonly IRepository<Group> _groupDataRepository;
        private IMapper _mapper;
        #endregion

        #region Constructor
        public MailSettingRepository(IRepository<MailSetting> mailSettingDataRepository, IOauthCallHttpContextRespository oauthCallRepository,
            IRepository<MailSettingMapping> mailSettingMappingDataRepository, IRepository<Group> groupDataRepository,
            IMapper mapper)
        {
            _mailSettingDataRepository = mailSettingDataRepository;
            _oauthCallRepository = oauthCallRepository;
            _mailSettingMappingDataRepository = mailSettingMappingDataRepository;
            _groupDataRepository = groupDataRepository;
            _mapper = mapper;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to get list of project from oauth server
        /// </summary>
        /// <returns>list of project</returns>
        public async Task<List<ProjectAc>> GetAllProjectAsync()
        {
            var projects = (await _oauthCallRepository.GetAllProjectsAsync()).FindAll(x => x.IsActive == true);
            return (projects);
        }

        /// <summary>
        /// Method to get mail setting configuration from projectId
        /// </summary>
        /// <param name="projectId">project Id</param>
        /// <param name="module">name of module{leave, task or scrum}</param>
        /// <returns>mail setting configuration details</returns>
        public async Task<MailSettingAC> GetMailSettingDetailsByProjectIdAsync(int projectId, string module)
        {
            MailSettingAC mailSetting = new MailSettingAC();
            var mailSettingDetails = await _mailSettingDataRepository.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Module == module);
            if (mailSettingDetails != null)
            {
                List<string> listOfTo = new List<string>();
                List<string> listOfCC = new List<string>();
                mailSetting = _mapper.Map<MailSetting, MailSettingAC>(mailSettingDetails);
                var mailSettingMappingList = (await _mailSettingMappingDataRepository.FetchAsync(x => x.MailSettingId == mailSettingDetails.Id)).ToList();
                var listOfMailSettingTo = mailSettingMappingList.FindAll(x => x.IsTo);
                var listOfMailSettingCC = mailSettingMappingList.FindAll(x => !x.IsTo);
                foreach (var to in listOfMailSettingTo)
                {
                    if (to.GroupId == null)
                        listOfTo.Add(to.Email);
                    else
                        listOfTo.Add((await _groupDataRepository.FirstOrDefaultAsync(x => x.Id == to.GroupId)).Name);
                }
                foreach (var cc in listOfMailSettingCC)
                {
                    if (cc.GroupId == null)
                        listOfCC.Add(cc.Email);
                    else
                        listOfCC.Add((await _groupDataRepository.FirstOrDefaultAsync(x => x.Id == cc.GroupId)).Name);
                }
                mailSetting.To = listOfTo;
                mailSetting.CC = listOfCC;
            }
            return mailSetting;
        }

        /// <summary>
        /// Method to add mail setting configuration
        /// </summary>
        /// <param name="mailSettingAC">mail setting details</param>
        public async Task AddMailSettingAsync(MailSettingAC mailSettingAC)
        {
            var mailSetting = _mapper.Map<MailSettingAC, MailSetting>(mailSettingAC);
            mailSetting.CreatedOn = DateTime.UtcNow;
            _mailSettingDataRepository.Insert(mailSetting);
            await _mailSettingDataRepository.SaveChangesAsync();
            foreach (var to in mailSettingAC.To)
            {
                var mailSettingMapping = await MailSettingMappingGeneratorAsync(to, true, mailSetting.Id);
                mailSettingMapping.CreatedOn = DateTime.UtcNow;
                _mailSettingMappingDataRepository.Insert(mailSettingMapping);
            }
            foreach (var cc in mailSettingAC.CC)
            {
                var mailSettingMapping = await MailSettingMappingGeneratorAsync(cc, false, mailSetting.Id);
                mailSettingMapping.CreatedOn = DateTime.UtcNow;
                _mailSettingMappingDataRepository.Insert(mailSettingMapping);
            }
            await _mailSettingMappingDataRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Method to get list of groups from group table
        /// </summary>
        /// <returns>list of group</returns>
        public async Task<List<string>> GetListOfGroupsNameAsync()
        {
            List<string> listOfGroupNames = new List<string>();
            return (await _groupDataRepository.GetAll().ToListAsync()).Select(x=>x.Name).ToList();
        }

        /// <summary>
        /// Method to update mail setting configuration
        /// </summary>
        /// <param name="mailSettingAC">mail setting details</param>
        public async Task UpdateMailSettingAsync(MailSettingAC mailSettingAC)
        {
            var previousMailSetting = await _mailSettingDataRepository.FirstOrDefaultAsync(x => x.Id == mailSettingAC.Id);
            var previousMailSettingMappingCreatedDateTime = (await _mailSettingMappingDataRepository.FirstOrDefaultAsync(x => x.MailSettingId == mailSettingAC.Id)).CreatedOn;
            previousMailSetting.SendMail = mailSettingAC.SendMail;
            previousMailSetting.UpdatedDate = DateTime.UtcNow;
            _mailSettingDataRepository.Update(previousMailSetting);
            await _mailSettingDataRepository.SaveChangesAsync();
            if (_mailSettingMappingDataRepository.Any(x => x.MailSettingId == mailSettingAC.Id))
                _mailSettingMappingDataRepository.RemoveRange(x => x.MailSettingId == mailSettingAC.Id);
            foreach (var to in mailSettingAC.To)
            {
                var mailSettingMapping = await MailSettingMappingGeneratorAsync(to, true, mailSettingAC.Id);
                mailSettingMapping.CreatedOn = previousMailSettingMappingCreatedDateTime;
                _mailSettingMappingDataRepository.Insert(mailSettingMapping);
            }
            foreach (var cc in mailSettingAC.CC)
            {
                var mailSettingMapping = await MailSettingMappingGeneratorAsync(cc, false, mailSettingAC.Id);
                mailSettingMapping.CreatedOn = previousMailSettingMappingCreatedDateTime;
                _mailSettingMappingDataRepository.Insert(mailSettingMapping);
            }
            await _mailSettingMappingDataRepository.SaveChangesAsync();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Method to get mailsetting mapping object from type, isTo and mailsetting Id
        /// </summary>
        /// <param name="type">string name - can be email Id or group name</param>
        /// <param name="isTo">true, if type is To else false for CC</param>
        /// <param name="mailSettingId">mail setting Id</param>
        /// <returns>mail setting mapping object</returns>
        private async Task<MailSettingMapping> MailSettingMappingGeneratorAsync(string type, bool isTo, int mailSettingId)
        {
            MailSettingMapping mailSettingMapping = new MailSettingMapping();
            mailSettingMapping.MailSettingId = mailSettingId;
            mailSettingMapping.IsTo = isTo;
            if (_groupDataRepository.Any(x => x.Name == type))
                mailSettingMapping.GroupId = (await _groupDataRepository.FirstOrDefaultAsync(x => x.Name == type)).Id;
            else
                mailSettingMapping.Email = type;
            return mailSettingMapping;
        }
        #endregion
    }
}
