using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.DataRepository;
using System;
using Promact.Erp.Util.ExceptionHandler;
using Promact.Erp.Util.StringConstants;
using AutoMapper;


namespace Promact.Core.Repository.SlackUserRepository
{
    public class SlackUserRepository : ISlackUserRepository
    {
        private readonly IRepository<SlackUserDetails> _slackUserDetails;
        private readonly IStringConstantRepository _stringConstant;
        public SlackUserRepository(IRepository<SlackUserDetails> slackUserDetails, IStringConstantRepository stringConstant)
        {
            _slackUserDetails = slackUserDetails;
            _stringConstant = stringConstant;
        }


        /// <summary>
        /// Method to add slack user 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        public void AddSlackUser(SlackUserDetails slackUserDetails)
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
        /// Method to update slack user 
        /// </summary>
        /// <param name="slackUserDetails"></param>
        public void UpdateSlackUser(SlackUserDetails slackUserDetails)
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


        /// <summary>
        /// Method to get slack user information by their slack user id
        /// </summary>
        /// <param name="slackId"></param>
        /// <returns>user</returns>
        public SlackUserDetails GetById(string slackId)
        {
            var user = _slackUserDetails.FirstOrDefault(x => x.UserId == slackId);
            return user;
        }
    }
}