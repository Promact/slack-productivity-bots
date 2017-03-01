using AutoMapper;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Promact.Core.Repository.GroupRepository
{
    public class GroupRepository : IGroupRepository
    {
        #region Private Variables
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<GroupEmailMapping> _groupEmailMappingRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public GroupRepository(IRepository<Group> groupRepository, IMapper mapper, IRepository<GroupEmailMapping> groupEmailMappingRepository)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
            _groupEmailMappingRepository = groupEmailMappingRepository;
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
                foreach (var groupEmailMapping in group.GroupEmailMapping)
                {
                    listOfEmails.Add(groupEmailMapping.Email);
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
            List<Group> listOfGroup = await _groupRepository.GetAll().ToListAsync();
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

        #endregion
    }
}
