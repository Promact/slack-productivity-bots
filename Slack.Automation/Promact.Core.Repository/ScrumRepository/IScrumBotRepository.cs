using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumRepository
{
    public interface IScrumBotRepository
    {

        /// <summary>
        /// This method will be called when the keyword "scrum time" is encountered
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserName"></param>
        /// <returns>The next Question Statement</returns>
        Task<string> StartScrum(string GroupName, string UserName);

        /// <summary>
        /// This method is called whenever a message other than "scrumn time" or "leave username" is written in the group during scrum meeting. - JJ
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Message"></param>
        /// <param name="GroupName"></param>
        /// <returns>The next Question Statement</returns>
        Task<string> AddScrumAnswer(string UserName, string Message, string GroupName);

        /// <summary>
        /// This method will be called when the keyword "leave username" is received as reply from a group member. - JJ
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserName"></param>
        /// <param name="LeaveApplicant"></param>
        /// <returns>Question to the next person</returns>
        Task<string> Leave(string GroupName, string UserName, string LeaveApplicant);

        /// <summary>
        /// This method is used to add Scrum answer to the database
        /// </summary>
        /// <param name="ScrumID"></param>
        /// <param name="QuestionId"></param>
        /// <param name="EmployeeId"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        bool AddAnswer(int ScrumID, int QuestionId, string EmployeeId, string Message);


    }
}
