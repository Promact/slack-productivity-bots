using System.Threading.Tasks;

namespace Promact.Core.Repository.BotRepository
{
    public interface IBotRepository
    {
        /// <summary>
        /// Method to return on bot by module name
        /// </summary>
        /// <param name="module">module name</param>
        Task TurnOnBot(string module);

        /// <summary>
        /// Method to return off bot by module name
        /// </summary>
        /// <param name="module">module name</param>
        Task TurnOffBot(string module);
    }
}
