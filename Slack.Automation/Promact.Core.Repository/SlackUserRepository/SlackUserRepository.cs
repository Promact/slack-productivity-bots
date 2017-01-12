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
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        private readonly IRepository<SlackBotUserDetail> _slackUserBotDetails;
        private readonly IStringConstantRepository _stringConstant;
        public SlackUserRepository(IRepository<SlackUserDetails> slackUserDetails,
            IRepository<SlackBotUserDetail> slackUserBotDetails,
            IStringConstantRepository stringConstant)
        {
            _slackUserDetails = slackUserDetails;
            _slackUserBotDetails = slackUserBotDetails;
            _stringConstant = stringConstant;
        }


        #region Public Methods

        /// <summary>
        /// Method to add slack user 
        /// </summary>
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        public async Task AddSlackUserAsync(SlackUserDetails slackUserDetails)
        {
            SlackUserDetails slackUser = await _slackUserDetails.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            SlackBotUserDetail slackBotUser = await _slackUserBotDetails.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            if (slackUser == null && slackBotUser == null)
            {
                if (!slackUserDetails.Deleted)
                {
                    if (slackUserDetails.IsBot || slackUserDetails.Name == _stringConstant.SlackBotStringName)
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
            if (slackUserDetails.IsBot || slackUserDetails.Name == _stringConstant.SlackBotStringName)
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
            SlackUserDetailAc slackUser = new SlackUserDetailAc();
            SlackUserDetails user = await _slackUserDetails.FirstOrDefaultAsync(x => x.UserId == slackId);
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackUserDetailAc>());
            slackUser = Mapper.Map(user, slackUser);
            return slackUser;
        }


        /// <summary>
        /// Method to get slack user information by their slack user name. - JJ
        /// </summary>
        /// <param name="slackName">slack user name</param>
        /// <returns>object of SlackUserDetailAc</returns>
        public async Task<SlackUserDetailAc> GetBySlackNameAsync(string slackName)
        {
            SlackUserDetailAc slackUser = new SlackUserDetailAc();
            SlackUserDetails user = await _slackUserDetails.FirstOrDefaultAsync(x => x.Name == slackName);
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackUserDetailAc>());
            slackUser = Mapper.Map(user, slackUser);
            return slackUser;
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
            _slackUserDetails.Insert(slackUserDetails);
            await _slackUserDetails.SaveChangesAsync();
        }


        /// <summary>
        /// Add slack BotUser details to the database. - JJ
        /// </summary>
        /// <param name="slackBotUserDetails">slack bot user details. Object of SlackUserDetails</param>
        private async Task AddSlackBotUserDetailAsync(SlackUserDetails slackBotUserDetails)
        {
            SlackBotUserDetail slackUserBotDetail = new SlackBotUserDetail();
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackBotUserDetail>()
               .ForMember(des => des.FirstName,opt => opt.MapFrom(src => src.Profile.FirstName))
               .ForMember(des => des.LastName,opt => opt.MapFrom(src => src.Profile.LastName))
               .ForMember(des => des.BotId,opt => opt.MapFrom(src => src.Profile.BotId))
            );

            // Perform mapping
            slackUserBotDetail = Mapper.Map(slackBotUserDetails, slackUserBotDetail);

            slackUserBotDetail.CreatedOn = DateTime.UtcNow;
            _slackUserBotDetails.Insert(slackUserBotDetail);
            await _slackUserBotDetails.SaveChangesAsync();
        }


        /// <summary>
        /// Update details of SlackUserDetails. - JJ
        /// </summary>
        /// <param name="slackUserDetails">slack user details. Object of SlackUserDetails</param>
        private async Task UpdateSlackUserDetailAsync(SlackUserDetails slackUserDetails)
        {
            SlackUserDetails user = await _slackUserDetails.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            if (user != null)
            {
                if (slackUserDetails.Deleted)
                {
                    _slackUserDetails.Delete(user.Id);
                    await _slackUserDetails.SaveChangesAsync();
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
                       .ForMember(des => des.Title,opt => opt.MapFrom(src => src.Profile.Title))
                       .ForMember(des => des.Email,opt => opt.MapFrom(src => src.Profile.Email))
                       .ForMember(des => des.Skype,opt => opt.MapFrom(src => src.Profile.Skype))
                       .ForMember(des => des.LastName,opt => opt.MapFrom(src => src.Profile.LastName))
                       .ForMember(des => des.FirstName,opt => opt.MapFrom(src => src.Profile.FirstName))
                       .ForMember(des => des.Phone,opt => opt.MapFrom(src => src.Profile.Phone))
                    );

                    // Perform mapping
                    user = Mapper.Map(slackUserDetails, user);
                    _slackUserDetails.Update(user);
                    await _slackUserDetails.SaveChangesAsync();
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
            SlackBotUserDetail botUser = await _slackUserBotDetails.FirstOrDefaultAsync(x => x.UserId == slackBotUserDetails.UserId);
            if (botUser != null)
            {
                if (slackBotUserDetails.Deleted)
                {
                    _slackUserBotDetails.Delete(botUser.Id);
                    await _slackUserBotDetails.SaveChangesAsync();
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

                   .ForMember(des => des.LastName,opt => opt.MapFrom(src => src.Profile.LastName))
                   .ForMember(des => des.FirstName,opt => opt.MapFrom(src => src.Profile.FirstName))
                   .ForMember(des => des.BotId,opt => opt.MapFrom(src => src.Profile.BotId))
                    );

                    // Perform mapping
                    botUser = Mapper.Map(slackBotUserDetails, botUser);
                    _slackUserBotDetails.Update(botUser);
                    await _slackUserBotDetails.SaveChangesAsync();
                }
            }
            else
                throw new SlackUserNotFoundException(_stringConstant.BotNotFound);
        }


        #endregion


    }
}