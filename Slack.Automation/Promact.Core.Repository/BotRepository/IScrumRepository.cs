namespace Promact.Core.Repository.BotRepository
{
    public interface IScrumRepository
    {
        /// <summary>
        /// Method to turn on scrum bot
        /// </summary>
        /// <param name="botToken">token of bot</param>
        void StartAndConnectScrumBot(string botToken);
    }
}
