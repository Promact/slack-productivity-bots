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
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        private readonly IRepository<SlackBotUserDetail> _slackUserBotDetails;
        private readonly IStringConstantRepository _stringConstant;
        public SlackUserRepository(IRepository<SlackUserDetails> slackUserDetails, IRepository<SlackBotUserDetail> slackUserBotDetails, IStringConstantRepository stringConstant)
        {
            _slackUserDetails = slackUserDetails;
            _slackUserBotDetails = slackUserBotDetails;
            _stringConstant = stringConstant;
        }

        #region Public Methods

        /// <summary>
        /// Method to add slack user 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        public void AddSlackUser(SlackUserDetails slackUserDetails)
        {
            SlackUserDetails slackUser = _slackUserDetails.FirstOrDefault(x => x.UserId == slackUserDetails.UserId);
            SlackBotUserDetail slackBotUser = _slackUserBotDetails.FirstOrDefault(x => x.UserId == slackUserDetails.UserId);
            if (slackUser == null && slackBotUser == null)
            {
                if (slackUserDetails.IsBot || slackUserDetails.Name == _stringConstant.SlackBotStringName)
                    AddSlackBotUserDetail(slackUserDetails);
                else
                    AddSlackUserDetail(slackUserDetails);
            }
            else
                UpdateSlackUser(slackUserDetails);
        }


        /// <summary>
        /// Method to update slack user 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        public void UpdateSlackUser(SlackUserDetails slackUserDetails)
        {
            if (slackUserDetails.IsBot || slackUserDetails.Name == _stringConstant.SlackBotStringName)
                UpdateSlackBotUser(slackUserDetails);
            else
                UpdateSlackUserDetail(slackUserDetails);
        }


        /// <summary>
        /// Method to get slack user information by their slack user id
        /// </summary>
        /// <param name="slackId"></param>
        /// <returns>user</returns>
        public SlackUserDetails GetById(string slackId)
        {
            SlackUserDetails user = _slackUserDetails.FirstOrDefault(x => x.UserId == slackId);
            return user;
        }


        /// <summary>
        /// Fetch the list of Slack Users
        /// </summary>
        /// <returns>list of object of SlackUserDetails</returns>
        public List<SlackUserDetailAc> GetAllSlackUsers()
        {
            List<SlackUserDetailAc> slackUserAcList = new List<SlackUserDetailAc>();
            List<SlackUserDetails> slackUserList = _slackUserDetails.Fetch(x => !x.IsBot).ToList();
            Mapper.Initialize(cfg => cfg.CreateMap<SlackUserDetails, SlackUserDetailAc>());
            slackUserAcList = Mapper.Map(slackUserList, slackUserAcList);
            return slackUserAcList;
        }


        /// <summary>
        /// Method to get slack user information by their slack user name
        /// </summary>
        /// <param name="slackName"></param>
        /// <returns>user</returns>
        public SlackUserDetailAc GetBySlackName(string slackName)
        {
            SlackUserDetailAc slackUser = new SlackUserDetailAc();
            SlackUserDetails user = _slackUserDetails.FirstOrDefault(x => x.Name == slackName);
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
            _slackUserDetails.Insert(slackUserDetails);
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
            _slackUserBotDetails.Insert(slackUserBotDetail);
        }


        private void UpdateSlackUserDetail(SlackUserDetails slackUserDetails)
        {
            var user = _slackUserDetails.FirstOrDefault(x => x.UserId == slackUserDetails.UserId);
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
                _slackUserDetails.Update(user);
            }
            else
                throw new SlackUserNotFoundException(_stringConstant.UserNotFound);
        }


        private void UpdateSlackBotUser(SlackUserDetails slackUserDetails)
        {
            var botUser = _slackUserBotDetails.FirstOrDefault(x => x.UserId == slackUserDetails.UserId);
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
                _slackUserBotDetails.Update(botUser);
            }
            else
                throw new SlackUserNotFoundException(_stringConstant.BotNotFound);
        }

        #endregion
    }
}