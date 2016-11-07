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
            slackUserDetails.Title = slackUserDetails.Profile.Title;
            slackUserDetails.Email = slackUserDetails.Profile.Email;
            slackUserDetails.Skype = slackUserDetails.Profile.Skype;
            slackUserDetails.LastName = slackUserDetails.Profile.LastName;
            slackUserDetails.FirstName = slackUserDetails.Profile.FirstName;
            slackUserDetails.Phone = slackUserDetails.Profile.Phone;
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
                 }));

                Mapper.AssertConfigurationIsValid();

                // Perform mapping
                user = Mapper.Map(slackUserDetails, user);

                user.Title = slackUserDetails.Profile.Title;
                user.Email = slackUserDetails.Profile.Email;
                user.Skype = slackUserDetails.Profile.Skype;
                user.LastName = slackUserDetails.Profile.LastName;
                user.FirstName = slackUserDetails.Profile.FirstName;
                user.Phone = slackUserDetails.Profile.Phone;
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