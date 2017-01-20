using AutoMapper;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;

namespace Promact.Core.Repository.AutoMapperConfig
{
    public static class AutoMapperConfiguration
    {
        public static IMapper ConfigureMap()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SlackUserDetails, SlackUserDetailAc>();


                cfg.CreateMap<SlackUserDetails, SlackUserDetails>()
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
                     .ForMember(des => des.Phone, opt => opt.MapFrom(src => src.Profile.Phone));


                cfg.CreateMap<SlackUserDetails, SlackBotUserDetail>()
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
                     .ForMember(des => des.BotId, opt => opt.MapFrom(src => src.Profile.BotId));
                
            });
            return config.CreateMapper();
        }
    }
}

