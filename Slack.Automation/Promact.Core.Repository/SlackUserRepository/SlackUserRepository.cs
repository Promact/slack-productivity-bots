using System;
using System.Threading.Tasks;
using AutoMapper;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.Util.StringLiteral;
using NLog;

namespace Promact.Core.Repository.SlackUserRepository
{
    public class SlackUserRepository : ISlackUserRepository
    {
        #region Private Variable 
        private readonly IRepository<SlackUserDetails> _slackUserDetailsRepository;
        private readonly AppStringLiteral _stringConstant;
        private readonly IMapper _mapper;
        private readonly ILogger _loggerSlackEvent;
        #endregion

        #region Constructor
        public SlackUserRepository(IRepository<SlackUserDetails> slackUserDetailsRepository,
        ISingletonStringLiteral stringConstant, IMapper mapper)
        {
            _slackUserDetailsRepository = slackUserDetailsRepository;
            _stringConstant = stringConstant.StringConstant;
            _mapper = mapper;
            _loggerSlackEvent = LogManager.GetLogger("SlackEvent");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to add/update slack user 
        /// </summary>
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        public async Task AddSlackUserAsync(SlackUserDetails slackUserDetails)
        {
            SlackUserDetails slackUser = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            _loggerSlackEvent.Debug("Slack user : " + slackUser);
            if (slackUser == null)
            {
                if (!slackUserDetails.Deleted)
                {
                    _loggerSlackEvent.Debug("AddSlackUserDetailAsync");
                    //Added to database only if the user is not deleted
                    await AddSlackUserDetailAsync(slackUserDetails);
                }
            }
            else
            {
                _loggerSlackEvent.Debug("UpdateSlackUserAsync");
                await UpdateSlackUserAsync(slackUserDetails);
            }
        }

        /// <summary>
        /// Method to update slack user. - JJ
        /// </summary>
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        public async Task UpdateSlackUserAsync(SlackUserDetails slackUserDetails)
        {
            SlackUserDetails user = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            _loggerSlackEvent.Debug("User to be update : " + user);
            if (slackUserDetails.Deleted)
            {
                _loggerSlackEvent.Debug("Deleting user");
                //delete the deleted user from database
                _slackUserDetailsRepository.Delete(user.Id);
                await _slackUserDetailsRepository.SaveChangesAsync();
                _loggerSlackEvent.Debug("User deleted");
            }
            else
            {
                _loggerSlackEvent.Debug("Updating user");
                // Perform mapping
                user = _mapper.Map(slackUserDetails, user);
                _slackUserDetailsRepository.Update(user);
                await _slackUserDetailsRepository.SaveChangesAsync();
                _loggerSlackEvent.Debug("User updated");
            }
        }

        /// <summary>
        /// Method to get slack user information by their slack user id
        /// </summary>
        /// <param name="slackId">slack user id</param>
        /// <returns>object of SlackUserDetailAc</returns>
        public async Task<SlackUserDetailAc> GetByIdAsync(string slackId)
        {
            SlackUserDetails slackUserDetail = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackId);
            SlackUserDetailAc slackUserDetailAc = _mapper.Map<SlackUserDetailAc>(slackUserDetail);
            return slackUserDetailAc;
        }

        /// <summary>
        /// Method to get slack user information by their slack user name. - JJ
        /// </summary>
        /// <param name="userSlackName">slack user name</param>
        /// <returns>object of SlackUserDetailAc</returns>
        public async Task<SlackUserDetailAc> GetBySlackNameAsync(string userSlackName)
        {
            SlackUserDetails slackUserDetail = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.Name == userSlackName);
            SlackUserDetailAc slackUserDetailAc = _mapper.Map<SlackUserDetailAc>(slackUserDetail);
            return slackUserDetailAc;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Add Slack User Details to the database - JJ
        /// </summary>
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        private async Task AddSlackUserDetailAsync(SlackUserDetails slackUserDetails)
        {
            // Perform mapping
            slackUserDetails = _mapper.Map<SlackUserDetails>(slackUserDetails);
            slackUserDetails.CreatedOn = DateTime.UtcNow;
            _loggerSlackEvent.Debug("Adding slack user details");
            _slackUserDetailsRepository.Insert(slackUserDetails);
            await _slackUserDetailsRepository.SaveChangesAsync();
            _loggerSlackEvent.Debug("Adding slack user details completed");
        }
        #endregion
    }
}