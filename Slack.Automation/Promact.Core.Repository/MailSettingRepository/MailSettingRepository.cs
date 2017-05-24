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
            // returns list of project which are active
            return ((await _oauthCallRepository.GetAllProjectsAsync()).FindAll(x => x.IsActive));
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
            // check mail setting for project and module is exist or not
            var mailSettingDetails = await _mailSettingDataRepository.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Module == module);
            if (mailSettingDetails != null)
            {
                mailSetting = _mapper.Map<MailSetting, MailSettingAC>(mailSettingDetails);
                // get list of To
                mailSetting.To = await GetListOfEmailByMailSettingAsync(true, mailSetting.Id);
                // get list of CC
                mailSetting.CC = await GetListOfEmailByMailSettingAsync(false, mailSetting.Id);
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
            // add list of To
            await AddMailSettingMappingAsync(mailSettingAC.To, true, mailSetting.Id, DateTime.UtcNow);
            // add list of CC
            if (mailSettingAC.CC != null)
                await AddMailSettingMappingAsync(mailSettingAC.CC, false, mailSetting.Id, DateTime.UtcNow);
            await _mailSettingMappingDataRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Method to get list of groups from group table
        /// </summary>
        /// <returns>list of group</returns>
        public async Task<List<string>> GetListOfGroupsNameAsync()
        {
            // return list of group name
            return (await _groupDataRepository.GetAll().ToListAsync()).Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Method to update mail setting configuration
        /// </summary>
        /// <param name="mailSettingAC">mail setting details</param>
        public async Task UpdateMailSettingAsync(MailSettingAC mailSettingAC)
        {
            // mail setting details to be updated
            var previousMailSetting = await _mailSettingDataRepository.FirstAsync(x => x.Id == mailSettingAC.Id);
            // mail setting mapping created date, to be updated
            var previousMailSettingMappingCreatedDateTime = (await _mailSettingMappingDataRepository.FirstAsync(x => x.MailSettingId == mailSettingAC.Id)).CreatedOn;
            previousMailSetting.SendMail = mailSettingAC.SendMail;
            previousMailSetting.UpdatedDate = DateTime.UtcNow;
            _mailSettingDataRepository.Update(previousMailSetting);
            await _mailSettingDataRepository.SaveChangesAsync();
            // removed all previous mail setting mapping
            _mailSettingMappingDataRepository.RemoveRange(x => x.MailSettingId == mailSettingAC.Id);
            // add list of To
            await AddMailSettingMappingAsync(mailSettingAC.To, true, previousMailSetting.Id, previousMailSettingMappingCreatedDateTime);
            // add list of CC
            if (mailSettingAC.CC != null)
                await AddMailSettingMappingAsync(mailSettingAC.CC, false, previousMailSetting.Id, previousMailSettingMappingCreatedDateTime);
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
            // check whether mail setting for To or CC is group type or not
            if (_groupDataRepository.Any(x => x.Name == type))
                mailSettingMapping.GroupId = (await _groupDataRepository.FirstAsync(x => x.Name == type)).Id;
            else
                mailSettingMapping.Email = type;
            return mailSettingMapping;
        }

        /// <summary>
        /// Method to insert list of mail setting mapping
        /// </summary>
        /// <param name="listOfMailSettingEmail">list of email</param>
        /// <param name="isTo">boolean value is To</param>
        /// <param name="mailSettingId">mail setting Id</param>
        /// <param name="createdOn">created on date</param>
        /// <returns></returns>
        private async Task AddMailSettingMappingAsync(List<string> listOfMailSettingEmail, bool isTo, int mailSettingId, DateTime createdOn)
        {
            foreach (var email in listOfMailSettingEmail)
            {
                var mailSettingMapping = await MailSettingMappingGeneratorAsync(email, isTo, mailSettingId);
                mailSettingMapping.CreatedOn = createdOn;
                _mailSettingMappingDataRepository.Insert(mailSettingMapping);
            }
        }

        /// <summary>
        /// Method to get list of email by mail setting 
        /// </summary>
        /// <param name="isTo">boolean value of isTo</param>
        /// <param name="mailSettingId">mail setting Id</param>
        /// <returns></returns>
        private async Task<List<string>> GetListOfEmailByMailSettingAsync(bool isTo, int mailSettingId)
        {
            List<string> listOfEmail = new List<string>();
            var listOfMailSetting = (await _mailSettingMappingDataRepository.FetchAsync(x => x.MailSettingId == mailSettingId)).ToList();
            listOfMailSetting = listOfMailSetting.FindAll(x => x.IsTo == isTo);
            foreach (var to in listOfMailSetting)
            {
                // check if group Id is null or not, if not null then its group details else email address
                if (to.GroupId == null)
                    listOfEmail.Add(to.Email);
                else
                    listOfEmail.Add((await _groupDataRepository.FirstAsync(x => x.Id == to.GroupId)).Name);
            }
            return listOfEmail;
        }
        #endregion
    }
}