using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <returns></returns>
        Task<string> StartScrum(string GroupName,string UserName);

        /// <summary>
        /// This method is called whenever a message other than "scrumn time" is written in the group during scrum meeting. - JJ
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Message"></param>
        /// <param name="GroupName"></param>
        /// <returns>Question statement</returns>
        Task<string> AddScrumAnswer(string UserName, string Message, string GroupName);

        /// <summary>
        /// This method will be called when the keyword "leave" is received as reply of a group member. - JJ
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserName"></param>
        /// <param name="LeaveApplicant"></param>
        /// <returns>Question to the next person</returns>
        Task<string> Leave(string GroupName,string UserName, string LeaveApplicant);
    }
}
