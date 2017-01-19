using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using System;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.StringConstants;
using AutoMapper;
using System.Threading.Tasks;

namespace Promact.Core.Repository.SlackUserRepository
{
    public class SlackUserRepository : ISlackUserRepository
    {

        #region Private Variable 

        private readonly IRepository<SlackUserDetails> _slackUserDetailsRepository;
        private readonly IRepository<SlackBotUserDetail> _slackUserBotDetailsRepository;
        private readonly IStringConstantRepository _stringConstant;

        #endregion


        #region Constructor

        public SlackUserRepository(IRepository<SlackUserDetails> slackUserDetailsRepository,
            IRepository<SlackBotUserDetail> slackUserBotDetailsRepository,
            IStringConstantRepository stringConstant)
        {
            _slackUserDetailsRepository = slackUserDetailsRepository;
            _slackUserBotDetailsRepository = slackUserBotDetailsRepository;
            _stringConstant = stringConstant;
        }

        #endregion


        #region Public Methods


        /// <summary>
        /// Method to add slack user 
        /// </summary>
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        public async Task AddSlackUserAsync(SlackUserDetails slackUserDetails)
        {
            SlackUserDetails slackUser = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            SlackBotUserDetail slackBotUser = await _slackUserBotDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            if (slackUser == null && slackBotUser == null)
            {
                if (!slackUserDetails.Deleted)
                {
                    if (slackUserDetails.IsBot || slackUserDetails.Name == _stringConstant.SlackBotName)
                        await AddSlackBotUserDetailAsync(slackUserDetails);
                    else
                        await AddSlackUserDetailAsync(slackUserDetails);
                }
            }
            else
                await UpdateSlackUserAsync(slackUserDetails);
        }


        /// <summary>
        /// Method to update slack user. - JJ
        /// </summary>
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        public async Task UpdateSlackUserAsync(SlackUserDetails slackUserDetails)
        {
            if (slackUserDetails.IsBot || slackUserDetails.Name == _stringConstant.SlackBotName)
                await UpdateSlackBotUserAsync(slackUserDetails);
            else
                await UpdateSlackUserDetailAsync(slackUserDetails);
        }


        /// <summary>
        /// Method to get slack user information by their slack user id
        /// </summary>
        /// <param name="slackId">slack user id</param>
        /// <returns>object of SlackUserDetailAc</returns>
        public async Task<SlackUserDetailAc> GetByIdAsync(string slackId)
        {
            SlackUserDetailAc slackUserDetailAc = new SlackUserDetailAc();
            SlackUserDetails slackUserDetail = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackId);
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackUserDetailAc>());
            slackUserDetailAc = Mapper.Map(slackUserDetail, slackUserDetailAc);
            return slackUserDetailAc;
        }


        /// <summary>
        /// Method to get slack user information by their slack user name. - JJ
        /// </summary>
        /// <param name="userSlackName">slack user name</param>
        /// <returns>object of SlackUserDetailAc</returns>
        public async Task<SlackUserDetailAc> GetBySlackNameAsync(string userSlackName)
        {
            SlackUserDetailAc slackUserDetailAc = new SlackUserDetailAc();
            SlackUserDetails slackUserDetail = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.Name == userSlackName);
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackUserDetailAc>());
            slackUserDetailAc = Mapper.Map(slackUserDetail, slackUserDetailAc);
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
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackUserDetails>()
                .ForMember(des => des.Title, opt => opt.MapFrom(src => src.Profile.Title))
                .ForMember(des => des.Email, opt => opt.MapFrom(src => src.Profile.Email))
                .ForMember(des => des.Skype, opt => opt.MapFrom(src => src.Profile.Skype))
                .ForMember(des => des.LastName, opt => opt.MapFrom(src => src.Profile.LastName))
                .ForMember(des => des.FirstName, opt => opt.MapFrom(src => src.Profile.FirstName))
                .ForMember(des => des.Phone, opt => opt.MapFrom(src => src.Profile.Phone))
            );

            // Perform mapping
            slackUserDetails = Mapper.Map(slackUserDetails, slackUserDetails);

            slackUserDetails.CreatedOn = DateTime.UtcNow;
            _slackUserDetailsRepository.Insert(slackUserDetails);
            await _slackUserDetailsRepository.SaveChangesAsync();
        }


        /// <summary>
        /// Add slack BotUser details to the database. - JJ
        /// </summary>
        /// <param name="slackBotUserDetails">slack bot user details. Object of SlackUserDetails</param>
        private async Task AddSlackBotUserDetailAsync(SlackUserDetails slackBotUserDetails)
        {
            SlackBotUserDetail slackUserBotDetail = new SlackBotUserDetail();
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackBotUserDetail>()
               .ForMember(des => des.FirstName, opt => opt.MapFrom(src => src.Profile.FirstName))
               .ForMember(des => des.LastName, opt => opt.MapFrom(src => src.Profile.LastName))
               .ForMember(des => des.BotId, opt => opt.MapFrom(src => src.Profile.BotId))
            );

            // Perform mapping
            slackUserBotDetail = Mapper.Map(slackBotUserDetails, slackUserBotDetail);

            slackUserBotDetail.CreatedOn = DateTime.UtcNow;
            _slackUserBotDetailsRepository.Insert(slackUserBotDetail);
            await _slackUserBotDetailsRepository.SaveChangesAsync();
        }


        /// <summary>
        /// Update details of SlackUserDetails. - JJ
        /// </summary>
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        private async Task UpdateSlackUserDetailAsync(SlackUserDetails slackUserDetails)
        {
            SlackUserDetails user = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            if (user != null)
            {
                if (slackUserDetails.Deleted)
                {
                    _slackUserDetailsRepository.Delete(user.Id);
                    await _slackUserDetailsRepository.SaveChangesAsync();
                }
                else
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackUserDetails>()
                       .ForMember(des => des.Id, opt =>
                       {
                           opt.UseDestinationValue();
                           opt.Ignore();
                       })
                       .ForMember(des => des.CreatedOn, opt =>
                       {
                           opt.UseDestinationValue();
                           opt.Ignore();
                       })
                       .ForMember(des => des.Title, opt => opt.MapFrom(src => src.Profile.Title))
                       .ForMember(des => des.Email, opt => opt.MapFrom(src => src.Profile.Email))
                       .ForMember(des => des.Skype, opt => opt.MapFrom(src => src.Profile.Skype))
                       .ForMember(des => des.LastName, opt => opt.MapFrom(src => src.Profile.LastName))
                       .ForMember(des => des.FirstName, opt => opt.MapFrom(src => src.Profile.FirstName))
                       .ForMember(des => des.Phone, opt => opt.MapFrom(src => src.Profile.Phone))
                    );

                    // Perform mapping
                    user = Mapper.Map(slackUserDetails, user);
                    _slackUserDetailsRepository.Update(user);
                    await _slackUserDetailsRepository.SaveChangesAsync();
                }
            }
            else
                throw new SlackUserNotFoundException(_stringConstant.UserNotFound);
        }


        /// <summary>
        /// Update details of SlackBotUser. JJ
        /// </summary>
        /// <param name="slackBotUserDetails">slack bot user details. Object of SlackUserDetails</param>
        private async Task UpdateSlackBotUserAsync(SlackUserDetails slackBotUserDetails)
        {
            SlackBotUserDetail botUser = await _slackUserBotDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackBotUserDetails.UserId);
            if (botUser != null)
            {
                if (slackBotUserDetails.Deleted)
                {
                    _slackUserBotDetailsRepository.Delete(botUser.Id);
                    await _slackUserBotDetailsRepository.SaveChangesAsync();
                }
                else
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackBotUserDetail>()
                    .ForMember(des => des.Id, opt =>
                    {
                        opt.UseDestinationValue();
                        opt.Ignore();
                    })
                    .ForMember(des => des.CreatedOn, opt =>
                    {
                        opt.UseDestinationValue();
                        opt.Ignore();
                    })
                   .ForMember(des => des.LastName, opt => opt.MapFrom(src => src.Profile.LastName))
                   .ForMember(des => des.FirstName, opt => opt.MapFrom(src => src.Profile.FirstName))
                   .ForMember(des => des.BotId, opt => opt.MapFrom(src => src.Profile.BotId))
                    );

                    // Perform mapping
                    botUser = Mapper.Map(slackBotUserDetails, botUser);
                    _slackUserBotDetailsRepository.Update(botUser);
                    await _slackUserBotDetailsRepository.SaveChangesAsync();
                }
            }
            else
                throw new SlackUserNotFoundException(_stringConstant.BotNotFound);
        }


        #endregion


    }
}