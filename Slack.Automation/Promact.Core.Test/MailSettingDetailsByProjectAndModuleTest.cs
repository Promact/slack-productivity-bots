using Autofac;
using Promact.Core.Repository.MailSettingDetailsByProjectAndModule;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class MailSettingDetailsByProjectAndModuleTest
    {
        #region Private Variables
        private readonly IComponentContext _componentContext;
        private readonly IMailSettingDetailsByProjectAndModuleRepository _mailSettingDetailsRepository;
        private readonly IRepository<MailSetting> _mailSettingDataRepository;
        private readonly IRepository<MailSettingMapping> _mailSettingMappingDataRepository;
        private readonly IRepository<Group> _groupDataRepository;
        private readonly IRepository<GroupEmailMapping> _groupEmailMappingDataRepository;
        private readonly IStringConstantRepository _stringConstant;
        MailSetting mailSetting = new MailSetting();
        MailSettingMapping mailSettingMapping = new MailSettingMapping();
        Group group = new Group();
        GroupEmailMapping groupEmailMapping = new GroupEmailMapping();
        #endregion

        #region Constructor
        public MailSettingDetailsByProjectAndModuleTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _mailSettingDataRepository = _componentContext.Resolve<IRepository<MailSetting>>();
            _mailSettingMappingDataRepository = _componentContext.Resolve<IRepository<MailSettingMapping>>();
            _groupDataRepository = _componentContext.Resolve<IRepository<Group>>();
            _groupEmailMappingDataRepository = _componentContext.Resolve<IRepository<GroupEmailMapping>>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            _mailSettingDetailsRepository = _componentContext.Resolve<IMailSettingDetailsByProjectAndModuleRepository>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Test for isTo true with static email Id
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingEmailIsToAsync()
        {
            await AddGroup();
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSetting(1, _stringConstant.TaskModule);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test for isTo true with group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingForGroupIsToAsync()
        {
            await AddGroup();
            mailSettingMapping.Email = null;
            mailSettingMapping.GroupId = group.Id;
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSetting(1, _stringConstant.TaskModule);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test for isTo true with static email Id
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingEmailIsToFalseAsync()
        {
            await AddGroup();
            mailSettingMapping.IsTo = false;
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSetting(1, _stringConstant.TaskModule);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test for isTo true with group
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async Task GetMailSettingForGroupIsToFalseAsync()
        {
            await AddGroup();
            mailSettingMapping.IsTo = false;
            mailSettingMapping.Email = null;
            mailSettingMapping.GroupId = group.Id;
            await AddMailSetting();
            var result = await _mailSettingDetailsRepository.GetMailSetting(1, _stringConstant.TaskModule);
            Assert.True(result.SendMail);
        }

        /// <summary>
        /// Test cases to check the method DeleteTheDuplicateString
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void DeleteTheDuplicateString()
        {
            List<string> listOfString = new List<string>();
            listOfString.Add(_stringConstant.Email);
            listOfString.Add(_stringConstant.Email);
            listOfString.Add(_stringConstant.Admin);
            var result = _mailSettingDetailsRepository.DeleteTheDuplicateString(listOfString);
            Assert.Equal(result.Count, 2);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialization
        /// </summary>
        private void Initialize()
        {
            mailSetting.CreatedOn = DateTime.UtcNow;
            mailSetting.Module = _stringConstant.TaskModule;
            mailSetting.ProjectId = 1;
            mailSetting.SendMail = true;
            mailSettingMapping.CreatedOn = DateTime.UtcNow;
            mailSettingMapping.Email = _stringConstant.EmailForTest;
            mailSettingMapping.IsTo = true;
            group.CreatedOn = DateTime.UtcNow;
            group.Name = _stringConstant.TeamLeader;
            group.Type = 1;
            groupEmailMapping.CreatedOn = DateTime.UtcNow;
            groupEmailMapping.Email = _stringConstant.Email;
        }

        /// <summary>
        /// Method to add mail setting
        /// </summary>
        private async Task AddMailSetting()
        {
            _mailSettingDataRepository.Insert(mailSetting);
            await _mailSettingDataRepository.SaveChangesAsync();
            mailSettingMapping.MailSettingId = mailSetting.Id;
            _mailSettingMappingDataRepository.Insert(mailSettingMapping);
            await _mailSettingMappingDataRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Method to add group
        /// </summary>
        private async Task AddGroup()
        {
            _groupDataRepository.Insert(group);
            await _groupDataRepository.SaveChangesAsync();
            groupEmailMapping.GroupId = group.Id;
            _groupEmailMappingDataRepository.Insert(groupEmailMapping);
            await _groupEmailMappingDataRepository.SaveChangesAsync();
        }
        #endregion
    }
}
