using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System.Threading.Tasks;

namespace Promact.Core.Repository.RedmineRepository
{
    public interface IRedmineRepository
    {
        /// <summary>
        /// Method to handle Redmine Slash command
        /// </summary>
        /// <param name="slashCommand">slash command</param>
        /// <returns>reply message</returns>
        Task SlackRequest(SlashCommand slashCommand);
    }
}
