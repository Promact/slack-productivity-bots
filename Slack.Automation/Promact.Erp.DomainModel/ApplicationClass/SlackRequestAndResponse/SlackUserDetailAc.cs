
namespace Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse
{
    public class SlackUserDetailAc
    {
        /// <summary>
        /// User Id of Slack for user
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Real Name of the slack user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Bool true for they exist in team or not
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Bit indicating whether this user is active in OAuth or Not
        /// </summary>     
        public bool IsActive { get; set; }

    }
}
