using Promact.Erp.DomainModel.ApplicationClass;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumRepository
{
    public interface IScrumBotRepository
    {

        /// <summary>
        /// This method will be called when the keyword "scrum time" or "scrum halt" or "scrum resume" is encountered
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserName"></param>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        Task<string> Scrum(string GroupName, string UserName, string Parameter);


        /// <summary>
        /// This method is called whenever a message other than "scrumn time" or "leave username" is written in the group during scrum meeting. - JJ
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Message"></param>
        /// <param name="GroupName"></param>
        /// <returns>The next Question Statement</returns>
        Task<string> AddScrumAnswer(string UserName, string Message, string GroupName);

        /// <summary>
        /// This method will be called when the keyword "leave @username" or "later @username" or "scrum later @username" is received as reply from a group member. - JJ
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserName"></param>
        /// <param name="LeaveApplicant"></param>
        /// <param name="Parameter"></param>
        /// <returns>Question to the next person</returns>
        Task<string> Leave(string GroupName, string UserName, string LeaveApplicant, string Parameter);

    }
}
