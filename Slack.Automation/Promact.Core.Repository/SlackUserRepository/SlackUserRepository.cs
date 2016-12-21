using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using System;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.StringConstants;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.SlackUserRepository
{
    public class SlackUserRepository : ISlackUserRepository
    {
        #region Private Methods
        private readonly IRepository<SlackUserDetails> _slackUserDetailsRepository;
        private readonly IRepository<SlackBotUserDetail> _slackUserBotDetailsRepository;
        private readonly IStringConstantRepository _stringConstant;
        #endregion

        #region Constructor
        public SlackUserRepository(IRepository<SlackUserDetails> slackUserDetailsRepository, 
            IRepository<SlackBotUserDetail> slackUserBotDetailsRepository, IStringConstantRepository stringConstant)
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
        /// <param name="slackUserDetails"></param>
        public async Task AddSlackUserAsync(SlackUserDetails slackUserDetails)
        {
            SlackUserDetails slackUser = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            SlackBotUserDetail slackBotUser = await _slackUserBotDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            if (slackUser == null && slackBotUser == null)
            {
                if (slackUserDetails.IsBot || slackUserDetails.Name == _stringConstant.SlackBotStringName)
                    AddSlackBotUserDetail(slackUserDetails);
                else
                    AddSlackUserDetail(slackUserDetails);
            }
            else
                await UpdateSlackUserAsync(slackUserDetails);
        }


        /// <summary>
        /// Method to update slack user 
        /// </summary>
        /// <param name="slackUserDetails"></param>
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
        /// <param name="slackId"></param>
        /// <returns>user</returns>
        public async Task<SlackUserDetails> GetByIdAsync(string slackId)
        {
            SlackUserDetails user = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackId);
            return user;
        }


        /// <summary>
        /// Method to get slack user information by their slack user name
        /// </summary>
        /// <param name="slackName"></param>
        /// <returns>user</returns>
        public async Task<SlackUserDetailAc> GetBySlackNameAsync(string slackName)
        {
            SlackUserDetailAc slackUser = new SlackUserDetailAc();
            SlackUserDetails user = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.Name == slackName);
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackUserDetailAc>());
            slackUser = Mapper.Map(user, slackUser);
            return slackUser;
        }


        #endregion


        #region Private Methods


        /// <summary>
        /// Add Slack User Details to the database
        /// </summary>
        /// <param name="slackUserDetails"></param>
        private void AddSlackUserDetail(SlackUserDetails slackUserDetails)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackUserDetails>()

                .ForMember(des => des.Title,
                    opt => opt.MapFrom(src => src.Profile.Title)
                  )
                .ForMember(des => des.Email,
                    opt => opt.MapFrom(src => src.Profile.Email)
                  )
                .ForMember(des => des.Skype,
                    opt => opt.MapFrom(src => src.Profile.Skype)
                  )
                .ForMember(des => des.LastName,
                    opt => opt.MapFrom(src => src.Profile.LastName)
                  )
                .ForMember(des => des.FirstName,
                    opt => opt.MapFrom(src => src.Profile.FirstName)
                  )
                .ForMember(des => des.Phone,
                    opt => opt.MapFrom(src => src.Profile.Phone)
                  )
               );

            // Perform mapping
            slackUserDetails = Mapper.Map(slackUserDetails, slackUserDetails);

            slackUserDetails.CreatedOn = DateTime.UtcNow;
            _slackUserDetailsRepository.Insert(slackUserDetails);
        }


        /// <summary>
        /// Add slack BotUser details to the database
        /// </summary>
        /// <param name="slackUserDetails"></param>
        private void AddSlackBotUserDetail(SlackUserDetails slackUserDetails)
        {
            SlackBotUserDetail slackUserBotDetail = new SlackBotUserDetail();
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackBotUserDetail>()
               .ForMember(des => des.FirstName,
                  opt => opt.MapFrom(src => src.Profile.FirstName)
               )
               .ForMember(des => des.LastName,
                   opt => opt.MapFrom(src => src.Profile.LastName)
               )
               .ForMember(des => des.BotId,
                  opt => opt.MapFrom(src => src.Profile.BotId)
               )
            );

            // Perform mapping
            slackUserBotDetail = Mapper.Map(slackUserDetails, slackUserBotDetail);

            slackUserBotDetail.CreatedOn = DateTime.UtcNow;
            _slackUserBotDetailsRepository.Insert(slackUserBotDetail);
        }


        private async Task UpdateSlackUserDetailAsync(SlackUserDetails slackUserDetails)
        {
            var user = await _slackUserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            if (user != null)
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
               .ForMember(des => des.Title,
                      opt => opt.MapFrom(src => src.Profile.Title)
                    )
               .ForMember(des => des.Email,
                       opt => opt.MapFrom(src => src.Profile.Email)
                       )
               .ForMember(des => des.Skype,
                         opt => opt.MapFrom(src => src.Profile.Skype)
                    )
               .ForMember(des => des.LastName,
                      opt => opt.MapFrom(src => src.Profile.LastName)
                       )
               .ForMember(des => des.FirstName,
                          opt => opt.MapFrom(src => src.Profile.FirstName)
                          )
               .ForMember(des => des.Phone,
                        opt => opt.MapFrom(src => src.Profile.Phone)
                        )
                 );

                // Perform mapping
                user = Mapper.Map(slackUserDetails, user);
                _slackUserDetailsRepository.Update(user);
            }
            else
                throw new SlackUserNotFoundException(_stringConstant.UserNotFound);
        }


        private async Task UpdateSlackBotUserAsync(SlackUserDetails slackUserDetails)
        {
            var botUser = await _slackUserBotDetailsRepository.FirstOrDefaultAsync(x => x.UserId == slackUserDetails.UserId);
            if (botUser != null)
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

               .ForMember(des => des.LastName,
                    opt => opt.MapFrom(src => src.Profile.LastName)
                   )
               .ForMember(des => des.FirstName,
                     opt => opt.MapFrom(src => src.Profile.FirstName)
                   )
               .ForMember(des => des.BotId,
                       opt => opt.MapFrom(src => src.Profile.BotId)
                     )
                 );

                // Perform mapping
                botUser = Mapper.Map(slackUserDetails, botUser);
                _slackUserBotDetailsRepository.Update(botUser);
            }
            else
                throw new SlackUserNotFoundException(_stringConstant.BotNotFound);
        }

        #endregion
    }
}