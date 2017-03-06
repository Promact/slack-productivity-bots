using AutoMapper;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.GroupRepository
{
    public class GroupRepository : IGroupRepository
    {
        #region Private Variables
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<GroupEmailMapping> _groupEmailMappingRepository;
        private readonly IOauthCallHttpContextRespository _oauthCallsRepository;
        private readonly IStringConstantRepository _stringConstantRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public GroupRepository(IRepository<Group> groupRepository, IMapper mapper, IRepository<GroupEmailMapping> groupEmailMappingRepository, IOauthCallHttpContextRespository oauthCallsRepository, IStringConstantRepository stringConstantRepository)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
            _oauthCallsRepository = oauthCallsRepository;
            _groupEmailMappingRepository = groupEmailMappingRepository;
            _stringConstantRepository = stringConstantRepository;
        }
        #endregion

        #region Public Method(s)

        /// <summary>
        /// This method used for insert group and return Id. - an
        /// </summary>
        /// <param name="groupAC">pass groupAC</param>
        /// <returns>Primary key(Id)</returns>
        public async Task<int> AddGroupAsync(GroupAC groupAC)
        {
            Group group = new Group();
            group = _mapper.Map(groupAC, group);
            group.CreatedOn = DateTime.UtcNow;
            _groupRepository.Insert(group);
            await _groupRepository.SaveChangesAsync();
            await AddGroupEmailMapping(groupAC.Emails, group.Id);
            return group.Id;
        }

        /// <summary>
        /// This method used for update group and return Id. - an
        /// </summary>
        /// <param name="groupAC">pass groupAC</param>
        /// <returns>Primary key(Id)</returns>
        public async Task<int> UpdateGroupAsync(GroupAC groupAC)
        {
            Group group = _groupRepository.GetById(groupAC.Id);
            group.Name = groupAC.Name;
            group.UpdatedDate = DateTime.UtcNow;
            _groupRepository.Update(group);
            await _groupRepository.SaveChangesAsync();
            _groupEmailMappingRepository.RemoveRange(x => x.GroupId == groupAC.Id);
            await AddGroupEmailMapping(groupAC.Emails, groupAC.Id);
            return groupAC.Id;
        }

        /// <summary>
        /// This method used for get group by id. -an
        /// </summary>
        /// <param name="id">passs group id</param>
        /// <returns>GroupAC object</returns>
        public async Task<GroupAC> GetGroupByIdAsync(int id)
        {
            Group group = (await _groupRepository.FirstOrDefaultAsync(x => x.Id == id));
            if (group != null)
            {
                GroupAC groupAc = new GroupAC();
                List<string> listOfEmails = new List<string>();
                groupAc = _mapper.Map(group, groupAc);
                //get active user email list
                List<string> listOfActiveUserEmail = await GetActiveUserEmailList();
                List<GroupEmailMapping> groupEmailMappings = group.GroupEmailMapping.ToList();
                foreach (var groupEmailMapping in groupEmailMappings)
                {
                    if (listOfActiveUserEmail.Contains(groupEmailMapping.Email)) //check email is active or not.
                        listOfEmails.Add(groupEmailMapping.Email);
                    else
                    {
                        _groupEmailMappingRepository.Delete(groupEmailMapping.Id);
                         await _groupEmailMappingRepository.SaveChangesAsync();
                    }
                }
                groupAc.Emails = listOfEmails;
                return groupAc;
            }
            else
                throw new GroupNotFound();
        }

        /// <summary>
        /// This method used for check group name is already exists or not.
        /// </summary>
        /// <param name="groupName">passs group name</param>
        /// <param name="isUpdate">pass group id When check group name is exists at update time
        /// other wise pass 0</param>
        /// <returns></returns>
        public async Task<bool> CheckGroupNameIsExistsAsync(string groupName, int groupId)
        {
            if (groupId == 0)
                return (await _groupRepository.FirstOrDefaultAsync(x => x.Name == groupName) != null);
            else
                return (await _groupRepository.FirstOrDefaultAsync(x => x.Name == groupName && x.Id != groupId) != null);
        }

        /// <summary>
        /// This method used for get list of group. - an
        /// </summary>
        /// <returns>list of group</returns>
        public async Task<List<GroupAC>> GetListOfGroupACAsync()
        {
            List<GroupAC> groupAc = new List<GroupAC>();
            List<Group> listOfGroup = await _groupRepository.GetAll().OrderByDescending(x => x.CreatedOn).ToListAsync();
            return _mapper.Map(listOfGroup, groupAc);
        }

        /// <summary>
        /// This mehod used for delete group by id. -an
        /// </summary>
        /// <param name="id">pass group id</param>
        /// <returns>true</returns>
        public async Task<bool> DeleteGroupById(int id)
        {
            _groupRepository.Delete(id);
            await _groupRepository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// This method used for added dynamic group. -an
        /// </summary>
        /// <returns></returns>
        public async Task AddDynamicGroupAsync()
        {
            UserEmailListAc userEmailListAc = await _oauthCallsRepository.GetUserEmailListBasedOnRoleAsync();
            if (userEmailListAc != null)
            {
                //create team leader group
                await InsertDynamicGroup(_stringConstantRepository.TeamLeaderGroup, userEmailListAc.TeamLeader);
                //create team member group
                await InsertDynamicGroup(_stringConstantRepository.TeamMembersGroup, userEmailListAc.TamMemeber);
                //create managment group
                await InsertDynamicGroup(_stringConstantRepository.ManagementGroup, userEmailListAc.Management);
            }
        }

        /// <summary>
        /// This method used for get active user email list. - an
        /// </summary>
        /// <returns>list of active user email list</returns>
        public async Task<List<string>> GetActiveUserEmailList()
        {
            UserEmailListAc userEmailListAc = await _oauthCallsRepository.GetUserEmailListBasedOnRoleAsync();
            List<string> listOfEmails = new List<string>();
            listOfEmails.AddRange(userEmailListAc.Management);
            listOfEmails.AddRange(userEmailListAc.TamMemeber);
            listOfEmails.AddRange(userEmailListAc.TeamLeader);
            return listOfEmails.Distinct().ToList();
        }

        #endregion

        #region Private Method(s)

        /// <summary>
        /// This method used for add group emails in GroupEmailMapping table.
        /// </summary>
        /// <param name="listOfEmails">pass list of emails</param>
        /// <param name="groupId">pass group id</param>
        /// <returns></returns>
        private async Task AddGroupEmailMapping(List<string> listOfEmails, int groupId)
        {
            foreach (var email in listOfEmails)
            {
                GroupEmailMapping groupEmailMapping = new GroupEmailMapping();
                groupEmailMapping.CreatedOn = DateTime.UtcNow;
                groupEmailMapping.Email = email;
                groupEmailMapping.GroupId = groupId;
                _groupEmailMappingRepository.Insert(groupEmailMapping);
                await _groupEmailMappingRepository.SaveChangesAsync();

            }

        }

        /// <summary>
        /// This method used for add dynamic group.
        /// </summary>
        /// <param name="groupName">pass group name</param>
        /// <param name="listOfEmails">pass list of email</param>
        /// <returns></returns>
        private async Task InsertDynamicGroup(string groupName, List<string> listOfEmails)
        {
            var group = await _groupRepository.FirstOrDefaultAsync(x => x.Name == groupName && x.Type == 1);
            if (group == null) //added group
            {
                Group newGroup = new Group();
                newGroup.Name = groupName;
                newGroup.Type = 1;
                newGroup.CreatedOn = DateTime.UtcNow;
                _groupRepository.Insert(newGroup);
                await _groupRepository.SaveChangesAsync();
                if (listOfEmails.Count > 0)
                    await AddGroupEmailMapping(listOfEmails, newGroup.Id);

            }//update group
            else
            {
                _groupEmailMappingRepository.RemoveRange(x => x.GroupId == group.Id);
                await AddGroupEmailMapping(listOfEmails, group.Id);
            }
        }

        #endregion
    }
}
